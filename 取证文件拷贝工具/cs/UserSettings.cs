using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sunny.UI;

namespace 取证文件拷贝工具.cs
{
    public partial class UserSettings : UIEditForm
    {
        
        public UserSettings()
        {
            InitializeComponent();
            init();
        }

        public void init() {
            this.uiTextBox1.Text = Setting.Current.PathTemp;
            this.uiTextBox2.Text = Setting.Current.PathReport;
        }

        private void uiTextBox1_ButtonClick(object sender, EventArgs e)
        {

            string dir = "";
            if (DirEx.SelectDirEx("扩展打开文件夹", ref dir))
            {
                Setting.Current.PathTemp = dir;
                Setting.Current.Save();
            }
            init();
        }
        private void uiTextBox2_ButtonClick(object sender, EventArgs e)
        {
            string dir = "";
            if (DirEx.SelectDirEx("扩展打开文件夹", ref dir))
            {
                Setting.Current.PathReport = dir;
                Setting.Current.Save();
            }
            init();
        }

    }
}
