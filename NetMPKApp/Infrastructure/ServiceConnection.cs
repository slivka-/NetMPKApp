using System.ServiceModel;
using System.ServiceModel.Channels;

namespace NetMPKApp.Infrastructure
{
    class ServiceConnection
    {
        private static ServiceConnection instance;
        private static object syncRoot = new object();

        public NetMPKService.MPKServiceClient client { get; }

        private ServiceConnection()
        {
            client = new NetMPKService.MPKServiceClient();
        }

        public static ServiceConnection GetInstance()
        {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ServiceConnection();
                    }
                }
                return instance;
        }
    }
}
