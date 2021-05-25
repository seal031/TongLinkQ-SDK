using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TongLinkQ_SDK;

namespace TonglinkQ_test_x86
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //创建发送客户端
            TlqSendMsgApi sendClient = TlqClientFactory.createSendMsgClient();
            //初始化发送客户端
            var initFlag = sendClient.initClient("TlqSdkConfig.xml");
            // 登录
            sendClient.login("sysbktobt190815", "20190815");
            // 建立连接
            sendClient.openConnection();
            //发送消息
            sendClient.sendMsg("123");
            sendClient.sendMsg("456");
            sendClient.sendMsg("789");
            //关闭连接
            sendClient.disconnect();

            
            // 创建接收客户端
            TlqGetMsgApi getClient = TlqClientFactory.createGetMsgClient();
            // 初始化接收客户端
            initFlag = getClient.initClient("TlqSdkConfig.xml");
            // 登录
            getClient.login("sysbktobt190815", "20190815");
            // 建立连接
            getClient.openConnection();
            //接收消息
            int i = 0;
            while (true)
            {
                try
                {
                    i++;
                    // 获取消息
                    string msg = getClient.getMsg();
                    Console.WriteLine("读取第" + i + "条消息" + msg);
                    Thread.Sleep(10);
                }
                catch (Exception)
                {
                    
                }
            }
        }
    }
}
