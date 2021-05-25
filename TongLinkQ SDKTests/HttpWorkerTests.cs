using Microsoft.VisualStudio.TestTools.UnitTesting;
using TongLinkQ_SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TongLinkQ_SDK.Tests
{
    [TestClass()]
    public class HttpWorkerTests
    {
        [TestMethod()]
        public void GetMD5StringTest()
        {
            var a= HttpWorker.GetMD5String("20200617");
            Assert.Fail();
        }
    }
}