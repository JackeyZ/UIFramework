using UnityEngine;
using UnityEditor;
using Excel;
using System.Data;
using System.IO;
using System;
using System.Text;

/// <summary>
/// 名称：Excel转换Json工具
/// 作用：把Excel文件转换成Json
/// </summary>
public class ExcelToJson : EditorWindow
{
    static UnityEngine.Object excelObj;
    static UnityEngine.Object outputDirectory;

    static string excelPath;
    static string jsonPath;

    static string defaultExcelPath = "Assets/Excel";
    static string defaultJsonPath = AssetBundleFramework.PathTool.JsonConfigDir; //"Assets/AssetBundleResources/JsonConfig";

    public static UnityEngine.Object ExcelObj
    {
        get
        {
            return excelObj;
        }

        set
        {
            if(value != null)
            {
                string path = AssetDatabase.GetAssetPath(value);
                if (Directory.Exists(path))
                {
                    excelPath = new DirectoryInfo(path).FullName;
                }
                else if (File.Exists(path) && (path.ToLower().EndsWith(".xlsx") || path.ToLower().EndsWith(".xls")))
                {
                    excelPath = new DirectoryInfo(path).FullName;
                }
                else
                {
                    excelPath = "文件有误！不是excel文件或文件夹";
                }

            }
            excelObj = value;
        }
    }
    public static UnityEngine.Object OutputDirectory
    {
        get
        {
            return outputDirectory;
        }

        set
        {
            if (value != null)
            {
                string path = AssetDatabase.GetAssetPath(value);
                if (Directory.Exists(path))
                {
                    jsonPath = new DirectoryInfo(path).FullName;
                }
                else
                {
                    jsonPath = "导出目录有误！！请选择文件夹。";
                }
            }
            outputDirectory = value;
        }
    }


    [UnityEditor.MenuItem("Window/ExcelToJson")]
    static void DisplayerExcelToJsonWindow()
    {
        ExcelToJson excelToJsonWindow = EditorWindow.GetWindow<ExcelToJson>();//获取窗口对象
        excelToJsonWindow.Show();

        // 读取目录
        ExcelObj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(defaultExcelPath);

        // 初始化输出目录
        UnityEngine.Object outputObj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(defaultJsonPath);
        if(outputObj == null)
        {
            int endIndex = defaultJsonPath.LastIndexOf("/");
            AssetDatabase.CreateFolder(defaultJsonPath.Substring(0, endIndex), defaultJsonPath.Substring(endIndex + 1));
            outputObj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(defaultJsonPath);
        }
        OutputDirectory = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(defaultJsonPath);
    }

    void OnGUI()
    {
        GUILayout.Label("Excel转换成Json");

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Excel文件或文件夹：");
        ExcelObj = EditorGUILayout.ObjectField(ExcelObj, typeof(UnityEngine.Object), true, GUILayout.Width(120)) as UnityEngine.Object;
        EditorGUILayout.EndHorizontal();
        GUILayout.Label(excelPath);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("导出文件夹：");
        OutputDirectory = EditorGUILayout.ObjectField(OutputDirectory, typeof(UnityEngine.Object), true, GUILayout.Width(120)) as UnityEngine.Object;
        EditorGUILayout.EndHorizontal();
        GUILayout.Label(jsonPath);
        
        
        if (GUILayout.Button("转换"))
        {
            string path = AssetDatabase.GetAssetPath(ExcelObj);
            string outputPath = AssetDatabase.GetAssetPath(OutputDirectory);
            DirectoryInfo outPutDirectoryInfo = new DirectoryInfo(outputPath);
            if (Directory.Exists(path))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                foreach (FileSystemInfo fileSysItem in directoryInfo.GetFileSystemInfos())
                {
                    if(fileSysItem is FileInfo && (fileSysItem.Extension == ".xlsx" || fileSysItem.Extension == ".xls"))
                    {
                        excelToJson(fileSysItem as FileInfo, outPutDirectoryInfo.FullName);
                    }
                }
            }
            else if(File.Exists(path))
            {
                FileInfo fileInfo = new FileInfo(path);
                if(fileInfo.Extension == ".xlsx" || fileInfo.Extension == ".xls")
                {
                    excelToJson(fileInfo, outPutDirectoryInfo.FullName);
                }
            }
            // Undo.RegisterCompleteObjectUndo(go, "create gameObject"); //注册到操作记录，就可以进行撤销
        }
    }

    private void excelToJson(FileInfo fileInfo, string outputPath)
    {
        string jsonStr = ConvertDataTableToJson(GetExcelTable(fileInfo.FullName));
        WrithJson(jsonStr, fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length), outputPath);
    }

    /// <summary>
    /// 获取excel表的所有分页
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    private DataTableCollection GetExcelTable(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return null;
        IExcelDataReader excelReader = null;

        try
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                if (fileName.ToLower().EndsWith(".xlsx"))
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                else if (fileName.ToLower().EndsWith(".xls"))
                    excelReader = ExcelReaderFactory.CreateBinaryReader(fs);

                if (excelReader == null) return null;
                DataSet result = excelReader.AsDataSet();

                fs.Close();
                fs.Dispose();
                excelReader.Close();
                return result.Tables;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return null;
        }
    }

    /// <summary>
    /// 将table转换为json
    /// </summary>
    /// <param name="sheetDataTable"></param>
    /// <returns></returns>
    public string ConvertDataTableToJson(DataTableCollection excelCollection)
    {
        StringBuilder sbs = new StringBuilder();
        sbs.Append("{\n");
        // 遍历分页
        for (int sheetIndex = 0; sheetIndex < excelCollection.Count; sheetIndex++)
        {
            DataTable sheetDataTable = excelCollection[sheetIndex];
            if (sheetDataTable.Rows.Count > 0)
            {
                sbs.Append("\t\"" + sheetDataTable.TableName + "\":[\n\t\t");
                string str = "";
                int startRow;
                int startCol;
                // 查找表头，确定开始行数和开始列数
                GetStartRowAndCol(sheetDataTable, out startRow, out startCol);
                DataRow startRowData = sheetDataTable.Rows[startRow];               // 开始的第一行，用作key值
                DataColumn startColData = sheetDataTable.Columns[startCol];         // 开始的第一列
                // 遍历行
                for (int row = startRow + 1; row < sheetDataTable.Rows.Count; row++)
                {
                    DataRow dr = sheetDataTable.Rows[row];
                    if (dr[startColData.ColumnName].ToString() == string.Empty)
                    {
                        break;
                    }
                    // 遍历列
                    string result = "";
                    for (int col = startCol; col < sheetDataTable.Columns.Count; col++)
                    {
                        DataColumn dc = sheetDataTable.Columns[col];
                        if (dr[dc.ColumnName].ToString() == string.Empty || startRowData[dc.ColumnName].ToString() == string.Empty)
                        {
                            break;
                        }
                        result += string.Format(",\n\t\t\t\"{0}\":\"{1}\"", startRowData[dc.ColumnName], dr[dc.ColumnName]);
                    }
                    result = result.Substring(1);
                    result = ",\n\t\t{" + result + "\n\t\t}";
                    str += result;
                }
                if(str == "")
                {
                    Debug.LogError(sheetDataTable.TableName + "没找到表头或表为空（表头名要和分页名相同）");
                }
                str = str.Substring(4);
                sbs.Append(str);
                if(sheetIndex == excelCollection.Count - 1)
                    sbs.Append("\n\t]");
                else
                    sbs.Append("\n\t],\n\t");
            }
        }
        sbs.Append("\n}");
        return sbs.ToString();
    }

    /// <summary>
    /// 查找表头，确定开始行数和开始列数，表头内容需和分页名相同
    /// </summary>
    /// <param name="excelDataTable"></param>
    /// <param name="startRow"></param>
    /// <param name="startCol"></param>
    public void GetStartRowAndCol(DataTable excelDataTable, out int startRow, out int startCol)
    {
        startRow = 0;
        startCol = 0;
        foreach (DataRow dr in excelDataTable.Rows)
        {
            startCol = 0;
            foreach (DataColumn dc in excelDataTable.Columns)
            {
                if (dr[dc.ColumnName].ToString() == excelDataTable.TableName)
                {
                    startCol++;
                    return;
                }
                startCol++;
            }
            startRow++;
        }
    }

    /// <summary>
    /// 写入Json数据到Json文件中
    /// </summary>
    public void WrithJson(string jsonStr, string fileName, string outputPath)
    {
        string path = outputPath + "/" + fileName + ".json";

        //System.IO.File.Exists(path);
        FileStream fs1 = new FileStream(path, FileMode.Create, FileAccess.Write);//搜索创建写入文件 
        StreamWriter sw = new StreamWriter(fs1);

        sw.Write(jsonStr);

        sw.Close();
        fs1.Close();

        //刷新
        AssetDatabase.Refresh();

        //打开对应文件
        //System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
        //info.FileName = path;
        //System.Diagnostics.Process.Start(info);
    }
}
