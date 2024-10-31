using MiniSoftware;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace 鉴定事项确认书
{
    public partial class Form1 : UIForm
    {
        public Form1()
        {
            InitializeComponent();

        }
        GetInfo getInfo = new GetInfo();

        private void uiButton1_Click(object sender, System.EventArgs e)
        {
        }

        private void uiButton1_Click_1(object sender, System.EventArgs e)
        {
            string allText = uiTextBox13.Text;
            string pattern = @"\s+";
            string result = Regex.Replace(allText, pattern, "");//去除所有空白符
            List<string> getuserName = getInfo.getUserName(result);
            List<string> getUserNumber = getInfo.getUserNumber(result);
            List<string> getUserPhone = getInfo.getUserPhone(result);
            string jdid = getInfo.getJDID(result);
            string workSpace = getInfo.getWorkSpace(result);
            string anJianName = getInfo.getAnJianName(result);
            string anJianNum = getInfo.getAnJianNum(result);
            string anjianZhaiYao = getInfo.getAnJianZhaiYao(result);
            string jianCaiInfo = getInfo.getJianCaiInfo(result);
            if (getuserName.Count == 0
                || getUserPhone.Count == 0
                || getUserNumber.Count == 0
                || anJianName == null
                || anjianZhaiYao == null)
            {
                this.ShowErrorDialog2("未能识别有效信息，请按照以下提示操作：\n 1. 选中具体案件，操作-打印委托书 \n 2. CTRL + A 全选, CTRL+C 复制 \n 3. 粘贴在本程序中点击【解析文本】按钮");
                return;
            }
            else
            {
                uiTextBox3.Text = getuserName[0];
                uiTextBox4.Text = getuserName[1];
                uiTextBox6.Text = getUserNumber[0];
                uiTextBox5.Text = getUserNumber[1];
                uiTextBox8.Text = getUserPhone[0];
                uiTextBox7.Text = getUserPhone[1];
                uiTextBox1.Text = jdid;
                uiTextBox2.Text = workSpace;
                uiTextBox9.Text = anJianName;
                uiTextBox10.Text = anJianNum;
                uiTextBox11.Text = anjianZhaiYao;
                uiTextBox12.Text = jianCaiInfo;
                if (this.ShowAskDialog("提示", " 成功解析文本 \n 点击确定打开文档 \n 点击取消预览信息"))
                {
                    uiButton2_Click(sender, e);
                }
                else
                {
                    this.ShowInfoTip("查看左侧窗口预览");
                }
            }
        }

        private void uiButton2_Click(object sender, System.EventArgs e)
        {
            if(uiTextBox1.TextLength <=0 || uiTextBox3.TextLength<=0 || uiTextBox4.TextLength <= 0|| 
                uiTextBox5.TextLength <= 0 || uiTextBox6.TextLength <= 0|| uiTextBox7.TextLength <= 0|| 
                uiTextBox8.TextLength <= 0)
            {
                this.ShowErrorDialog2("未能识别有效信息，请按照以下提示操作：\n 1. 选中具体案件，操作-打印委托书 \n 2. CTRL + A 全选, CTRL+C 复制 \n 3. 粘贴在本程序中点击【解析文本】按钮");
                return;
            }
            String dataTime = DateTime.Now.ToString("yyyyMMdd");
            String yy = DateTime.Now.ToString("yy");
            String mm = DateTime.Now.ToString("MM");
            String dd = DateTime.Now.ToString("dd");
            var value = new Dictionary<string, object>()
            {
                ["受理编号"] = uiTextBox1.Text,
                ["委托单位"] = uiTextBox2.Text,
                ["送检N1"] = uiTextBox3.Text,
                ["送检N2"] = uiTextBox4.Text,
                ["送检I1"] = uiTextBox6.Text,
                ["送检I2"] = uiTextBox5.Text,
                ["送检P1"] = uiTextBox8.Text,
                ["送检P2"] = uiTextBox7.Text,
                ["案件名"] = uiTextBox9.Text,
                ["案件号"] = uiTextBox10.Text,
                ["简要案情"] = uiTextBox11.Text,
                ["检材情况"] = uiTextBox12.Text,
                ["yy"] = yy,
                ["mm"] = mm,
                ["dd"] = dd,
            };
            MiniWord.SaveAsByTemplate("output/" + uiTextBox9.Text + dataTime + ".doc", "./config/Template.dotx", value);
            System.Diagnostics.Process.Start(Application.StartupPath + "\\output\\" + uiTextBox9.Text + dataTime + ".doc");
        }

        private void uiButton3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Application.StartupPath + "\\output\\");
        }

        private void uiTextBox13_Leave(object sender, EventArgs e)
        {
        }
        /// <summary>
        /// 窗口激活时，检查剪切板。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Activated(object sender, EventArgs e)
        {

            if (Clipboard.ContainsText())
            {

                string result = Clipboard.GetText();
                string[] keywords = { "委托鉴定单位", "泌阳县公安局物证鉴定室", "鉴定机构名称", "我单位向你鉴定机构介绍的情况客观真实，提交的检材和样本等来源清楚可靠。" };
                bool allKeywordsFound = true;
                foreach (string keyword in keywords)
                {
                    if (!result.Contains(keyword))
                    {
                        allKeywordsFound = false;
                        break; // 如果找到一个不存在的关键字，就停止检查
                    }
                }
                if (allKeywordsFound) {

                    uiTextBox13.Text = result;
                    if (this.ShowAskDialog2("识别到鉴定书文本，是否解析"))
                    {
                        uiButton1_Click_1(sender, e);
                    }
                    else 
                    {

                    }
                    Clipboard.Clear();

                }
            }
        }

        private void 帮助ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UIMessageBox.Show(" 1. 选中具体案件，操作-打印委托书 \n 2. CTRL + A 全选, CTRL+C 复制 \n 3. 打开本程序，自动识别剪切板是否为鉴定书内容，点击确定自动填充，点击取消则只会粘贴到输入框中 \n 4. 粘贴在本程序中点击【解析文本】按钮，根据提示进行下一步操作","帮助");
        }
    }
}
