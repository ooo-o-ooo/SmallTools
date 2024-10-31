using Sunny.UI;

namespace 取证文件拷贝工具.cs 
{
    [ConfigFile("Config\\Setting.ini")]
    public class Setting : IniConfig<Setting>
    {
        [ConfigSection("Setup")]
        public string PathTemp { get; set; }

        public string PathReport { get; set; }

        public string FcpCMD { get; set; }
        public int Flag { get; set; }


        public override void SetDefault()
        {
            base.SetDefault();
            PathTemp = @"D:\";
            PathReport = @"D:\";
            FcpCMD = "/cmd=diff";
            Flag = 1;
        }
    }
}
