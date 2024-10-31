using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 取证文件拷贝工具.cs
{
    public class DriveInfoWrapper
    {

        public string DisplayValue { get; set; } 
        public object ActualValue { get; set; }

        public DriveInfoWrapper(string displayValue, object actualValue)
        {
            DisplayValue = displayValue;
            ActualValue = actualValue;
        }
        public override string ToString()
        {
            return DisplayValue;
        }

    }
}
