using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Sunny.UI;
using System.Threading.Tasks;
using System.Diagnostics;

namespace 取证文件拷贝工具.cs
{
    public class MyClassForm
    {
        Form1 form1 = new Form1();

        public void InitForm()
        {

        }


        public static string FcpConsole(string cmd,string sourcePath, string toPath)
        {
            string exePath = AppDomain.CurrentDomain.BaseDirectory + @"tools\FastCopy\fcp.exe";
            StringBuilder argumentsBuilder = new StringBuilder();
            argumentsBuilder.Append(cmd);
            argumentsBuilder.Append(" \"");
            argumentsBuilder.Append(sourcePath);
            argumentsBuilder.Append("\" ");
            argumentsBuilder.Append("/to=");
            argumentsBuilder.Append("\"");
            argumentsBuilder.Append(toPath);
            argumentsBuilder.Append("\" ");
            string arguments = argumentsBuilder.ToString();
            return StartProcess(exePath, arguments).ToString();
        }
        private static string StartProcess(string exePath, string arguments)
        {
            string output = "";
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.GetEncoding("UTF-8"),
                StandardErrorEncoding = Encoding.GetEncoding("UTF-8")
            };

            try
            {
                using (Process process = Process.Start(startInfo))
                {
                    if (process != null)
                    {
                        output = process.StandardOutput.ReadToEnd();
                        string error = process.StandardError.ReadToEnd();
                        process.WaitForExit();
                        // 处理输出和错误
                        if (!string.IsNullOrEmpty(error))
                        {
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            return output;
        }
        private void ShowWaitForm(string v)
        {
            throw new NotImplementedException();
        }

        public static String ShowDiskInfo(object param)
        {

            StringBuilder argumentsBuilder = new StringBuilder();
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.ToString().Equals(param))
                {
                    argumentsBuilder.Append("盘符：");
                    argumentsBuilder.Append(drive.Name);
                    argumentsBuilder.Append("   名称：");
                    argumentsBuilder.Append(drive.VolumeLabel);
                    argumentsBuilder.Append("   容量：");
                    argumentsBuilder.Append(drive.TotalSize / (1000 * 1000 * 1000));
                    argumentsBuilder.Append("GB   剩余：");
                    argumentsBuilder.Append(drive.TotalFreeSpace / (1000 * 1000 * 1000));
                    argumentsBuilder.Append("GB");

                }
            }
            return argumentsBuilder.ToString();
        }

        /// <summary>
        /// 将 DirectoryInfo 对象转换为 DataTable 对象。
        /// 根据 flag 参数的不同，可以按目录名称或创建时间进行升序或降序排序。
        /// </summary>
        /// <param name="directoryInfo">要转换的 DirectoryInfo 对象。</param>
        /// <param name="flag">排序标志：
        /// 1 - 按目录名称升序；
        /// 2 - 按目录名称降序；
        /// 3 - 按创建时间升序；
        /// 4 - 按创建时间降序。</param>
        /// <returns>填充了目录信息的 DataTable 对象。</returns>
        public static DataTable ConvertDirectoryInfoToDataTable(DirectoryInfo directoryInfo, int flag)
        {
            
            DataTable dt = new DataTable("DirectoryContents");
            dt.Columns.Add("Name", typeof(string)); // 文件或子目录的名称
            dt.Columns.Add("CreationTime", typeof(DateTime)); // 创建时间
            dt.Columns.Add("LastWriteTime", typeof(DateTime)); // 最后写入时间
            DirectoryInfo dirInfo = new DirectoryInfo(Setting.Current.PathTemp);
            DirectoryInfo[] sortedFolders = null;
            switch (flag)
            {
                case 1:
                    sortedFolders = dirInfo.GetDirectories().OrderBy(d => d.Name).ToArray();
                    break;
                case 2:
                    sortedFolders = dirInfo.GetDirectories().OrderByDescending(d => d.Name).ToArray();
                    break;
                case 3:
                    sortedFolders = dirInfo.GetDirectories().OrderBy(d => d.CreationTime).ToArray();
                    break;
                case 4:
                    sortedFolders = dirInfo.GetDirectories().OrderByDescending(d => d.CreationTime).ToArray();
                    break;
                default:
                    break;
            }
            foreach (DirectoryInfo folder in sortedFolders)
            {
                DataRow row = dt.NewRow();
                row["Name"] = folder.Name;
                row["CreationTime"] = folder.CreationTime;
                row["LastWriteTime"] = folder.LastWriteTime;
                dt.Rows.Add(row);
            }

            return dt; // 返回填充了数据的 DataTable

        }
    }

    public class DiskList
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Grade { get; set; }

        // 构造函数
        public DiskList(string name, int age, string grade)
        {
            Name = name;
            Age = age;
            Grade = grade;
        }
    }
}
