using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityGenerator.Type;

namespace EntityGenerator.Utility
{
    class DBConnectionStringEntityFactory
    {
        public static ConnectionObjectConstructor GetEntityConnectionStringConstructor(string type, string connStr)
        {
            switch (type)
            {
                case DBTYPE.ORACLE:
                    return new OracleConnectionObjectConstructor(connStr);
                default:
                    throw new Exception("指定的数据库类型不存在");
            }
        }
    }
}
