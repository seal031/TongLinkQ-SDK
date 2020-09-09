using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TongLinkQ_SDK
{
    public class TlqClientFactory
    {
        public static TlqGetMsgApi createGetMsgClient()
        {
            return new TlqGetMsgApiImpl();
        }
        public static TlqSendMsgApi createSendMsgClient()
        {
            return new TlqSendMsgApiImpl();
        }
        public static TlqSendRequestApi createSendRequestClient()
        {
            return new TlqSendRequestApiImpl();
        }
    }
}
