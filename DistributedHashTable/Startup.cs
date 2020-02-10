using DistributedHashTable.Services;
using k8s;
using Library;
using Library.Broker;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DistributedHashTable
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            //services.AddApplicationInsightsTelemetry();
            services.AddDistributedHashTable();
            services.AddSingleton<IHostedService, ConsistencyService>();
            ConfigureKubernetes(services);
        }

        public void ConfigureKubernetes(IServiceCollection services)
        {
            services.AddSingleton<IKubernetes>(new Kubernetes(KubeConfigHelper.GetConfig()));
            services.AddSingleton<IAddressTranslation, KubernetesAddressTranslation>();
            services.AddSingleton<ILivenessCheck, KubernetesLivenessCheck>();
            services.AddSingleton<IRemoteNodeHashTable, KubernetesNodeHashTable>();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<GreeterService>();
                endpoints.MapGrpcService<DHTService>();
                endpoints.MapGrpcService<BrokerService>();
                    
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
