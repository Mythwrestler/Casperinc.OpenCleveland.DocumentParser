using System;
using System.Collections.Generic;
using Casperinc.OpenCleveland.DocumentParser.Bridge.Models;

namespace Casperinc.OpenCleveland.DocumentParser.Bridge.Data
{
    public interface IDocumentSource
    {
        bool DocumentExists(DocumentDataDTO document);
        bool SaveDocument(DocumentDataDTO document);
        // bool SaveWords(List<WordDataDTO> newWordList);
        // bool SaveWord(WordDataDTO newWord);
        // bool SaveWordMap(WordMapDataDTO newWordMap);
        // bool SaveWordPositions(List<int> wordPostions);

    } 
}
