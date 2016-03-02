using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EntityGenerator.Type
{
    abstract class ConnectionObjectConstructor
    {
        private readonly string connStr;

        protected ConnectionObjectConstructor(string connStr)
        {
            this.connStr = connStr;
        }

        protected abstract string ConstructUser();
        protected abstract string ConstructDataBase();
        protected abstract string ConstructPassword();

        protected string ConnStr {
            get { return connStr; }
        }

        public string User { get; protected set; }

        public string DataBase { get; protected set; }

        public string Password { get; protected set; }
    }

    class OracleConnectionObjectConstructor : ConnectionObjectConstructor
    {
        private readonly Dictionary<string, string> dict;
        public OracleConnectionObjectConstructor(string connStr):base(connStr)
        {
            dict = ResolveConnectionString();
            this.User = ConstructUser();
            this.DataBase = ConstructDataBase();
            this.Password = ConstructPassword();
        }

        private Dictionary<string,string> ResolveConnectionString()
        {
            var dict = new Dictionary<string, string>();
            var regex = new Regex("[ ]+");
            var kvPairs=ConnStr.Split(';');
            
            foreach (var kv in kvPairs.Select(item => item.Split('=')))
            {
                var attr = regex.Replace(kv[0].Trim(), "_");
                var value = regex.Replace(kv[1].Trim(), "_");
                dict.Add(attr, value);
            }
            return dict;
        }

        protected sealed override string ConstructUser()
        {
            return dict[dict.Keys.First((x) => x.Contains("USER_ID"))];
        }
        protected sealed override string ConstructDataBase()
        {
            return dict[dict.Keys.First((x) => x.Contains("DATA_SOURCE"))];
        }
        protected sealed override string ConstructPassword()
        {
            return dict[dict.Keys.First((x) => x.Contains("PASSWORD"))];
        }

        
    }     
}
