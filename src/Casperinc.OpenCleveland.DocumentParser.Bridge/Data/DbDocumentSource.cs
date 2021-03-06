using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Casperinc.OpenCleveland.DocumentParser.Bridge.Models;
using Dapper;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Numerics;
using Casperinc.OpenCleveland.DocumentParser.Bridge.Helpers;

namespace Casperinc.OpenCleveland.DocumentParser.Bridge.Data
{
    public class DbDocumentSource : IDocumentSource
    {

        private string _dbConnectionString;
        private ILoggerFactory _logger;



        // Interface Methods
        public DbDocumentSource(string dbConnectionString, ILoggerFactory logger)
        {
            _dbConnectionString = dbConnectionString;
            _logger = logger;
        }

        public bool DocumentExists(DocumentDataDTO document)
        {
            if (String.IsNullOrWhiteSpace(document.Hash)) throw new ArgumentOutOfRangeException("Hash Integer Value needed for Search.");
            using (var dbConnection = GetConnection())
            {
                try
                {
                    dbConnection.Open();

                    var guidFromHashValue = dbConnection.Query<Guid>(
                            @"Select uniqueIdGuid 
                                From Documents_TBL 
                               Where hashValue = @hashValue",
                        new { hashValue = document.Hash }
                    ).FirstOrDefault();

                    if (guidFromHashValue != null && guidFromHashValue != new Guid())
                    {
                        document.GuidId = guidFromHashValue;
                        //if (WordMapsExist(dbConnection, document))
                        //{
                            return true;
                        //}
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to determine if document existed.", ex);
                }
                finally
                {
                    dbConnection.Close();
                }
            }
            return false;
        }

        private bool WordMapsExist(MySqlConnection dbConnection, DocumentDataDTO newDocument)
        {
            try
            {
                var queryString =
                    @"Select a.uniqueIdGuid as GuidId
                            ,b.uniqueIdGuid as GuidId
                            ,b.wordText as Text
                            ,c.positionInDocument as Positions
                        From WordMaps_TBL as a
                        Join Words_TBL as b 
                          On a.wordIdNumeric = b.uniqueIdNumeric
                        Join WordsMapPositions_TBL as c 
                          On a.uniqueIdGuid = c.wordMapIdNumeric   
                       where documentIdNumeric = @documentId";   //this is wrong, it needs to point to document Guid.  also wordMapId join is incorrect.

                var wordMapsFromSource = dbConnection.Query<WordMapDataDTO>(
                    queryString,
                    new { documentId = newDocument.GuidId }
                ).ToList();

                if (wordMapsFromSource.Count() != 0)
                {
                    newDocument.WordMaps = wordMapsFromSource;
                    return true;
                }
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Failed to determine if document existed.", ex);
            }
            finally
            {
                dbConnection.Close();
            }

        }

        private bool WordExists(MySqlConnection dbConnection, WordDataDTO word)
        {
            try
            {
                var wordCheck = dbConnection.Query<Guid>(
                            @"Select uniqueIdGuid
                                From ParsedDocuments.Words_TBL
                               Where wordText = @word",
                               new { word = word.Text.ToLowerInvariant() }
                        ).FirstOrDefault();

                if (wordCheck != null && wordCheck != new Guid())
                {
                    word.GuidId = wordCheck;
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save Word Positions", ex);
            }

            return false;
        }

        public bool SaveDocument(DocumentDataDTO newDocument)
        {

            if (String.IsNullOrWhiteSpace(newDocument.Hash)) throw new ArgumentOutOfRangeException("Cannot save document without valid Hash Integer Value.");
            if (newDocument.GuidId == null || newDocument.GuidId == new Guid()) throw new ArgumentOutOfRangeException("Cannot save document without valid Guid.");
            if (String.IsNullOrWhiteSpace(newDocument.FullText)) throw new ArgumentOutOfRangeException("Cannot save document without valid document text.");

            using (var dbConnection = GetConnection())
            {
                try
                {
                    dbConnection.Open();

                    // var parms = new DynamicParameters();
                    // parms.Add("guid", newDocument.GuidId, DbType.StringFixedLength, ParameterDirection.InputOutput);
                    // parms.Add("hashValue", newDocument.Hash, DbType.StringFixedLength, ParameterDirection.Input);
                    // parms.Add("allText", newDocument.FullText, DbType.String, ParameterDirection.Input);

                    // var savedDocument = dbConnection.Query<DocumentDataDTO>("CreateDocument", parms, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    // savedDocument.WordMaps = newDocument.WordMaps;
                    var tempGuid = Guid.NewGuid();

                    var sqlStatement =
                        @"
                            Insert Into ParsedDocuments.Documents_TBL
                                   (uniqueIdGuid, hashValue, fullDocumentText)
                            Values (@guid, @hash, @documentText)
                        ";

                    var result = dbConnection.Execute(
                        sqlStatement,
                        new { guid = tempGuid, hash = newDocument.Hash, documentText = newDocument.FullText }
                    );


                    if (result >= 0)
                    {
                        newDocument.GuidId = tempGuid;
                        if (SaveWordMaps(dbConnection, newDocument))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }

                    }
                    else
                    {
                        return false;
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to save Document.", ex);
                }
                finally
                {
                    dbConnection.Close();
                }

            }


        }

        private bool SaveWordMaps(MySqlConnection dbConnection, DocumentDataDTO newDocument)
        {
            // if (newDocument.GuidId == null || newDocument.GuidId == new Guid()) throw new ArgumentOutOfRangeException("Cannot save WordMap without valid WordMap Guid.");
            // if (newWordMap.Word.GuidId == null || newWordMap.Word.GuidId == new Guid()) throw new ArgumentOutOfRangeException("Cannot save WordMap without Word Guid.");
            // if (newWordMap.Positions.Count == 0) throw new ArgumentOutOfRangeException("Cannot save document without valid positions.");
            try
            {

                foreach (var wordMap in newDocument.WordMaps)
                {
                    // Check for / Create word
                    if (!WordExists(dbConnection, wordMap.Word))
                    {
                        wordMap.Word.GuidId = Guid.NewGuid();
                        if (!SaveWord(dbConnection, wordMap.Word))
                        {
                            return false;
                        }
                    }

                    // Create Word Map
                    // var parms = new DynamicParameters();
                    // parms.Add("guid", Guid.NewGuid(), DbType.StringFixedLength, ParameterDirection.InputOutput);
                    // parms.Add("wordGUID", wordMap.Word.GuidId, DbType.StringFixedLength, ParameterDirection.InputOutput);
                    // parms.Add("documentGUID", newDocument.GuidId, DbType.StringFixedLength, ParameterDirection.Input);
                    // var savedWordMap = dbConnection.Query<WordMapDataDTO>(
                    //           "CreateWordMap", parms, commandType: CommandType.StoredProcedure
                    //       ).FirstOrDefault();

                    var tempGuid = Guid.NewGuid();

                    var wordIdInt = dbConnection.Query<Int64>(
                        @"Select uniqueIdNumeric 
                            From ParsedDocuments.Words_TBL 
                           Where uniqueIdGuid = @wordGUID",
                        new { wordGUID = wordMap.Word.GuidId }).FirstOrDefault();

                    var documentIdInt = dbConnection.Query<Int64>(
                        @"Select uniqueIdNumeric 
                            From ParsedDocuments.Documents_TBL 
                           Where uniqueIdGuid = @documentGUID",
                        new { documentGUID = newDocument.GuidId }).FirstOrDefault();

                    if (documentIdInt != 0 && wordIdInt != 0)
                    {
                        var result = dbConnection.Execute(
                            @"Insert into ParsedDocuments.WordMaps_TBL
				                 (uniqueIdGuid, documentIdNumeric, wordIdNumeric)
			              Values (@guid, @documentId, @wordId)",
                            new
                            {
                                guid = tempGuid,
                                documentId = documentIdInt,
                                wordId = wordIdInt
                            });

                        if (result != 0)
                        {
                            wordMap.GuidId = tempGuid;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }


                    var wordMapIdInt = dbConnection.Query<Int64>(
                        @"Select uniqueIdNumeric
                                From ParsedDocuments.WordMaps_TBL
                               Where uniqueIdGuid = @guidId",
                        new { guidId = wordMap.GuidId }
                    ).FirstOrDefault();

                    if (wordMapIdInt == 0)
                    {
                        return false;
                    }

                    // save wordmap positions
                    if (!SaveWordPositions(dbConnection, wordMapIdInt, wordMap.Positions))
                    {
                        return false;
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to save Document.", ex);
            }

            return true;
        }

        private bool SaveWord(MySqlConnection dbConnection, WordDataDTO word)
        {
            try
            {
                if (!WordExists(dbConnection, word))
                {
                    dbConnection.Execute(
                        @"insert into ParsedDocuments.Words_TBL
			                     (uniqueIdGuid, wordText)
		                  Values (@guid, @word)",
                        new { guid = word.GuidId, word = word.Text }
                    );
                    if (WordExists(dbConnection, word))
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to save Document.", ex);
            }
            return false;
        }

        private bool SaveWordPositions(MySqlConnection dbConnection, Int64 wordMapId, List<int> positions)
        {
            try
            {
                foreach (var position in positions)
                {
                    var result = dbConnection.Execute(
                        @"Insert Into ParsedDocuments.WordMapPositions_TBL
                                 (wordMapIdNumeric, positionInDocument)
                          Values (@wordMapId, @wordPosition)",
                          new { wordMapId = wordMapId, wordPosition = position }
                    );
                    if (result == 0)
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save Word Positions", ex);
            }

            return true;
        }

        // Private Methods
        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(_dbConnectionString);
        }

    }
}
