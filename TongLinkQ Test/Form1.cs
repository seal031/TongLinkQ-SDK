using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using TongLinkQ_SDK;

namespace TongLinkQ_Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 创建客户端
            TlqGetMsgApi getClient = TlqClientFactory.createGetMsgClient();
            // 初始化客户端
            var initFlag = getClient.initClient("TlqSdkConfig.xml");
            // 登录
            getClient.login("sysbktobt190815", "20190815");
            // 建立连接
            getClient.openConnection();
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
                catch (Exception ex)
                {

                    throw;
                }
            }

        }
    }
}
