using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityGenerator.Type;
using Oracle.DataAccess.Client;

namespace EntityGenerator.Utility
{
    //静态类无法被继承
    public abstract class DBInfoTool
    {
        public abstract List<string> AllTableNames();
        public abstract List<string> AllTableNamesByUser(string user);
    }


    public class DBInfoFactory
    {
        public static DBInfoTool GetDBInfoTool(string type,string connStr)
        {
            switch (type)
            {
                case DBTYPE.ORACLE:
                    return new OracleDBInfo(connStr);
                default:
                    throw new Exception("指定的数据库类型不存在");
            }
        }
    }

    class OracleDBInfo : DBInfoTool
    {
        private readonly OracleHelper oracleHelper;

        public OracleDBInfo(string connStr)
        {
            oracleHelper = new OracleHelper(connStr);
        }

        public override List<string> AllTableNames()
        {
            var reader = oracleHelper.ExecuteReader("SELECT TABLE_NAME FROM ALL_TABLES",System.Data.CommandType.Text,null);
            var tableName=new List<string> ();
            while (reader.Read())
            {
                tableName.Add(reader.GetString(0).Trim());
            }
            return tableName;
        }

        public override List<string> AllTableNamesByUser(string userName)
        {
           
            var reader = oracleHelper.ExecuteReader("SELECT TABLE_NAME FROM ALL_TABLES WHERE OWNER = :userName", System.Data.CommandType.Text, new OracleParameter(":userName", userName.ToUpper()));
            var tableName = new List<string>();
            while (reader.Read())
            {
                tableName.Add(reader.GetString(0).Trim());
            }
            return tableName;
        }
    }
}
