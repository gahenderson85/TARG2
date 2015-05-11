using System;

namespace Targ20
{
    public class TargInfo : IDisposable
    {

        #region Constructor
        public TargInfo(Main m)
        {
            mainForm = m;
            host = new Host();
            local = new Local();
            logs = new TargLogs(this);
        }
        #endregion

        public Main mainForm;
        private bool disposed;

        public Host host;
        public Local local;
        public TargLogs logs;
        

        public const string English = "ADMINISTRATORS",
                            French = "ADMINISTRATEURS";

        

        public class Host
        {
            public Connection connection = new Connection();
            public TaskInfo removalTask = new TaskInfo();
            public UserInfo user = new UserInfo();

            public class Connection
            {
                private bool isWmiActive = true;
                private string hostName = string.Empty;
                private string hostIP = string.Empty;
                private string hostLocale = string.Empty;
                private string pathAdminLang = string.Empty;

                public bool IsWmiActive { get { return isWmiActive; } set { isWmiActive = value; } }
                public string HostLocale { get { return hostLocale; } set { hostLocale = value; } }
                public string PathAdminLang { get { return pathAdminLang; } set { pathAdminLang = value; } }
                public string HostName { get { return hostName; } set { hostName = value; } }
                public string HostIP { get { return hostIP; } set { hostIP = value; } }
            }

            public class TaskInfo
            {
                private string endDate = string.Empty;
                private string endTime = string.Empty;
                private string hostTime = string.Empty;
                private string status = string.Empty;

                public string Status { get { return status; } set { status = value; } }
                public string EndDate { get { return endDate; } set { endDate = value; } }
                public string EndTime { get { return endTime; } set { endTime = value; } }
                public string HostTime { get { return hostTime; } set { hostTime = value; } }
            }

            public class UserInfo
            {
                private string userFullName = string.Empty;
                private bool userIDExists;
                private bool adminRightsExist;
                private string userID = string.Empty;
                private string currentUser = string.Empty;

                public string UserID { get { return userID; } set { userID = value; } }
                public string CurrentUser { get { return currentUser; } set { currentUser = value; } }
                public string UserFullName { get { return userFullName; } set { userFullName = value; } }
                public bool UserIDExists { get { return userIDExists; } set { userIDExists = value; } }
                public bool AdminRightsExist { get { return adminRightsExist; } set { adminRightsExist = value; } }
            }
        }

        public class Local
        {
            private readonly string localDomain = Environment.UserDomainName;
            private readonly string testDomain = Environment.MachineName;
            private DateTime localTime = DateTime.Now;
            private bool autoCopy = true;
            private bool slowText = true;

            public string LocalDomain { get { return localDomain; } }
            public string TestDomain { get { return testDomain; } }
            public DateTime LocalTime { get { return localTime; } set { localTime = value; } }
            public bool AutoCopy { get { return autoCopy; } set { autoCopy = value; } }
            public bool SlowText { get { return slowText; } set { slowText = value; } }
        }
        

        #region sessionInfo()
        public string[] SessionInfo()
        {
            string[] sessionPropValue =
            {
                "Host Name: " + host.connection.HostName, 
                "Host IP: " + host.connection.HostIP,  
                "User ID: " + host.user.UserID,
                "Full Name: " + host.user.UserFullName,
                "Local: " + host.connection.HostLocale,
                "Language: " + host.connection.PathAdminLang,
                "Current User: " + host.user.CurrentUser, 
                "End Date: " + host.removalTask.EndDate, 
                "End Time: " + host.removalTask.EndTime ,
                "Host Time: " + host.removalTask.HostTime, 
                "Local Domain: " + local.LocalDomain, 
                "Test Domain: " + local.TestDomain, 
                "Local Time: " + local.LocalTime,
                "User ID Exists: " + host.user.UserIDExists,
                "Admin Rights Exists: " + host.user.AdminRightsExist
            };
                return sessionPropValue;
        }
        #endregion

        #region ShallowCopy() & Dispose()
        public TargInfo ShallowCopy()
        {
            return (TargInfo)MemberwiseClone();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // free other managed objects that implement
                // IDisposable only
                //host.HostName = null;
                //hostIP = null;
                //userID = null;
                //hostLocale = null;
                //userIDExists = false;
                //currentUser = null;
                //endDate = null;
                //endTime = null;
                //hostTime = null;
                //adminRightsExist = false;
            }

            // release any unmanaged objects
            // set the object references to null

            disposed = true;
        }
        #endregion
    }
}