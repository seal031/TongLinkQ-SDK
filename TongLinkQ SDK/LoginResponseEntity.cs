using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TongLinkQ_SDK
{
    public class LoginResponseEntity
    {
        public string connId
        {
            get;
            set;
        }
        public string qcuName
        {
            get;
            set;
        }
        public string receiveQueueName
        {
            get;
            set;
        }
        public string releaseQueueName
        {
            get;
            set;
        }
        public string requestAuth
        {
            get;
            set;
        }
        public string rtcd
        {
            get;
            set;
        }
        public string rtmsg
        {
            get;
            set;
        }
    }
}
