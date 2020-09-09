using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace TongLinkQ_SDK
{
    public class HttpWorker
    {
        public static string GetMD5String(string str)
        {
            MD5 mD = MD5.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            byte[] data = mD.ComputeHash(bytes);
            return HttpWorker.GetbyteToString(data).ToLower();
        }
        private static string GetbyteToString(byte[] data)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                stringBuilder.Append(data[i].ToString("x2"));
            }
            return stringBuilder.ToString();
        }
        public static string HttpGet(string Url, string postDataStr)
        {
            string result;
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Url + ((postDataStr == "") ? "" : "?") + postDataStr);
                httpWebRequest.Timeout = 30000;
                httpWebRequest.ReadWriteTimeout = 30000;
                httpWebRequest.Method = "GET";
                httpWebRequest.ContentType = "text/html;charset=UTF-8";
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                string text = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                result = text;
            }
            catch (WebException var_6_9D)
            {
                result = "error";
            }
            catch (Exception var_7_A9)
            {
                result = "error";
            }
            return result;
        }
        public static string PostJson(string Url, string postDataStr)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Url);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "text/html;charset=utf-8";
            Stream requestStream = httpWebRequest.GetRequestStream();
            StreamWriter streamWriter = new StreamWriter(requestStream, Encoding.GetEncoding("utf-8"));
            streamWriter.Write(postDataStr);
            streamWriter.Close();
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream responseStream = httpWebResponse.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
            string result = streamReader.ReadToEnd();
            streamReader.Close();
            responseStream.Close();
            return result;
        }
        public static string PostStr(string url, string content)
        {
            string result;
            try
            {
                string text = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json";
                byte[] bytes = Encoding.UTF8.GetBytes(content);
                httpWebRequest.ContentLength = (long)bytes.Length;
                using (Stream requestStream = httpWebRequest.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                }
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream responseStream = httpWebResponse.GetResponseStream();
                using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
                {
                    text = streamReader.ReadToEnd();
                }
                result = text;
            }
            catch (WebException var_8_B1)
            {
                result = "error";
            }
            catch (Exception var_9_BD)
            {
                result = "error";
            }
            return result;
        }
        public static string PostDic(string url, Dictionary<string, string> dic)
        {
            string result = "";
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            StringBuilder stringBuilder = new StringBuilder();
            int num = 0;
            foreach (KeyValuePair<string, string> current in dic)
            {
                bool flag = num > 0;
                if (flag)
                {
                    stringBuilder.Append("&");
                }
                stringBuilder.AppendFormat("{0}={1}", current.Key, current.Value);
                num++;
            }
            byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
            httpWebRequest.ContentLength = (long)bytes.Length;
            using (Stream requestStream = httpWebRequest.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream responseStream = httpWebResponse.GetResponseStream();
            using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
            {
                result = streamReader.ReadToEnd();
            }
            return result;
        }
    }
}
