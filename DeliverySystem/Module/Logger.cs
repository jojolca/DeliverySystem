using DeliverySystem.Interface;
using DeliverySystem.Variables.Repository;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeliverySystem.Module
{
    public class Logger : ILog
    {
        private readonly IRepositoryOperater _repositoryOperater;

        /// <summary>
        /// 建構子
        /// </summary>
        public Logger(IRepositoryOperater repositoryOperater)
        {
            _repositoryOperater = repositoryOperater;

            writeLogTimer = new Timer(new TimerCallback(this.timerWriteLog), null, 0, Timeout.Infinite);
        }

        #region 變數

        /// <summary>
        /// 寫log的timer
        /// </summary>
        private Timer writeLogTimer;

        /// <summary>
        /// 需要被寫入的log
        /// </summary>
        private ConcurrentQueue<LogInformation> logsNeedToWrite = new ConcurrentQueue<LogInformation>();

        /// <summary>
        /// 間隔1秒
        /// </summary>
        private const int RECORD_INTERVAL = 1000;

        /// <summary>
        /// 最大批次寫入資料筆數(先設10000筆看看)
        /// </summary>
        private const int MAX_ROWS_OF_WRITE_TO_LOGFILE = 10000;
       

        #endregion

        /// <summary>
        /// 增加log
        /// </summary>
        /// <param name="content">log內容</param>
        ///  <param name="dirName">資料夾名稱</param>
        public void AddLog(LogInformation content)
        {
            logsNeedToWrite.Enqueue(content);
        }

        /// <summary>
        /// 寫log
        /// </summary>
        /// <param name="content"></param>
        private void writeLog()
        {
            string path = string.Empty;

            while (logsNeedToWrite.TryDequeue(out var content))
            {
                
                try
                {
                    _repositoryOperater.InsertLog(content);
                }
                catch(Exception ex) 
                {
                    //todo
                    var err= ex.ToString();
                }
            }
        }

        /// <summary>
        /// 寫log的timer工作內容
        /// </summary>
        /// <param name="item"></param>
        private void timerWriteLog(object item)
        {            
            if (logsNeedToWrite.Count > 0)
            {
                try
                {
                    writeLog();
                }
                catch (Exception ex)
                {

                }

            }

            //完成後再觸發下一次Timer
            this.writeLogTimer.Change(RECORD_INTERVAL, Timeout.Infinite);
        }
    }
}

