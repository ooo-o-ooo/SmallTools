using System;
using System.Collections.Generic;
using System.ComponentModel;
using Sunny.UI;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using 取证文件拷贝工具.cs;

namespace 取证文件拷贝工具
{
    public partial class Form1 : UIForm
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(Setting.Current.PathTemp);
        public Form1()
        {
            InitializeComponent();
            Init();
            

        }

        private void UiComboDataGridView1_SelectIndexChange(object sender, int index)
        {
            uiComboDataGridView1.Text = directoryInfo.Name[index].ToString();
            MessageBox.Show(directoryInfo.Name[index].ToString());
        }


        /// <summary>
        /// 获取磁盘信息
        /// </summary>    
        public void initDriverData()
        {

            uiComboBox1.ValueMember = "Name";
            uiComboBox1.DisplayMember = "VolumeLabel";
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                uiComboBox1.Items.Add(new DriveInfoWrapper(drive.VolumeLabel, drive.Name));
            }
        }
        /// <summary>
        /// 获取案件列表
        /// </summary>
        public void initDiskData(int flag)
        {
            /*读取配置文件*/
            try
            {
                // 获取目录下的文件夹列表
                DirectoryInfo dirInfo = new DirectoryInfo(Setting.Current.PathTemp);
                DirectoryInfo[] sortedFolders = null;

                /// <summary>
                /// 根据不同的标志（flag）对目录进行排序。
                /// </summary>
                /// <param name="flag">排序标志：
                /// 1 - 按目录名称升序排序；
                /// 2 - 按目录名称降序排序；
                /// 3 - 按目录创建时间升序排序；
                /// 4 - 按目录创建时间降序排序。</param>
                /// <returns>排序后的目录数组。</returns>
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


                uiComboBox2.DisplayMember = "Name";
                foreach (DirectoryInfo folder in sortedFolders)
                {
                    uiComboBox2.Items.Add(folder);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }


        }




        /// <summary>
        /// 将消息记录到调试窗口。
        /// </summary>
        /// <param name="message">要记录的消息内容。</param>
        private void LogToDebugWindow(string message)
        {
            if (rtbDebugLog.InvokeRequired)
            {
                var d = new Action<string>(LogToDebugWindow);
                rtbDebugLog.Invoke(d, new object[] { message });
            }
            else
            {
                rtbDebugLog.AppendText(message + Environment.NewLine);
                rtbDebugLog.ScrollToCaret(); // 滚动到最新添加的文本处
            }
        }

        /// <summary>
        /// 当用户点击uiButton2按钮时触发的事件处理程序。
        /// </summary>
        /// <param name="sender">触发事件的对象。</param>
        /// <param name="e">包含事件数据的EventArgs对象。</param>
        private void uiButton2_Click(object sender, EventArgs e)
        {

            uiComboBox2.Items.Clear();
            int flag = Setting.Current.Flag;
            if (flag<4)
            {
                flag++;
            }
            else
            {
                flag = 1;
            }
            Setting.Current.Flag = flag;
            Setting.Current.Save();
            Init();
        }
        /// <summary>
        /// 启动指定路径的可执行文件，并传递参数。
        /// </summary>
        /// <param name="exePath">可执行文件的完整路径。</param>
        /// <param name="arguments">传递给可执行文件的参数。</param>
        private void StartProcess(string exePath, string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding= Encoding.GetEncoding("UTF-8"),
                StandardErrorEncoding = Encoding.GetEncoding("UTF-8")
            };

            try
            {
                using (Process process = Process.Start(startInfo))
                {
                    if (process != null)
                    {
                        string output = process.StandardOutput.ReadToEnd();
                        string error = process.StandardError.ReadToEnd();
                        process.WaitForExit();
                        // 处理输出和错误
                        LogToDebugWindow(output);
                        if (!string.IsNullOrEmpty(error))
                        {
                            LogToDebugWindow("Error:"+error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        /// <summary>
        /// 当用户点击uiButton1按钮时触发的事件处理程序。
        /// </summary>
        /// <param name="sender">触发事件的对象。</param>
        /// <param name="e">包含事件数据的EventArgs对象。</param>
        private void uiButton1_Click(object sender, EventArgs e)
        {

            this.ShowWaitForm("正在复制，请耐心等待...");
            LogToDebugWindow(
            MyClassForm.FcpConsole(Setting.Current.FcpCMD, Setting.Current.PathTemp + "\\" + uiComboBox2.SelectedItem.ToString(), uiComboBox1.SelectedItem.ToString())
                );
            this.HideWaitForm();
        }

        private void uiButton5_Click(object sender, EventArgs e)
        {
            string exePath = AppDomain.CurrentDomain.BaseDirectory + @"tools\FastCopy\fcp.exe";
            StringBuilder argumentsBuilder = new StringBuilder();
            argumentsBuilder.Append(Setting.Current.FcpCMD);
            argumentsBuilder.Append(" \"");
            argumentsBuilder.Append(Setting.Current.PathTemp + "\\" + uiComboBox2.SelectedItem.ToString());
            argumentsBuilder.Append("\" ");
            argumentsBuilder.Append("/to=");
            argumentsBuilder.Append("\"");

/*            if (uiComboBox1.SelectedItem != null)
            {
                DriveInfoWrapper selectedItems = (DriveInfoWrapper)uiComboBox1.SelectedItem;
                selectedItem = selectedItems.ActualValue;
                LogToDebugWindow("Selected drive name: " + selectedItem);

            }*/
            argumentsBuilder.Append(new DriveInfoWrapper(uiComboBox1.SelectedItem.ToString(),"").ActualValue);
            argumentsBuilder.Append("\" ");

            this.ShowWaitForm("正在复制，请耐心等待...");
            string arguments = argumentsBuilder.ToString();
            LogToDebugWindow("命令：" + arguments);
            StartProcess(exePath, arguments);
            this.HideWaitForm();
        }
        public override void Init()
        {

            uiComboBox1.Items.Clear();
            uiComboBox2.Items.Clear();
            uiComboDataGridView1.DataGridView.Columns.Clear();
            ///初始化
            initDriverData();
            initDiskData(Setting.Current.Flag);

            uiComboBox1.SelectedIndex = 0;
            uiComboBox2.SelectedIndex = 0;
            DataTable directoryTable = MyClassForm.ConvertDirectoryInfoToDataTable(directoryInfo, Setting.Current.Flag);
            uiComboDataGridView1.Text = directoryTable.Rows[0][0].ToString();
            uiComboDataGridView1.DataGridView.Init();
            uiComboDataGridView1.ItemSize = new System.Drawing.Size(60, 40);
            uiComboDataGridView1.DataGridView.AddColumn("案件目录", "Name");
            uiComboDataGridView1.DataGridView.AddColumn("创建时间", "CreationTime");
            uiComboDataGridView1.FilterColumnName = "Name";
            uiComboDataGridView1.DataGridView.ReadOnly = true;
            uiComboDataGridView1.ShowFilter = true;
            uiComboDataGridView1.DataGridView.DataSource = directoryTable;//用DataTable做数据源过滤，用List不行
        }

        private void 设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserSettings frm = new UserSettings();
            frm.Render();
            frm.ShowDialog();
            if (frm.IsOK)
            {
                this.ShowSuccessDialog("OK");
            }
            frm.Dispose();
            Init();
        }

        private void uiButton8_Click(object sender, EventArgs e)
        {
            Init();
        }
        

        private void uiComboDataGridView1_ValueChanged(object sender, object value)
        {
            uiComboDataGridView1.Text = "";
            if (value != null && value is DataGridViewRow)
            {
                DataGridViewRow row = (DataGridViewRow)value;
                uiComboDataGridView1.Text = row.Cells["案件目录"].Value.ToString();//通过ColumnName显示值
            }
        }

        private void uiComboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            
        }

        private void uiComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            object selectedItem = "";
            this.uiLabel1.Text = "";

            if (uiComboBox1.SelectedItem != null)
            {
                DriveInfoWrapper selectedItems = (DriveInfoWrapper)uiComboBox1.SelectedItem;
                selectedItem = selectedItems.ActualValue;
            }
            this.uiLabel1.Text = MyClassForm.ShowDiskInfo(selectedItem);

            LogToDebugWindow("获取 ：" + selectedItem.ToString());
            LogToDebugWindow("D:" + uiComboBox1.SelectedText.ToString());
        }
    }

}
