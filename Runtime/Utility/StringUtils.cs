using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Utils
{
    public static class StringUtils
    {
        public static char[] CommaSplit = new char[1] { ';' };

        public static List<string> _emojiPatten = new List<string>() { @"\p{Cs}", @"\p{Co}", @"\p{Cn}", @"[\u2702-\u27B0]" };
        /// <summary>
        /// 向下取整
        /// </summary>
        /// <param name="num"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static string ToFixedFloor(int num, int rate)
        {
            return (num / rate).ToString();
        }

        public static string ToFixed(int num, int rate)
        {
            var numStr = (num * 1.0f / rate).ToString();
            return ToFixed(numStr);
        }

        public static string ToFixed(long num, long rate)
        {
            var numStr = (num * 1.0f / rate).ToString();
            return ToFixed(numStr);
        }

        public static string ToFixed(string numStr, int fillNum = -1)
        {
            var _sb = new StringBuilder();
            int i = 0;
            for (; i < numStr.Length; i++)
            {
                if (numStr[i] == '.')
                {
                    break;
                }
                _sb.Append(numStr[i]);
            }
            i++;
            if (i < numStr.Length && numStr[i] != '0')
            {
                _sb.Append('.');
                _sb.Append(numStr[i]);
            }
            fillNum = fillNum - _sb.Length;
            if (fillNum > 0)
            {
                char[] t = new char[fillNum];
                for (int j = 0; j < fillNum; j++)
                {
                    t[j] = ' ';
                }
                _sb.Insert(0,t);
            }
            var str = _sb.ToString();
            return str;
        }

        public static bool IsChinese()
        {
            return true;
            // string lang = LangUtils.curLang;
            // return lang.StartsWith("zh") || lang == "local" || lang == "test" || lang == "banshu";
        }

        public static string FormatNumber(long num)
        {
            var result = num.ToString();
            var lang = "zh_cn";//LangUtils.curLang.Replace("_test", "");
            switch (lang)
            {
                case "zh_cn":
                    // 中文万进制
                    if (num >= 100000000)
                    {
                        result = $"{ToFixed(num, 100000000)}亿";
                    }
                    else if (num >= 10000)
                    {
                        result = $"{ToFixed(num, 10000)}万";
                    }
                    else
                    {
                        result = ToFixed(num, 1);
                    }
                    break;
                case "zh_tw":
                    if (num >= 100000000)
                    {
                        result = $"{ToFixed(num, 100000000)}億";
                    }
                    else if (num >= 10000)
                    {
                        result = $"{ToFixed(num, 10000)}萬";
                    }
                    else
                    {
                        result = ToFixed(num, 1);
                    }
                    break;
                default:
                    //其它国家千进制
                    if (num >= 1000000000)
                    {
                        result = $"{ToFixed(num, 1000000000)}B";
                    }
                    else if (num >= 1000000)
                    {
                        result = $"{ToFixed(num, 1000000)}M";
                    }
                    else if (num >= 1000)
                    {
                        result = $"{ToFixed(num, 1000)}K";
                    }
                    else
                    {
                        result = ToFixed(num, 1);
                    }
                    break;
            }
            
            return result;
        }

        public static string FormatNumber(int num)
        {
            var result = num.ToString();
            if (IsChinese())
            {
                if (num >= 100000000)
                {
                    result = $"{ToFixed(num, 100000000)}亿";
                }
                else if (num >= 10000)
                {
                    result = $"{ToFixed(num, 10000)}万";
                }
                else
                {
                    result = ToFixed(num, 1);
                }
            }
            else
            {
                if (num >= 1000000000)
                {
                    result = $"{ToFixed(num, 1000000000)}B";
                }
                else if (num >= 1000000)
                {
                    result = $"{ToFixed(num, 1000000)}M";
                }
                else if (num >= 1000)
                {
                    result = $"{ToFixed(num, 1000)}K";
                }
                else
                {
                    result = ToFixed(num, 1);
                }
            }
            return result;
        }

        public static string FormatNumber4ResBar(int num)
        {
            var result = num.ToString();
            if (IsChinese())
            {
                if (num >= 100000000)
                {
                    result = $"{ToFixedFloor(num, 100000000)}亿";
                }
                else if (num >= 10000000)
                {
                    result = $"{ToFixedFloor(num, 10000)}万";
                }
                else if (num >= 10000)
                {
                    result = $"{ToFixed(num, 10000)}万";
                }
                else
                {
                    result = ToFixed(num, 1);
                }
            }
            else
            {
                if (num >= 1000000000)
                {
                    result = $"{ToFixed(num, 1000000000)}B";
                }
                else if (num >= 1000000)
                {
                    result = $"{ToFixed(num, 1000000)}M";
                }
                else if (num >= 1000)
                {
                    result = $"{ToFixed(num, 1000)}K";
                }
                else
                {
                    result = ToFixed(num, 1);
                }
            }
            return result;
        }

        /// <summary>
        /// 用于任务的较大数字显示规则
        /// </summary>
        /// <param name="numStr"></param>
        /// <returns></returns>
        public static string FormatNumberTaskProgress(int num)
        {
            var result = num.ToString();
            if (IsChinese())
            {
                if (num >= 100000000)
                {
                    result = $"{ToFixedFloor(num, 100000000)}亿";
                }
                else if (num >= 1000000)
                {
                    result = $"{ToFixedFloor(num, 10000)}万";
                }
                else
                {
                    result = ToFixed(num, 1);
                }
            }
            else
            {
                if (num >= 1000000000)
                {
                    result = $"{ToFixed(num, 1000000000)}B";
                }
                else if (num >= 1000000)
                {
                    result = $"{ToFixed(num, 1000000)}M";
                }
                else
                {
                    result = ToFixed(num, 1);
                }
            }
            return result;
        }

        /// <summary>
        /// 判断字符串是否为纯数字
        /// </summary>
        /// <param name="numStr"></param>
        /// <returns></returns>
        public static bool IsNumber(string numStr)
        {
            if (Regex.IsMatch(numStr, @"^\d+$"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 从汉字转换到16进制
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GetHexFromChs(string s)
        {
            if ((s.Length % 2) != 0)
            {
                s += " ";//空格
                         //throw new ArgumentException("s is not valid chinese string!");
            }

            System.Text.Encoding chs = System.Text.Encoding.GetEncoding("gb2312");

            byte[] bytes = chs.GetBytes(s);

            string str = "";

            for (int i = 0; i < bytes.Length; i++)
            {
                str += string.Format("{0:X}", bytes[i]);
            }

            return str;
        }

        public static string GetChineseFromHex(string hexString)
        {
            var a = hexString;

            if (a.Length % 2 == 0)
            {
                int length = a.Length / 2;
                byte[] bytes = new byte[length];
                for (int i = 0; i < length; i++)
                {
                    if (!byte.TryParse(a.Substring(2 * i, 2), System.Globalization.NumberStyles.HexNumber, null, out bytes[i]))
                    {
                        return "";
                    }
                }
                // Logger.Debug.Log("“" + a + "”对应的中文是：" + Encoding.UTF8.GetString(bytes));
                return Encoding.UTF8.GetString(bytes);
            }
            else
            {
                UnityEngine.Debug.Log("输入字符串长度有误，无法转换。");
                return "";
            }
        }


        /// <summary>
        /// </summary>
        /// <param name="barray"></param>
        /// <param name="toLowerCase"></param>
        /// <returns></returns>
        public static unsafe string _BytesToHex(this byte[] barray, bool toLowerCase = true)
        {
            byte addByte = 0x37;
            if (toLowerCase) addByte = 0x57;//大写的0起点,字母W
            char* c = stackalloc char[barray.Length * 2];
            byte b;
            for (int i = 0; i < barray.Length; ++i)
            {
                b = ((byte)(barray[i] >> 4));
                c[i * 2] = (char)(b > 9 ? b + addByte : b + 0x30);
                b = ((byte)(barray[i] & 0xF));
                c[i * 2 + 1] = (char)(b > 9 ? b + addByte : b + 0x30);
            }
            return new string(c);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static unsafe string _GetChineseFromHex(string hexString)
        {
            var a = hexString;

            if (a.Length % 2 == 0)
            {
                int length = a.Length / 2;
                byte* bytes = stackalloc byte[length];
                for (int i = 0; i < length; i++)
                {
                    var c = a[i * 2];
                    bytes[i] = (byte)((c < 0x40 ? c - 0x30 : (c < 0x47 ? c - 0x37 : c - 0x57)) << 4);
                    c = a[i * 2 + 1];
                    bytes[i] += (byte)(c < 0x40 ? c - 0x30 : (c < 0x47 ? c - 0x37 : c - 0x57));
                }
                // Logger.Debug.Log("“" + a + "”对应的中文是：" + Encoding.UTF8.GetString(bytes));
                return Encoding.UTF8.GetString(bytes, length);
            }
            else
            {
                UnityEngine.Debug.Log("输入字符串长度有误，无法转换。");
                return "";
            }
        }

        public static int GetLength(string str)
        {
            if (str.Length == 0)
                return 0;
            ASCIIEncoding ascii = new ASCIIEncoding();
            int tempLen = 0;
            byte[] s = ascii.GetBytes(str);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                {
                    tempLen += 2;
                }
                else
                {
                    tempLen += 1;
                }
            }
            return tempLen;
        }


        public static unsafe string ToUpperFirst(this string str)
        {
            if (str == null) return null;
            string ret = string.Copy(str);
            fixed (char* ptr = ret)
                *ptr = char.ToUpper(*ptr);
            return ret;
        }

        public static string FilterHTMLChar(string retString)
        {
            retString = retString.Replace("<!DOCTYPE html>", string.Empty);
            retString = Regex.Replace((string)retString, "(\n|<p>|<p class=\"author\">|<h\\d>|<strong>|</strong>)", string.Empty);
            retString = Regex.Replace((string)retString, " (target|title)=\"(.*?)\"", string.Empty);
            retString = Regex.Replace((string)retString, "(</p>|</h\\d>)", "\n");
            retString = retString.Replace("&", "&amp;");
            //html = html.Replace("<strong>", "<b>");
            //html = html.Replace("</strong>", "</b>");
            retString = Regex.Replace((string)retString, "<head>[\\w\\s\\W]+?</head>", string.Empty);
            return retString;
        }
        public static string FixError(string retString)
        {
            retString = Regex.Replace((string)retString, "\n", string.Empty);
            retString = Regex.Replace((string)retString, "\r", string.Empty);
            retString = Regex.Replace((string)retString, "\t", string.Empty);
            retString = Regex.Replace((string)retString, "\0", string.Empty);
            retString = Regex.Replace((string)retString, " ", string.Empty);
            return retString;
        }


        private static char[] numStr = "0123456789".ToCharArray();
        private static char[] chineseStr = "零一二三四五六七八九".ToCharArray();
        public static string DigitalToCN(int numberStr)
        {
            // UnityEngine.Debug.Assert(numberStr >= 0 && numberStr < 10);
            int index = System.Array.IndexOf(numStr, (char)(48 + numberStr));
            return new string(chineseStr[index], 1);
        }


        static string[] chnNumChar = new string[] { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
        static string[] chnUnitSection = new string[] { "", "万", "亿", "万亿", "亿亿" };
        static string[] chnUnitChar = new string[] { "", "十", "百", "千" };

        private static string SectionToChinese(int section)
        {
            var strIns = string.Empty;
            var chnStr = string.Empty;
            var unitPos = 0;
            var zero = true;
            while (section > 0)
            {
                var v = section % 10;
                if (v == 0)
                {
                    if (!zero)
                    {
                        zero = true;
                        chnStr = chnNumChar[v] + chnStr;
                    }
                }
                else
                {
                    zero = false;
                    strIns = chnNumChar[v];
                    strIns += chnUnitChar[unitPos];
                    chnStr = strIns + chnStr;
                }
                unitPos++;
                section = section / 10;
            }
            return chnStr;
        }

        public static string NumberToChinese(int num)
        {
            var unitPos = 0;
            var strIns = string.Empty;
            StringBuilder chnStr = new StringBuilder();
            var needZero = false;
            var origin = num;
            if (num == 0)
            {
                return chnNumChar[0];
            }

            while (num > 0)
            {
                var section = num % 10000;
                if (needZero)
                {
                    chnStr.Append(chnNumChar[0]);
                    chnStr.Append(chnStr);
                }
                strIns = SectionToChinese(section);
                strIns += (section != 0) ? chnUnitSection[unitPos] : chnUnitSection[0];
                chnStr.Insert(0, strIns);
                //chnStr = strIns + chnStr;  
                needZero = (section < 1000) && (section > 0);
                num = num / 10000;
                unitPos++;
            }
            if (origin < 20 && origin >= 10)
            {
                chnStr.Remove(0, 1);
            }
            return chnStr.ToString();
        }

        private static readonly Regex s_ColorRegex_Left = new Regex(@"<color=([^>\n\s]+)>", RegexOptions.Singleline);
        public static string ClearAllColor(string content)
        {
            if (content == null) return content;
            if (s_ColorRegex_Left.IsMatch(content))
            {
                content = s_ColorRegex_Left.Replace(content, "");
                content = content.Replace("</color>", "");
            }
            return content;
        }

        private static readonly Regex s_SizeRegex_Left = new Regex(@"<size=([^>\n\s]+)>", RegexOptions.Singleline);
        public static string ClearAllSize(string content)
        {
            if (content == null) return content;
            if (s_SizeRegex_Left.IsMatch(content))
            {
                content = s_SizeRegex_Left.Replace(content, "");
                content = content.Replace("</size>", "");
            }
            return content;
        }

        //去除字符串里得 HTML标签
        public static string ReplaceHtmlTag(string html, int length = 0)
        {
            string strText = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]+>", "");
            strText = System.Text.RegularExpressions.Regex.Replace(strText, "&[^;]+;", "");

            if (length > 0 && strText.Length > length)
                return strText.Substring(0, length);

            return strText;
        }
        public static int GetStrLen(string str)
        {
            int len = 0;
            int tmplen = str.Length;
            byte[] bytes = new byte[20];
            bytes = Encoding.Default.GetBytes(str);
            for (int i = 0; i < bytes.Length; i++)
            {
                if(bytes[i]>=228)
                {
                }
                else
                {
                    len++;
                }
            }
            return len;
        }

        public static string RandomStr(int len)
        {
            string key="ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            string str = "";
            while (len > 0)
            {
                if (len <= 0){
                    return str;
                }
                int n = UnityEngine.Random.Range(0,61);
                str = str + key.Substring(n,1);
                len = len - 1;
            }
            return str;
        }

        /// <summary>
        /// 验证只包含汉字(不包含标点符号)
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsChineseCharacter(string inputStr)
        {
            if (string.IsNullOrWhiteSpace(inputStr))//
                return false;
            string pattern = @"^[\u4e00-\u9fa5]+$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(inputStr);
        }
        private static string[] _sizeName = new string[] { "KB", "MB", "GB" };
        /// <summary>
        /// 转换容量单位
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string CovertSize(ulong val)
        {
            float size = val / 1024f;
            int flag = 0;
            while (size > 1000)
            {
                size = size / 1024f;
                flag++;
            }

            return size.ToString("F1") + _sizeName[flag];
        }
        public static string CovertSize(long val)
        {
            float size = val / 1024f;
            int flag = 0;
            while (size > 1000)
            {
                size = size / 1024f;
                flag++;
            }

            return size.ToString("F1") + _sizeName[flag];
        }
        public static string RemoveEmoji(string str)
        {
            for (int i = 0; i < _emojiPatten.Count-1; i++)
            {
                str = Regex.Replace(str, _emojiPatten[i],"");
            }
            return str;
        }
    }

}