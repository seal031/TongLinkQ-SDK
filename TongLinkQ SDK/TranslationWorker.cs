using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TongLinkQ_SDK
{
    public class TranslationWorker
    {
        public static T ConvertStringToEntity<T>(string str) where T : class
        {
            T result;
            try
            {
                T t = JsonConvert.DeserializeObject<T>(str);
                result = t;
            }
            catch (Exception var_2_0D)
            {
                result = default(T);
            }
            return result;
        }
    }
}
