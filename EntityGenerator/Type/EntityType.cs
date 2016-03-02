using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityGenerator.Utility;

namespace EntityGenerator.Type
{
    public class DBTYPE
    {
        public const  string ORACLE = "ORACLE";
        public const  string MYSQL = "MYSQL";
        
    } 

    public class EntityTableType
    {
        //待以后使用
    }

    public class EntityRecordType : IEnumerable<EntityColumnEntity>, IFieldTypePairs
    {
        private readonly Dictionary<string, EntityColumnEntity> columnTypeList;
        private readonly string entityName;
        public EntityRecordType(string entityName)
        {
            columnTypeList = new Dictionary<string, EntityColumnEntity>(20);
            this.entityName = entityName;
        }


        public EntityColumnEntity Find(string columnName)
        {
            return columnTypeList.ContainsKey(columnName) ? columnTypeList[columnName] : null;
        }

        public void AddColumn(EntityColumnEntity colEntity)
        {
            try
            {
                columnTypeList.Add(colEntity.ColumnName, colEntity);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public bool RemoveColumn(EntityColumnEntity colEntity)
        {
            if (colEntity==null)
            {
                throw new ArgumentNullException("colEntity");
            }
            return columnTypeList.Remove(colEntity.ColumnName);
        }

        public string EntityName {
            get { return entityName; }
        }

        public IEnumerator<EntityColumnEntity> GetEnumerator()
        {
            return columnTypeList.Select(kvPair => kvPair.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public List<Tuple<string, string, string>> GetFieldTypeList()
        {
            return this.Select(item => new Tuple<string, string, string>(item.ColumnName.ToLower(), item.ColumnName.ToUpper(), TypeMapper.Map(item.ColumnType))).ToList();
        }
    }
     
    public class EntityColumnEntity
    {
        private readonly string columnName;
        private readonly string type;
        public EntityColumnEntity(string columnName, string type)
        {
            this.columnName = columnName;
            this.type =type;
        }
        public string ColumnName
        {
            get
            {
                return columnName;
            }
        }

        public string ColumnType
        {
            get
            {
                return type;
            }
        }

    }
}
