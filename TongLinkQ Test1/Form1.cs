using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TongLinkQ_SDK;

namespace TongLinkQ_Test1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //SDKClient client;

        private void Form1_Load(object sender, EventArgs e)
        {
            //client = new SDKClient("192.168.3.195", 10261, "qcu1", "", "lq", "lq", 0);
            //var flag = client.openConnection();
            ////var flag1 = client.sendMessage("1");
            ////var flag2 = client.sendMessage("2");
            ////var flag3 = client.sendMessage("3");
            //var msg = client.getMessage();
            //Debug.WriteLine(msg);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SDKClient client = new SDKClient("192.168.3.196", 10261, "qcu1", "", "lq", "lq", 0);
            var flag = client.openConnection();
            while (true)
            {
                try
                {
                    client.sendMessage(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                finally
                {
                    Thread.Sleep(1000);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SDKClient client = new SDKClient("192.168.3.196", 10261, "qcu1", "", "lq", "lq", 0);
            while (true)
            {
                var flag = client.openConnection();
                var msg = client.getMessage();
                if (msg == string.Empty) break;
                Debug.WriteLine(msg);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SDKClient client = new SDKClient("192.168.3.195", 10261, "qcu1", "", "lq", "lq", 0);
            while (true)
            {
                var flag = client.openConnection();
                var msg = client.getMessage();
                if (msg == string.Empty) break;
                Debug.WriteLine(msg);
            }
        }
    }
}
