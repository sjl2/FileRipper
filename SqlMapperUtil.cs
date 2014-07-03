using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using DapperORM;

namespace FileRipper
{


    public static class SqlMapperUtil
    {

        private static string connectionString = ConfigurationManager.ConnectionStrings["Fitzway"].ConnectionString;



        public static SqlConnection GetOpenConnection()
        {

            var connection = new SqlConnection(connectionString);

            connection.Open();

            return connection;

        }



        /// <summary>

        /// Stored proc.

        /// </summary>

        /// <typeparam name="T"></typeparam>

        /// <param name="procname">The procname.</param>

        /// <param name="parms">The parms.</param>

        /// <returns></returns>

        public static List<T> StoredProcWithParams<T>(string procname, dynamic parms)
        {

            using (SqlConnection connection = GetOpenConnection())
            {

                return connection.Query<T>(procname, (object)parms, commandType: CommandType.StoredProcedure).ToList();

            }

        }





        /// <summary>

        /// SQL with params.

        /// </summary>

        /// <typeparam name="T"></typeparam>

        /// <param name="sql">The SQL.</param>

        /// <param name="parms">The parms.</param>

        /// <returns></returns>

        public static List<T> SqlWithParams<T>(string sql, dynamic parms)
        {

            using (SqlConnection connection = GetOpenConnection())
            {

                return connection.Query<T>(sql, (object)parms).ToList();

            }

        }



        /// <summary>

        /// Insert update or delete SQL.

        /// </summary>

        /// <param name="sql">The SQL.</param>

        /// <param name="parms">The parms.</param>

        /// <returns></returns>

        public static int InsertUpdateOrDeleteSql(string sql, dynamic parms)
        {

            using (SqlConnection connection = GetOpenConnection())
            {

                return connection.Execute(sql, (object)parms);

            }

        }



        /// <summary>

        /// Insert update or delete stored proc.

        /// </summary>

        /// <param name="procName">Name of the proc.</param>

        /// <param name="parms">The parms.</param>

        /// <returns></returns>

        public static int InsertUpdateOrDeleteStoredProc(string procName, dynamic parms)
        {

            using (SqlConnection connection = GetOpenConnection())
            {

                return connection.Execute(procName, (object)parms, commandType: CommandType.StoredProcedure);

            }

        }



    }

 

}
