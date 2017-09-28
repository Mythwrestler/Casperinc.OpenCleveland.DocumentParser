using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Casperinc.OpenCleveland.DocumentParser.Bridge.Data.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace Casperinc.OpenCleveland.DocumentParser.Bridge.Data
{
    public class DbDocumentSource : IDocumentSource
    {
        private string _dbConnectionString;

        public DbDocumentSource(string dbConnectionString)
        {
            _dbConnectionString = dbConnectionString;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(_dbConnectionString);
        }

        public bool DocumentExists(string documentHash)
        {

            using (var dbConnection = GetConnection())
            {
                var hashCheck = dbConnection.Query<bool>("select true from `Documents` where `Hash Value` = @hashValue",
                                                new { hashValue = documentHash });
                if (hashCheck.Count() > 0)
                {
                    dbConnection.Close();
                    return true;
                }
                dbConnection.Close();
            }

            return false;
        }

        public bool DocumentExists(DocumentDataDTO document)
        {
            using (var dbConnection = GetConnection())
            {
                dbConnection.Open();
                try
                {
                    var hashCheck = dbConnection.Query<Guid>("select `Unique Id GUID` from `Documents` where `Hash Value` = @hashValue",
                                                    new { hashValue = document.FullText.GetHashCode() });
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
            using (var dbConnection = GetConnection())
            {
                try
                {
                    dbConnection.Open();
                    var parms = new DynamicParameters();
                    parms.Add("guid", document.GuidId, DbType.StringFixedLength, ParameterDirection.InputOutput);
                    parms.Add("hashValue", document.FullText.GetHashCode(), DbType.StringFixedLength, ParameterDirection.Input);
                    parms.Add("fullText", document.FullText, DbType.String, ParameterDirection.Input);

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

    }
}
