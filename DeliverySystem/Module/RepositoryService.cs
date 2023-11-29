using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using DeliverySystem.Interface;
using DeliverySystem.Variables.Repository;
using Dapper;
using System.Data.SqlClient;

namespace DeliverySystem.Module
{
    public class RepositoryService : IRepositoryOperater
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString"></param>
        public RepositoryService(string connectionString)
        {
            ConnectionString = connectionString;
        }

        private string ConnectionString;

        public async Task<IEnumerable<ShippingInformation>> GetShippingInformation()
        {
            IEnumerable<ShippingInformation> resutlt = new ShippingInformation[0];

            string cmd = $@"select * 
                            from [ExampleDB].[dbo].[ShippingInformation]";

            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                resutlt = await sqlConnection.QueryAsync<ShippingInformation>(cmd);
            }

            return resutlt;
        }
    }
}
