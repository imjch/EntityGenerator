using System;
using EntityGenerator.Type;
using Oracle.DataAccess.Client;

namespace EntityGenerator.Utility
{
    public abstract class EntityGenerator
    {
        public abstract EntityRecordType ConstructRecordEntity(string tableName);
    }

    public class EntityGeneratorFactory
    {
        public static EntityGenerator GetEntityGeneratorFactory(string type, string connStr)
        {
            switch (type)
            {
                case DBTYPE.ORACLE:
                    return new OracleEntityGenerator(connStr);
                default:
                    throw new Exception("指定的数据库类型不存在");
            }
        }
    }


    class OracleEntityGenerator:EntityGenerator
    {
        private readonly OracleHelper _oracleHelper;
        public OracleEntityGenerator(string connStr)
        {
            try
            {
                _oracleHelper = new OracleHelper(connStr);
            } 
            catch (Exception)
            {
                throw;
            }
        }
        public override EntityRecordType ConstructRecordEntity(string tableName)
        {
            OracleDataReader reader;
            try
            {
                 reader = _oracleHelper.ExecuteReader("SELECT COLUMN_NAME,DATA_TYPE FROM USER_TAB_COLUMNS WHERE TABLE_NAME = :tableName",
                     System.Data.CommandType.Text, new OracleParameter(":tableName",tableName));
            }
            catch (Exception)
            {
                _oracleHelper.Close();
                throw;
            }

           var recordType = new EntityRecordType(tableName);
           while (reader.Read())
           {
               recordType.AddColumn(new EntityColumnEntity(reader.GetString(0), reader.GetString(1)));
           }
            return recordType;
        }
    }
}
