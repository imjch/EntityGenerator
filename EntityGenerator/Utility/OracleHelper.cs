using System;
using System.Configuration;
using System.Data;
using Oracle.DataAccess.Client;

namespace EntityGenerator.Utility
{
    class OracleHelper
    {
        private readonly OracleConnection _oracleConnection;
        public OracleHelper(string connectionString)
        {
            try
            {
                var connString = connectionString.Trim();
                _oracleConnection = new OracleConnection(connString);
             
                Open();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public OracleDataReader ExecuteReader(string cmdText, CommandType cmdType, params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            try
            {
                //Prepare the command to execute
                PrepareCommand(cmd, _oracleConnection, null, cmdType, cmdText, commandParameters);
                //Execute the query, stating that the connection should close when the resulting datareader has been read
                OracleDataReader rdr = cmd.ExecuteReader();
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                //If an error occurs close the connection as the reader will not be used and we expect it to close the connection
                _oracleConnection.Close();
                throw;
            }
        }

        public int ExecuteNonQuery(CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(cmd, _oracleConnection, null, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        public object ExecuteScalar(CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(cmd, _oracleConnection, null, cmdType, cmdText, commandParameters);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

        private void Open()
        {
            try
            {
                _oracleConnection.Open();
            }
            catch (OracleException ex)
            {
                throw new Exception("数据库打开失败");
            }
           
        }

        public void Close()
        {
            if (_oracleConnection.State!=ConnectionState.Closed)
            {
                _oracleConnection.Close();
            }
            
        }

        private void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans, CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();

            //Set up the command
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;

            //Bind it to the transaction if it exists
            if (trans != null)
                cmd.Transaction = trans;

            // Bind the parameters passed in
            if (commandParameters == null) return;

            foreach (OracleParameter parm in commandParameters)
                cmd.Parameters.Add(parm);
        }
    }
}
