/*
 * 
 * DevTools is used for general testing of methods and process flow validation.
 * 
 * ***Can be accessed by double clicking the "User ID" label on Main form.
 * 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;
using System.Net;

namespace Targ20
{
    public partial class DevTools : Form
    {
        #region Declarations

        protected List<string> CommandList;
        protected bool DevOutputFalse = false;
        protected bool DevOutputTrue = true;
        protected bool NoComboAdd = false;
        protected bool disposed = false;

        protected TargInfo targDev;
        private TargInfo.Host.Connection connection;
        private TargInfo.Local local;
        private TargInfo.Host.UserInfo user;

        private enum Command
        {
            RightsExist = 0,
            RightsExistWmi = 1,
            ListAdmins = 2,
            ListAdminsWmi = 3,
            GetInfoByScripts = 4,
            GetInfoByWmi = 5,
            UserExists = 6,
            AddTestAdmins = 7
        }

        #endregion

        #region Constructor

        public DevTools(TargInfo t)
        {
            InitializeComponent();
            targDev = t;
            connection = t.host.connection;
            local = t.local;
            user = t.host.user;          

            #region commandList List<string>

            CommandList = new List<string>
            {
                "Rights Exist on Host? (LDAP)",
                "Rights Exist on Host? (WMI)",
                "List Admins on Host (LDAP)",
                "List Admins on Host (WMI)",
                "Get Host Info (SCRIPT)",
                "Get Host Info (WMI)",
                "User Exists in AD?",
                "Add Test Admins"
            };
            comboBox2.DataSource = CommandList;

            #endregion

            #region dataGridHostInfo Rows / Cols

            dataGridHostInfo.Rows.Add("AdminRightsExist");
            dataGridHostInfo.Rows.Add("CurrentUser");
            dataGridHostInfo.Rows.Add("EndDate");
            dataGridHostInfo.Rows.Add("EndTime");
            dataGridHostInfo.Rows.Add("HostIP");
            dataGridHostInfo.Rows.Add("HostLocale");
            dataGridHostInfo.Rows.Add("HostName");
            dataGridHostInfo.Rows.Add("HostTime");
            dataGridHostInfo.Rows.Add("LocalTime");
            dataGridHostInfo.Rows.Add("UserID");
            dataGridHostInfo.Rows.Add("UserFullName");
            dataGridHostInfo.Rows.Add("UserIDExists");
            dataGridHostInfo.Rows.Add("IsWmiActive");

            dataGridHostInfo.RowHeadersVisible = false;
            dataGridHostInfo.ScrollBars = ScrollBars.None;

            #endregion

            #region Populate form fields

            //If IP not blank copy IP from Main Form
            txtIP.Text = targDev.mainForm.GetTxtHostIP != string.Empty ? targDev.mainForm.GetTxtHostIP : "127.0.0.1";
            txtUser.Text = targDev.mainForm.GetTxtUserID != string.Empty ? targDev.mainForm.GetTxtUserID : Environment.UserName;
            #endregion
        }

        #endregion

        #region Execute Command

        private void btnExecute_Click(object sender, EventArgs e)
        {
            connection.HostIP = txtIP.Text.Trim();
            Util.ConnectionTest.TestWmi(targDev);
            int cmdChoice = comboBox2.SelectedIndex;

            var execute = (Command) cmdChoice;
            RemoteTasks r;
            AdTasks ad;

            string[] vpnScope =
            {
                "10.40.244", "10.32.244",
                "10.40.245", "10.32.245",
                "10.40.246", "10.32.246"
            };

            bool isVpn = vpnScope.Any(s => txtIP.Text.Contains(s));

            Cursor.Current = Cursors.WaitCursor;

            switch (execute)
            {
                case Command.RightsExist:

                    #region Check for Exsisting Rights (Script)

                    user.AdminRightsExist = false;
                    if (txtIP.Text == string.Empty || txtUser.Text == string.Empty)
                    {
                        MessageBox.Show("IP & ID Cannot be blank");
                    }
                    else if (Util.ConnectionTest.IsHostAlive(txtIP.Text))
                    {
                        user.UserID = txtUser.Text;
                        connection.HostIP = txtIP.Text;

                        ad = new AdTasks(targDev);
                        connection.IsWmiActive = false; //DevTools Only
                        ad.CheckLocale();
                        ad.CheckExistingRights(NoComboAdd, DevOutputFalse);

                        targDev.mainForm.WriteToConsole(user.AdminRightsExist ? "Rights Exist." : "Rights DO NOT Exist.");
                    }

                    #endregion

                    break;

                case Command.RightsExistWmi:

                    #region Check for Existing Rights(WMI)

                    user.AdminRightsExist = false;
                    if (txtIP.Text == string.Empty || txtUser.Text == string.Empty)
                    {
                        MessageBox.Show("IP & ID Cannot be blank");
                    }
                    else if (Util.ConnectionTest.IsHostAlive(txtIP.Text))
                    {
                        if (isVpn)
                        {
                            MessageBox.Show("Fuction does not work with VPN clients.");
                            return;
                        }

                        user.UserID = txtUser.Text;
                        connection.HostIP = txtIP.Text;

                        ad = new AdTasks(targDev);
                        r = new RemoteTasks(targDev);

                        r.GetHostInfo();
                        ad.CheckLocale();
                        ad.CheckExistingRights(NoComboAdd, DevOutputFalse);


                        targDev.mainForm.WriteToConsole(user.AdminRightsExist ? "Rights Exist." : "Rights DO NOT Exist.");
                    }
                    else
                    {
                        MessageBox.Show("Host is not Online.");
                    }

                    #endregion

                    break;

                case Command.UserExists:

                    #region Check if User Esists in AD

                    if (Util.ValidationCheck.isTestEnvironment())
                    {
                        MessageBox.Show("Function does not work in TEST Environment");
                        return;
                    }
                    if (txtUser.Text == string.Empty)
                    {
                        MessageBox.Show("IP & ID Cannot be blank");
                    }
                    else
                    {
                        user.UserID = txtUser.Text;

                        ad = new AdTasks(targDev);
                        ad.FindUserActiveD();
                    }

                    #endregion

                    break;

                case Command.ListAdmins:

                    #region List all Admins on Host

                    if (txtIP.Text == string.Empty)
                    {
                        MessageBox.Show("IP cannot be blank");
                    }
                    else
                    {
                        connection.HostIP = txtIP.Text;
                        connection.IsWmiActive = false; //DevTools Only
                        ad = new AdTasks(targDev);
                        ad.CheckLocale();
                        ad.CheckExistingRights(NoComboAdd, DevOutputTrue);
                    }

                    #endregion

                    break;

                case Command.ListAdminsWmi:

                    #region List all Admins on Host(WMI)

                    if (txtIP.Text == string.Empty)
                    {
                        MessageBox.Show("IP cannot be blank");
                    }
                    else
                    {
                        if (isVpn)
                        {
                            MessageBox.Show("Fuction does not work with VPN clients.");
                            return;
                        }
                        connection.HostIP = txtIP.Text;

                        ad = new AdTasks(targDev);
                        r = new RemoteTasks(targDev);
                        ad.CheckLocale();
                        r.GetHostInfo();
                        ad.CheckExistingRights(NoComboAdd, DevOutputTrue);
                    }

                    #endregion

                    break;

                case Command.AddTestAdmins:

                    //To use create a file named AddAdmin.bat in the System32 dir.
                    //add a line for each user you want to add admin rights for
                    //ex. NET LOCALGROUP ADMINISTRATORS {USERID} /ADD
                    #region Add Test Admins
                    if (!File.Exists(@"c:\windows\system32\AddAdmin.bat"))
                    {
                        MessageBox.Show(@"File ""AddAdmin.bat"" does not exist.");
                    }
                    else
                    {
                        const string args = "/c AddAdmin.bat";
                        Util.Tools.RunProcess(args, false);
                        MessageBox.Show("--Added test users--");
                    }

                    #endregion

                    break;

                case Command.GetInfoByScripts:

                    #region Get Info By Scripts

                    if (txtIP.Text == string.Empty)
                    {
                        MessageBox.Show("IP cannot be blank.");
                    }
                    else
                    {
                        connection.HostIP = txtIP.Text;

                        r = new RemoteTasks(targDev);
                        r.GetHostInfo(DevOutputTrue);

                        targDev.mainForm.WriteToConsole(string.Format(
                            "==========================={0}" +
                            "GetInfoByScripts{0}" +
                            "==========================={0}" +
                            "Current User: {1}{0}" +
                            "Computer Name: {2}{0}" +
                            "Local Time: {3}{0}" +
                            "===========================",
                            Environment.NewLine,
                            user.CurrentUser,
                            connection.HostName,
                            targDev.host.removalTask.HostTime));
                    }

                    #endregion

                    break;

                case Command.GetInfoByWmi:

                    #region Get Info By WMI

                    if (txtIP.Text == string.Empty)
                    {
                        MessageBox.Show("IP cannot be blank.");
                    }
                    else
                    {
                        if (isVpn)
                        {
                            MessageBox.Show("Fuction does not work with VPN clients.");
                            return;
                        }

                        connection.HostIP = txtIP.Text;
                        r = new RemoteTasks(targDev);
                        r.GetHostInfo(DevOutputTrue);
                        targDev.mainForm.WriteToConsole(string.Format(
                            "==========================={0}" +
                            "GetInfoByWMI{0}" +
                            "==========================={0}" +
                            "Current User: {1}{0}" +
                            "Computer Name: {2}{0}" +
                            "Local Time: {3}{0}" +
                            "===========================",
                            Environment.NewLine,
                            user.CurrentUser,
                            connection.HostName,
                            targDev.host.removalTask.HostTime));
                    }

                    #endregion

                    break;
            }

            Cursor.Current = Cursors.Default;
        }

        #endregion

        #region Check for removal task (no VPN)

        private void button1_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            connection.HostIP = txtIP.Text.ToUpper().Trim();
            user.UserID = txtUser.Text.ToUpper().Trim();
            Util.ConnectionTest.TestWmi(targDev);
            Util.ValidationCheck.FinalCheck(targDev,true);
            Cursor.Current = Cursors.Default;
        }

        #endregion
        
        //***Do not use, not complete
        #region Run Report / Delete Logs for removed rights

        private void button2_Click(object sender, EventArgs e)
        {
            //Check online vpn
            //string scope = "10.32.244.";
            //for (int i = 0; i < 255; i++)
            //{
            //    if (Util.ConnectionTest.IsHostAlive(scope + i,false,100))
            //    {
            //        if (Directory.Exists(string.Format(@"\\{0}{1}\c$",scope,i)))
            //        {
            //            targDev.mainForm.WriteToConsole(scope + i);
            //        }
            //    }
            //}

            //Reads all log files in C:\temp\targ\rights and attempts to verify
            //rights were removed. If successful log file is deleted.
            #region RunReport & Delete Logs
            //int count = 0;

            //if (!(Environment.MachineName == "USATONHENDEG12" ||
            //      Environment.MachineName == "WIN7"))
            //{
            //    MessageBox.Show("Command locked by application administrator.");
            //    return;
            //}

            //connection.HostName = string.Empty;
            //connection.HostIP = string.Empty;
            //user.UserID = string.Empty;

            //Process the list of files found in the directory. 
            //string[] fileEntries = Directory.GetFiles(@"c:\temp\targ\rights\");
            //foreach (string fileName in fileEntries)
            //{
            //    bool pingfailed = false;
            //    const string strHostName = "Host Name: ",
            //        strUserID = "User ID: ";
            //    string fileInfo;

            //    char[] delimiters = {'\r', '\n'};

            //    var reader = new StreamReader(fileName);
            //    targDev.mainForm.WriteToConsole("Current File: " + Util.Tools.ExtractSubstring(fileName, @"\"));
            //    fileInfo = reader.ReadToEnd();
            //    reader.Close();

            //    string[] parts = fileInfo.Split(delimiters);
            //    count += 1;

            //    foreach (string s in parts)
            //    {
            //        if (pingfailed)
            //            continue;

            //        if (s.Contains(strHostName))
            //        {
            //            connection.HostName = Util.Tools.ExtractSubstring(s, strHostName);
            //            connection.HostIP = connection.HostName;
            //        }

            //        if (s.Contains(strUserID))
            //        {
            //            user.UserID = Util.Tools.ExtractSubstring(s, strUserID);
            //        }


            //        if (!(!(connection.HostName == string.Empty |
            //                user.UserID == string.Empty) &
            //              File.Exists(fileName))) continue;
            //        if (Util.ConnectionTest.IsHostAlive(connection.HostName, true))
            //        {
            //            if (!Util.QueryTaskScheduler(targDev,true))
            //            {
            //                RunReportAndDelete(connection.HostName, fileName);
            //            }
            //        }
            //        else
            //        {
            //            pingfailed = true;
            //        }
            //    }
            //}

            //if (targDev.mainForm.GetTxtConsole == string.Empty) return;
            //Clipboard.SetText(targDev.mainForm.GetTxtConsole);
            //targDev.mainForm.WriteToConsole(count.ToString());
            #endregion
        }

        #endregion

        #region CheckRightsAndDelete()

        private void RunReportAndDelete(string host, string fileName)
        {
            Util.ConnectionTest.TestWmi(targDev);
            bool taskExists;

            user.AdminRightsExist = false;


            var ad = new AdTasks(targDev);
            ad.CheckLocale();
            ad.CheckExistingRights(NoComboAdd, DevOutputFalse);

            taskExists = Util.QueryTaskScheduler(targDev, true);

            if (!user.AdminRightsExist &
                host == connection.HostName)
            {
                FileSystem.DeleteFile(fileName,
                    UIOption.OnlyErrorDialogs,
                    RecycleOption.SendToRecycleBin);

                targDev.mainForm.WriteToConsole(string.Format(
                    "{1}*::CONFIRMED REMOVAL::*{1}" +
                    "Deleted: {0}{1}",
                    Util.Tools.ExtractSubstring(fileName, @"\"),
                    Environment.NewLine));
            }
            else if (user.AdminRightsExist & !taskExists)
            {
                ad.RemoveRights();
                targDev.mainForm.WriteToConsole(string.Format(
                    "{1}::RIGHTS STILL PRESENT::{1}" +
                    "Deleted:{0}{1}",
                    Util.Tools.ExtractSubstring(fileName, @"\"),
                    Environment.NewLine));
            }
            else if (taskExists)
            {
                targDev.mainForm.WriteToConsole("Removal Task Still exists");
                targDev.mainForm.WriteToConsole(targDev.host.removalTask.Status);
            }

            targDev.mainForm.WriteToConsole(string.Format("Host Name: {0}{2}User ID:{1}{2}",
                connection.HostName,
                user.UserID,
                Environment.NewLine));
        }

        #endregion

        #region Misc Methods

        private void btnClearConsole_Click(object sender, EventArgs e)
        {
            targDev.mainForm.WriteToConsole("CLEAR");
        }

        private void btnPing_Click(object sender, EventArgs e)
        {
            if (Util.ConnectionTest.IsHostAlive(txtIP.Text))
                MessageBox.Show("Success!");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var rowInfo = new[]
            {
                user.AdminRightsExist.ToString(),
                user.CurrentUser,
                targDev.host.removalTask.EndDate,
                targDev.host.removalTask.EndTime,
                connection.HostIP,
                connection.HostLocale,
                connection.HostName,
                targDev.host.removalTask.HostTime,
                local.LocalTime.ToString(),
                user.UserID,
                user.UserFullName,
                user.UserIDExists.ToString(),
                connection.IsWmiActive.ToString()
            };


            //Update info from hostInfo for view in dataGridView1
            for (int i = 0; i < rowInfo.Length; i++)
            {
                DataGridViewRow R = dataGridHostInfo.Rows[i];
                R.Cells["VALUE"].Value = rowInfo[i];
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Width = Width == 585 ? 285 : 585;
        }

        #endregion

        #region FormClosed - Dispose()

        private void DevTools_FormClosed(object sender, FormClosedEventArgs e)
        {
            Dispose(disposed);
        }

        #endregion

        private void button4_Click(object sender, EventArgs e)
        {
            connection.HostIP = txtIP.Text.Trim();
            user.UserID = txtUser.Text.Trim();
            string queryRemoteRegistry = string.Format(
                        @"/k sc \\{0} query remoteregistry | findstr ""RUNNING""", connection.HostIP);

            Util.RemoteRegistry(true, connection.HostIP);
            Util.QueryTaskScheduler(targDev, true);
            Util.RemoteRegistry(false, connection.HostIP);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (!targDev.mainForm.GetTxtConsole.Contains("Online"))
            {
                Util.MsgBox.Info("You must connect to host first.");
                return;
            }
            RemoteTasks r = new RemoteTasks(targDev);
            r.CreateRemovalTask(2);
            button4.PerformClick();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string ip = txtIP.Text.Trim();
            string result = System.Net.Dns.GetHostEntry(ip).HostName;
            Util.MsgBox.Info(ip + " = " + result);
        }
    }
}