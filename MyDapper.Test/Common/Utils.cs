using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDapper.Test.Common
{
    public class Utils
    {
        /// <summary>
        /// 获取配置文件值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static string GetConfig(string key, string defValue = "")
        {
            var v = System.Configuration.ConfigurationManager.AppSettings[key];
            if (v == null)
                return defValue;
            return v.ToString();
        }
        /// <summary>
        /// 获取配置文件值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static bool GetConfig(string key, bool defValue = false)
        {
            var v = System.Configuration.ConfigurationManager.AppSettings[key];
            if (v == null)
                return defValue;
            return Convert.ToBoolean(v);
        }
        /// <summary>
        /// 获取配置文件值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static int GetConfig(string key, int defValue = 0)
        {
            var v = System.Configuration.ConfigurationManager.AppSettings[key];
            if (v == null)
                return defValue;
            return Convert.ToInt32(v);
        }
    }
}
