using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Casperinc.OpenCleveland.DocumentParser.Bridge.Data;
using Casperinc.OpenCleveland.DocumentParser.Bridge.Data.Models;
using Casperinc.OpenCleveland.DocumentParser.Bridge.Helpers;
using Casperinc.OpenCleveland.DocumentParser.Core.Helpers;
using Casperinc.OpenCleveland.DocumentParser.Core.Models;
using Microsoft.Extensions.Logging;

namespace Casperinc.OpenCleveland.DocumentParser.Core.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private IDocumentSource _documentDb;
        private ILoggerFactory _logger;

        public DocumentRepository(IDocumentSource documentDb, ILoggerFactory logger)
        {
            _documentDb = documentDb;
            _logger = logger;
        }

        public async Task<Document> GetDocumentFromPathAsync(string path)
        {
            var lines = GetAllLinesForText(path);
            var fullText = String.Join("\r\n", lines);
            //var wordMaps = await GetWordMaps(fullText, lines);
            var wordMaps = new List<WordMap>();

            var documentForReturn = new Document();

            if(DocumentExists(fullText)){
                var documentFromDB = new Document();
                documentForReturn = Mapper.Map<Document>(documentFromDB);
            } else 
            {
                var newDocument = new Document()
                {
                    FullText = fullText,
                    Hash = fullText.GetSHA256Hash(),
                    WordMaps = wordMaps
                };

                var documentFromDB = _documentDb.SaveDocument(
                        Mapper.Map<DocumentDataDTO>(new Document(){
                            Id = Guid.NewGuid(),
                            FullText = fullText,
                            Hash = fullText.GetSHA256Hash(),
                            WordMaps = wordMaps
                            }
                        )
                    );

                documentForReturn = Mapper.Map<Document>(documentFromDB);
            }

            return documentForReturn;

        }

        public List<string> GetObjectListForDirectory(string path)
        {
            var reviewPaths = new List<string>();
            reviewPaths.AddRange(Directory.GetFiles(path));
            reviewPaths.AddRange(Directory.GetDirectories(path));

            var paths = new List<string>();
            foreach (var newPath in reviewPaths)
            {
                if (File.Exists(newPath)) paths.Add(newPath);

                if (Directory.Exists(newPath)) paths.AddRange(GetObjectListForDirectory(newPath));
            }

            return paths;

        }

        private List<string> GetAllLinesForText(string path)
        {
            string lineOfText;
            var lines = new List<string>();
            FileStream fileStream = new FileStream(path, FileMode.Open);
            using (StreamReader reader = new StreamReader(fileStream, System.Text.Encoding.UTF8, true, 1024))
            {
                while ((lineOfText = reader.ReadLine()) != null)
                {
                    lines.Add(lineOfText);
                }
            }

            return lines;
        }

        private async Task<List<WordMap>> GetWordMaps(string fullText, List<string> lines)
        {
            var chars = new char[] {
                    ',','.','\'','\"',')','(','-'
                };

            var wordList = lines.Where(word => word.Length >= 2)
                                .Distinct()
                                .OrderBy(word => word);


            // word => 
            //     new WordMap(){
            //             Word = new Word(){Value = word.ToLowerInvariant().Trim(chars)},
            //             Positions = GetPostionton(word, fullText),
            //             NeighborWords = new NeighborWords()
            //     }

            var buildMaps = wordList.Where(word => word.Length >= 2)
                                    .Distinct()
                                    .Select(word => Task.Run(() => BuildWordMap(word, fullText)))
                                    .ToArray();
                                

            var result = await Task.WhenAll(buildMaps);

            var wordMapList = new List<WordMap>();
            wordMapList.AddRange(result);

            return wordMapList.ToList();
        }

        private WordMap BuildWordMap(string word, string text)
        {      
            var thing =  new WordMap(){
                Word = new Word(){Value = word},
                Positions = text.ToLowerInvariant().AllPositions(word),
                NeighborWords = new NeighborWords()
            };

            return thing;

        }

        private bool DocumentExists(string fullText)
        {
            var checkDoc = new DocumentDataDTO(){
                Hash = fullText.GetSHA256Hash()
            };
            return _documentDb.DocumentExists(checkDoc);
        }


    }

}