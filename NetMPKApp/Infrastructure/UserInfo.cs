using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMPKApp.Infrastructure
{
    class UserInfo
    {

        public string _userId { get; set; }

        private static object syncRoot = new object();

        public static UserInfo instance
        {
            get
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
            private set
            {
                instance = value;
            }
        }

        private UserInfo()
        {
            _userId = "";
        }
    }
}
