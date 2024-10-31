using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Linq;


namespace 鉴定事项确认书
{
    class GetInfo
    {
        public List<string> getUserName(String result)
        {
            List<string> userNameList = new List<string>();
            string pattern = @"姓名(.*?)职务";
            MatchCollection matchs = Regex.Matches(result, pattern);
            if (matchs.Count > 0)
            {
                foreach (Match match in matchs)
                {
                    userNameList.Add(match.Groups[1].Value);
                }
            }
            return userNameList;
        }
        public List<string> getUserNumber (String result)
        {
            List<string> userNumberList = new List<string>();
            string pattern =  @"警官证/(\w{6})";
            MatchCollection matchs = Regex.Matches(result, pattern);
            if (matchs.Count>0)
            {
                foreach (Match match in matchs)
                {
                    userNumberList.Add(match.Groups[1].Value);
                }
            }
            return userNumberList;
        }
        public List<string> getUserPhone(String result)
        {
            List<string> userPhoneList = new List<string>();
            string pattern = @"联系电话(.*?)传真号码";
            Match match = Regex.Match(result, pattern);

            if (match.Success)
            {
                String phones = match.Groups[1].Value;
                string[] phone = phones.Split("/");
                foreach (String phoneStr in phone)
                {
                    userPhoneList.Add(phoneStr);
                }

            }
            return userPhoneList;
        }
        public string getJDID(String result)
        {
            string pattern = @"委托书编号：(.*?)委托鉴定单位";
            Match match = Regex.Match(result, pattern);
            if (match.Success)
            {

                String jdID = match.Groups[1].Value;
                return jdID;
            }
            return null;
        }
        //委托单位
        public string getWorkSpace(String result)
        {
            string matchJDID = @"委托鉴定单位(.*?)委托时间";
            Match match = Regex.Match(result, matchJDID);
            String workSpace = match.Groups[1].Value;
            return workSpace;
        }
        //案件名
        public string getAnJianName(String result)
        {
            string matchAnJian = @"案（事）件名称(.*?)案件编号";
            Match match = Regex.Match(result, matchAnJian);
            String anJianName = match.Groups[1].Value;
            return anJianName;
        }
        //案件编号
        public string getAnJianNum(String result)
        {
            string matchAnJianNum = @"案件编号(.*?)被鉴定人的情况";
            Match match = Regex.Match(result, matchAnJianNum);
            String anJianNum = match.Groups[1].Value;
            return anJianNum;
        }
        //案件摘要
        public string getAnJianZhaiYao(String result)
        {
            string matchAnJianZhaiYao = @"件简要情况(.*?)原鉴定情况";
            Match match = Regex.Match(result, matchAnJianZhaiYao);
            String anJianZhaiYao = match.Groups[1].Value;
            return anJianZhaiYao;
        }
        //检材信息
        public string getJianCaiInfo(String result)
        {
            string pattern = @"提取部位方法(.*?)委托鉴定单位的鉴";
            string[] jianCaiInfo = {};
            Match match = Regex.Match(result, pattern);
            if (match.Success)
            {
                String temp = match.Groups[1].Value;
                jianCaiInfo = temp.Split("第1页,共2页");
                return jianCaiInfo[0];
            }
            return "";
        }

        
    }
}