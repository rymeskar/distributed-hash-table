using k8s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
