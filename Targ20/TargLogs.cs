using System;
using System.IO;
using System.Windows.Forms;

namespace Targ20
{
    public class TargLogs
    {
        #region Declarations
        private const string targBuild = "TARG v2.0 Build Date 5/10/2015 - Release 1";
        private const string DateTimeFormat = "MM-dd-yy hh.mm.ss";
        //private const string LOGHOST = "USATONHENDEG12"; 
        private const string LOGHOST = "127.0.0.1"; //**Change to any pc on the network, otherwise user Local Host

        protected TargInfo.Host.Connection connection;
        protected TargInfo.Local local;
        protected TargInfo.Host.UserInfo user;
        protected TargInfo targ;

        private readonly string errorLogPath;
        private readonly string rightsLogPath;
        private string headerInfo;
        #endregion

        #region Constructor
        public TargLogs(TargInfo t)
        {
            connection = t.host.connection;
            local = t.local;
            user = t.host.user;
            targ = t;

            //Create log paths///
            string errorLogDir = @"C:\Temp\Targ\ErrorLogs";
            string rightsLogDir = @"C:\Temp\Targ\Rights";

            if (!Directory.Exists(errorLogDir))
                Directory.CreateDirectory(errorLogDir);

            if (!Directory.Exists(rightsLogDir))
                Directory.CreateDirectory(rightsLogDir);
            /////////////////////

            if (!Util.ValidationCheck.isTestEnvironment()) //Production
            {
                errorLogPath = string.Format(
                    @"\\" + LOGHOST + @"\c$\Temp\Targ\ErrorLogs\{0} - {1}.txt",
                    Environment.UserName, DateTime.Now.ToString(DateTimeFormat));

                rightsLogPath = string.Format(
                    @"\\" + LOGHOST + @"\c$\Temp\Targ\Rights\{0} - {1}.txt",
                    Environment.UserName, DateTime.Now.ToString(DateTimeFormat));
            }
            #region TEST ENVIRONMENT
            else //Test
            {
                errorLogPath = string.Format(
                    @"c:\Temp\Targ\ErrorLogs\{0} - {1}.txt",
                    Environment.UserName, DateTime.Now.ToString(DateTimeFormat));

                rightsLogPath = string.Format(
                    @"c:\Temp\Targ\Rights\{0} - {1}.txt",
                    Environment.UserName, DateTime.Now.ToString(DateTimeFormat));
            }
            #endregion
        }
        #endregion

        #region LogError(Exception e)
        public void LogError(Exception e = null, string message = null)
        {
            if (!Util.ConnectionTest.IsHostAlive(LOGHOST))
                return;
            try
            {
                string[] sessionInfo = targ.SessionInfo();
                StreamWriter writer = new StreamWriter(errorLogPath);

                if (!string.IsNullOrWhiteSpace(message))
                {
                    writer.Write("Build: {2}{0}" +
                             "{1}{0}", Environment.NewLine, targBuild, message);
                }
                else
                {
                    writer.Write("=============================================================={0}" +
                                "                           EXCEPTION                          {0}" +
                                "=============================================================={0}" +
                                "{1}{0}" +
                                "=============================================================={0}" +
                                "                          SESSION INFO                        {0}" +
                                "=============================================================={0}" +
                                "Build: {2}{0}", Environment.NewLine, e, targBuild);
                }

                foreach (string s in sessionInfo)
                {
                    writer.WriteLine(s);
                }
                writer.Close(); 
                
            }
            catch (Exception f)
            {
                Util.MsgBox.Info("Failed to write error log.");
                Util.MsgBox.Info(f.ToString());
            }
        }
        #endregion

        #region LogAction
        public void LogAction(int timeSpan, string manageChoice)
        {
            if (!Util.ConnectionTest.IsHostAlive(LOGHOST))
                return;
            
            headerInfo = string.Format(
                    "BUILD: {1}{0}" +
                    "Host Name: {2}{0}" +
                    "Host IP: {3}{0}" +
                    "User ID: {4}{0}",
                    Environment.NewLine, targBuild, connection.HostName,
                    connection.HostIP, user.UserID);
            try
            {
                string addRemoveMsg = "***RIGHTS REMOVED***";

                if (manageChoice == AdTasks.ADD)
                {
                    addRemoveMsg = string.Format(
                     "***Rights to be removed: {0} (Local Time)***",
                     DateTime.Now.AddHours(timeSpan));
                }

                StreamWriter writer = new StreamWriter(rightsLogPath);
                writer.Write(headerInfo + Environment.NewLine + addRemoveMsg);
                writer.Close();
            }
            catch (Exception)
            {
                Util.MsgBox.Info("Failed to write log file.");
            }
        }
        #endregion
    }
}