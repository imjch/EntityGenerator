using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityGenerator.Type
{
    enum LanguageType
    {
        CSharp,
        Java
    }

    class ClassTemplateGenerator
    {
        public static ClassTemplate Go(LanguageType type, string className)
        {
            return Go(type,className, null);
        }
        public static ClassTemplate Go(LanguageType type, string className, IFieldTypePairs properties)
        {
            switch (type)
            {
                case LanguageType.CSharp:
                    return new CSharpTemplate(className, properties);
                default:
                    throw new Exception("没有该语言对应的模板");
            }
        }
    }
    abstract class ClassTemplate
    {
        public virtual string ClassName { set; get; }

        public virtual ClassBodyTemplate Body { set; get; }

        public abstract override string ToString();
    }
    class CSharpTemplate : ClassTemplate
    {
        public CSharpTemplate(string className):this(className,null)
        {
           
        }
        public CSharpTemplate(string className, IFieldTypePairs properties)
        {
            this.ClassName = string.IsNullOrEmpty(className) ? TemplatesResources.CSClassDef : className;
            this.Body = properties == null ? null : new CSharpBodyTemplate(properties);
        }
        public override string ToString()
        {
            var str = new StringBuilder(TemplatesResources.CSClassDef);
            return str.Replace("{#class}", this.ClassName).Replace("{#body}", this.Body.ToString()).ToString();
        }
    }
    abstract class ClassBodyTemplate
    {
        protected ClassPropertiesTemplate _properties=null;
       
        public virtual ClassPropertiesTemplate Properties
        {
            get;
            set;
        }
        public abstract override string ToString();
    }

    class CSharpBodyTemplate : ClassBodyTemplate
    {
        public CSharpBodyTemplate(IFieldTypePairs properties)
        {
            _properties = new CSharpPropertiesTemplate(properties);
        }
        // public CSharpBodyTemplate()

        public override string ToString()
        {
            return _properties.ToString();
        }
    }

    abstract class ClassPropertyTemplate
    {
        protected ClassPropertyTemplate(string field, string property, string type)
        {
            Field = field;
            PropertyName = property;
            PropertyType = type;
        }
        public virtual string Field { set; get; }
        public virtual string PropertyName { set; get; }
        public virtual string PropertyType { set; get; }
        public abstract override string ToString();
    }

    class CSharpPropertyTemplate : ClassPropertyTemplate
    {
        private string CSClassPropertyDef = "public {#type} {#property}\r\n{\t\tget;\r\n\t\tset;\r\n\t}";
        public CSharpPropertyTemplate(string field, string property, string type):base(field,property,type)
        {
            
        }
        public override string ToString()
        {
            var strProperty = new StringBuilder(TemplatesResources.CSClassPropertyDef);
            var strField= new StringBuilder(TemplatesResources.CSClassFieldDef);
            strProperty.Replace("{#type}", this.PropertyType).Replace("{#property}", this.PropertyName);
            strField.Replace("{#field}", this.Field).Replace("{#type}", this.PropertyType).Append("\r\n\t"+strProperty);
            return strField.ToString();
        }
    }

    abstract class ClassPropertiesTemplate
    {
        protected readonly List<ClassPropertyTemplate> _properties = new List<ClassPropertyTemplate>();

        public abstract override string ToString();
    }

    class CSharpPropertiesTemplate : ClassPropertiesTemplate
    {
        public CSharpPropertiesTemplate(IFieldTypePairs properties)
        {
            foreach (var item in properties.GetFieldTypeList())
            {
                _properties.Add(new CSharpPropertyTemplate(item.Item1, item.Item2, item.Item3));
            }
        }
        public override string ToString()
        {
             var sb = new StringBuilder();
            _properties.ForEach(x => sb.AppendFormat("\t{0}\r\n", x.ToString()));
            return sb.ToString();
        }
    }
        
}
