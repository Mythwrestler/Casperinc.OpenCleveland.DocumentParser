using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Casperinc.OpenCleveland.DocumentParser.Bridge.Data.Models;
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
                dbConnection.Open();
                try
                {

                    var hashCheck = dbConnection.Query<Guid>("select `Unique Id GUID` from `Documents` where `Hash Value` = @hashValue",
                                                    new { hashValue = document.Hash });
                    if (hashCheck.Count() > 0)
                    {
                        document.GuidId = hashCheck.FirstOrDefault();
                        dbConnection.Close();
                        return true;
                    }
                    dbConnection.Close();
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

        public DocumentDataDTO SaveDocument(DocumentDataDTO document)
        {

            if (String.IsNullOrWhiteSpace(document.Hash)) throw new ArgumentOutOfRangeException("Cannot save document without valid Hash Integer Value.");
            if (document.GuidId == null || document.GuidId == new Guid()) throw new ArgumentOutOfRangeException("Cannot save document without valid Guid.");
            if (String.IsNullOrWhiteSpace(document.FullText)) throw new ArgumentOutOfRangeException("Cannot save document without valid document text.");

            using (var dbConnection = GetConnection())
            {
                try
                {
                    dbConnection.Open();

                    var parms = new DynamicParameters();
                    parms.Add("guid", document.GuidId, DbType.StringFixedLength, ParameterDirection.InputOutput);
                    parms.Add("hashValue", document.Hash, DbType.StringFixedLength, ParameterDirection.Input);
                    parms.Add("allText", document.FullText, DbType.String, ParameterDirection.Input);

                    var savedDocument = dbConnection.Query<DocumentDataDTO>("CreateDocument", parms, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    
                    dbConnection.Close();
                    return savedDocument;
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


        // Private Methods
        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(_dbConnectionString);
        }

    }
}
