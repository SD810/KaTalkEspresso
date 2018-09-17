using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaTalkEspresso
{
    class Logger
    {
        // 프로그램 오류 발생시 원활한 수집을 위해 카톡 에스프레소 내부의 기록을 전담.

        // 로그 내용이 기억될 StringBuilder
        private StringBuilder log = new StringBuilder();

        // 경고 또는 오류 로그가 있는지 기억하는 변수
        private bool logWarn = false;
        private bool logError = false;

        // 날짜시간 형식
        private const string DATETIME_FORMAT = "yyyy/MM/dd/ HH:mm:ss";

        // 싱글턴 객체를 위한 것
        private Logger() { }
        private static Logger instance = new Logger();

        public static Logger getInstance()
        {
            return instance;
        }

        /// <summary>
        /// 로그 기록: 종류와 시각, 내용이 붙는다.
        /// </summary>
        /// <param name="text"></param>
        public void info(string text)
        {
            log.Append("[I]" + DateTime.Now.ToString(DATETIME_FORMAT) + " : " +  text + "\r\n");
        }

        public void warn(string text)
        {
            if( !logWarn)
            {
                //경고 있었다고 기억
                logWarn = true;
            }
            log.Append("[W]" + DateTime.Now.ToString(DATETIME_FORMAT) + " : " + text + "\r\n");
        }

        public void error(string text)
        {
            if ( ! logError)
            {
                // 오류 있었다고 기억
                logError = true;
            }
            log.Append("[E]" + DateTime.Now.ToString(DATETIME_FORMAT) + " : " + text + "\r\n");
        }

        public bool showLogNow()
        {
            //로그 표시여부
            bool logShown = false;

            if (log.Length > 0)
            {
                info("---------LOG SHOWING---------");
                System.Windows.Forms.MessageBox.Show(log.ToString());

                logShown = true;
            }

            return logShown;
        }
    }
}
