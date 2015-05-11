using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Windows.Forms;
using System.Collections;
using System.Management;

namespace Targ20
{
    
    public class AdTasks
    {
        #region Declarations
        //Functions called in AD/LDAP used by Group.Invoke()
        public const string ADD = "Add",
                            REMOVE = "Remove",
                            EnglishLang = "1033",
                            FrenchLang = "1036";        

        protected bool ExceptionThrown = false;

        protected TargInfo targ;
        protected TargLogs EventLogs;
        protected TargInfo.Host.Connection connection;
        protected TargInfo.Local local;
        protected TargInfo.Host.UserInfo user;
        #endregion

        #region ADTasks - Constructor
        public AdTasks(TargInfo t) 
        { 
            targ = t;
            connection = t.host.connection;
            local = t.local;
            user = t.host.user;
        }
        #endregion
        
        /// <summary>
        /// CheckExistingRights() - is used to query the host machine for user's in the Administrators group
        /// Uses WMI if available / fails over to LDAP query
        /// </summary>
        /// <param name="addToCombo">Add administrators to comboAdminList on the Main form</param>
        /// <param name="devOutput">Dev/testing output</param>
        #region CheckExistingRights()
        public void CheckExistingRights(bool addToCombo = false, bool devOutput = false)
        {
            string adminUsers = string.Empty;
            string currAdmin = string.Empty;

            Cursor.Current = Cursors.WaitCursor;
            
            try
            {
                if (connection.IsWmiActive)
                {
                    //Get info via WMI Query
                    string queryString = @"GroupUser where GroupComponent = ""Win32_Group.Domain='" +
                                        connection.HostName + @"',Name='" + connection.PathAdminLang + @"'""";

                    ManagementObjectCollection wmiCollection = Util.Tools.WmiQuery(queryString, connection.HostIP);

                    foreach (ManagementObject m in wmiCollection)
                    {
                        const int trimEnd = 3; //Remove 3 char's from end of query results
                        const string strToFind = @",NAME=\"""; //Precursor to UserID

                        //Extract UserID from query
                        currAdmin = Util.Tools.ExtractSubstring(m.ToString().ToUpper(), strToFind, trimEnd);
                        //Compile list of admin's found on host for Dev output to console
                        adminUsers = string.Concat(adminUsers, currAdmin, Environment.NewLine);
                        
                        if (addToCombo) 
                        {
                            if (!CheckSuperUser(currAdmin)) //Do not add Super User's for right's removal
                            {
                                targ.mainForm.UpdateCombo(currAdmin); //Add UserID to comboAdminList on Main form.
                            }
                        }

                        if (currAdmin.Contains(user.UserID)) //UserID found in Adminstrators group
                        user.AdminRightsExist = true; 
                        //Can add a return, but we want to skim the entire admin group
                    }
                }
                else
                {
                    //Get data using DicectoryEntry/LDAP
                    using (DirectoryEntry groupEntry = new DirectoryEntry(connection.HostLocale))
                    {
                        foreach (object member in (IEnumerable)groupEntry.Invoke("Members"))
                        {
                            using (DirectoryEntry memberEntry = new DirectoryEntry(member))
                            {
                                const string strToFind = "/"; //last char in string before UserID
                                string adminPath = memberEntry.Path.ToUpper();
                                //Extract UserID from LDAP Query.
                                currAdmin = Util.Tools.ExtractSubstring(adminPath, strToFind);
                                
                                adminUsers = string.Concat(adminUsers, currAdmin, Environment.NewLine);

                                //Add each result to Main.comboAdminList
                                //Omit Domain Administrators
                                if (addToCombo)
                                {
                                    if (!CheckSuperUser(currAdmin))
                                    {
                                        targ.mainForm.UpdateCombo(currAdmin);
                                    }
                                }

                                //Specified user exists in "Administrator" group
                                //flag rights exist == true.
                                if (currAdmin.Contains(user.UserID))
                                user.AdminRightsExist = true;
                            }
                        }
                    } 
                }

                //DEV / TEST
                if (devOutput)
                {
                    WriteAdminList(adminUsers);
                }
                
                Cursor.Current = Cursors.Default;
            }
            catch (Exception e)
            {
                Util.MsgBox.Info(e.ToString());
                targ.mainForm.WriteToConsole("::Failed to check for existing rights::");
                EventLogs.LogError(e);
                ExceptionThrown = true;
            }
        }
        #endregion

        /// <summary>
        /// CheckLocale() - Checks for Host's Locale (English / FrencH) - admin group names differ 
        /// (Administrators / Administrateurs)
        /// 
        /// Updates connection.HostLocale & connection.PathAdminLang
        /// 
        /// Uses WMI if available / fails over to LDAP Query
        /// </summary>
        #region CheckLocale()
        public void CheckLocale()
        {
            bool isEnglishHost = true;
            string groupPathEnglish = string.Format("WinNT://{0}/Administrators,group", connection.HostIP),
                   groupPathFrench = string.Format("WinNT://{0}/Administrateurs,group", connection.HostIP);

            Cursor.Current = Cursors.WaitCursor;

            try
            {
                if (connection.IsWmiActive)
                {   
                    //Get pc lang via WMI Query
                    ManagementObjectCollection wmiCollection = 
                        Util.Tools.WmiQuery("OperatingSystem", connection.HostIP);

                    foreach (ManagementObject m in wmiCollection)
                    {
                        isEnglishHost = m["OSLanguage"].ToString() == EnglishLang;
                        //**Add exception for non- ENG / FRENCH languages.
                    }
                }
                else
                {
                    if (DirectoryEntry.Exists(groupPathEnglish))
                    {
                        isEnglishHost = true;
                    }
                    else if (DirectoryEntry.Exists(groupPathFrench))
                    {
                        isEnglishHost = false;
                    }
                    else
                    {
                        ExceptionThrown = true;
                        Util.MsgBox.Info("Host Locale is not supported(English/French)");
                    }
                }

                if (isEnglishHost)
                {
                    connection.HostLocale = groupPathEnglish;
                    connection.PathAdminLang = TargInfo.English;
                }
                else
                {
                    connection.HostLocale = groupPathFrench;
                    connection.PathAdminLang = TargInfo.French;
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception e)
            {
                targ.mainForm.WriteToConsole("--Failed to verify Host Locale--");
                EventLogs.LogError(e);
                ExceptionThrown = true;
            }
        }
        #endregion

        /// <summary>
        /// FindUserActiveD() - Queries AD to validate UserID exist's and fetch's Full Name
        /// </summary>
        /// <param name="consoleWrite">Write results to console, false if test environment</param>
        #region FindUserActiveD()
        public void FindUserActiveD(bool consoleWrite = true)
        {
            PrincipalContext domain = new PrincipalContext(ContextType.Domain);
            UserPrincipal foundUser = UserPrincipal.FindByIdentity(
                domain, IdentityType.SamAccountName, targ.host.user.UserID);

             Cursor.Current = Cursors.WaitCursor;

             if (foundUser != null)
             {
                 user.UserIDExists = true;
                 user.UserFullName = foundUser.ToString();
                 if (consoleWrite)
                 {
                     targ.mainForm.WriteToConsole(string.Format(
                            "Full Name: {0}", user.UserFullName));
                 }
             }
             else
             {
                 user.UserIDExists = false;
                 if (consoleWrite)
                 {
                     targ.mainForm.WriteToConsole(string.Format(
                            "{0} - Does Not Exist.", user.UserID));
                 }
             }
             Cursor.Current = Cursors.Default;
        }
        #endregion

        #region GrantRights(), RemoveRights(), ManageRights()
        public void GrantRights(int timeSpan)
        {
            ManageRights(ADD);
        }

        public void RemoveRights()
        {
            ManageRights(REMOVE);
        }

        /// <summary>
        /// Add/Remove user rights on host.
        /// </summary>
        /// <param name="manageChoice">Constants ADD/REMOVE in ADTasks</param>
        private void ManageRights(string manageChoice)
        {
            string memberPath = string.Format("WinNT://{0}/{1}", local.LocalDomain, user.UserID);

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                using (DirectoryEntry group = new DirectoryEntry(connection.HostLocale))
                {
                    //Invoke AD Method to "Add" / "Remove" rights.
                    group.Invoke(manageChoice, memberPath);
                    group.CommitChanges();
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception e)
            {
                targ.mainForm.WriteToConsole(string.Format("--Failed to {0} rights--", manageChoice));
                targ.logs.LogError(e);
                ExceptionThrown = true;
            }
        }
        #endregion

        #region ADTasks Helper Methods
        //Writes formatted output of adminList to Main.txtConsole
        private void WriteAdminList(string adminUsers)
        {
            targ.mainForm.WriteToConsole(string.Format(
                    "=============={1}" +
                    "--ALL ADMINS--{1}" +
                    "=============={1}" +
                    "{0}" +
                    "=============={1}",
                    adminUsers,
                    Environment.NewLine));
        }

        private bool CheckSuperUser(string currAdmin)
        {
            if (!currAdmin.Contains("ROOT") &
                !currAdmin.Contains("ADMIN") &
                !currAdmin.Contains("CTX"))
	        {
                return false;
	        }
            return true;
        }
        #endregion
    }
}