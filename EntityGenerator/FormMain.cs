using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EntityGenerator.Type;
using EntityGenerator.Utility;

namespace EntityGenerator
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private bool ValidateParameters()
        {
            return comboBoxDB.SelectedItem.ToString().Trim() != string.Empty &&
                   textBoxConnStr.Text.Trim() != string.Empty;
        }

        private void GenerateTypeInfo()
        {
            var currentDomain = AppDomain.CurrentDomain;
            var aName = new AssemblyName("TypeInfo");
            var ab = currentDomain.DefineDynamicAssembly(
                aName, AssemblyBuilderAccess.RunAndSave);
            var mb = ab.DefineDynamicModule(aName.Name, aName.Name + ".dll");


            var types =
                from file in
                    new DirectoryInfo(AppDomain.CurrentDomain.SetupInformation.ApplicationBase).GetFiles(
                        "*.csv", SearchOption.TopDirectoryOnly)
                select
                    new
                    {
                        Fields = File.ReadAllText(file.Name).Split(','),
                        FileName = file.Name.Substring(0, file.Name.IndexOf('.'))
                    };
           

            foreach (var item in types)
            {
                var tb = mb.DefineType(item.FileName + "TableColumn", TypeAttributes.Public);
                foreach (var field in item.Fields)
                {
                    tb.DefineField(field.Trim(), typeof (string),
                        FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.Literal)
                        .SetConstant(field.Trim());
                }
                tb.CreateType();
            }

            //            var connStrs = textBoxConnStr.Text;
            //            for (var i = 2; i < connStrs.Count; i++)
            //            {
            //                var newType = mb.DefineType("ConnectionStringEntity" + connStrs[i].Name);
            //                var connStr = connStrs[i].ConnectionString.Split(';');
            //                foreach (var kvPair in connStr.Select(kvPairs => kvPairs.Trim().Replace(' ','_').Split('=')))
            //                {
            //                    newType.DefineField(kvPair[0].ToUpper(), typeof(string),
            //                        FieldAttributes.Literal | FieldAttributes.Static | FieldAttributes.Public).SetConstant(kvPair[1]);
            //                }
            //                newType.CreateType();
            //            }
            ab.Save(aName.Name + ".dll");
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            GenerateTypeInfo();
            comboBoxDB.SelectedIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!ValidateParameters())
            {
                MessageBox.Show("请填上必填项");
                return;
            }
            try
            {

                var type = DBInfoFactory.GetDBInfoTool(comboBoxDB.Text.Trim(), textBoxConnStr.Text.Trim());
                var connectionEntity = DBConnectionStringEntityFactory.GetEntityConnectionStringConstructor(comboBoxDB.Text.Trim(), textBoxConnStr.Text.Trim());

                textBoxUserName.Text = connectionEntity.User;
                var tableNameList = type.AllTableNamesByUser(connectionEntity.User);
                foreach (var item in tableNameList)
                {
                    comboBoxTableName.Items.Add(item);
                }
                MessageBox.Show("连接成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void comboBoxTableName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var tableRecord= EntityGeneratorFactory.GetEntityGeneratorFactory(comboBoxDB.Text.Trim(), textBoxConnStr.Text.Trim()).ConstructRecordEntity(comboBoxTableName.SelectedItem.ToString().Trim());
            var template = ClassTemplateGenerator.Go(LanguageType.CSharp, comboBoxTableName.SelectedItem.ToString(), tableRecord);
            textBoxResult.Text = template.ToString();
            //classTemplate.AppendFormat("public class {0}Entity\r\n{{\r\n",comboBoxTableName.SelectedItem.ToString());
            //var entityGenerator = EntityGeneratorFactory.GetEntityGeneratorFactory(comboBoxDB.Text.Trim(),textBoxConnStr.Text.Trim());
            //var tableEntity =entityGenerator.ConstructRecordEntity(comboBoxTableName.SelectedItem.ToString().Trim());
            //foreach (var item in tableEntity)
            //{
            //    classTemplate.AppendFormat("\tprivate {0} _{1};\r\n",TypeMapper.Map(item.ColumnType.ToLower()),item.ColumnName.ToLower());
            //    classTemplate.AppendFormat("\tpublic {0} {1}\r\n\t{{\r\n\t\tget;\r\n\t\tset;\r\n\t}}\r\n\r\n", TypeMapper.Map(item.ColumnType), item.ColumnName.ToUpper());
            //}
            //classTemplate.AppendFormat("}}\r\n");

            //textBoxResult.Text = classTemplate.ToString();
        }
    }
}
