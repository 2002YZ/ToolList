using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;


namespace Helper
{
    public class JsonHelper
    {
        /// <summary>
        /// 序列化对象为Json字符串，并保存到指定文件路径
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool SaveToJson<T>(T data, string filePath) where T : class
        {
            bool result = false;
            try
            {
                string strJson = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(filePath, strJson);
                result = true;
                return result;
            }
            catch (Exception ex)
            {

                Console.WriteLine("SaveToJson error: " + ex.Message);//序列化失败
                return result;
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strPath"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string strPath) where T : class
        {
            try
            {
                T result = default(T);
                using (StreamReader sr = new StreamReader(strPath, Encoding.UTF8))
                {
                    string strJson = sr.ReadToEnd();
                    result = JsonConvert.DeserializeObject<T>(strJson);
                    return result;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Deserialize error: " + ex.Message);//反序列化失败
                return null;
            }
        }

    }
}
