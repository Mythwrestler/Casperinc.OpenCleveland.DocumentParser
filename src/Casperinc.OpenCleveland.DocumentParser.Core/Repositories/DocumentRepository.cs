using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Casperinc.OpenCleveland.DocumentParser.Bridge.Data;
using Casperinc.OpenCleveland.DocumentParser.Bridge.Models;
using Casperinc.OpenCleveland.DocumentParser.Bridge.Helpers;
using Casperinc.OpenCleveland.DocumentParser.Core.Helpers;
using Casperinc.OpenCleveland.DocumentParser.Core.Models;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

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

        public async Task<Document> GetDocumentFromPath(string path)
        {
            var lines = GetAllLinesForText(path);
            var fullText = String.Join(" \n", lines);
            //var wordMaps = await GetWordMaps(fullText, lines);
            var wordMaps = new List<WordMap>();

            var documentForReturn = new Document()
                {
                    FullText = fullText,
                    Hash = fullText.GetSHA256Hash(),
                    WordMaps = wordMaps
                };
            var documentFromSource = Mapper.Map<DocumentDataDTO>(documentForReturn);

            if(_documentDb.DocumentExists(documentFromSource))
            {
                documentForReturn = Mapper.Map<Document>(documentFromSource);
            }
            else
            {
                documentForReturn.Id = Guid.NewGuid();
                documentForReturn.WordMaps = await GetWordMaps(fullText, lines);
                var documentForSave = Mapper.Map<DocumentDataDTO>(documentForReturn);

                if(!_documentDb.SaveDocument(documentForSave))
                {
                    return null;
                }
                documentForReturn = Mapper.Map<Document>(documentForSave);
            }

            return documentForReturn;

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
            var wordList = new List<string>();
            foreach (var line in lines)
            {
                wordList.AddRange(
                    Regex.Replace(
                        Regex.Replace(line, @"[^a-zA-Z-]+", " ")
                        , @"\s+", " ")
                    .Split(' ')
                );
            }


            wordList = wordList
                        .Select(word => word.Trim(chars).ToLowerInvariant())
                        .Where(word => word.Length >= 2)
                        .Distinct()
                        .OrderBy(word => word)
                        .ToList();
            
            var buildMaps = wordList
                                .Select(word => Task.Run(() => BuildWordMap(word, fullText)))
                                .ToArray();
                                

            var result = await Task.WhenAll(buildMaps);

            var wordMapList = new List<WordMap>();
            wordMapList.AddRange(result);

            return wordMapList
                    .Where(wordMap => wordMap.Positions.Count > 0)
                    .ToList();
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

    }

}