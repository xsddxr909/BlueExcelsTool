
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using System.Data;
using NPOI.HSSF.Extractor;
namespace OPenExcel
{
    public partial class Form1 : Form
    {
        string DataOutPutPath = "";
        string DataOutPutLuaPath = "";
        string inputDataPath = "";
        public Form1()
        {
            InitializeComponent();
            initUI();
        }

        public string ReadStreamFile(string strName)
        {
            FileInfo fi = new FileInfo(strName);
            if (!fi.Exists)
            {
                return "";
            }
            //使用流的形式读取
            StreamReader sr = null;
            try
            {
                sr = File.OpenText(strName);
            }
            catch (Exception e)
            {
                //路径与名称未找到文件则直接返回空 
                return "";
            }

            string strContent = sr.ReadToEnd();

            //关闭流  
            sr.Close();
            //销毁流  
            sr.Dispose();
            //将数组链表容器返回     
            return strContent;
        }

        public void SaveFilePath(string strName = "PathConfig.txt")
        {
            StringBuilder content = new StringBuilder();
            content.Append(DataOutPutPath);
            content.Append(",");
            content.Append(inputDataPath);
            content.Append(",");
            content.Append(DataOutPutLuaPath);

            FileStream stream = new FileStream(strName, FileMode.Create);
            byte[] data = Encoding.UTF8.GetBytes(content.ToString());
            stream.Write(data, 0, data.Length);
            stream.Flush();
            stream.Close();
        }

        private void initUI()
        {
            string content = ReadStreamFile("PathConfig.txt");
            if (!string.IsNullOrEmpty(content))
            {
                string[] paths = content.Split(',');
                if(paths.Length == 3)
                {
                    DataOutPutPath = paths[0];
                    inputDataPath = paths[1];
                    DataOutPutLuaPath = paths[2];
                }
                DataOutPutPathLab.Text = DataOutPutPath;
                DataOutPutLuaPathLab.Text = DataOutPutLuaPath;
            }

        }
        private void setInputDataPath(string fileName)
        {
            int index = fileName.LastIndexOf("\\");
            inputDataPath = fileName.Substring(0, index); 
            SaveFilePath();
        }

        public string[] FileNames;
        private int index;
        private int total;

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Multiselect = true;
            if(!string.IsNullOrEmpty(inputDataPath))
            {
                OFD.InitialDirectory = inputDataPath;
            }
            
            OFD.Filter = "xls files (*.xls)|*.xls|All files (*.*)|*.*"; //只选取xls文件
            OFD.RestoreDirectory = true; //还原当前目录
           
            if (OFD.ShowDialog() == DialogResult.OK)
            {
                FileNames = OFD.FileNames;
                string[] strArr = OFD.SafeFileNames;
                textBox1.Text = "";
                for (int i = 0, len = strArr.Length; i < len; i++)
                {
                    textBox1.Text += (strArr[i] + "\r\n");
                }
                setInputDataPath(FileNames[0]);
            }   

        }

        private StringBuilder classSource;
        private int dataOutputPathIndex = 0;//0：两个输出路径都不合法  1：路径1合法  2：路径2合法  3：两个路径都合法
        private void button2_Click(object sender, EventArgs e)
        {
            if (FileNames == null || FileNames.Length == 0)
            {
                return;
            }
            if (!checkOutPutPath(DataOutPutLuaPath))
            {
                MessageBox.Show("未选择Lua输出路径");
                return;
            }
            dataOutputPathIndex = 0;
            if (checkOutPutPath(DataOutPutPath))
            {
                dataOutputPathIndex = 1;
            }
            if (dataOutputPathIndex == 0)
            {
                MessageBox.Show("未选择Data输出路径");
                return;
            }
            index = 0;
            total = FileNames.Length;
            
            List<string> csFileNames = new List<string>();
            List<string> luaFileNames = new List<string>();
            for (int i = 0, len = FileNames.Length; i < len; i++)
            {
                string str = FileNames[i];
                if (str.IndexOf("lua_s_") >= 0)
                {
                    luaFileNames.Add(str);
                }
                else
                {
                    csFileNames.Add(str);
                }
            }

            #region xls表生成.data
            StringBuilder sData  = new StringBuilder();
            StringBuilder sBaseData = new StringBuilder();
            classSource = new StringBuilder();
           // classSource.Append("using System;\n");
           //构建类;
            for (int i = 0, len = csFileNames.Count; i < len; i++)
            {
                string path = csFileNames[i];
                if (path != null)
                {
                    readTitle(path, sData, sBaseData);
                    index++;
                    progressBar1.Value = (int)(100 * (float)index / (float)total);
                }
            }
            //添加数据;
            classSource.Append("export class ConfigXls{ \n");
            classSource.Append("private static m_pInstance: ConfigXls; \n");
            classSource.Append("public static Get(): ConfigXls{  \n");
            classSource.Append("if(null == ConfigXls.m_pInstance){ ConfigXls.m_pInstance = new ConfigXls(); } return ConfigXls.m_pInstance; } \n");
            classSource.Append("private inited:boolean=false; \n");
            classSource.Append(sData.ToString());
            classSource.Append("public init(){ \n");
            classSource.Append("if(this.inited)return; \n");
        
            classSource.Append(sBaseData.ToString());
            classSource.Append("\n");

            classSource.Append("this.inited=true; \n");
            classSource.Append("} \n");
            classSource.Append("} \n");
            string file = DataOutPutPath + "\\ConfigXls.ts";
            File.Delete(file);
            //File.Create(file);

            using (StreamWriter textWriter = new StreamWriter(file, false))
            {
                textWriter.Write(classSource.ToString());
                textWriter.Flush();
                textWriter.Close();
            }
            SaveFilePath();
            #endregion

            #region xls表生成.lua
            if(luaFileNames.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("LuaCfgData = {}");
                sb.AppendLine("local dList = nil;");
                for (int i = 0, len = luaFileNames.Count; i < len; i++)
                {
                    sb.AppendLine("");
                    readParseToLua(luaFileNames[i], ref sb);
                    index++;
                    progressBar1.Value = (int)(100 * (float)index / (float)total);
                }
                
                file = DataOutPutLuaPath + "\\LuaCfgData.lua";
                File.Delete(file);
                 //File.Create(file);

                using (StreamWriter textWriter = new StreamWriter(file, false))
                {
                    textWriter.Write(sb.ToString());
                    textWriter.Flush();
                    textWriter.Close();
                 }
            }
            #endregion

            if (index == total)
            {
                MessageBox.Show("已经生成完毕!");
            }
        }

        //解释成lua
        private void readParseToLua(string path , ref StringBuilder sb)
        {
            using (FileStream fs = File.OpenRead(path))  //打开myxls.xls文件
            {
                HSSFWorkbook wk = new HSSFWorkbook(fs);
                ExcelExtractor extractor = new ExcelExtractor(wk);
                ISheet sheet = wk.GetSheetAt(0);

                string xmlName = Path.GetFileNameWithoutExtension(path); //Lua_x_config.xml
                string className = xmlName.Substring(6, xmlName.Length - 6); //lua_s_
                LogtextBox.Text += string.Format("处理表{0}\r\n", xmlName);

                //第一行
                IRow row0 = sheet.GetRow(1);
                IRow row1 = sheet.GetRow(2);
                int clomn_Count = row0.LastCellNum;
                int row_Count = sheet.LastRowNum + 1;

                

                List<int> rList = new List<int>(); //有效的列号
                List<string> pList = new List<string>(); //属性名
                List<string> tList = new List<string>(); //类型名

                //记录属性
                for (int i = 0, len = clomn_Count; i < len; i++)
                {
                    string pName = row0.GetCell(i).ToString();
                    string tName = row1.GetCell(i).ToString();
                    if (string.IsNullOrEmpty(pName))
                    {
                        string strtips = string.Format("{0}表 {1}列 属性无效", xmlName , i);
                        MessageBox.Show(strtips);
                        return;
                    }

                    if (string.IsNullOrEmpty(tName))
                    {
                        string strtips = string.Format("{0}表 {1}列 数据类型无效", xmlName , i);
                        MessageBox.Show(strtips);
                        return;
                    }
                    pList.Add(pName);
                    tList.Add(tName);
                    rList.Add(i);
                }

                string localDListStr = tList[0] == "string" ? "DList_string_LuaInterface_LuaTable" : "DList_int_LuaInterface_LuaTable";
                sb.AppendLine(string.Format("dList = {0}.New()", localDListStr));
                sb.AppendLine(string.Format("LuaCfgData.{0} = dList;", className));

                //记录数据
                for (int i = 3; i < row_Count; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;

                    ICell v = row.GetCell(rList[0]);
                    if (v == null) continue;

                    string str = "";
                    //关键id
                    for (int j = 0, jlen = rList.Count; j < jlen; j++)
                    {
                        int ii = rList[j];
                        v = row.GetCell(ii);
                        if (tList[j] == "string")
                        {
                            str += string.Format("{0}=\"{1}\";", pList[ii], v);
                        }
                        else if(tList[j] == "int[]")
                        {
                            str += string.Format("{0}={1};", pList[ii], "{" + v + "}");
                        }
                        else if (tList[j] == "int")
                        {
                            if (string.IsNullOrEmpty(v.ToString()))
                            {
                                str += string.Format("{0}=0;", pList[ii]);
                            }
                            else
                            {
                                str += string.Format("{0}={1};", pList[ii], v);
                            }
                        }
                        else
                        {
                            str += string.Format("{0}={1};", pList[ii], v);
                        }
                    }

                    //写lua
                    string dbStr = tList[0] == "string" ? "dList:Add(\"{0}\" , {1});" : "dList:Add({0} , {1});";
                    sb.AppendLine(string.Format(dbStr, row.GetCell(rList[0]), "{" + str + "}"));
                }
                LogtextBox.Text += string.Format("处理表{0}结束\r\n", xmlName);
            }
        }

        private void copyDll(string strSrcDir, string strDestDir)
        {
            strSrcDir.Replace("\\", "/");
            strDestDir.Replace("\\", "/");
            try
            {
                if (!Directory.Exists(strDestDir))
                {
                    Directory.CreateDirectory(strDestDir);

                }
                if (File.Exists(strSrcDir))
                {
                    System.IO.File.Copy(strSrcDir, strDestDir + "/STG_MF_Age_cfgdata.dll", true);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show("复制DLL错误");
            }

        }

        private void createCode(string outputPath, StringBuilder classSource)
        {
            string path = string.Format("{0}/STG_MF_Age_cfgdata.cs", outputPath);
            if(File.Exists(path))
            {
                File.Delete(path);
            }
            FileStream aFile = new FileStream(path, FileMode.OpenOrCreate);
            using (StreamWriter sw = new StreamWriter(aFile, Encoding.UTF8))
            {
                sw.Write(classSource);
                sw.Close();
            }
            //Console.Write(classSource);  

        }

        private void readTitle(string path, StringBuilder sData, StringBuilder sBaseData)
        {
            using (FileStream fs = File.OpenRead(path))  //打开myxls.xls文件
            {
                HSSFWorkbook wk = new HSSFWorkbook(fs);
                ExcelExtractor extractor = new ExcelExtractor(wk);
                ISheet sheet = wk.GetSheetAt(0);
                
                string className = Path.GetFileNameWithoutExtension(path);
                string propertyName, propertyType,info;
                //第一行
                IRow rowName = sheet.GetRow(0);

                IRow row0 = sheet.GetRow(1);
                IRow row1 = sheet.GetRow(2);
                int clomn_Count = row0.LastCellNum;
                int row_Count = sheet.LastRowNum + 1;

                List<string> columnHead = new List<string>();
                List<string> columnType = new List<string>();
                List<string> creatList = new List<string>();
                List<string> creatList2 = new List<string>();

                sData.AppendFormat("{0}:Map<number,{0}>=new Map<number,{0}>();\n", className);
                sBaseData.AppendFormat("//{0} \n", className);

                classSource.AppendFormat("export   class   {0} \n", className);
                classSource.Append("{\n");
                //  classSource.AppendFormat("public {0}(){{}}\n", className);
                string creatStr;
                string creatStr2;
                for (int j = 0, len = clomn_Count; j < len; j++)
                {
                    
                    info = rowName.GetCell(j).ToString();
                    info= info.Replace("\n", " ");
                    propertyName = row0.GetCell(j).ToString();
                    columnHead.Add(propertyName);

                    propertyType = row1.GetCell(j).ToString();
                    switch (propertyType) {
                        case "int":
                        case "float":
                            propertyType = "number";
                            break;
                        case "int[]":
                        case "float[]":
                            propertyType = "number[]";
                            break;
                        case "string":
                            propertyType = "string";
                            break;
                        case "string[]":
                            propertyType = "string[]";
                            break;
                        case "bool":
                            propertyType = "boolean";
                            break;
                    }
                    columnType.Add(propertyType);

                    if (propertyName.Equals("") || propertyType.Equals("")) continue;

                    classSource.AppendFormat("//{0} \n", info);
                    classSource.AppendFormat(" public  {0}:{1} ;\n",  propertyName, propertyType);
                    creatStr=string.Format("{0}:{1}", propertyName, propertyType);
                    creatList.Add(creatStr);
                    creatStr2 = string.Format("data.{0}={0}; \n", propertyName);
                    creatList2.Add(creatStr2);
                }
                classSource.Append("public static Creat(");
                for (int j = 0; j < creatList.Count; j++)
                {
                    if (j + 1 == creatList.Count)
                    {
                        classSource.AppendFormat("{0} ):{1}", creatList[j],className);
                        classSource.Append("{\n");
                        classSource.AppendFormat("let data:{0}=new {0}(); \n", className);
                        for (int k = 0; k < creatList2.Count; k++)
                        {
                            classSource.Append(creatList2[k]);

                        }
                        classSource.Append("return data; \n");
                        classSource.Append("}\n");
                    }
                    else {
                        classSource.AppendFormat("{0},", creatList[j], className);
                    }

                }
                //创建类方法;
                classSource.Append("}\n");

                //数据
                for (int i = 3; i < row_Count; i++)//  
                {
                    IRow curow = sheet.GetRow(i);
                    if (curow == null) continue;
                  
                    for (int j = 0; j < clomn_Count; j++)
                    {
                        propertyName = row0.GetCell(j).ToString();
                        propertyType = row1.GetCell(j).ToString();
                        switch (propertyType)
                        {
                            case "int":
                            case "float":
                                propertyType = "number";
                                break;
                            case "int[]":
                            case "float[]":
                                propertyType = "number[]";
                                break;
                            case "string":
                                propertyType = "string";
                                break;
                            case "string[]":
                                propertyType = "string[]";
                                break;
                            case "bool":
                                propertyType = "boolean";
                                break;
                        }
                        if (propertyName.Equals("") || propertyType.Equals("")) {
                            if (j + 1 == clomn_Count)
                            {
                                sBaseData.Append(")); \n");
                            }
                            continue;
                        }
                        string itemValue;
                        if (curow.GetCell(j) != null)
                        {
                            if (curow.GetCell(j).CellType == CellType.Formula)
                            {
                                CellType type = curow.GetCell(j).CachedFormulaResultType;
                                switch (type)
                                {
                                    case CellType.Numeric:
                                        {
                                            itemValue = curow.GetCell(j).NumericCellValue.ToString();
                                        }
                                        break;
                                    default:
                                        itemValue = curow.GetCell(j).StringCellValue.ToString();
                                        break;
                                }
                            }
                            else
                            {
                                itemValue = curow.GetCell(j).ToString();
                            }
                        }
                        else
                        {
                            itemValue = "";
                        }
                        if (j == 0) {
                            //补个头;
                            sBaseData.AppendFormat("this.{0}.set({1},{0}.Creat(", className, itemValue);
                        }
                        else
                        {
                            sBaseData.Append(",");
                        }
                        switch (propertyType)
                        {
                            case "number":
                                if (itemValue == "")
                                {
                                    sBaseData.Append("0");
                                }
                                else {
                                    sBaseData.AppendFormat("{0}", itemValue);
                                }
                                break;
                            case "number[]":
                                //多维数组;
                                if (itemValue == "")
                                {
                                    sBaseData.Append("[]");
                                }
                                else
                                {
                                    sBaseData.AppendFormat("[{0}]", itemValue);
                                }
                                break;
                            case "string[]":
                                //多维数组;
                                if (itemValue == "")
                                {
                                    sBaseData.Append("[]");
                                }
                                else
                                {
                                    string[] sst = itemValue.Split(',');
                                    StringBuilder vvStr = new StringBuilder();
                                    vvStr.Append("[");
                                    for (int oo = 0; oo < sst.Length; oo++) {
                                        vvStr.AppendFormat("'{0}'", sst[oo]);
                                        if (oo + 1 < sst.Length)
                                        {
                                            vvStr.Append(",");
                                        }
                                    }
                                    vvStr.Append("]");
                                    sBaseData.Append(vvStr.ToString());
                                }
                                break;
                            case "string":
                                if (itemValue == "")
                                {
                                    sBaseData.Append("''");
                                }
                                else
                                {
                                    sBaseData.AppendFormat("'{0}'", itemValue);
                                }
                                break;
                            case "boolean":
                                if (itemValue == "")
                                {
                                    sBaseData.Append("false");
                                }
                                else
                                {
                                    if (itemValue == "0")
                                    {
                                        sBaseData.Append("false");
                                    }
                                    else {
                                        sBaseData.Append("true");
                                    }
                                }
                                break;
                        }
                        if (j + 1 == clomn_Count)
                        {
                            sBaseData.Append(")); \n");
                        }
                    }
                    sBaseData.Append("\n");
                }
            }
        }

        public void readParse(string path)
        {
            bool isCan = true;
            using (FileStream fs = File.OpenRead(path))  //打开myxls.xls文件
            {
                StringBuilder classSourceTemp = new StringBuilder();
                DataTable objectData = new DataTable();
                HSSFWorkbook wk = new HSSFWorkbook(fs);
                ExcelExtractor extractor = new ExcelExtractor(wk);
                ISheet sheet = wk.GetSheetAt(0);

                string className = Path.GetFileNameWithoutExtension(path);
                LogtextBox.Text += string.Format("处理表{0}\r\n", className);
                string propertyName, propertyType;
                //第一行
                IRow row0 = sheet.GetRow(1);
                IRow row1 = sheet.GetRow(2);
                int clomn_Count = row0.LastCellNum;
                int row_Count = sheet.LastRowNum + 1;

                List<string> columnHead = new List<string>();
                List<string> columnType = new List<string>();
                classSourceTemp.Append("using System;\n");
                classSourceTemp.Append("[Serializable]\n");
                classSourceTemp.AppendFormat("public   class   {0} \n", className);
                classSourceTemp.Append("{\n");
                classSourceTemp.AppendFormat("public {0}(){{}}\n", className);

                for (int j = 0, len = clomn_Count; j < len; j++)
                {
                    propertyName = row0.GetCell(j).ToString();
                    columnHead.Add(propertyName);

                    propertyType = row1.GetCell(j).ToString();
                    columnType.Add(propertyType);

                    if (propertyName.Equals("") || propertyType.Equals("")) continue;
                    classSourceTemp.AppendFormat(" public  {0} {1} ;\n", propertyType, propertyName);
                    //classSourceTemp.Append(" public   " + propertyType + "   " + "" + propertyName + "\n");
                    //classSourceTemp.Append(" {\n");
                    //classSourceTemp.Append(" get{   return   _" + propertyName + ";}   \n");
                    //classSourceTemp.Append(" set{   _" + propertyName + "   =   value;   }\n");
                    classSourceTemp.Append(" }\n");
                    //classSource.Append("\tpublic " + ((Excel.Range)m_Worksheet.Cells[4, j]).Text.ToString() + " " + ((Excel.Range)m_Worksheet.Cells[3, j]).Text.ToString() + ";\n");  
                }
                classSourceTemp.Append("}\n");
                initDataTable(columnHead, columnType, row_Count, objectData);

                for (int i = 3; i < row_Count; i++)//  
                {
                    IRow curow = sheet.GetRow(i);
                    if (curow == null) continue;
                    DataRow dr = objectData.NewRow();
                    for (int j = 0; j < clomn_Count; j++)
                    {
                        if (isCan)
                        try
                        {

                            if (curow.GetCell(j) != null)
                            {
                                string item;
                                if (curow.GetCell(j).CellType == CellType.Formula)
                                {
                                    CellType type = curow.GetCell(j).CachedFormulaResultType;
                                    switch (type)
                                    {
                                        case CellType.Numeric:
                                            {
                                                item = curow.GetCell(j).NumericCellValue.ToString();
                                            }
                                            break;
                                        default:
                                            item = curow.GetCell(j).StringCellValue.ToString();
                                            break;
                                    }
                                }
                                else
                                {
                                    item = curow.GetCell(j).ToString();
                                }

                                objectData.Rows[i - 2][j] = item;
                            }
                            else
                            {
                                objectData.Rows[i - 2][j] = "";
                            }
                        }
                        catch (Exception ex)
                        {
                            string strtips = string.Format("表{0}发生错误\n\t\t 行{1}列{2}错误{3}", className, i, j, ex.Message);
                            MessageBox.Show(strtips);
                            isCan = false;
                        }
                    }
                }
//                 if (dataOutputPathIndex == 1 || dataOutputPathIndex == 3)
//                 {
//                     seriData(className, objectData, DataOutPutPath);
//                 }
                LogtextBox.Text += string.Format("处理表{0}结束\r\n", className);
                
            }
        }

        private void initDataTable(List<String> columnName, List<String> columnType, int Rows, DataTable ptb)
        {
            for (int i = 0, len = columnName.Count; i < len; i++)
            {
                ptb.Columns.Add(columnName[i]);
            }
            DataRow dr1 = ptb.NewRow();
            for (int i = 0, len = columnType.Count; i < len; i++)
            {
                dr1[i] = columnType[i];
            }
            ptb.Rows.Add(dr1);
            Rows = Rows - 3;
            for (int i = 0; i < Rows; i++)
            {
                DataRow dr = ptb.NewRow();
                ptb.Rows.Add(dr);
            }

        }

        /// <summary>
        /// 生成对象
        /// </summary>
        /// <param name="className"></param>
        /// <param name="objectData"></param>
        public void seriData(string className, DataTable objectData,string rootDic)
        {
            Dictionary<int, object> dic = ConsoleApplication1.DynamicClass.NewInstances(objectData, className,0);
            string strFile = className + ".data";
            DataSetSerializerCompression(dic, strFile, rootDic);
        }
  
        public static string FileOutP = "/OutData/";
        void DataSetSerializerCompression(Dictionary<int, object> dic, string filename,string rootDic)
        {
            
            IFormatter formatter = new BinaryFormatter();//定义BinaryFormatter以序列化DataSet对象  
            MemoryStream input = new MemoryStream();//创建内存流对象  
            formatter.Serialize(input, dic);//把DataSet对象序列化到内存流  
            input.Seek(0, SeekOrigin.Begin);
            string DicName = rootDic;
            string filefullname = string.Format("{0}/{1}", DicName, filename);
            if (!Directory.Exists(DicName))
            {
                Directory.CreateDirectory(DicName);
            }
            else
            {
                if (File.Exists(filefullname))
                {
                    File.Delete(filefullname);
                }
            }


            FileStream output = new FileStream(filefullname, FileMode.Create);
            //fs.Write(buffer, 0, buffer.Length);

            //GZipOutputStream gzipStream = new GZipOutputStream(fs);//创建压缩对象 
            SevenZip.Compression.LZMA.Encoder coder = new SevenZip.Compression.LZMA.Encoder();
            coder.WriteCoderProperties(output);
		    output.Write(BitConverter.GetBytes(input.Length), 0, 8);
		    coder.Code(input, output, input.Length, -1, null);
            output.Flush();
            input.Close();//关闭内存流对象  
            input.Dispose();//释放资源  
            output.Close();//关闭流  
            output.Dispose();//释放对象  
            progressBar1.Value = 100;
            return;
        }
      
        private void AddColumnType(List<String> pdata,DataTable ptb)
        {
            DataRow dr = ptb.NewRow();
            for (int i = 0, len = pdata.Count; i < len; i++)
            {
                dr[i] = pdata[i];
            }
            ptb.Rows.Add(dr);
        }


        private void DataOutPutPathBtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                DataOutPutPath = folderBrowserDialog.SelectedPath;
                DataOutPutPathLab.Text = DataOutPutPath;
                SaveFilePath();
            }    
        }

        private bool checkOutPutPath(string path)
        {
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                return false;
            }
            return true;
        }


        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                DataOutPutLuaPath = folderBrowserDialog.SelectedPath;
                DataOutPutLuaPathLab.Text = DataOutPutLuaPath;
                SaveFilePath();
            }   
        }
    }
}
