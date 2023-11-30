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

        public async Task<long> InsertTask(TaskObject task, TaskSlave taskSlave)
        {
            long taskId = 0;
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    taskId = await connection.ExecuteScalarAsync<long>(@"INSERT INTO [dbo].[Task]
                                                               ([Task_Status]
                                                               ,[Task_StatusUpdatedDateTime]
                                                               ,[Task_CreatedUser]
                                                               ,[Task_CreatedDateTime]
                                                               ,[Task_UpdatedUser]
                                                               ,[Task_UpdatedDateTime]
                                                               ,[Task_IsDeleted])
                                                         VALUES
                                                               (@Task_Status
                                                               ,@Task_StatusUpdatedDateTime
                                                               ,@Task_CreatedUser
                                                               ,@Task_CreatedDateTime
                                                               ,@Task_UpdatedUser
                                                               ,@Task_UpdatedDateTime
                                                               ,@Task_IsDeleted);
                                                         SELECT SCOPE_IDENTITY()",task);
                    if (taskId <= 0)
                    {
                        tran.Rollback();
                    }

                    taskSlave.TaskSlave_TaskId = taskId;
                    var taskSlaveId = await connection.ExecuteScalarAsync<long>(@"INSERT INTO [dbo].[TaskSlave]
                                                                                       ([TaskSlave_TaskId]
                                                                                       ,[TaskSlave_Status]
                                                                                       ,[TaskSlave_StatusUpdatedDateTime]
                                                                                       ,[TaskSlave_CreatedUser]
                                                                                       ,[TaskSlave_CreatedDateTime]
                                                                                       ,[TaskSlave_UpdatedUser]
                                                                                       ,[TaskSlave_UpdatedDateTime]
                                                                                       ,[TaskSlave_Data]
                                                                                       ,[TaskSlave_ErrorMsg]
                                                                                       ,[TaskSlave_IsDeleted])
                                                                                 VALUES
                                                                                       (@TaskSlave_TaskId
                                                                                       ,@TaskSlave_Status
                                                                                       ,@TaskSlave_StatusUpdatedDateTime
                                                                                       ,@TaskSlave_CreatedUser
                                                                                       ,@TaskSlave_CreatedDateTime
                                                                                       ,@TaskSlave_UpdatedUser
                                                                                       ,@TaskSlave_UpdatedDateTime
                                                                                       ,@TaskSlave_Data
                                                                                       ,@TaskSlave_ErrorMsg
                                                                                       ,@TaskSlave_IsDeleted)
                                                                            SELECT SCOPE_IDENTITY()",taskSlave);
                    if(taskSlaveId <= 0)
                    {
                        tran.Rollback();
                    }
                    else
                    {
                        tran.Commit();
                    }
                }
            }

            return taskId;
        }
    }
}
