using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Data;
using System.Windows.Forms;
using System.IO; 

namespace ConsoleApplication1
{
    class DynamicClass
    {
        public static string debugstr = "";
        public static  int progress = 0;
        public static int total = 0;
        public static Assembly assembly;  
        public static string myclassName="DynamicClass";  
        //创建编译器实例。     
        public static CSharpCodeProvider provider = new CSharpCodeProvider();  
        //设置编译参数。     
        public static CompilerParameters paras = new CompilerParameters();  
        //动态创建类  
        public static void NewAssembly(string classSource,string outpath)  
        {
            //Console.Write(classSource);  
            paras.GenerateExecutable = false;  
            //paras.ReferencedAssemblies.Add("System.dll");  
            paras.GenerateInMemory = false;
            paras.OutputAssembly = string.Format("{0}/STG_MF_Age_cfgdata.dll", outpath);  
            System.Diagnostics.Debug.WriteLine(classSource);  
            //编译代码。     
            if (!Directory.Exists(outpath))
            {
                Directory.CreateDirectory(outpath);
            }
            if (File.Exists(paras.OutputAssembly))
            {
                File.Delete(paras.OutputAssembly);
            }
            CompilerResults result = provider.CompileAssemblyFromSource(paras, classSource);  
  
            //获取编译后的程序集。     
            assembly = result.CompiledAssembly;  
        }

        //利用数据进行实例化对象，并返回Dictionary进行存储  
        public static Dictionary<int, object> NewInstances(DataTable objectData, string className, int keyIndex)  
        {
            myclassName = className;
            debugstr = "";
            int rowCount = objectData.Rows.Count;
            int columnCount = objectData.Columns.Count;
            Dictionary<int, object> genObject = new Dictionary<int, object>();  
 
             bool isCan = true;
            for (int j = 1; j < rowCount; j++)     
            {
                if (j == 835)
                {
                    j = 835;
                }
                object generatedClass = assembly.CreateInstance(myclassName);
                DataRow row = objectData.Rows[j];
                if (row == null) continue;
                FieldInfo[] infos = generatedClass.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                
                for (int i = 0, len = infos.Length; i < len; i++)  
                {
                    if (!isCan)
                        break;
                    string columnName = infos[i].Name;
                    string columnType = infos[i].FieldType.Name;
                    string value = row[columnName].ToString().Trim();
                    try
                    {
                        //Console.WriteLine(infos[i].PropertyType.Name);
                      
                        if (columnType.Equals("string") || columnType.Equals("String"))
                        {
                            infos[i].SetValue(generatedClass, value);
                        }
                        else if (columnType.Contains("[]"))
                        {
                            //数组转换
                            if (columnType.Contains("Int32"))
                            {
                                infos[i].SetValue(generatedClass, converStringToIArr(value));
                                //Console.WriteLine(infos[i].PropertyType.Name + " = " + converStringToIArr(strs[i]));
                            }
                            else if (columnType.Contains("String") || columnType.Contains("string"))
                            {
                                infos[i].SetValue(generatedClass, converStringToSArr(value));
                                //Console.WriteLine(infos[i].PropertyType.Name + " = " + converStringToSArr(strs[i]));
                            }
                            else if (columnType.Contains("Single"))
                            {
                                infos[i].SetValue(generatedClass, converStringToFArr(value));
                                // Console.WriteLine(infos[i].PropertyType.Name + " = " + converStringToFArr(strs[i]));
                            }
                        }
                        else
                        {
                            System.Type t = infos[i].FieldType;
                            System.Reflection.MethodInfo method = t.GetMethod("Parse", new Type[] { typeof(string) });
                            Object obj = Activator.CreateInstance(t);
                            System.Reflection.BindingFlags flag = System.Reflection.BindingFlags.Public | BindingFlags.Static | System.Reflection.BindingFlags.Instance;
                            //GetValue方法的参数  
                            //infos[i].SetValue(generatedClass, null, null);  
                            if (value.Equals("") || value == null)
                            {
                                infos[i].SetValue(generatedClass, null);
                            }
                            else
                            {
                                object[] parameters = new object[] { value };

                                //if (DebugText != null)
                                //{
                                //    DebugText.Text += "\n" + t.Name + " " + value;
                                //}
                                //取得方法返回的值  
                                object returnValue = method.Invoke(obj, flag, Type.DefaultBinder, parameters, null);

                                //Console.WriteLine(method.Invoke(obj, flag, Type.DefaultBinder, parameters, null));
                                infos[i].SetValue(generatedClass, returnValue);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string strtips = "策划王子,表" + className + "发生错误\n\t\t [行:" + (int)(j+3) + "列:" + columnName + "]错误" + ex.Message;
                        MessageBox.Show(strtips);
                        isCan = false;
                    }    
                }
                if (row[keyIndex].ToString().Equals("") || row[keyIndex] == null)
                {
                    //MessageBox.Show("策划王子,表" + className + "发生错误\n\t\t ID=" + keyIndex + "[ID列错误]");
                }
                else
                {
                    int key;
                    if (int.TryParse(row[keyIndex].ToString(), out key))
                    {
                        if (genObject.ContainsKey(key))
                        {
                            MessageBox.Show("策划王子,表" + className + "发生错误\n\t\t ID=" + key + "[ID列错误]");
                            continue;
                        }
                        genObject[key] = generatedClass;
                    }
                    else
                    {
                        MessageBox.Show("策划王子,表" + className + "发生错误\n\t\t ID=" + key + "[ID列错误]");
                    }
                }
            }
            return genObject;  
              
        }


        public static int[] converStringToIArr(string myString)
        {
            if (myString == "" || myString == null)
            {
                return null;
            }
            List<int> piArr = new List<int>();
            string tmpString1 = myString.Trim();
            string[] tmpString2 = tmpString1.Split(',');

            for (int i = 0; i < tmpString2.Length; i++)
            {
                int myInt;
                if (tmpString2[i] != "" && tmpString2[i] != null)
                {
                    myInt = int.Parse(tmpString2[i]);
                }
                else{
                    myInt = 0;
                }
                piArr.Add(myInt);
            }
            return piArr.ToArray();
        }

        public static string[] converStringToSArr(string myString)
        {
            if (myString == "" || myString == null)
            {
                return null;
            }
            List<string> piArr = new List<string>();
            string tmpString1 = myString.Trim();
            string[] tmpString2 = tmpString1.Split(',');

            for (int i = 0; i < tmpString2.Length; i++)
            {
                string myInt;
                if (tmpString2[i] != "" && tmpString2[i] != null)
                {
                    myInt =  tmpString2[i];;
                }
                else
                {
                    myInt = "";
                }
                piArr.Add(myInt);
            }
            return piArr.ToArray();
        }

        public static float[] converStringToFArr(string myString)
        {
            if (myString == "" || myString == null)
            {
                return null;
            }
            List<float> piArr = new List<float>();
            string tmpString1 = myString.Trim();
            string[] tmpString2 = tmpString1.Split(',');

            for (int i = 0; i < tmpString2.Length; i++)
            {
                float myInt;
                if (tmpString2[i] != "" && tmpString2[i] != null)
                {
                    myInt = float.Parse(tmpString2[i]);
                }
                else
                {
                    myInt = 0f;
                }
                piArr.Add(myInt);
            }
            return piArr.ToArray();
        }
    }
}
