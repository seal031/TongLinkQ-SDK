using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;

namespace TongLinkQ_SDK
{
    public class TlqBaseClass
    {
        protected SDKClient client;
        protected static string tlqHost;
        protected static int tlqPort;
        protected static string loginUrl;
        protected string qcuName = null;
        protected string topicName = null;
        protected string queueName = null;
        protected string rcvQueName = null;
        protected int pWaitInterval = 0;
        protected bool getConfigElement(string configPath)
        {
            bool result;
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(configPath);
                XmlElement xmlElement = xmlDocument["config"];
                foreach (XmlNode xmlNode in xmlElement.ChildNodes)
                {
                    XmlElement xmlElement2 = xmlNode as XmlElement;
                    if (xmlElement2 != null)
                    {
                        bool flag = xmlElement2.GetAttributeNode("tlq_hostname") != null;
                        if (flag)
                        {
                            TlqBaseClass.tlqHost = xmlElement2.GetAttributeNode("tlq_hostname").Value;
                        }
                        else
                        {
                            bool flag2 = xmlElement2.GetAttributeNode("tlq_listenport") != null;
                            if (flag2)
                            {
                                TlqBaseClass.tlqPort = int.Parse(xmlElement2.GetAttributeNode("tlq_listenport").Value);
                            }
                            else
                            {
                                bool flag3 = xmlElement2.GetAttributeNode("tlq_login_context") != null;
                                if (flag3)
                                {
                                    TlqBaseClass.loginUrl = xmlElement2.GetAttributeNode("tlq_login_context").Value;
                                }
                            }
                        }
                    }
                    //if (xmlElement2!=null)
                    //{
                    //    bool flag = xmlElement2.GetAttributeNode("key") != null;
                    //    if (flag)
                    //    {
                    //        switch (xmlElement2.GetAttributeNode("key").Value)
                    //        {
                    //            case "tlq_hostname":
                    //                TlqBaseClass.tlqHost = xmlElement2.GetAttributeNode("value").Value;
                    //                break;
                    //            case "tlq_listenport":
                    //                TlqBaseClass.tlqPort = int.Parse(xmlElement2.GetAttributeNode("value").Value);
                    //                break;
                    //            case "tlq_login_context":
                    //                TlqBaseClass.loginUrl = xmlElement2.GetAttributeNode("value").Value;
                    //                break;
                    //            default:
                    //                break;
                    //        }
                    //    }
                    //}
                }
                result = true;
            }
            catch (Exception var_10_FA)
            {
                result = false;
            }
            return result;
        }
    }
    public class TlqGetMsgApiImpl : TlqBaseClass, TlqGetMsgApi
    {
        public static string TLQ_SEND_RESPONSE_QUEUE = "whdwh.tlq.response.common";
        //private SDKClient responseClient;
        public void disconnect()
        {
            this.client.closeConnection();
        }
        public string getMsg()
        {
            string message = this.client.getMessage();
            try
            {
                //bool flag = this.responseClient == null;
                //if (flag)
                //{
                //    this.responseClient = new SDKClient(TlqBaseClass.tlqHost, TlqBaseClass.tlqPort, this.qcuName, this.topicName, TlqGetMsgApiImpl.TLQ_SEND_RESPONSE_QUEUE, this.rcvQueName, this.pWaitInterval);
                //}
                //this.responseClient.openConnection();
                //this.responseClient.sendMessage(message);
            }
            catch (Exception var_2_6B)
            {
            }
            return message;
        }
        public bool initClient(string configPath)
        {
            return base.getConfigElement(configPath);
        }
        public bool login(string userName, string password)
        {
            bool result;
            try
            {
                string str = HttpWorker.HttpGet(TlqBaseClass.loginUrl + "/auth/tlqSdkAuthRestService", "userName=" + userName + "&password=" + HttpWorker.GetMD5String(password));
                LoginResponseEntity loginResponseEntity = TranslationWorker.ConvertStringToEntity<LoginResponseEntity>(str);
                bool flag = loginResponseEntity.rtcd == "0";
                if (flag)
                {
                    this.qcuName = loginResponseEntity.qcuName;
                    this.queueName = loginResponseEntity.releaseQueueName;
                    this.rcvQueName = loginResponseEntity.receiveQueueName;
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                result = false;
                throw new Exception(ex.Message);
            }
            return result;
        }
        public bool openConnection()
        {
            bool result;
            try
            {
                this.client = new SDKClient(TlqBaseClass.tlqHost, TlqBaseClass.tlqPort, this.qcuName, this.topicName, this.queueName, this.rcvQueName, this.pWaitInterval);
                bool flag = this.client.openConnection() == 0;
                if (flag)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("连接异常："+ex.Message);
                throw new Exception("连接异常：" + ex.Message+Environment.NewLine+string.Format("连接信息 tlqHost:{0},tlqPort:{1},qcuName:{2},topicName:{3},queueName:{4},recQueName:{5},pWaitInterval:{6}",tlqHost,tlqPort,qcuName,topicName,queueName,rcvQueName,pWaitInterval));
                result = false;
            }
            return result;
        }
        public int reconnect()
        {
            return this.client.ReConnectToTLQ();
        }
    }

    public class TlqSendMsgApiImpl : TlqBaseClass, TlqSendMsgApi
    {
        public void disconnect()
        {
            this.client.closeConnection();
        }
        public bool initClient(string configPath)
        {
            return base.getConfigElement(configPath);
        }
        public bool login(string userName, string password)
        {
            bool result;
            try
            {
                string str = HttpWorker.HttpGet(TlqBaseClass.loginUrl + "/auth/tlqSdkAuthRestService", "userName=" + userName + "&password=" + HttpWorker.GetMD5String(password));
                LoginResponseEntity loginResponseEntity = TranslationWorker.ConvertStringToEntity<LoginResponseEntity>(str);
                bool flag = loginResponseEntity.rtcd != "0";
                if (flag)
                {
                    result = false;
                }
                else
                {
                    this.qcuName = loginResponseEntity.qcuName;
                    this.queueName = loginResponseEntity.releaseQueueName;
                    this.rcvQueName = loginResponseEntity.receiveQueueName;
                    result = true;
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
        public bool openConnection()
        {
            bool result;
            try
            {
                this.client = new SDKClient(TlqBaseClass.tlqHost, TlqBaseClass.tlqPort, this.qcuName, this.topicName, this.queueName, this.rcvQueName, this.pWaitInterval);
                bool flag = this.client.openConnection() == 0;
                if (flag)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
        public int reconnect()
        {
            return this.client.ReConnectToTLQ();
        }
        public bool sendMsg(string msg)
        {
            return this.client.sendMessage(msg);
        }
    }

    public class TlqSendRequestApiImpl : TlqBaseClass, TlqSendRequestApi
    {
        public static string TLQ_SEND_REQUEST_QUEUE = "whdwh.tlq.request.common";
        public void disconnect()
        {
            this.client.closeConnection();
        }
        public bool initClient(string configPath)
        {
            return base.getConfigElement(configPath);
        }
        public bool login(string userName, string password)
        {
            bool result;
            try
            {
                string str = HttpWorker.HttpGet(TlqBaseClass.loginUrl + "/auth/tlqSdkAuthRestService", "userName=" + userName + "&password=" + HttpWorker.GetMD5String(password));
                LoginResponseEntity loginResponseEntity = TranslationWorker.ConvertStringToEntity<LoginResponseEntity>(str);
                bool flag = loginResponseEntity.rtcd != "0";
                if (flag)
                {
                    result = false;
                }
                else
                {
                    bool flag2 = loginResponseEntity.requestAuth != "1";
                    if (flag2)
                    {
                        result = false;
                    }
                    else
                    {
                        this.qcuName = loginResponseEntity.qcuName;
                        this.queueName = loginResponseEntity.releaseQueueName;
                        this.rcvQueName = loginResponseEntity.receiveQueueName;
                        result = true;
                    }
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
        public bool openConnection()
        {
            bool result;
            try
            {
                this.client = new SDKClient(TlqBaseClass.tlqHost, TlqBaseClass.tlqPort, this.qcuName, this.topicName, TlqSendRequestApiImpl.TLQ_SEND_REQUEST_QUEUE, this.rcvQueName, this.pWaitInterval);
                bool flag = this.client.openConnection() == 0;
                if (flag)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
        public int reconnect()
        {
            return this.client.ReConnectToTLQ();
        }
        public bool sendRequest(string request)
        {
            return this.client.sendMessage(request);
        }
    }
}
