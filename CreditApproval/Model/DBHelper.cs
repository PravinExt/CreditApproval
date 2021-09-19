using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Web;

namespace ApplicationSubmission.Model
{
    public class DBHelper
    {
        MySqlConnection _connection = null;
        MySqlCommand _sqlcommand = null;
        MySqlDataAdapter _adapter = null;
        MySqlDataReader _reader = null;

        string _sLogFilePath = string.Empty;

        /// <summary>
        /// Constructor of the class 
        /// </summary>
        /// <param name="ConnectionString">Connecction string to initize connection string for connection</param>
        public DBHelper()
        {
            //string appPath = HttpContext.Current.Request.ApplicationPath;
            //string physicalPath = HttpContext.Current.Request.MapPath(appPath);
            //_sLogFilePath = physicalPath + "\\Logs";

            //Setting the connection string.
            //_logger.WriteLog("Initilizing the DBHelper class", "DB Helper Constructor", Intsol.Utilities.Logger.LogType.Information);
        }

        public string GetConnStr()
        {
            string sConnectionString = "server = applicationsubmission.cikv7fwlsku8.ap-south-1.rds.amazonaws.com; port=3306; uid=admin; pwd=admin8910; database=CreditApproval";
            return sConnectionString;
        }

        /// <summary>
        /// Query type to process the query
        /// Query type may be text or stored procedure.
        /// </summary>
        public enum QueryType
        {
            Text,
            StotedProcedure
        }

        /// <summary>
        /// To connect(Open connection for) the SQL database securely through connection string.
        /// </summary>
        public void Connect(string ConnectionString)
        {
            _connection = new MySqlConnection(ConnectionString);
            try
            {
                _connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public MySqlTransaction ConnectTran(string ConnectionString,MySqlTransaction sqlTran)
        {
            _connection = new MySqlConnection(ConnectionString);
            try
            {
                _connection.Open();
                sqlTran = _connection.BeginTransaction();
                return sqlTran;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// To disconnect or close the connection which is already opened.
        /// </summary>
        public void DisConnect()
        {
            try
            {
                if (_connection != null && _connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                    _connection.Dispose();
                    _connection = null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_connection != null && _connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                    _connection.Dispose();
                    _connection = null;
                }
            }
        }

        /// <summary>
        /// To fire the given query on database and get the returned data from query
        /// </summary>
        /// <param name="Query">Query text which you want to fire on database(may be simple query text or stored proc name)</param>
        /// <param name="queryType">Which type of query is text or stored procedure.</param>
        /// <param name="CommandParamtres">Parameter list required to execute the specified query on server</param>
        /// <returns>Returns the dataset retured by the fired query.</returns>
        public DataSet ExecuteDS(string Query, QueryType queryType, MySqlParameter[] CommandParamtres)
        {
            DataSet dsData = null;
            MySqlCommand selectcommand = new MySqlCommand();

            try
            {
                selectcommand.Connection = _connection;

                //set command type
                if (queryType == QueryType.Text)
                {
                    selectcommand.CommandType = CommandType.Text;
                }
                else
                {
                    selectcommand.CommandType = CommandType.StoredProcedure;
                }

                selectcommand.CommandText = Query;

                //adding paramters

                if (CommandParamtres != null)
                {
                    for (int i = 0; i < CommandParamtres.Length; i++)
                    {
                        selectcommand.Parameters.AddWithValue(CommandParamtres[i].ParameterName, CommandParamtres[i].Value);
                    }
                }
                
                //adding commnad to adapter
                _adapter = new MySqlDataAdapter();
                _adapter.SelectCommand = selectcommand;
                dsData = new DataSet();
                _adapter.Fill(dsData);

                return dsData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// To fire the given query on database and get the number of records affetcted with the given query
        /// </summary>
        /// <param name="Query">Query text which you want to fire on database(may be simple query text or stored proc name)</param>
        /// <param name="queryType">Which type of query is text or stored procedure.</param>
        /// <param name="CommandParamtres">Parameter list required to execute the specified query on server</param>
        /// <returns>returns integer value which is the no of records affcted by fired query</returns>
        public int Execute(string Query, QueryType queryType, MySqlParameter[] CommandParamtres)
        {
            //No of records affected by execute command -1,0, >0
            //-1 : Insert/update/delete statement not executed
            // 0 : Insert/update/delete statement executed but 0 records affected.
            //>0 : Records inserted/updated/deleted Successfully
            int iReturn = 0;

            MySqlCommand sqlCommand = new MySqlCommand();

            try
            {
                sqlCommand.Connection = _connection;

                //set command type
                if (queryType == QueryType.Text)
                {
                    sqlCommand.CommandType = CommandType.Text;
                }
                else
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                }

                sqlCommand.CommandText = Query;

                //adding paramters
                if (CommandParamtres != null)
                {
                    for (int i = 0; i < CommandParamtres.Length; i++)
                    {
                        sqlCommand.Parameters.AddWithValue(CommandParamtres[i].ParameterName, CommandParamtres[i].Value);
                    }
                }

                iReturn = sqlCommand.ExecuteNonQuery();

                return iReturn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public MySqlTransaction ExecuteTran(string Query, QueryType queryType, MySqlParameter[]CommandParamtres,MySqlTransaction sqlTran)
        {
            //No of records affected by execute command -1,0, >0
            //-1 : Insert/update/delete statement not executed
            // 0 : Insert/update/delete statement executed but 0 records affected.
            //>0 : Records inserted/updated/deleted succesfully
            int iReturn = 0;

            MySqlCommand sqlCommand = new MySqlCommand();

            try
            {
                sqlCommand.Connection = _connection;
                sqlCommand.Transaction = sqlTran;
                //set command type
                if (queryType == QueryType.Text)
                {
                    sqlCommand.CommandType = CommandType.Text;

                }
                else
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                }

                sqlCommand.CommandText = Query;

                //adding paramters
                if (CommandParamtres != null)
                {
                    for (int i = 0; i < CommandParamtres.Length; i++)
                    {
                        sqlCommand.Parameters.AddWithValue(CommandParamtres[i].ParameterName, CommandParamtres[i].Value);
                    }
                }

                iReturn = sqlCommand.ExecuteNonQuery();

                return sqlTran;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// To fire the given query on database and get the specific data item returned from query
        /// </summary>
        /// <param name="Query">Query text which you want to fire on database(may be simple query text or stored proc name)</param>
        /// <param name="queryType">Which type of query is text or stored procedure.</param>
        /// <param name="CommandParamtres">Parameter list required to execute the specified query on server</param>
        /// <returns>returns object as the specific data item returned from query</returns>
        public object ExecuteScalar(string Query, QueryType queryType, MySqlParameter[] CommandParamtres)
        {
            object objReturn = null;



            MySqlCommand selectcommand = new MySqlCommand();

            try
            {
                selectcommand.Connection = _connection;

                //set command type
                if (queryType == QueryType.Text)
                {
                    selectcommand.CommandType = CommandType.Text;

                }
                else
                {
                    selectcommand.CommandType = CommandType.StoredProcedure;
                }

                selectcommand.CommandText = Query;

                //adding paramters

                if (CommandParamtres != null)
                {
                    for (int i = 0; i < CommandParamtres.Length; i++)
                    {
                        selectcommand.Parameters.AddWithValue(CommandParamtres[i].ParameterName, CommandParamtres[i].Value);
                    }
                }

                objReturn = selectcommand.ExecuteScalar();

                return objReturn;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// To fire the given query on database and get the sql database reader object to perform operation 
        /// </summary>
        /// <param name="Query">Query text which you want to fire on database(may be simple query text or stored proc name)</param>
        /// <param name="queryType">Which type of query is text or stored procedure.</param>
        /// <param name="CommandParamtres">Parameter list required to execute the specified query on server</param>
        /// <returns>returns sql reader object as the specific data item returned from query</returns>
        public MySqlDataReader ExecuteReader(string Query, QueryType queryType, MySqlParameter[] CommandParamtres)
        {
            MySqlDataReader reader = null;

            MySqlCommand selectcommand = new MySqlCommand();

            try
            {
                selectcommand.Connection = _connection;

                //set command type
                if (queryType == QueryType.Text)
                {
                    selectcommand.CommandType = CommandType.Text;
                }
                else
                {
                    selectcommand.CommandType = CommandType.StoredProcedure;
                }

                selectcommand.CommandText = Query;
                selectcommand.CommandTimeout = 60;

                //adding paramters

                if (CommandParamtres != null)
                {
                    for (int i = 0; i < CommandParamtres.Length; i++)
                    {
                        selectcommand.Parameters.AddWithValue(CommandParamtres[i].ParameterName, CommandParamtres[i].Value);
                    }
                }

                reader = selectcommand.ExecuteReader();

                return reader;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static object checkNullString(object ob)
        {
            if (!string.IsNullOrEmpty((string)ob))
            {
                return ob;
            }
            else
            {
                return DBNull.Value;
            }
        }

        public static object checkNullParam(object ob)
        {

            if (ob != null)
            {
                return ob;
            }
            else
            {
                return DBNull.Value;
            }

        }

        public static bool ColumnExists(System.Data.IDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}