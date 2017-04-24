using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMPKApp.Infrastructure
{
    class UserInfo
    {
        private static UserInfo instance;
        public string _userId { get; set; }
        public string _userLogin { get; set; }

        private static object syncRoot = new object();

        public static UserInfo GetInstance()
        {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new UserInfo();
                    }
                }
                return instance;
        }

        private UserInfo()
        {
            _userId = "";
            _userLogin = "";
        }

        public static void ClearUser()
        {
            instance = null;
        }
    }
}
