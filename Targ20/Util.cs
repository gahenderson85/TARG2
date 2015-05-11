////////////////////////////////////////
//Class: Util.cs
//
//Usage: Utility class that provides 
//shared functionality within Targ 2.0
//
////////////////////////////////////////

using System;
using System.Management;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Targ20
{
    public static class Util
    {

        /// <summary>
        /// Used to write a VB Script name targinfo.vbs which is run by a windows service to
        /// collect host data (current user, host name, local time).
        /// </summary>
        #region class Script
        public static class Script
        {
            public static void WriteScript(TargInfo t)
            {
                string targInfoPath = string.Format(
                               @"\\{0}\c$\windows\system32\targinfo.vbs", t.host.connection.HostIP);

                StreamWriter writer = new StreamWriter(targInfoPath);
                #region targinfo.vbs
                //Writes targinfo.vbs used to collect Host data.
                //Collected data is written to targinfo.txt
                writer.Write(
                    @"On Error Resume Next{0}" +
                    @"Dim fso, infopath, localTime, computername, username, n{0}" +
                    @"Set objNetwork = CreateObject(""Wscript.Network""){0}" +
                    @"Set objWMI = GetObject( ""winmgmts:\\.\root\cimv2"" ){0}" +
                    @"Set colItems = objWMI.ExecQuery( ""Select * from Win32_ComputerSystem"" ){0}" +
                    @"For Each objItem in colItems{0}" +
                    @"username = objItem.UserName{0}" +
                    @"username = Replace(username, ""{1}\"", """"){0}" +
                    @"next{0}" +
                    @"if IsEmpty(username) or IsNull(username) then{0}" +
                    @"username = ""NOT FOUND""{0}" +
                    @"End If{0}" +
                    @"n = vbNewLine{0}" +
                    @"computername = objNetwork.ComputerName{0}" +
                    @"Set fso = CreateObject(""Scripting.FileSystemObject""){0}" +
                    @"Set infopath = fso.CreateTextFile(""c:\windows\system32\targinfo.txt"", True){0}" +
                    @"infopath.WriteLine(username & n & computername & n & Now()){0}" +
                    @"infopath.Close",
                    Environment.NewLine,
                    Environment.UserDomainName);
                #endregion
                writer.Close();
            }
            
            public static void ReadHostInfo(TargInfo t)
            {
                string filePath = string.Format(
                        @"\\{0}\c$\windows\system32\targinfo.txt", t.host.connection.HostIP);

                //Read targinfo.txt from host
                //Contains: Current User, Host Name, Time
                if (ValidationCheck.FileExists(filePath))
                {
                    StreamReader reader = new StreamReader(filePath);
                    t.host.user.CurrentUser = reader.ReadLine();
                    t.host.connection.HostName = reader.ReadLine();
                    t.host.removalTask.HostTime = reader.ReadLine();

                    //Match local time with host time
                    //Used later to calculate time difference 
                    //between connection and granting rights.
                    //see Tools.FormatDateTime()
                    t.local.LocalTime = DateTime.Now;
                    reader.Close();
                }
            }    
        }
        #endregion

        #region class ConnectionTest
        public static class ConnectionTest
        {
            //Tests WMI Connection to host
            //Stores result in Targinfo.host.connect.IsWmiActive
            public static bool TestWmi(TargInfo t)
            {
                if (!string.IsNullOrWhiteSpace(t.host.connection.HostIP))
                {
                    try
                    {
                        ManagementPath path = new ManagementPath(string.Format(
                                        @"\\{0}\root\cimv2", t.host.connection.HostIP));
                        ManagementScope scope = new ManagementScope(path);

                        scope.Connect();

                        return t.host.connection.IsWmiActive = true;
                    }
                    catch (Exception)
                    {
                        return t.host.connection.IsWmiActive = false;
                    }
                }

                t.mainForm.WriteToConsole("::WMI Failed. Using Script Method::\n");
                return t.host.connection.IsWmiActive = false;
            }

            //Test if host is pingable
            public static bool IsHostAlive(string hostIP, bool dev = false, int timeout = 2000)
            {
                string pingFailed = "Failed to connect to host! Is host online?";
                Ping ping = new Ping();
                try
                {
                    PingReply reply = ping.Send(hostIP, timeout);
                    if (reply != null && reply.Status == IPStatus.Success)
                        return true;
                    
                    if (dev)
                        MsgBox.Info(pingFailed);
                    
                    return false;
                }
                catch (Exception)
                {
                    if (dev)
                        MsgBox.Info(pingFailed);

                    return false;
                }
            }
        }
        #endregion

        //Preconfigured message boxes
        #region class MsgBox
        public static class MsgBox
        {
            public static bool Confirm(string message)
            {
                DialogResult result =
                        MessageBox.Show(message,
                        "Targ 2.0",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                return result == DialogResult.Yes;
            }
            
            public static void Info(string message)
            {
                MessageBox.Show(
                    message,
                    "Targ 2.0",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            
        }
        #endregion

        /// <summary>
        /// Methods used to check if scheduled task succeeded.
        /// </summary>
        #region class ValidationCheck
        public static class ValidationCheck
        {
            #region FinalCheck()
            //***Not implemented in production, test only
            //After rights granted verifies rights exist on host,
            //& Scheduled Task exists.
            public static bool FinalCheck(TargInfo targ, bool displayTask = false)
            {
                bool removalTaskExists;
                RemoteTasks remote;
                try
                {
                    remote = new RemoteTasks(targ);
                    remote.GetHostInfo();

                    AdTasks ad = new AdTasks(targ);
                    ad.CheckLocale();
                    ad.CheckExistingRights();
                }
                catch (Exception)
                {
                    MsgBox.Info("Failed to complete Final Check");
                }

                if (QueryTaskScheduler(targ, true))
                {
                    removalTaskExists = true;
                }
                else
                {
                    RemoteTasks remote1 = new RemoteTasks(targ);
                    removalTaskExists = remote1.jobFileExists();
                }

                bool adminExists = targ.host.user.AdminRightsExist;

                targ.mainForm.WriteToConsole(string.Format(
                        "Task Exists:  {0}{2}" +
                        "Admin Exists: {1}{2}",
                        removalTaskExists,
                        adminExists,
                        Environment.NewLine));

                return removalTaskExists && adminExists;
            }

            //If no domain we are in test environment.
            //does not run checks against Active D.
            public static bool isTestEnvironment()
            {
                if (Environment.UserDomainName == Environment.MachineName)
                    return true;
                
                return false;
            }

            /// <summary>
            /// Checks if file exists, allows timeout
            /// </summary>
            /// <param name="filePath">File path to find</param>
            /// <param name="timeout">Defaults to 10 seconds</param>
            /// <returns></returns>
            public static bool FileExists(string filePath, int timeout = 10)
            {
                int timeoutCount = 0;
                while (!File.Exists(filePath))
                {
                    Thread.Sleep(1000);
                    timeoutCount++;

                    if (timeoutCount < timeout) continue;
                    MessageBox.Show("::Failed to read host data::");
                    return false;
                }
                return true;
                }
            }
            #endregion
            
            /// <summary>
            /// Enables/Disables remote registry
            /// </summary>
            /// <param name="enable">Enable/Disable remote registry</param>
            /// <param name="hostIP">Host's ip address</param>
            public static void RemoteRegistry(bool enable, string hostIP)
            {
                //Starts/Stops remote registry service on host machine.
                string argStartRemoteReg = string.Format(@"/c sc \\{0} start remoteregistry", hostIP);
                string argStopRemoteReg = string.Format(@"/c sc \\{0} stop remoteregistry", hostIP);
                if (enable)
                {
                    Tools.RunProcess(argStartRemoteReg, false);
                    return;
                }
                Tools.RunProcess(argStopRemoteReg, false);
            }
            
            /// <summary>
            /// Queries task scheduler on host machine.
            /// </summary>
            /// <param name="targ">TargInfo object</param>
            /// <param name="displayTask">Output results</param>
            /// <returns>True if task is found.</returns>
            /// ***No longer used in Production, test only.
            public static bool QueryTaskScheduler(TargInfo targ, bool displayTask = false)
            {
                //Queries schtasks on remote machine to check for
                //the scheduled removal task.
                bool queryResults;

                //******XP FIX********
                //SCHTASKS throws an access denied error
                //when running a query.
                string taskPath = string.Format(
                        @"\\{0}\c$\windows\tasks\{1}.job", targ.host.connection.HostIP, targ.host.user.UserID);
                string xpfixUserPath = string.Format(@"\\{0}\c$\users", targ.host.connection.HostIP);

                if (!Directory.Exists(xpfixUserPath))
                {
                    if (File.Exists(taskPath))
                    {
                        if (displayTask)
                        {
                            targ.mainForm.WriteToConsole("Task Found ::XP MACHINE::");
                        }
                        return true;
                    }
                }
                //********************

                string argQueryTaskScheduler = string.Format(
                                @"/k schtasks /query /s {0} /tn {1} | findstr ""{1}""", 
                                targ.host.connection.HostIP, targ.host.user.UserID);

                RemoteRegistry(true, targ.host.connection.HostIP);
                Tools.TimeOut(1500);
                string output = Tools.RunProcess(argQueryTaskScheduler, true);

                if (output.Contains(targ.host.user.UserID))
                {
                    if (displayTask)
                    {
                        targ.mainForm.WriteToConsole("Task Found: " + output);
                    }
                    queryResults = true;
                }
                else
                {
                    targ.mainForm.WriteToConsole("Task Not Found...");
                    queryResults = false;
                }
                RemoteRegistry(false, targ.host.connection.HostIP);
                return queryResults;
            }
            #endregion

        public static class Tools
        {
            #region ExtractSubstring()
            //Returns string containing all text after findLastOf - trim (if specified)
            public static string ExtractSubstring(string strToSearch, string findLastOf, int trim = 0)
            {
                int indexCount = strToSearch.Length - trim;
                int lastEntryOf = strToSearch.LastIndexOf(findLastOf) + findLastOf.Length;
                return strToSearch.Substring(lastEntryOf, indexCount - lastEntryOf);
            }
            #endregion

            /// <summary>
            /// Runs a cmd shell with specified command.
            /// </summary>
            /// <param name="args">User specified argument</param>
            /// <param name="readOutput">Gets output of cmd shell</param>
            /// <returns>Output of cmd shell</returns>
            #region RunProcess()
            public static string RunProcess(string args, bool readOutput)
            {
                Process p = new Process();

                //No longer used in production.
                if (readOutput)
                {
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                }

                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = "CMD.EXE";
                p.StartInfo.Arguments = args;
                p.Start();

                if (!readOutput)
                {
                    return null;
                }
                else
                {
                    string outBuffer = string.Empty;
                    while (!p.StandardOutput.EndOfStream)
                    {
                        //Extract and scrub output
                        string[] outArray;
                        outBuffer = p.StandardOutput.ReadToEnd();
                        outArray = outBuffer.Split('\n');
                        outBuffer = outArray[0].Replace("    ", "");
                        p.WaitForExit();
                    }
                    return outBuffer;
                }
            }
            #endregion

            #region TimeOut()
            public static void TimeOut(int milliseconds)
            {
                Thread.Sleep(milliseconds);
            }
            #endregion

            /// <summary>
            /// Formats date/time in English / French time format
            /// </summary>
            /// <param name="info">TargInfo object</param>
            /// <param name="timeSpan">Timeframe in hours to grant rights</param>
            /// <param name="forceEnglish">Force US time format</param>
            #region FormatDateTime()
            public static void FormatDateTime(TargInfo info, int timeSpan, bool forceEnglish = false)
            {
                DateTime dt;

                if (timeSpan == 2)
                {
                    //DevTools = create removal task for 2 minutes, test only.
                    dt = Convert.ToDateTime(info.host.removalTask.HostTime).AddMinutes(timeSpan);
                }
                else
                {
                    dt = Convert.ToDateTime(info.host.removalTask.HostTime).AddHours(timeSpan);
                }

                //Subtract time from connection to time rights are granted
                TimeSpan timediff = DateTime.Now.Subtract(info.local.LocalTime);
                dt = dt + timediff; // add difference
                #region Time Formats
                const string ENGLISH = "MM/dd/yyyy"; //English - DD/MM/YYYY HH:MM
                const string FRENCH = "yyyy/MM/dd"; //French - YYYY/MM/DD HH:MM
                const string UNIVERSALTIME = "HH:mm"; //Military/Universal Time
                #endregion
                if (forceEnglish)
                {
                    info.host.removalTask.EndDate = dt.ToString(ENGLISH);
                }
                else
                {
                    info.host.removalTask.EndDate = dt.ToString(info.host.connection.PathAdminLang == TargInfo.English ? ENGLISH : FRENCH);
                }
                info.host.removalTask.EndTime = dt.ToString(UNIVERSALTIME);


            }
            #endregion

            public static string settingsFile = @"C:\windows\system32\targsettings.txt";
            /// <summary>
            /// Read targ settings.
            /// </summary>
            /// <param name="t">TargInfo Object</param>
            public static void ReadTargSettings(TargInfo t)
            {
                
                StreamWriter writer;
                StreamReader reader;

                if (!File.Exists(settingsFile))
                {
                    writer = new StreamWriter(settingsFile);
                    writer.WriteLine("TRUE");
                    writer.WriteLine("TRUE");
                    writer.Close();
                    return;
                }
                else
                {
                    reader = new StreamReader(settingsFile);
                    string autoCopy = reader.ReadLine().ToUpper();
                    string slowText = reader.ReadLine().ToUpper();
                    reader.Close();

                    if (autoCopy != "TRUE")
                    {
                        t.local.AutoCopy = false;
                    }
                    else { t.local.AutoCopy = true; }

                    if (slowText != "TRUE")
                    {
                        t.local.SlowText = false;
                    }
                    else { t.local.SlowText = true; }
                }

            }

            /// <summary>
            /// Save targ settings.
            /// </summary>
            /// <param name="t">TargInfo Object</param>
            public static void SaveTargSettings(TargInfo t)
            {
                StreamWriter writer = new StreamWriter(settingsFile, false);
                writer.WriteLine(t.local.AutoCopy);
                writer.WriteLine(t.local.SlowText);
                writer.Close();
            }

            /// <summary>
            /// Run a WMI query to get data from Win32
            /// </summary>
            /// <param name="queryString">Win32 Class Object</param>
            /// <param name="hostIP">Host ip address</param>
            /// <returns></returns>
            public static ManagementObjectCollection WmiQuery(string queryString, string hostIP)
            {
                //Create path & scope
                ManagementPath path = new ManagementPath(string.Format(@"\\{0}\root\cimv2", hostIP));
                ManagementScope scope = new ManagementScope(path);
                scope.Connect();

                ObjectQuery wmiQuery = new ObjectQuery("SELECT * FROM Win32_" + queryString);
                ManagementObjectSearcher wmiSearcher = new ManagementObjectSearcher(scope, wmiQuery);
                ManagementObjectCollection wmiCollection = wmiSearcher.Get();
                return wmiCollection;
            }
        }
    }
}