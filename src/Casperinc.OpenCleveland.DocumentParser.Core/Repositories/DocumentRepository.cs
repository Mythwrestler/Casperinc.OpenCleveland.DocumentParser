using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Casperinc.OpenCleveland.DocumentParser.Bridge.Data;
using Casperinc.OpenCleveland.DocumentParser.Core.Helpers;
using Casperinc.OpenCleveland.DocumentParser.Core.Models;

namespace Casperinc.OpenCleveland.DocumentParser.Core.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private IDocumentSource _documentDb;

        public DocumentRepository(IDocumentSource documentDb) 
        {
            _documentDb = documentDb;
        }

        public Document CreateDocumentFromFile(string path)
        {
            var lines = GetAllLinesForText(path);
            var fullText = String.Join("\r\n", lines);
            var wordMaps = BuildWordMaps(fullText, lines);
            return new Document()
            {
                FullText = fullText,
                Hash = fullText.GetHashCode(),
                WordMaps = wordMaps
            };
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

        private List<WordMap> BuildWordMaps(string fullText, List<string> lines)
        {

            var wordMaps = new List<WordMap>();

            var text = new List<string>();
            var wordPostions = new List<int>();
            foreach (var line in lines)
            {
                text.AddRange(line.Split(' '));
            }

            var chars = new char[] {
                    ',','.','\'','\"',')','(','-'
                };

            var wordList = text.Select(word => word.ToLowerInvariant().Trim(chars))
                                .Where(word => word.Length >= 2)
                                .Distinct();

            foreach (var reviewWord in wordList)
            {
                
                var positions = fullText.ToLowerInvariant().AllPositions(reviewWord);
                wordMaps.Add(
                    new WordMap()
                    {
                        Word = new Word(){Value = reviewWord},
                        Positions = positions,
                        NeighborWords = new NeighborWords()
                    }
                    );
            }
            return wordMaps;
        }

    }

}