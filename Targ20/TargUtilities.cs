using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace Targ20
{
    public static class TargUtilities
    {
        #region isWmiActive()
        public static bool isWmiActive(string hostIP)
        {
            if (!string.IsNullOrWhiteSpace(hostIP))
            {
                try
                {
                    ManagementPath path = new ManagementPath(string.Format(
                                    @"\\{0}\root\cimv2", hostIP));
                    ManagementScope scope = new ManagementScope(path);

                    scope.Connect();

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region MsgBoxConfirm()
        public static bool MsgBoxConfirm(string message)
        {
            DialogResult result =
                    MessageBox.Show(message,
                    "Targ 2.0",
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region MsgBoxInfo()
        public static void MsgBoxInfo(string message)
        {
            MessageBox.Show(
                message,
                "Targ 2.0",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        #endregion

        #region IsHostOnline()
        public static bool IsHostAlive(string hostIP)
        {
            Ping ping = new Ping();
            try
            {
                PingReply reply = ping.Send(hostIP, 2000);
                if (reply.Status == IPStatus.Success)
                {
                    return true;
                }
                else
                {
                    TargUtilities.MsgBoxInfo("Failed to connect to host! Is host online?");
                    return false;
                }
            }
            catch (Exception)
            {
                TargUtilities.MsgBoxInfo("Failed to connect to host! Is host online?");
                return false;
            }
        }
        #endregion

        #region ExtractInfo()
        //Returns string containing all text after findLastOf - trim
        public static string ExtractInfo(string strToSearch, string findLastOf, int trim = 0)
        {
            int indexCount = strToSearch.Length - trim,
                lastEntryOf = strToSearch.LastIndexOf(findLastOf) + findLastOf.Length;

            string results = strToSearch.Substring(lastEntryOf, indexCount - lastEntryOf);

            return results;
        }
        #endregion

        #region verifyRemovalTask()
        public static bool FinalCheck(string hostIP, string userID, Main f, bool isWmiActive, bool displayTask = false)
        {
            HostInfo hostInfoUtil = new HostInfo();
            hostInfoUtil.HostIP = hostIP;
            hostInfoUtil.UserID = userID;

            bool removalTaskExists

            try
            {
                RemoteTasks remote = new RemoteTasks(hostInfoUtil, f);
                remote.GetHostInfo(true, false);

                ADTasks ad = new ADTasks(hostInfoUtil, f);
                ad.CheckLocale(true);
                ad.CheckExistingRights(true, false, false);
            }
            catch (Exception)
            {
                MsgBoxInfo("Failed to complete Final Check");
            }
           
            if (TargUtilities.QueryTaskScheduler(hostIP, userID))
            {
                removalTaskExists = true;
            }
            else
            {
                RemoteTasks remote = new RemoteTasks(hostInfoUtil,f);
                rem
            }

            //ADTasks adTasksVerify = new ADTasks(hostInfoUtil,f);
            //adTasksVerify.CheckLocale(isWmiActive);
            //adTasksVerify.CheckExistingRights(isWmiActive,false,false);

            string batchPath = string.Format(@"\\{0}\c$\windows\system32\RemAdmin.bat", hostIP);
            
            bool batchRemovalExists = File.Exists(batchPath);
            bool adminExists = hostInfoUtil.AdminRightsExist;

            TargUtilities.MsgBoxInfo(string.Format(
                    "Batch Exists: {0}{3}" +
                    " Task Exists: {1}{3}" +
                    "Admin Exists: {2}{3}",
                    batchRemovalExists,
                    removalTaskExists,
                    adminExists,
                    Environment.NewLine));

            if (batchRemovalExists && removalTaskExists && adminExists)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }

        public static bool QueryTaskScheduler(string hostIP, string userID, bool displayTask = false)
        {
            string args = string.Format(@"/c schtasks /query /s {0} /tn {1} | findstr ""{1}""", hostIP, userID);

            

            Process p = new Process();
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "CMD.EXE";
            p.StartInfo.Arguments = args;
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            if (output.Contains(userID))
	        {
                if (displayTask)
                {
                    MsgBoxInfo(output);
                }
		        return true;
	        }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
