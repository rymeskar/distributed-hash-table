using k8s;

namespace DistributedHashTable
{
    public static class KubeConfigHelper
    {
        public static KubernetesClientConfiguration GetConfig()
        {
            try
            {
                var config = KubernetesClientConfiguration.InClusterConfig();
                config.UserAgent = "system:anonymous";
                return config;
            }
            catch
            {
                var config = KubernetesClientConfiguration.BuildDefaultConfig();
                config.UserAgent = "system:anonymous";
                return config;
            }
        }
    }
}
