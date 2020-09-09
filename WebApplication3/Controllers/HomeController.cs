using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TongLinkQ_SDK;

namespace WebApplication3.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            // 创建客户端
            TlqGetMsgApi getClient = TlqClientFactory.createGetMsgClient();
            // 初始化客户端

            var initFlag = getClient.initClient("TlqSdkConfig.xml");
            ViewBag.Message = "Your application description page.";
            ViewBag.Message = getClient.ToString() + "5555";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}