using System;
using System.IO;
using System.Reflection.Emit;

namespace 取证文件拷贝工具.cs
{
    public class DriveInfoWrapper
    {
        private string Name { get; set; }
        private string VolumeLabel { get; set; }

        public DriveInfoWrapper(DriveInfo drive)
        {
            Name = drive.Name;
            VolumeLabel = drive.VolumeLabel;
        }

        public override string ToString()
        {
            return $"{VolumeLabel}( {Name})";
        }

        // 可选：如果你需要在其他地方使用 DriveInfo 的其他属性，可以在这里添加
        public string ActualValue => Name+DateTime.Now.ToString("yyyy_MM_dd_HH_mm"); // 例如，提供一个获取实际驱动器名称的属性
        
    }
}
