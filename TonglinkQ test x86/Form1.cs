using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
            // 创建客户端
            TlqGetMsgApi getClient = TlqClientFactory.createGetMsgClient();
            // 初始化客户端

            var initFlag = getClient.initClient("TlqSdkConfig.xml");
        }
    }
}
