using System;
using System.Data.SqlClient;

namespace DeliverySystem.Variables.Repository
{
    /// <summary>
    /// 資料庫連線資訊
    /// </summary>
    public class SqlConnectionInfo
    {
        /// <summary>
        /// 資料庫IP位址
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        /// 來源應用程式名稱
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// 資料庫名稱
        /// </summary>
        public string InitialCatalog { get; set; }

        /// <summary>
        /// 取得連線字串
        /// </summary>
        /// <returns></returns>
        public string GetSqlConnectionStringBuilder()
        {
            var result = $"Data Source={DataSource};Initial Catalog={InitialCatalog};Persist Security Info=True;User ID=sa;Password=Feel1227;Application Name=ApplicationName;MultiSubnetFailover=Yes;MultipleActiveResultSets=true;TrustServerCertificate=True";
            Console.WriteLine($"DB {InitialCatalog} : {DataSource}");
            return result;
        }
    }
}
