using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TongLinkQ_SDK
{
    public interface TlqGetMsgApi
    {
        string getMsg();
        int reconnect();
        void disconnect();
        bool openConnection();
        bool login(string userName, string password);
        bool initClient(string configPath);
    }
    public interface TlqSendMsgApi
    {
        bool sendMsg(string msg);
        int reconnect();
        void disconnect();
        bool openConnection();
        bool login(string userName, string password);
        bool initClient(string configPath);
    }

    public interface TlqSendRequestApi
    {
        bool sendRequest(string request);
        int reconnect();
        void disconnect();
        bool openConnection();
        bool login(string userName, string password);
        bool initClient(string configPath);
    }
}
