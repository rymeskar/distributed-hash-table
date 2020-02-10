# Kubernetes + Docker
Docker has been around for quite some time. 
I remember playing with it here and there. 
My main problem has been my endeavours have been very inconsistent; just tutorials, no production stuff.
Main hassles were:
* Remember commands
* Remember how to layer images
* Remember what to expose/connect (mount points, ports, etc.)
* Take care of instrumentation

I decided to actually have a look what Kubernetes as it tries to solve the last two pain points.

Note: there were also other efforts to solve some of the pain points (swarm, compose). However, Kubernetes is the one enjoying major success.

## Docker + AspNet.Core
Microsoft has created [many different images](https://hub.docker.com/_/microsoft-dotnet-core) for .Net environment.

* sdk — .NET Core SDK images, which include the .NET Core CLI, the .NET Core runtime and ASP.NET Core.
* aspnetcore-runtime — ASP.NET Core images, which include the .NET Core runtime and ASP.NET Core.
* runtime — .NET Core runtime images, which include the .NET Core runtime.
* runtime-deps — .NET Core runtime dependency images, which include only the dependencies of .NET Core and not .NET Core itself. This image is intended for self-contained applications and is only offered for Linux. For Windows, you can use the operating system base image directly for self-contained applications, since all .NET Core dependencies are satisfied by it.

[Here's](https://docs.microsoft.com/en-us/virtualization/windowscontainers/quick-start/building-sample-app#write-the-dockerfile) a nice break-down of a typical .Net application docker file. Most importantly, the docker file uses thicker images to build and thinner images to run!

```
# Builds an image and locally stores an image
docker build -f Dockerfile -t hello-python:latest .
# lists images
docker image ls
# If you really, really want to run locally.
# i is interactive, t Allocate a pseudo-TTY; 
# rm Automatically remove the container when it exits
# name Assign a name to the container
docker run -it --rm --name sample aspnetapp
```

## Kubernetes
Kubernetes has many different objects.
* *Pod* is the smallest deployable unit on a Node. It’s a group of containers which must run together. Quite often, but not necessarily, a Pod usually contains one container.
* *Service* is used to define a logical set of Pods and related policies used to access them.
* *Volume* is essentially a directory accessible to all containers running in a Pod.
* *Namespaces* are virtual clusters backed by the physical cluster.
* *Node* is a worker machine in Kubernetes, previously known as a minion . A node may be a VM or physical machine, depending on the cluster. Each node contains the services necessary to run pods and is managed by the master components.

There are a few different types of services:
* *ClusterIP* service is the default Kubernetes service. It gives you a service inside your cluster that other apps inside your cluster can access. There is no external access.
* *NodePort* service is the most primitive way to get external traffic directly to your service. NodePort, as the name implies, opens a specific port on all the Nodes (the VMs), and any traffic that is sent to this port is forwarded to the service.
* *LoadBalancer* service is the standard way to expose a service to the internet. On GKE, this will spin up a Network Load Balancer that will give you a single IP address that will forward all traffic to your service.
![LoadBalancing](load_balancer.png)

For more complicated network and routing setups, there is ingress. here are many types of Ingress controllers, from the Google Cloud Load Balancer, Nginx, Contour, Istio, and more. There are also plugins for Ingress controllers, like the cert-manager, that can automatically provision SSL certificates for your services.
Ingress is the most useful if you want to expose multiple services under the same IP address, and these services all use the same L7 protocol (typically HTTP).
![Ingress](ingress.png)


Kubectl is a command line tool for controlling Kubernetes clusters. 
Most often one `applies` prescriptive yaml configuration files
Here is a [kubectl cheatsheet](https://kubernetes.io/docs/reference/kubectl/cheatsheet/)

Types of services:
```
kubectl apply -f deployment.yaml
```

Really nice and easy [kubernetes configs](https://github.com/dockersamples/example-voting-app/tree/master/k8s-specifications) to understand.

## Blog Posts
* https://www.freecodecamp.org/news/a-friendly-introduction-to-kubernetes-670c50ce4542/
* https://auth0.com/blog/kubernetes-tutorial-step-by-step-introduction-to-basic-concepts/
* https://medium.com/google-cloud/kubernetes-nodeport-vs-loadbalancer-vs-ingress-when-should-i-use-what-922f010849e0

## DevCert Problem
AspNet.Core refuses to bind on localhost inside docker. Testing cert needs to be published. Description [here](https://docs.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-3.1).

Part of Properties/launchsettings.json, rest visible in this project's setup.
```
    "Docker": {
      "commandName": "Docker",
      "launchUrl": "{Scheme}://{ServiceHost}:{ServicePort}",
      "environmentVariables": {
        "ASPNETCORE_Kestrel__Certificates__Default__Path": "/app/aspnetapp.pfx",
        "ASPNETCORE_Kestrel__Certificates__Default__Password": "password"
      },
      "publishAllPorts": true
    }
```
## Logging Problem
Color control characters are weirdly encoded on Linux. Thus, it is recommended to:

If you are using WebHostBuilder.CreateDefault setting ASPNETCORE_LOGGING__CONSOLE__DISABLECOLORS environment variable to true would disable colors too.

## Local Image Repository
Tumbled here, was looking for info on doing that, ssi, using local image from local docker registry for a deployment in Docker for Mac K8s.

Find out that simply adding imagePullPolicy: Never do the trick.

## Problem With Project References
Either copy each project reference separate or copy the whole root folder.

## Docker Image Rebuild
On rebuild, image is not automatically picked up by Kubernetes.

## Docker build fails
One can just 'hook-up' to the failed step: `docker run -ti 9e9d24782450` by the thumbprint.

## Console App Extensions Logging
Not all log streams get flushed automatically! That is why, one needs to properly dispose of all objects at the end. 
```
    var serviceProvider = serviceCollection.BuildServiceProvider();
    serviceProvider.Dispose();
```

## Named Services
Named services were quite confusing, especially for pods. Pods in reality don't have any DNS name. On the other hand, can be within the namespace be referred to just with their familiar name.

That is why I learned how to setup a debugging dns querier. Important: queries always bound to namespace.

```
kubectl exec --namespace=dht -ti dnsutils -- nslookup "dht"
```

## Time Sync
Hyper-V host is to blame.
Run in powershell Admin:
```
Get-VMIntegrationService -VMName DockerDesktopVM -Name "Time Synchronization" | Disable-VMIntegrationService
Get-VMIntegrationService -VMName DockerDesktopVM -Name "Time Synchronization" | Enable-VMIntegrationService
```
Restart pods afterwards.

## Authentication and Authorization
Current recommendation is to use RBAC for authorization.
k8s folder containc rbac and role-binding configs.
Great to find out who is in the current context `kubectl config view`.
Also, one can evaluate his/her permissions `kubectl auth can-i get pods --as system:anonymous`.

## Future Readings
[Helm](https://helm.sh/) helps you manage Kubernetes applications — Helm Charts help you define, install, and upgrade even the most complex Kubernetes application.

