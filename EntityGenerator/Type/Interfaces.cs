using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityGenerator.Type
{
    interface IFieldTypePairs
    {
        List<Tuple<string, string, string>> GetFieldTypeList();
    }
}
