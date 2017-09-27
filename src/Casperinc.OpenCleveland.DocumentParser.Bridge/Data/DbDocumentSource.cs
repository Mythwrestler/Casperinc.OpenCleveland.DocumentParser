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

            using(var dbConnection = GetConnection())
            {
                var hashCheck = dbConnection.Query<bool>("select true from `Documents` where `Hash Value` = @hashValue",
                                                new {hashValue = documentHash});
                if(hashCheck.Count() > 0)
                {
                    dbConnection.Close();
                    return true;
                }
                dbConnection.Close();
            }
            
            return false;
        }

        public bool DocumentExistsGetGuid(DocumentDataDTO document)
        {
            using(var dbConnection = GetConnection())
            {
                var hashCheck = dbConnection.Query<Guid>("select `Unique Id GUID` from `Documents` where `Hash Value` = @hashValue",
                                                new {hashValue = document.Hash});
                if(hashCheck.Count() > 0)
                {
                    document.GuidId = hashCheck.FirstOrDefault();
                    dbConnection.Close();
                    return true;
                }
                dbConnection.Close();
            }
            return false;
        }


        public IEnumerable<DocumentDataDTO> SaveDocument(DocumentDataDTO document)
        {
             using(var dbConnection = GetConnection())
            {   
                var parms = new DynamicParameters();
                parms.Add("guid", document.GuidId, DbType.StringFixedLength, ParameterDirection.InputOutput);
                parms.Add("hashValue", document.GuidId, DbType.StringFixedLength, ParameterDirection.Input);
                parms.Add("fullText", document.GuidId, DbType.String, ParameterDirection.Input);

                var test = dbConnection.Query<DocumentDataDTO>("CreateDocument", parms, commandType: CommandType.StoredProcedure);
            }
            throw new NotImplementedException();
        }

    }
        
}
