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

        /// <summary>
        /// 取得所有輸入的配送資料
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ShippingInformation>> GetShippingInformation()
        {
            IEnumerable<ShippingInformation> resutlt = new ShippingInformation[0];

            string cmd = $@"select * 
                            from [ExampleDB].[dbo].[ShippingInformation](NOLOCK)
                            where ShippingInformation_IsDeleted = 0";

            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                resutlt = await sqlConnection.QueryAsync<ShippingInformation>(cmd);
            }

            return resutlt;
        }


        /// <summary>
        /// 取得所有配送標籤資料
        /// </summary>
        /// <returns></returns>
        public async Task<ShippingLabel> GetShippingLabel(string originalTrackingNumber)
        {
            ShippingLabel? resutlt = new ShippingLabel();

            string cmd = $@"select * 
                            from [ExampleDB].[dbo].[ShippingLabel](NOLOCK)
                            where ShippingLabel_IsDeleted = 0
                            and ShippingLabel_ShippingOriginalTrackingNumber = @originalTrackingNumber";

            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                resutlt = await sqlConnection.QueryFirstOrDefault(cmd, new { originalTrackingNumber }) ;
                
            }

            return resutlt;
        }

        /// <summary>
        /// 新增task
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public async Task<long> InsertTask(TaskObject task)
        {
            long taskId = 0;
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

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
                                                         SELECT SCOPE_IDENTITY()", task);
            }

            return taskId;
        }

        /// <summary>
        /// 新增taskSlave
        /// </summary>
        /// <param name="taskSlave"></param>
        /// <returns></returns>
        public async Task<long> InsertTaskSlave(TaskSlave taskSlave)
        {
            long taskSlaveId = 0;
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                taskSlaveId = await connection.ExecuteScalarAsync<long>(@"INSERT INTO [dbo].[TaskSlave]
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
                                                                            SELECT SCOPE_IDENTITY()", taskSlave);
            }

            return taskSlaveId;
        }

        public async Task<long> InsertShippingLabel(ShippingLabel shippingLabel)
        {
            long shippingLabelId = 0;
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                shippingLabelId = await connection.ExecuteScalarAsync<long>(@"INSERT INTO [dbo].[ShippingLabel]
                                                                                   ([ShippingLabel_BarCode]
                                                                                   ,[ShippingLabel_SalesOffice]
                                                                                   ,[ShippingLabel_ZipCode]
                                                                                   ,[ShippingLabel_ZipCodeVersion]
                                                                                   ,[ShippingLabel_PakageSize]
                                                                                   ,[ShippingLabel_CreatedDateTime]
                                                                                   ,[ShippingLabel_EstimatedDeliveryDateTime]
                                                                                   ,[ShippingLabel_CustomID]
                                                                                   ,[ShippingLabel_ShippingOriginalTrackingNumber]
                                                                                   ,[ShippingLabel_ShippingCollectedMoney]
                                                                                   ,[ShippingLabel_ShippingRecipientName]
                                                                                   ,[ShippingLabel_ShippingRecipientAddress]
                                                                                   ,[ShippingLabel_ShippingRecipientPhoneNumber]
                                                                                   ,[ShippingLabel_ShippingSenderName]
                                                                                   ,[ShippingLabel_ShippingSenderAddress]
                                                                                   ,[ShippingLabel_ShippingSenderPhoneNumber]
                                                                                   ,[ShippingLabel_ShippingSenderCompany]
                                                                                   ,[ShippingLabel_CreatedUser])
                                                                             VALUES
                                                                                   (@ShippingLabel_BarCode
                                                                                   ,@ShippingLabel_SalesOffice
                                                                                   ,@ShippingLabel_ZipCode
                                                                                   ,@ShippingLabel_ZipCodeVersion
                                                                                   ,@ShippingLabel_PakageSize
                                                                                   ,@ShippingLabel_CreatedDateTime
                                                                                   ,@ShippingLabel_EstimatedDeliveryDateTime
                                                                                   ,@ShippingLabel_CustomID
                                                                                   ,@ShippingLabel_ShippingOriginalTrackingNumber
                                                                                   ,@ShippingLabel_ShippingCollectedMoney
                                                                                   ,@ShippingLabel_ShippingRecipientName
                                                                                   ,@ShippingLabel_ShippingRecipientAddress
                                                                                   ,@ShippingLabel_ShippingRecipientPhoneNumber
                                                                                   ,@ShippingLabel_ShippingSenderName
                                                                                   ,@ShippingLabel_ShippingSenderAddress
                                                                                   ,@ShippingLabel_ShippingSenderPhoneNumber
                                                                                   ,@ShippingLabel_ShippingSenderCompany
                                                                                   ,@ShippingLabel_CreatedUser)
                                                                                    SELECT SCOPE_IDENTITY()", shippingLabel);
            }

            return shippingLabelId;
        }

        public async Task<long> InsertShippingInformation(ShippingInformation shippingInformation)
        {

            long shippingInformationId = 0;
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                shippingInformationId = await connection.ExecuteScalarAsync<long>(@"INSERT INTO [dbo].[ShippingInformation]
                                                                                       ([ShippingInformation_TrackingNumber]
                                                                                       ,[ShippingInformation_SlaveTrackingNumber]
                                                                                       ,[ShippingInformation_OriginalTrackingNumber]
                                                                                       ,[ShippingInformation_OriginalTrackingNumber2]
                                                                                       ,[ShippingInformation_OriginalTrackingNumber3]
                                                                                       ,[ShippingInformation_UnionTrackingNumber]
                                                                                       ,[ShippingInformation_UnionContractId]
                                                                                       ,[ShippingInformation_TotalCount]
                                                                                       ,[ShippingInformation_Weight]
                                                                                       ,[ShippingInformation_ProductName]
                                                                                       ,[ShippingInformation_CollectedMoney]
                                                                                       ,[ShippingInformation_Tax]
                                                                                       ,[ShippingInformation_CollectedFee]
                                                                                       ,[ShippingInformation_SiteId]
                                                                                       ,[ShippingInformation_SiteName]
                                                                                       ,[ShippingInformation_RecipientCompany]
                                                                                       ,[ShippingInformation_RecipientName]
                                                                                       ,[ShippingInformation_RecipientAddress]
                                                                                       ,[ShippingInformation_RecipientPhoneNumber]
                                                                                       ,[ShippingInformation_SenderPhoneNumber]
                                                                                       ,[ShippingInformation_SenderCompany]
                                                                                       ,[ShippingInformation_SenderName]
                                                                                       ,[ShippingInformation_SenderAddress]
                                                                                       ,[ShippingInformation_Memo]
                                                                                       ,[ShippingInformation_Status]
                                                                                       ,[ShippingInformation_CreatedDateTime]
                                                                                       ,[ShippingInformation_StatusUpdatedDateTime]
                                                                                       ,[ShippingInformation_CreatedUser]
                                                                                       ,[ShippingInformation_UpdatedUser]
                                                                                       ,[ShippingInformation_IsDeleted])
                                                                                 VALUES
                                                                                       (@ShippingInformation_TrackingNumber
                                                                                       ,@ShippingInformation_SlaveTrackingNumber
                                                                                       ,@ShippingInformation_OriginalTrackingNumber
                                                                                       ,@ShippingInformation_OriginalTrackingNumber2
                                                                                       ,@ShippingInformation_OriginalTrackingNumber3
                                                                                       ,@ShippingInformation_UnionTrackingNumber
                                                                                       ,@ShippingInformation_UnionContractId
                                                                                       ,@ShippingInformation_TotalCount
                                                                                       ,@ShippingInformation_Weight
                                                                                       ,@ShippingInformation_ProductName
                                                                                       ,@ShippingInformation_CollectedMoney
                                                                                       ,@ShippingInformation_Tax
                                                                                       ,@ShippingInformation_CollectedFee
                                                                                       ,@ShippingInformation_SiteId
                                                                                       ,@ShippingInformation_SiteName
                                                                                       ,@ShippingInformation_RecipientCompany
                                                                                       ,@ShippingInformation_RecipientName
                                                                                       ,@ShippingInformation_RecipientAddress
                                                                                       ,@ShippingInformation_RecipientPhoneNumber
                                                                                       ,@ShippingInformation_SenderPhoneNumber
                                                                                       ,@ShippingInformation_SenderCompany
                                                                                       ,@ShippingInformation_SenderName
                                                                                       ,@ShippingInformation_SenderAddress
                                                                                       ,@ShippingInformation_Memo
                                                                                       ,@ShippingInformation_Status
                                                                                       ,@ShippingInformation_CreatedDateTime
                                                                                       ,@ShippingInformation_StatusUpdatedDateTime
                                                                                       ,@ShippingInformation_CreatedUser
                                                                                       ,@ShippingInformation_UpdatedUser
                                                                                       ,@ShippingInformation_IsDeleted)
                                                                                    SELECT SCOPE_IDENTITY()", shippingInformation);

            }

            return shippingInformationId;
        }

        public async Task<bool> UpdateShippingInformationStatus(string status, long id)
        {
            int effectiveRows = 0;
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    try 
                    {
                        effectiveRows = await connection.ExecuteScalarAsync<int>(@"Update [dbo].[ShippingInformation]
                                                               Set [ShippingInformation_Status] = @status
                                                               ,[ShippingInformation_StatusUpdatedDateTime] = @updatedTime
                                                                where ShippingInformation_Id = @id;
                                                         SELECT @@ROWCOUNT", new { status, id, updatedTime = DateTime.UtcNow.GetTWTime() }, tran);
                        tran.Commit();
                    }
                    catch (Exception ex) 
                    {
                        var err = ex.ToString();
                        //Todo: add log
                        tran.Rollback();
                    }                   
                }
            }

            return effectiveRows == 1;
        }

        public async Task<bool> UpdateTaskStatus(string status, long id)
        {
            int effectiveRows = 0;
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    try
                    {
                        effectiveRows = await connection.ExecuteScalarAsync<int>(@"Update [dbo].[Task]
                                                               Set [Task_Status] = @status
                                                               ,[Task_StatusUpdatedDateTime] = @updatedTime
                                                                where Task_Id = @id;
                                                         SELECT @@ROWCOUNT", new { status, id, updatedTime = DateTime.UtcNow.GetTWTime() }, tran);
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        var err = ex.ToString();
                        //Todo: add log
                        tran.Rollback();
                    }
                }
            }

            return effectiveRows == 1;
        }

        public async Task<bool> UpdateTaskSlaveStatus(string status, long id)
        {
            int effectiveRows = 0;
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    try
                    {
                        effectiveRows = await connection.ExecuteScalarAsync<int>(@"Update [dbo].[TaskSlave]
                                                               Set [TaskSlave_Status] = @status
                                                               ,[TaskSlave_StatusUpdatedDateTime] = @updatedTime
                                                                where TaskSlave_Id = @id;
                                                         SELECT @@ROWCOUNT", new { status, id, updatedTime = DateTime.UtcNow.GetTWTime() }, tran);
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        var err = ex.ToString();
                        //Todo: add log
                        tran.Rollback();
                    }
                }
            }

            return effectiveRows == 1;
        }
    }
}
