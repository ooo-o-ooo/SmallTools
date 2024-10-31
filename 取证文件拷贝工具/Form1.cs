using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sunny.UI;
using 取证文件拷贝工具.cs;

namespace 取证文件拷贝工具
{
    public partial class Form1 : UIForm
    {
        private readonly DirectoryInfo directoryInfo = new DirectoryInfo(Setting.Current.PathTemp);

        public Form1()
        {
            InitializeComponent();
            InitForm() ;
        }

        private void UiComboDataGridView1_SelectIndexChange(object sender, int index)
        {
            uiComboDataGridView1.Text = directoryInfo.Name[index].ToString();
            MessageBox.Show(directoryInfo.Name[index].ToString());
        }


        /// <summary>
        ///     获取磁盘信息
        /// </summary>
        private void initDriverData()
        {
            foreach (var drive in DriveInfo.GetDrives()) uiComboBox1.Items.Add(new DriveInfoWrapper(drive));
        }

        /// <summary>
        ///     获取案件列表
        /// </summary>
        private void initDiskData(int flag)
        {
            /*读取配置文件*/
            try
            {
                // 获取目录下的文件夹列表
                var dirInfo = new DirectoryInfo(Setting.Current.PathTemp);
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
                }


                uiComboBox2.DisplayMember = "Name";
                foreach (var folder in sortedFolders) uiComboBox2.Items.Add(folder);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }


        /// <summary>
        ///     将消息记录到调试窗口。
        /// </summary>
        /// <param name="message">要记录的消息内容。</param>
        private void LogToDebugWindow(string message)
        {
            if (rtbDebugLog.InvokeRequired)
            {
                var d = new Action<string>(LogToDebugWindow);
                rtbDebugLog.Invoke(d, message);
            }
            else
            {
                rtbDebugLog.AppendText(message + Environment.NewLine);
                rtbDebugLog.ScrollToCaret(); // 滚动到最新添加的文本处
            }
        }

        /// <summary>
        ///     当用户点击uiButton2按钮时触发的事件处理程序。
        /// </summary>
        /// <param name="sender">触发事件的对象。</param>
        /// <param name="e">包含事件数据的EventArgs对象。</param>
        private void uiButton2_Click(object sender, EventArgs e)
        {
            uiComboBox2.Items.Clear();
            var flag = Setting.Current.Flag;
            if (flag < 4)
                flag++;
            else
                flag = 1;
            Setting.Current.Flag = flag;
            Setting.Current.Save();
            InitForm();
        }

        /// <summary>
        ///     启动指定路径的可执行文件，并传递参数。
        /// </summary>
        /// <param name="exePath">可执行文件的完整路径。</param>
        /// <param name="arguments">传递给可执行文件的参数。</param>
        private void StartProcess(string exePath, string arguments)
        {
            var startInfo = new ProcessStartInfo
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
                using (var process = Process.Start(startInfo))
                {
                    if (process != null)
                    {
                        var output = process.StandardOutput.ReadToEnd();
                        var error = process.StandardError.ReadToEnd();
                        process.WaitForExit();
                        // 处理输出和错误
                        LogToDebugWindow(output);
                        if (!string.IsNullOrEmpty(error)) LogToDebugWindow("Error:" + error);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        /// <summary>
        ///     当用户点击uiButton1按钮时触发的事件处理程序。
        /// </summary>
        /// <param name="sender">触发事件的对象。</param>
        /// <param name="e">包含事件数据的EventArgs对象。</param>
        private void 获取照片_Click(object sender, EventArgs e)
        {
            var selectedItems = (DriveInfoWrapper)uiComboBox1.SelectedItem;
            this.ShowWaitForm("正在复制，请耐心等待...");
            LogToDebugWindow(
                MyClassForm.FcpConsole(Setting.Current.FcpCMD,
                    Setting.Current.PathTemp + "\\" + uiComboDataGridView1.Text, selectedItems.ActualValue)
            );
            this.HideWaitForm();
        }

        private void 案件报告_Click(object sender, EventArgs e)
        {
            var selectedItems = (DriveInfoWrapper)uiComboBox1.SelectedItem;
            this.ShowWaitForm("正在复制，请耐心等待...");
            LogToDebugWindow(
                MyClassForm.FcpConsole(Setting.Current.FcpCMD,
                    Setting.Current.PathReport + "\\" + uiComboDataGridView1.Text, selectedItems.ActualValue)
            );
            this.HideWaitForm();
        }

        public void InitForm()
        {
            uiComboBox1.Items.Clear();
            uiComboBox2.Items.Clear();
            uiComboDataGridView1.DataGridView.Columns.Clear();
            ///初始化
            initDriverData();
            initDiskData(Setting.Current.Flag);

            uiComboBox1.SelectedIndex = 0;
            uiComboBox2.SelectedIndex = 0;
            var directoryTable = MyClassForm.ConvertDirectoryInfoToDataTable(directoryInfo, Setting.Current.Flag);
            uiComboDataGridView1.Text = directoryTable.Rows[0][0].ToString();
            uiComboDataGridView1.DataGridView.Init();
            uiComboDataGridView1.ItemSize = new Size(60, 40);
            uiComboDataGridView1.DataGridView.AddColumn("案件目录", "Name");
            uiComboDataGridView1.DataGridView.AddColumn("创建时间", "CreationTime");
            uiComboDataGridView1.FilterColumnName = "Name";
            uiComboDataGridView1.DataGridView.ReadOnly = true;
            uiComboDataGridView1.ShowFilter = true;
            uiComboDataGridView1.DataGridView.DataSource = directoryTable; //用DataTable做数据源过滤，用List不行



            uiComboDataGridView2.Text = directoryTable.Rows[0][0].ToString();
            uiComboDataGridView2.DataGridView.Init();
            uiComboDataGridView2.DataGridView.MultiSelect = true;//设置可多选
            uiComboDataGridView2.ItemSize = new Size(60, 40);
            uiComboDataGridView2.DataGridView.AddColumn("案件目录", "Name");
            uiComboDataGridView2.DataGridView.AddColumn("创建时间", "CreationTime");
            uiComboDataGridView2.FilterColumnName = "Name";
            uiComboDataGridView2.DataGridView.ReadOnly = true;
            uiComboDataGridView2.ShowFilter = true;
            uiComboDataGridView2.DataGridView.DataSource = directoryTable; //用DataTable做数据源过滤，用List不行

        }

        private void 设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frm = new UserSettings();
            frm.Render();
            frm.ShowDialog();
            if (frm.IsOK) this.ShowSuccessDialog("OK");
            frm.Dispose();
            InitForm();
        }

        private void uiButton8_Click(object sender, EventArgs e)
        {
            InitForm();
        }


        private void uiComboDataGridView1_ValueChanged(object sender, object value)
        {
            uiComboDataGridView1.Text = "";
            if (value != null && value is DataGridViewRow)
            {
                var row = (DataGridViewRow)value;
                uiComboDataGridView1.Text = row.Cells["案件目录"].Value.ToString(); //通过ColumnName显示值
            }
        }

        private void uiComboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
        }

        private void uiComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            object selectedItem = "";
            uiLabel1.Text = "";

            if (uiComboBox1.SelectedItem != null)
            {
                var selectedItems = (DriveInfoWrapper)uiComboBox1.SelectedItem;
                selectedItem = selectedItems.ActualValue;
            }

            uiLabel1.Text = MyClassForm.ShowDiskInfo(selectedItem);

            LogToDebugWindow("目标路径：" + selectedItem);
        }

        private void uiComboDataGridView2_ValueChanged(object sender, object value)
        {
            uiComboDataGridView2.Text = "";
            if (value != null && value is DataGridViewSelectedRowCollection)
            {
                DataGridViewSelectedRowCollection collection = (DataGridViewSelectedRowCollection)value;
                foreach (var item in collection)
                {
                    DataGridViewRow row = (DataGridViewRow)item;
                    uiComboDataGridView2.Text += row.Cells[0].Value.ToString();//通过索引显示值
                    uiComboDataGridView2.Text += "; ";
                }
            }
        }
    }
}