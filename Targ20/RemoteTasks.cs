using System;
using System.Windows.Forms;
using System.Management;
using System.IO;

namespace Targ20
{
    public class RemoteTasks
    {
        #region Declarations
        protected TargInfo targ;
        protected TargInfo.Host.Connection connection;
        protected TargInfo.Local local;
        protected TargInfo.Host.UserInfo user;
        protected TargInfo.Host host;
        protected string jobFilePath;
        protected string targInfoPath;
        #endregion

        #region RemoteManagement() - Constructor
        public RemoteTasks(TargInfo t) 
        { 
            targ = t; 
            connection = t.host.connection;
            local = t.local;
            user = t.host.user;
            jobFilePath = string.Format(@"\\{0}\C$\Windows\Tasks\{1}.job",
                            connection.HostIP,
                            user.UserID);
            targInfoPath = string.Format(@"\\{0}\C$\Windows\System32\targinfo.txt",
                            connection.HostIP);
        }
        #endregion

        /// <summary>
        /// Queries host for active user, host name & local time.
        /// </summary>
        /// <param name="devOutput">Output dev test info to Main.txtConsole</param>
        #region GetHostInfo()
        public void GetHostInfo(bool devOutput = false)
        {
            Cursor.Current = Cursors.WaitCursor;

            //Query data via WMI
            if (connection.IsWmiActive)
            {
                string[] queryString = {"OperatingSystem", "ComputerSystem", "LocalTime"};
                ManagementObjectCollection wmiCollection;

                //Get host name
                wmiCollection = Util.Tools.WmiQuery(queryString[0], connection.HostIP);
                foreach (ManagementObject m in wmiCollection)
                {
                    connection.HostName = m["csname"].ToString();
                }

                //Get current user
                wmiCollection = Util.Tools.WmiQuery(queryString[1], connection.HostIP);
                foreach (ManagementObject m in wmiCollection)
                {
                    if (m["username"] != null)
                    {
                        user.CurrentUser = m["username"].ToString().Replace(local.LocalDomain + "\\", "");
                    }
                    else
                    {
                        targ.mainForm.WriteToConsole(
                            "Active user not found. May be logged on via RDP.");
                    }
                }

                //Get Local Time
                wmiCollection = Util.Tools.WmiQuery(queryString[2], connection.HostIP);
                foreach (ManagementObject m in wmiCollection)
                {
                    targ.host.removalTask.HostTime = string.Format(
                        "{0:00}/{1:00}/{2:00} {3:00}:{4:00}",
                        m["Month"], m["Day"], m["Year"], m["Hour"], m["Minute"]);
                } 
            }
            //Get data via Service / Script
            else 
            {
                if (File.Exists(targInfoPath))
                {
                    File.Delete(targInfoPath);
                }
                //Original TARG fails to connect to WMI with Hosts on VPN
                //This method omits the use of WMI(via RPC) and prepares data client side
                //SC.EXE Arg used to create/run/delete a service to run targinfo.vbs
                string getHostInfoArg = string.Format(
                    @"/c sc \\{0} create targinfo binPath= " +
                    @"""cmd /c start cscript.exe targinfo.vbs"" " +
                    @"type= own & sc \\{0} start targinfo " +
                    @"& sc \\{0} delete targinfo & exit",
                    connection.HostIP);
                
                Util.Script.WriteScript(targ); //Write vb script to host
                Util.Tools.RunProcess(getHostInfoArg, false); //Run SC.EXE under cmd shell
                Util.Script.ReadHostInfo(targ); //Fetch data on host machine.
            }
            Cursor.Current = Cursors.Default;
        }
        #endregion

        /// <summary>
        /// Creates a scheduled task on host to remove admin rights.
        /// </summary>
        /// <param name="timeSpan">Time in hours to grant rights.</param>
        /// <param name="forceEnglish">Failover if French lang machine uses US time format.</param>
        #region CreateRemovalTask()
        public void CreateRemovalTask(int timeSpan = 0, bool forceEnglish = false)
        {
            Cursor.Current = Cursors.WaitCursor;
            //Delete prior job/task file
            if (File.Exists(jobFilePath))
                File.Delete(jobFilePath);

            try
            {
                //Scrub date/time for use in schtasks.exe
                Util.Tools.FormatDateTime(targ, timeSpan, forceEnglish);
                if (connection.IsWmiActive)
                {
                    bool taskFail = CreateTaskRemoteRegistry();

                    //Recursive user of CreateRemovalTask()
                    //Some French language pc's use US time format
                    if (taskFail)
                    {
                        CreateRemovalTask(timeSpan, true); //Force English / US time format
                    }

                    if (!jobFileExists()) //Final fail over - user Service / VB Script to create task
                    {
                        CreateTaskLocalService();
                        Util.Tools.TimeOut(500);

                        //Creating scheduled task failed. 
                        //Rights will need to be removed manually.
                        if (!jobFileExists()) 
                        {
                            TaskCreationFailed();
                        }
                    }
                }
                else
                {
                    Util.Tools.FormatDateTime(targ, timeSpan, forceEnglish);
                    CreateTaskLocalService();
                    if (!jobFileExists())
                    {
                        if (forceEnglish) //Final attempt failed.
                        {
                            TaskCreationFailed();
                            return;
                        }
                        //Recursion try again with english language format
                        CreateRemovalTask(timeSpan, true);
                    }
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception e)
            {
                targ.mainForm.WriteToConsole("::Failed to create rights removal task::");
                targ.logs.LogError(e);
            }
        }

        private void TaskCreationFailed()
        {
            targ.mainForm.WriteToConsole(string.Format(
            "{0}::Failed to verify removal task created::{0}" +
            "::Manually remove rights or try granting rights again::",
            Environment.NewLine));
            targ.logs.LogError(null, "Failed to create Scheduled Removal Task." + Environment.NewLine);
        }

        #region CreateTaskLocalService(), CreateTaskRemoteRegistry()
        private void CreateTaskLocalService()
        {
            //Creates service on host, runs service using createSvcArg which will run
            //the command in schtaksArgNoRemoteReg - creating a Scheduled Task
            //service is deleted and exit's.
            //Schedule task contains command - CMD /C NET LOCALGROUP {ADMINISTATORS} {USERID} /DELETE
            //When command runs specified user will be removed from local admin group.
            string schtasksArgNoRemoteReg = string.Format(
               @"/k schtasks /create /sc once /tn \""{1}\"" " +
               @"/tr \""CMD /C NET LOCALGROUP {0} {1} /DELETE\"" " + // requires escaped \"
               @"/s {2} /ru System /st {3} /sd {4} /f /v1 /z",
               connection.PathAdminLang,
               user.UserID, connection.HostIP,
               targ.host.removalTask.EndTime, targ.host.removalTask.EndDate);
            
            string createSvcArg = string.Format(
                @"/c sc \\{0} create targinfo binPath= " +
                @"""cmd " + schtasksArgNoRemoteReg + @""" " +
                @"type= own & sc \\{0} start targinfo " +
                @"& sc \\{0} delete targinfo & exit",
                targ.host.connection.HostIP);

            Cursor.Current = Cursors.WaitCursor;
            Util.Tools.RunProcess(createSvcArg, false);
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Uses SCHTASKS.EXE to create Scheduled Task, needs Remote Registry enabled.
        /// </summary>
        /// <returns></returns>
        private bool CreateTaskRemoteRegistry()
        {
            string outBuffer; //CMD window output
            bool taskFailed = false;
            #region SCHTASKS.EXE INFO
            /*
                         * Arg for SCHTASKS.EXE to create a scheduled task, Does not work over VPN.
                         * --LEGEND--
                         * /C - Close CMD when completed
                         * /create - Create new task
                         * /sc - Schedule task to run once
                         * /tn - Task Name (User ID)
                         * /tr - File to run (CMD /C NET LOCALGROUP ADMINISTATORS %USER% /DELETE)
                         * /s - Remote Host
                         * /ru - Ruan As (System)
                         * /st - Start Time (HH:MM)
                         * /sd - Start Date (Regional - ENG(MM/DD/YYYY) - FRENCH(YYYY/MM/DD))
                         * /f - Suppress warnings / messages
                         * /v1 - Create task visible to older systems (XP, 2000, 2003)
                         * /z - Delete task after run
                         * --END LEGEND--
                         */
            #endregion
            string schtasksArg = string.Format(
                @"/k schtasks /create /sc once /tn ""{1}"" " +
                @"/tr ""CMD /C NET LOCALGROUP {0} {1} /DELETE"" " +
                @"/s {2} /ru System /st {3} /sd {4} /f /v1 /z",
                connection.PathAdminLang,
                targ.host.user.UserID, targ.host.connection.HostIP,
                targ.host.removalTask.EndTime, targ.host.removalTask.EndDate);
            

            Cursor.Current = Cursors.WaitCursor;
            //Util.Script.WriteScript(Util.Script.Write.BatchRemoval, targ); //**See method below
            Util.RemoteRegistry(true, connection.HostIP);
            Util.Tools.TimeOut(1500);
            outBuffer = Util.Tools.RunProcess(schtasksArg, true);
            if (outBuffer.Length < 5)
                taskFailed = true;
            Util.RemoteRegistry(false, connection.HostIP);
            Cursor.Current = Cursors.Default;
            return taskFailed;
        }
        #endregion
        #endregion

        public bool jobFileExists()
        {
            
            int count = 0;
            while (!File.Exists(jobFilePath))
            {
                if (count > 40)
                {
                    return false;
                }
                count++;
                Util.Tools.TimeOut(500);
            }
            return true;
        }

        //Method omitted in favor or more reliable use of RemoteRegistry and time.
        //problems with querying task scheduler - works intermittently.
        //instead of querying task scheduler we can very task was created
        //by checking if file exits - C:\Windows\Tasks\USERID.JOB
        #region CreateRemovalTask() - Original Method using QueryTaskScheduler
        /*
        public void CreateRemovalTask(int timeSpan = 0, bool forceEnglish = false, bool secondRun = false)
        {
            string taskFilePath = string.Format(@"\\{0}\Windows\{1}.job",
                        connection.HostIP,
                        user.CurrentUser);

            Cursor.Current = Cursors.WaitCursor;
            try
            {
                Util.Tools.FormatDateTime(targ, timeSpan, forceEnglish);

                if (connection.IsWmiActive)
                {
                    CreateTaskRemoteRegistry();
                    if (File.Exists(taskFilePath))
                        return;

                    if (!Util.QueryTaskScheduler(targ))
                    {
                        CreateTaskLocalService();
                        if (File.Exists(taskFilePath))
                            return;

                        if (!Util.QueryTaskScheduler(targ))
                        {
                            //Recursion 2nd check.
                            if (!secondRun)
                            {
                                Util.Tools.FormatDateTime(targ, timeSpan, true);
                                CreateRemovalTask(timeSpan, true, true);
                            }
                            else
                            {
                                targ.mainForm.WriteToConsole(string.Format(
                                "{0}::Failed to verify removal task created::{0}" +
                                "::Manually remove rights or try granting rights again::",
                                Environment.NewLine));
                            }
                        }
                    }
                }
                else
                {
                    CreateTaskLocalService();
                    if (File.Exists(taskFilePath))
                        return;

                    if (!Util.QueryTaskScheduler(targ))
                    {
                        if (!secondRun)
                        {
                            Util.Tools.FormatDateTime(targ, timeSpan, true);
                            CreateRemovalTask(timeSpan, true, true);
                        }
                        else
                        {
                            targ.mainForm.WriteToConsole(string.Format(
                            "{0}::Failed to verify removal task created::{0}" +
                            "::Manually remove rights or try granting rights again::",
                            Environment.NewLine));
                            targ.logs.LogError(null, "Failed to create Scheduled Removal Task." + Environment.NewLine);
                        }
                    }
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception e)
            {
                targ.mainForm.WriteToConsole("::Failed to create rights removal task::");
                targ.logs.LogError(e);
            }
        }
         */
        #endregion
    }
}