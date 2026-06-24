using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class LogHelper
    {
        #region  单列模式
        public static LogHelper _instance;
        private static readonly object _lock = new object();
        public static LogHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new LogHelper();
                        }
                    }
                }
                return _instance;
            }
        }

        private LogHelper()
        {
        }
        #endregion


        /// <summary>
        /// 命名格式为"yyyy-MM-dd HH.txt"
        /// </summary>
        private string timeLog = DateTime.Now.ToString("yyyy-MM-dd HH");
        /// <summary>
        /// 日志文件路径，按照小时创建一个新的日志文件，命名格式为"yyyy-MM-dd HH.txt"
        /// </summary>
        private string logPath = null;

        /// <summary>
        /// 创建日志文件，按照小时创建一个新的日志文件，命名格式为"yyyy-MM-dd HH.txt"
        /// </summary>
        public void CreateLogFile()
        {
            // string logDir = Directory.GetCurrentDirectory();
            // if (!Directory.Exists(logDir))
            // {
            //     Directory.CreateDirectory(logDir);
            // }
            // logPath = logDir + @"\log\" + timeLog + ".txt";
            string logDir =Path.Combine(
                Directory.GetCurrentDirectory(),
                "log"
            );

            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }

            logPath = Path.Combine(
                    logDir,
                    $"{DateTime.Now:yyyy-MM-dd HH}.txt"
                );
            if (!File.Exists(logPath))
            {
                File.Create(logPath).Close();
            }
        }

        /// <summary>
        /// 写日志，按照小时创建一个新的日志文件，命名格式为"yyyy-MM-dd HH.txt"
        /// </summary>
        /// <param name="log"></param>
        public void WriteLog(string log, int status)
        {
            CreateLogFile();
            string msg = "";
            if (status == 1)
            {
                msg = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Success {log}{Environment.NewLine}";
                FileHelper.WriteToTxt(logPath, msg, true);
            }
            else if (status == 0)
            {
                msg = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Error {log}{Environment.NewLine}";
                FileHelper.WriteToTxt(logPath, msg, true);
            }


        }
    }
}
