using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace CommonLib
{
    public static class Helper
    {

        #region GetClientIP 客户端IP
        /// <summary>
        /// 客户端IP(穿过代理服务器取远程用户真实IP地址)
        /// </summary>
        /// <returns></returns>
        public static string GetClientIP()
        {
            //try
            //{
            //    if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
            //        return HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            //    else
            //        return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
            //}
            //catch { return ""; }

            string result = String.Empty;
            result = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (result != null && result != String.Empty)
            {
                //可能有代理     
                if (result.IndexOf(".") == -1)    //没有"."肯定是非IPv4格式     
                    result = null;
                else
                {
                    if (result.IndexOf(",") != -1)
                    {
                        //有","，估计多个代理。取第一个不是内网的IP。     
                        result = result.Replace(" ", "").Replace("\"", "");
                        string[] temparyip = result.Split(",;".ToCharArray());
                        for (int i = 0; i < temparyip.Length; i++)
                        {
                            if (IsIPAddress(temparyip[i])
                                && temparyip[i].Substring(0, 3) != "10."
                                && temparyip[i].Substring(0, 7) != "192.168"
                                && temparyip[i].Substring(0, 7) != "172.16.")
                            {
                                return temparyip[i];    //找到不是内网的地址     
                            }
                        }
                    }
                    else if (IsIPAddress(result)) //代理即是IP格式     
                        return result;
                    else
                        result = null;    //代理中的内容 非IP，取IP     
                }
            }
            string IpAddress = (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null && System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != String.Empty) ? System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] : System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            if (null == result || result == String.Empty)
                result = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            if (result == null || result == String.Empty)
                result = System.Web.HttpContext.Current.Request.UserHostAddress;
            return result;
            //return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
        }
        #endregion

        #region IsIPAddress 判断是否为IP地址
        /// <summary>
        /// 判断是否为IP地址
        /// </summary>
        /// <param name="sIp"></param>
        /// <returns></returns>
        public static bool IsIPAddress(string sIp)
        {
            if (sIp == null || sIp == string.Empty || sIp.Length < 7 || sIp.Length > 15) return false;
            string regformat = @"^\d{1,3}[\.]\d{1,3}[\.]\d{1,3}[\.]\d{1,3}$";
            Regex regex = new Regex(regformat, RegexOptions.IgnoreCase);
            return regex.IsMatch(sIp);
        }
        #endregion

        #region IsNumber 判断是否为纯数字
        /// <summary>
        /// 判断是否为纯数字
        /// </summary>
        /// <param name="sNumber">待测试值</param>
        /// <param name="iSize">数字满足最小长度</param>
        /// <returns></returns>
        public static bool IsNumber(string sNumber, int iSize)
        {
            bool result = false;

            Regex reg = new Regex("^[0-9]+$");
            Match ma = reg.Match(sNumber);
            if (iSize > 0)
            {
                if (ma.Success && sNumber.Length >= iSize)
                {
                    result = true;
                }
            }
            else
            {
                if (ma.Success)
                {
                    result = true;
                }
            }

            return result;
        }
        #endregion

        #region Md5Hash 获取字符串MD5哈希值
        /// <summary>
        /// 获取字符串MD5哈希值
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Md5Hash(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        #endregion

        #region EncryptDES DES加密字符串
        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="Key">密钥</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string EncryptDES(string encryptString, byte[] Key)
        {
            try
            {
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                des.Key = Key;
                des.Mode = CipherMode.ECB;
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, des.CreateEncryptor(), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                StringBuilder ret = new StringBuilder();
                foreach (byte b in mStream.ToArray())
                {
                    ret.AppendFormat("{0:X2}", b);
                }
                return ret.ToString();
            }
            catch
            {
                return encryptString;
            }
        }
        #endregion

        #region DecryptDES DES解密字符串
        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string DecryptDES(string decryptString, byte[] Key)
        {
            try
            {
                byte[] inputByteArray = new byte[decryptString.Length / 2];
                for (int x = 0; x < decryptString.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(decryptString.Substring(x * 2, 2), 16));
                    inputByteArray[x] = (byte)i;
                }

                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                des.Key = Key;
                des.Mode = CipherMode.ECB;
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, des.CreateDecryptor(), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return decryptString;
            }
        }
        #endregion

        #region EncodeBase64 Base64加密字符串
        /// <summary>
        /// 使用Base64算法加密字符串
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string EncodeBase64(string code)
        {
            string encode = "";
            byte[] bytes = Encoding.GetEncoding("utf-8").GetBytes(code);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = code;
            }
            return encode;
        }
        #endregion

        #region DecodeBase64 Base64解密字符串
        /// <summary>
        /// 解密Base64算法加密字符串
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string DecodeBase64(string code)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(code);
            try
            {
                decode = Encoding.GetEncoding("utf-8").GetString(bytes);
            }
            catch
            {
                decode = code;
            }
            return decode;
        }
        #endregion

        #region CreateUUID 压缩Guid编码(12位)
        /// <summary>
        /// 生成压缩为12位的Guid编码
        /// </summary>
        /// <returns></returns>
        public static string CreateUUID()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            long long_guid = BitConverter.ToInt64(buffer, 0);

            string _Value = System.Math.Abs(long_guid).ToString();

            byte[] buf = new byte[_Value.Length];
            int p = 0;
            for (int i = 0; i < _Value.Length; )
            {
                byte ph = System.Convert.ToByte(_Value[i]);

                int fix = 1;
                if ((i + 1) < _Value.Length)
                {
                    byte pl = System.Convert.ToByte(_Value[i + 1]);
                    buf[p] = (byte)((ph << 4) + pl);
                    fix = 2;
                }
                else
                {
                    buf[p] = (byte)(ph);
                }

                if ((i + 3) < _Value.Length)
                {
                    if (System.Convert.ToInt16(_Value.Substring(i, 3)) < 256)
                    {
                        buf[p] = System.Convert.ToByte(_Value.Substring(i, 3));
                        fix = 3;
                    }
                }
                p++;
                i = i + fix;
            }
            byte[] buf2 = new byte[p];
            for (int i = 0; i < p; i++)
            {
                buf2[i] = buf[i];
            }
            string cRtn = System.Convert.ToBase64String(buf2);
            if (cRtn == null)
            {
                cRtn = "";
            }
            cRtn = cRtn.ToLower();
            cRtn = cRtn.Replace("/", "");
            cRtn = cRtn.Replace("+", "");
            cRtn = cRtn.Replace("=", "");
            if (cRtn.Length == 12)
            {
                return cRtn.ToUpper();
            }
            else
            {
                return CreateUUID();
            }

        }
        #endregion

        #region SHA1_Encrypt SHA1加密函数
        /// <summary>
        /// SHA1加密函数
        /// </summary>
        /// <param name="sourceString"></param>
        /// <returns></returns>
        public static string SHA1_Encrypt(string sourceString)
        {
            byte[] strRes = Encoding.UTF8.GetBytes(sourceString);
            HashAlgorithm hashSha = new SHA1CryptoServiceProvider();
            strRes = hashSha.ComputeHash(strRes);
            var enText = new StringBuilder();
            foreach (byte iByte in strRes)
            {
                enText.AppendFormat("{0:x2}", iByte);
            }
            return enText.ToString();
        }
        #endregion

        #region GetTimeStamp 获取时间戳
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        #endregion

        #region GetRandomNum 获得随机数(6位长度)
        /// <summary>
        /// 获得随机数(6位长度)
        /// </summary>
        /// <returns></returns>
        public static string GetRandomNum()
        {
            Random r = new Random(DateTime.Now.Millisecond);
            return r.Next(100000, 999999).ToString();
        }
        #endregion

        #region SendHttpPost 发送Post请求
        /// <summary>
        /// 发送Post请求
        /// </summary>
        /// <param name="httpUrl">请求地址</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public static string SendHttpPost(string httpUrl, Dictionary<string, string> parameters)
        {
            return HttpUtility.HttpPost(httpUrl, null, parameters, null);
        }
        #endregion
        #region SendHttpPostJson 发送Post Json 请求
        /// <summary>
        /// 发送PostJson请求
        /// </summary>
        /// <param name="httpUrl"></param>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static string SendHttpPostJson(string httpUrl, string jsonStr)
        {
            return HttpUtility.HttpPostJson(httpUrl, null, jsonStr);
        }

        #endregion


        #region SendHttpGet 发送Get请求
        /// <summary>
        /// 发送Get请求
        /// </summary>
        /// <param name="sHttpUrl">Url地址</param>
        /// <returns></returns>
        public static string SendHttpGet(string httpUrl, IDictionary<string, string> parameters)
        {
            string strParameter = string.Empty;
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                    }
                    i++;
                }
                strParameter = "?" + buffer.ToString();
            }

            WebRequest req = WebRequest.Create(httpUrl + strParameter);
            HttpWebRequest httpreg = (HttpWebRequest)req;
            httpreg.Method = "GET";
            httpreg.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0)";
            WebResponse resp = httpreg.GetResponse();
            System.IO.StreamReader reader = new System.IO.StreamReader(resp.GetResponseStream(), Encoding.GetEncoding("utf-8"));
            return reader.ReadToEnd();
        }

        #endregion

        #region JsonSerializeObject 序列化对象为Json字符串
        /// <summary>
        /// 序列化对象为Json字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string JsonSerializeObject(object obj)
        {
            fastJSON.JSON.Instance.Parameters.SerializeNullValues = true;
            fastJSON.JSON.Instance.Parameters.ShowReadOnlyProperties = true;
            fastJSON.JSON.Instance.Parameters.UseUTCDateTime = true;
            fastJSON.JSON.Instance.Parameters.UsingGlobalTypes = false;
            fastJSON.JSON.Instance.Parameters.EnableAnonymousTypes = true;

            return fastJSON.JSON.Instance.ToJSON(obj);
        }
        #endregion

        #region JsonDeserializeObject 反序列化Json对象
        /// <summary>
        /// 反序列化Json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strJson"></param>
        /// <returns></returns>
        public static T JsonDeserializeObject<T>(string strJson)
        {
            fastJSON.JSON.Instance.Parameters.SerializeNullValues = true;
            fastJSON.JSON.Instance.Parameters.ShowReadOnlyProperties = true;
            fastJSON.JSON.Instance.Parameters.UseUTCDateTime = false;
            fastJSON.JSON.Instance.Parameters.UsingGlobalTypes = false;
            fastJSON.JSON.Instance.Parameters.EnableAnonymousTypes = true;

            return fastJSON.JSON.Instance.ToObject<T>(strJson);
        }
        #endregion

        #region JsonDeserializeObject 反序列化Json为dynamic对象
        /// <summary>
        /// 反序列化Json为dynamic对象
        /// </summary>
        /// <param name="strJson"></param>
        /// <returns></returns>
        public static dynamic JsonDeserializeObject(string strJson)
        {
            fastJSON.JSON.Instance.Parameters.SerializeNullValues = true;
            fastJSON.JSON.Instance.Parameters.ShowReadOnlyProperties = true;
            fastJSON.JSON.Instance.Parameters.UseUTCDateTime = false;
            fastJSON.JSON.Instance.Parameters.UsingGlobalTypes = false;
            fastJSON.JSON.Instance.Parameters.EnableAnonymousTypes = true;

            return fastJSON.JSON.Instance.ToDynamic(strJson);
        }
        #endregion


        /// <summary>
        /// 产生范围内的随机小数
        /// </summary>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static double GetRandomNumber(double minimum, double maximum, int len)   //Len小数点保留位数
        {
            Random random = new Random();
            return Math.Round(random.NextDouble() * (maximum - minimum) + minimum, len);
        } 
    }
}
