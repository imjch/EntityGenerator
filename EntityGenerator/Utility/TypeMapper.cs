using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityGenerator.Utility
{
    class TypeMapper
    {
        public static string Map(string type)
        {
            switch (type.ToUpper())
            {
                case OracleTableColumn.BLOB:
                    return "OracleBlob";
                case OracleTableColumn.CLOB:
                    return "OracleClob";
                case OracleTableColumn.CHAR:
                case OracleTableColumn.NVARCHAR2:
                case OracleTableColumn.VARCHAR2:
                case OracleTableColumn.VARCHAR:
                case OracleTableColumn.LONG:
                    return "string";
                case OracleTableColumn.LONG_RAW:
                    return "OracleBinary";
                case OracleTableColumn.BINARY_DOUBLE:
                    return "double?";
                case OracleTableColumn.BINARY_FLOAT:
                    return "float?";
                case OracleTableColumn.DATE:
                case OracleTableColumn.TIMESTAMP:
                case OracleTableColumn.TIMESTAMP_WITH_LOCAL_TIME_ZONE:
                case OracleTableColumn.TIMESTAMP_WITH_TIME_ZONE:
                    return "DateTime?";
                case OracleTableColumn.NUMBER:
                    return type.Contains("(") ? "decimal?" : "double?";
                default:
                    return type;
            }

        }
    }
}
