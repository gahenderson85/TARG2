// <code-header>
// <author>Greg Henderson - gahenderson85@gmail.com</author>
// <copyright>Greg Henderson, 2014</copyright>
//
// ABOUT TARG 2.0 (Temporary Admin Rights Granted 2.0):
//
//      TARG 2.0 was created to improve on draw backs of it's predecessor which was coded by a different author in VB .Net 2.5.
// The original version did not support client machine's connected through VPN (due to WMI issues) or client machine's using
// the French language.  I tried my best to make this modular and reusable, please read code comments. If you have any questions
// or comments please feel free to contact me.
//
//      TARG 2.0 is currently being used by my company's help desk. It has been tested and works on Win XP & Win 7. Furthermore
// it work's in both domain and non-domain(test) environments.
//
// GNU License:
// 
//      You are free to use / modify this code as you wish. Please give credit for any reuse of this applicaton/code.
//
// </code-header>
using System;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;

namespace Targ20
{
    public partial class Main : Form
    {

        #region Declarations
        private TargInfo targ;
        private TargInfo.Host.Connection connection;
        private TargInfo.Local local;
        private TargInfo.Host.UserInfo user;

        private RemoteTasks remoteTasksGrant = null;
        private RemoteTasks remoteTasksRemove = null;
        private AdTasks adTasksGrant = null;
        private AdTasks adTasksRemove = null;
        private TargLogs eventLogs = null;
        private DevTools dev = null;
        private int timeSpan = 8;
        private bool isProdEnv = true;
        private bool isVpn = false;
        private const string CLEAR_CONSOLE = "CLEAR";
        private const string CLIPBOARD = "clipboard";

        
         //Main.cs is passed to TargInfo.cs to allow other classes
         //access to text data elements in the main form.
        public string GetTxtHostIP { get { return txtHostIP.Text; } }
        public string GetTxtUserID { get { return txtUserID.Text; } }
        public string GetTxtConsole { get { return txtConsole.Text; } }
        #endregion

        #region Main()
        public Main()
        {
            InitializeComponent();
        }
        
        private void Main_Load(object sender, EventArgs e)
        {
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 30, 30)); //Create rounded edge form
            isProdEnv = (Environment.UserDomainName == Environment.MachineName) ? false : true; //if no domain test env 

            PlaceFormControls();
            CreateTargInfoObject();
            CreateEventLogObject();
            CreateSettingsToolTip();
        }
        #endregion

        
        #region Connect Scope
        private void btnConnect_Click(object sender, EventArgs e)
        {

            targ.Dispose();                //Dispose of any prior data **BROKEN, FIX
            user.AdminRightsExist = false; //**Remove once Dispose is fixed.
            
            if(PreConnectCheck(txtHostIP.Text.Trim()))
            {
                try
                {
                    adTasksGrant = new AdTasks(targ);   //Before we can connect and fetch host info
                    adTasksGrant.CheckLocale();         //we need to check it's Locale (english / french)

                    remoteTasksGrant = new RemoteTasks(targ);
                    remoteTasksGrant.GetHostInfo();
                }
                catch (Exception connectError)
                {
                    Util.MsgBox.Info(connectError.ToString());
                    WriteToConsole("::Failed to connect to host::");
                    eventLogs.LogError(connectError);
                    ClearForm(false);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtUserID.Text)) //populate txtUserID & update UserID
                {
                    txtUserID.Text = user.CurrentUser;
                    user.UserID = user.CurrentUser;
                }

                StatusReport(0); 
                ToggleButtonsAndHostIP(false, false); //Online with host, disable Remove button & txtHostIP
            }
            else
            {
                ClearForm(false); //Set initial form load state
            }
        }
        #endregion

        #region GrantRights Scope
        private void btnGrantRights_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUserID.Text))
            {
                Util.MsgBox.Info("User ID cannot be blank when granting rights.");
                return;
            }

            user.UserID = txtUserID.Text.Trim(); //Update UserID in case user has changed it.
            ToggleRadioControls(false);
            ToggleButtonsOnOff(false);

            try
            {
                adTasksGrant = new AdTasks(targ);
                //If Production environment query Active Directory
                //for user's full name and verify the ID exists.
                if (isProdEnv)
                {
                    adTasksGrant.FindUserActiveD();
                    if (!user.UserIDExists) //User not found in AD, cannot proceed.
                    {
                        StatusReport(2); //cancelling...
                        return;
                    }
                }
                //Found user in AD, let's see if they already have
                //admin rights on the host machine.
                adTasksGrant.CheckExistingRights(false, false);
                remoteTasksGrant = new RemoteTasks(targ);
            }
            catch (Exception)
            {
                ClearForm(false);
                return;
            }


            bool result = MsgBoxConfirmRights();
            if (result)
            {
                if (!user.AdminRightsExist)
                    adTasksGrant.GrantRights(timeSpan); //Add user to the Administrators group on Host.
                
                remoteTasksGrant.CreateRemovalTask(timeSpan); //Create Scheduled task to remove rights.
                eventLogs.LogAction(timeSpan, AdTasks.ADD); //Create a log file
                StatusReport(1);
                CopyToClipboard(); //Attempts to copy contents of txtConsole to clipboard.
            }
            else
            {
                StatusReport(2); //cancelling...
            }
            ToggleRadioControls(true);
            ToggleButtonsAndHostIP(true, true);
            ToggleButtonsOnOff(true);
            ClearForm(false);
        }

        
        #endregion

        #region Remove Scope
        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHostIP.Text))
            {
                Util.MsgBox.Info("Host IP cannot be blank.");
                return;
            }

            targ.Dispose();
            ToggleButtonsOnOff(false);
            user.AdminRightsExist = false;

            if (PreConnectCheck(txtHostIP.Text.Trim()))
            {
                bool checkRights,
                     addAdminToCombo = false;
                const bool devOutput = false;

                if (string.IsNullOrWhiteSpace(user.UserID))
                {
                    checkRights = false; //User did not specify a user id, query host for admins.
                    addAdminToCombo = true; //Add found admins to comboAdminList
                }
                else
                {
                    checkRights = true; //user id specified, check if they have admin rights.
                }

                try
                {
                    adTasksRemove = new AdTasks(targ);
                    adTasksRemove.CheckLocale();
                    remoteTasksRemove = new RemoteTasks(targ);
                    remoteTasksRemove.GetHostInfo();
                }
                catch (Exception connectError)
                {
                    eventLogs.LogError(connectError);
                    ClearForm(false);
                    return;
                }

                //Only runs when User ID is blank.
                //Checking rights and adding admin users to comboAdminList
                if (!checkRights)
                {
                    adTasksRemove.CheckExistingRights(addAdminToCombo, devOutput);

                    if (comboAdminList.Items.Count == 0) //No admin's found
                    {
                        MessageBox.Show("No user's found with admin rights.");
                    }
                    else
                    {
                        ToggleButtonsOnOff(false);
                        //display comboAdminList box to select user to remove.
                        comboAdminList.Visible = true;
                        selectUserBox.Visible = true;
                    }
                }
                else
                {
                    adTasksRemove.CheckExistingRights(addAdminToCombo, devOutput);

                    if (user.AdminRightsExist)
                    {
                        bool result = MsgBoxConfirmRemoval();
                        if (result)
                        {
                            adTasksRemove.RemoveRights();
                            adTasksRemove.CheckExistingRights(false, false);
                            string jobFilePath = string.Format(
                                        @"\\{0}\c$\windows\tasks\{1}.job", connection.HostIP, user.UserID);
                            try 
	                        {
                                if (File.Exists(jobFilePath)) //Delete existing scheduled tasks
                                    File.Delete(jobFilePath); 
	                        }
	                        catch (Exception)
	                        {
		                        throw;
	                        }
                            
                            //**Can implement a final check to make sure rights
                            //were removed. In tests/production use this has
                            //not failed yet.

                            StatusReport(3);
                            CopyToClipboard();
                        }
                        else if(!user.AdminRightsExist) //User specified ID does not have admin rights.
                        {
                            Util.MsgBox.Info(string.Format(
                            "User {0} does not have admin rights on {1}.",
                            user.UserID, connection.HostName));
                        }
                        else
                        {
                            StatusReport(2);
                        }
                    }
                    else
                    {
                        Util.MsgBox.Info(string.Format(
                            "User {0} does not have admin rights on {1}.",
                            user.UserID, connection.HostName));
                    }
                }
            }
            else 
            {
                Util.MsgBox.Info("Failed to connect to host. Is host online?");
                ClearForm(false);
            }

            if (!comboAdminList.Visible)
            {
                ToggleButtonsOnOff(true);  
            }
        }

        private bool MsgBoxConfirmRemoval()
        {
            bool result = Util.MsgBox.Confirm(string.Format(
            "Are you sure you want to remove {0} admin rights from {1}?",
            user.UserID, connection.HostName));
            return result;
        }

        private void CopyToClipboard()
        {
            if (local.AutoCopy) //Auto copy results enabled.
            {
                try
                {
                    Clipboard.SetText(txtConsole.Text);
                    WriteToConsole("^^ Auto copied to clipboard :) ^^");
                }
                catch (Exception)
                {
                    WriteToConsole("Clipboard in use, unable to auto copy.");
                } 
            }
        }
        #endregion

        #region Disconnect Scope
        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            ClearForm(true);
            StatusReport(2);
        }
        #endregion

        #region Form Design Methods
        //Helper methods to update form fields
        #region Rounded Form / Move with no title bar
        //Used to create rounded form edge
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect, // x-coordinate of upper-left corner
            int nTopRect, // y-coordinate of upper-left corner
            int nRightRect, // x-coordinate of lower-right corner
            int nBottomRect, // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
        );

        //Move form with no title bar
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd,
                         int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        /////////////////////////////
        private void Main_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }

            if (dev == null) return;
            dev.BringToFront(); //Move DevTools window next to Main form.
            dev.DesktopLocation = new Point(Location.X + Width, Location.Y);
        }
        #endregion

        #region Close / Minimize image updates & Click commands
        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            //Fade out form on exit
            for (int i = 0; i < 95; i++)
            {
                this.Opacity -= .01;
                Util.Tools.TimeOut(5);
            }
            Close();
        }


        // Close & Minimize image buttons
        // Update image on MouseMove
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Bitmap imgCloseGloss = Properties.Resources.imgclosegloss;
            pictureBox1.Image = imgCloseGloss;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            Bitmap imgClose = Properties.Resources.imgclose;
            pictureBox1.Image = imgClose;
        }

        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            Bitmap imgMinButton = Properties.Resources.minbuttongloss;
            pictureBox2.Image = imgMinButton;
        }

        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
            Bitmap imgMinButton = Properties.Resources.minbutton;
            pictureBox2.Image = imgMinButton;
        }
        #endregion

        #region comboAdminList Updates / Changes / Selection
        //Called when user selects a UserID to remove from Host machine
        //Sends click to Remove Button to proceed with next action.
        private void comboAdminList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboAdminList.SelectedIndex == -1) return;
            txtUserID.Text = comboAdminList.SelectedItem.ToString();
            user.UserID = comboAdminList.SelectedItem.ToString();
            user.AdminRightsExist = true;
            ClearCombo();
            comboAdminList.Visible = false;
            selectUserBox.Visible = false;
            Update();
            btnRemoveRights.Enabled = true;
            btnRemoveRights.PerformClick();
        }

        //clear comboAdminList
        private void ClearCombo()
        {
            comboAdminList.SelectedIndex = -1;
            comboAdminList.Items.Clear();
        }

        public void UpdateCombo(string message)
        {
            comboAdminList.Items.Add(message);
        }
        #endregion

        #region WriteToConsole & ClearForm
        public void WriteToConsole(string message)
        {
            if (message == CLEAR_CONSOLE)
            {
                txtConsole.Clear();
                return;
            }
            else
            {
                if (!local.SlowText)
                {
                    //Print full line
                    this.txtConsole.AppendText(message);
                }
                else
                {
                    //Parse message and print 1 character at a time.
                    char[] chars = message.ToCharArray();
                    foreach (char c in chars)
                    {
                        txtConsole.AppendText(c.ToString());
                        Util.Tools.TimeOut(15);
                    } 
                }

                if (!message.Contains(CLIPBOARD))
                {
                    txtConsole.AppendText(Environment.NewLine);
                }
            }
        }

        private void ClearForm(bool clearConsole)
        {
            ToggleButtonsAndHostIP(true, true);
            ToggleRadioControls(true);
            txtUserID.Clear();
            txtHostIP.Clear();
            comboAdminList.Items.Clear();
            comboAdminList.Text = string.Empty;
            
            if (clearConsole)
                txtConsole.Clear(); 

            txtHostIP.Focus();
        }
        #endregion
        #endregion

        #region Form Control Methods
        //Used to toggle form controls on/off
        private void ToggleRadioControls(bool enable)
        {
            radio8hours.Enabled = enable;
            radio24hours.Enabled = enable;
            radio48hours.Enabled = enable;
            radio72hours.Enabled = enable;
        }

        private void ToggleButtonsAndHostIP(bool initialState, bool enableHostIP)
        {
            btnConnect.Visible = initialState;
            btnRemoveRights.Visible = initialState;
            btnGrantRights.Visible = !initialState;
            btnDisconnect.Visible = !initialState; 
            txtHostIP.Enabled = enableHostIP;
        }

        private void ToggleButtonsOnOff(bool choice)
        {
            btnConnect.Enabled = choice;
            btnRemoveRights.Enabled = choice;
            btnGrantRights.Enabled = choice;
            btnDisconnect.Enabled = choice; 
        }
        #endregion

        #region Time Selection
        private void Radio8HoursCheckedChanged(object sender, EventArgs e)
        {
            timeSpan = 8;
        }

        private void Radio24HoursCheckedChanged(object sender, EventArgs e)
        {
            timeSpan = 24;
        }

        private void Radio48HoursCheckedChanged(object sender, EventArgs e)
        {
            timeSpan = 48;
        }

        private void Radio72HoursCheckedChanged(object sender, EventArgs e)
        {
            timeSpan = 72;
        }
        #endregion

        #region Dev Test Tools
        private void label2_DoubleClick(object sender, EventArgs e) //Open DevTools by dbl clicking UserID Label
        {
            if (dev != null) return;
            dev = new DevTools(targ);
            dev.Show();
            dev.DesktopLocation = new Point(Location.X + Width, Location.Y);
        }
        #endregion

        /// <summary>
        /// Checks for valid hostIP. Pings hostIP before proceeding.
        /// 
        /// vpnScope explanation: The environment this was built for has many pc's on VPN which
        /// do not support WMI while connected. Check's if hostIP is within vpnScope.
        /// </summary>
        /// <param name="hostIP"></param>
        /// <returns></returns>
        #region preCheck()
        private bool PreConnectCheck(string hostIP)
        {
            if (!(string.IsNullOrWhiteSpace(hostIP)))
            {
                if (Util.ConnectionTest.IsHostAlive(hostIP))
                {
                    //Scope of VPN Subnet's 
                    //Can be modified or omitted 
                    string[] vpnScope =  { 
                                             "10.40.244", "10.32.244",
                                             "10.40.245", "10.32.245",
                                             "10.40.246", "10.32.246", 
                                         };

                    //pre Connection ToDo
                    ClearCombo();
                    txtConsole.Clear();

                    //Update host info in our class object "targ"
                    connection.HostIP = txtHostIP.Text.Trim();
                    user.UserID = txtUserID.Text.Trim();

                    //Check if hostIP falls in the vpnScope
                    if (vpnScope.Any(s => connection.HostIP.Contains(s)))
                    {
                        isVpn = true;
                        connection.IsWmiActive = false; //Disable use of WMI, not supported on VPN
                        WriteToConsole("::VPN Client::\n");
                    }

                    if (isVpn) return true;

                    //Not a vpn client, let's test if WMI is working.
                    //Even if WMI fails we can use the Script Method 
                    //to fetch host data used for VPN clients. - **See RemoteTasks.cs - GetHostInfo()
                    Util.ConnectionTest.TestWmi(targ);
                    
                    return true;
                }
                Util.MsgBox.Info("Failed to connect to host. Is host online?");
                return false;
            }
            Util.MsgBox.Info("Host IP Address cannot be blank.");
            return false;
        }
        #endregion

        #region Form Helper methods
        //Auto fill txtUserID if clipboard (User ID) contains values { "USA", "CAN", "ORB", "PRI", "IDY" } &
        //clipboard > 6 or clipboard < 10
        //
        //This can be modified for environments with different User ID's
        private void txtUserID_Click(object sender, EventArgs e)
        {
            string clipboard = Clipboard.GetText().ToUpper().Trim();
            if (!(clipboard.Length > 6 & clipboard.Length < 10)) return;
            string[] userFormats = { "USA", "CAN", "ORB", "PRI", "IDY" };

            foreach (string s in userFormats)
            {
                if (!clipboard.Contains(s)) continue;
                txtUserID.Clear();
                txtUserID.Text = clipboard;
            }
        }

        //Auto fill txtHostIP if clipboard contains values ("CAN", "USA" or ".") &
        //clipboard < 16 or clipboard > 8
        //
        //This can be modified depending on what general Host/PC Name's are used in your environment.
        private void txtHostIP_Click(object sender, EventArgs e)
        {
            string clipboard = Clipboard.GetText().ToUpper().Trim();
            if (!(clipboard.Length < 16 & clipboard.Length > 8)) return;
            if ((!clipboard.Contains("CAN") && !clipboard.Contains("USA")) && !clipboard.Contains(".")) return;
            txtHostIP.Clear();
            txtHostIP.Text = clipboard;
        }

        //If ENTER key pressed send click to btnConnect
        private void txtHostIP_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter & btnConnect.Visible)
            {
                btnConnect.PerformClick();
            }
        }

        //If ENTER key pressed send click to visible button(btnConnect/btnGrantRights)
        private void txtUserID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter & btnConnect.Visible)
            {
                btnConnect.PerformClick();
            }
            else if (e.KeyCode == Keys.Enter & btnGrantRights.Visible)
            {
                btnGrantRights.PerformClick();
            }
        }
        
        //Allow Ctrl + A (Select All) in the Console Textbox
        private void txtConsole_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A & ModifierKeys == Keys.Control)
                txtConsole.SelectAll();
        }
        #endregion

        //Show settings
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            ToggleSettings(true);
        }

        private void ToggleSettings(bool visible)
        {
            groupBoxSettings.Visible = visible;
            chkBoxAutoCopy.Visible = visible;
            chkBoxScrollTxt.Visible = visible;
        }

        //Update targ settings Auto Copy feature
        private void chkBoxAutoCopy_CheckedChanged(object sender, EventArgs e)
        {
            local.AutoCopy = chkBoxAutoCopy.Checked;
            Util.Tools.SaveTargSettings(targ);
        }

        //Update targ settings Scroll Text feature
        private void chkBoxScrollTxt_CheckedChanged(object sender, EventArgs e)
        {
            local.SlowText = chkBoxScrollTxt.Checked;
            Util.Tools.SaveTargSettings(targ);
        }

        //Hide settings
        private void pixBoxSettings1_Click(object sender, EventArgs e)
        {
            ToggleSettings(false);
        }

        private void CreateEventLogObject()
        {
            //TargLogs.cs used for logging of errors, rights
            //granted and removed.
            eventLogs = new TargLogs(targ);

            Util.Tools.ReadTargSettings(targ); //Read in targsettings.txt
            chkBoxAutoCopy.Checked = local.AutoCopy; //Update settings
            chkBoxScrollTxt.Checked = local.SlowText;
        }

        private void CreateTargInfoObject()
        {
            //Abstract Data Object - shared by most classes.
            //See TargInfo.cs for more info
            targ = new TargInfo(this);
            //Subclasses contained within TargInfo.cs
            //simplifies fetching data and calling methods
            connection = targ.host.connection;
            local = targ.local;
            user = targ.host.user;
        }

        private void CreateSettingsToolTip()
        {
            // Create the ToolTip.
            ToolTip toolTip1 = new ToolTip();

            // Set up the delays for the ToolTip.
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 1000;
            toolTip1.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip1.ShowAlways = true;

            // Set up the ToolTip text for the Button and Checkbox.
            toolTip1.SetToolTip(this.picBoxSettings, "Open Settings");
            toolTip1.SetToolTip(this.pixBoxSettings1, "Close Settings");
        }

        private void PlaceFormControls()
        {
            //move non-visible comboAdminList & selectUserBox 
            //to correct placement on form over the select time box.
            selectUserBox.Left = 373;
            selectUserBox.Top = 40;
            comboAdminList.Left = 379;
            comboAdminList.Top = 77;

            //Move non-visible button placement, on top of Connect / Remove.
            btnGrantRights.Top = btnConnect.Top;
            btnGrantRights.Left = btnConnect.Left;
            btnDisconnect.Top = btnRemoveRights.Top;
            btnDisconnect.Left = btnRemoveRights.Left;
        }

        private void StatusReport(int choice)
        {
            string onlineWith = string.Format(
                            "Online with: {0}{2}" +
                            "IP Address: {1}",
                            connection.HostName,
                            connection.HostIP,
                            Environment.NewLine);

            string rightsGranted = string.Format(
                            "=================={2}" +
                            "::RIGHTS GRANTED::{2}" +
                            "=================={2}" +
                            "USER: {0}{2}" +
                            "EXPIRES: {1} (Local Time){2}" +
                            "=================={2}",
                            user.UserID,
                            DateTime.Now.AddHours(timeSpan),
                            Environment.NewLine);

            string cancelling = "Cancelling...";

            string rightsRemoved = string.Format(
                            "================={2}" +
                            "::RIGHTS REMOVED::{2}" +
                            "================={2}" +
                            "USER ID: {0}{2}" +
                            "HOST: {1}{2}" +
                            "================={2}",
                            user.UserID,
                            connection.HostName,
                            Environment.NewLine);

            string message = string.Empty;
            switch (choice)
            {
                case 0:
                    message = onlineWith;
                    break;
                case 1:
                    message = rightsGranted;
                    break;
                case 2:
                    message = cancelling;
                    break;
                case 3:
                    message = rightsRemoved;
                    break;
            }
            WriteToConsole(message);
        }

        private bool MsgBoxConfirmRights()
        {
            if (!user.AdminRightsExist) //User does not have admin rights on the Host machine.
            {
                return Util.MsgBox.Confirm(string.Format(
                "Are you sure you want to grant rights to {0} on {1} for {2} hours?\n\n",
                user.UserID, connection.HostName, timeSpan));
            }
            else //User already has admin rights on host.
            {
                return Util.MsgBox.Confirm(string.Format(
                "User {0} already has Admin rights on {1}.\n\n" +
                "Do you want rights to be removed in {2} hours?",
                user.UserID, connection.HostName, timeSpan));
            }
        }
    }
}