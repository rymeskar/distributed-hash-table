# Distributed Systems and Computing
This is a term project for the subject 'Distributed Systems and Computing' taught at CTU FIT.

## Subject
The subject of my project is 'Distributed Hash Table'. 
A distributed hash table (DHT) is a distributed system that provides a lookup service similar to a hash table. (key, value) pairs are stored in a DHT, and any participating node can efficiently retrieve the value associated with a given key. The main advantage of a DHT is that nodes can be added/removed with minimum work around re-distributing keys. Responsibility for maintaining the mapping from keys to values is distributed among the nodes, in such a way that a change in the set of participants causes a minimal amount of disruption.

Best DHTs tend to achieve:
* Autonomy and decentralization: the nodes collectively form the system without any central coordination.
* Fault tolerance: the system should be reliable (in some sense) even with nodes continuously joining, leaving, and failing.
* Scalability: the system should function efficiently even with thousands or millions of nodes.

The structure of a DHT can be decomposed into two main components. The foundation is an abstract keyspace. A keyspace partitioning scheme splits ownership of this keyspace among the participating nodes. An overlay network then connects the nodes, allowing them to find the owner of any given key in the keyspace.

## Algorithm Choice
I decided to use the basic fully connected mesh algorithm. Nodes and keys are assigned an m-bit identifier using consistent hashing. Based on position in the keyspace, the key gets assigned to the proper handling node.

In the future, I would also like to implement the Chord algorithm.

## Implementation Considerations
I have decided to use the following technologies for building this project:
* C# language as that is the language I am most proficient in.
* .Net Core framework as that is the modern multi-platform runtime provided by Microsoft.
* AspNet.Core application framework.
* gRPC endpoints and clients because that was one of the topics of DSV.
* Docker containers.
* Kubernetes instrumentation so as to learn more about this technology.
* Deployment to Azure so as to mimic production environment.
* Possibly allow sending logs and telemetry to Azure App Insights.

## Repo Structure
* DistributedHashTable - actual application with gRPS services as well as dependency on kubernetes. Contains dockerfile and can be packaged as a Linux container.
* DistributedHashTableClient - console application containing gRPC client to communicate with the cluster. Also contains manager that keeps tracks of keyspace partitioning.
* docs - contains documentation and findings during the development of this project.
* k8s - kubernetes configuration files.
* Library - contains the basic constructs of distributed hash table cluster. All the components are build in a pluggable way (interfaces and dependency injection). This library does not directly depend on neither kubernetes nor gRPC. In orderd to implement the Chord algoritm, one can just change the discovery of new nodes and keeping track of current cluster structure.
* Library.Test - contains NUnit tests covering major components of Library.

## Approach Decisions
* I have deployed 1 kubernetes service with many nodes (pods).
    * Unfortunately, the pods addresses are not translatable through DNS, so I had to query the Kubernetes API server to get the translation.
* Discovery of new nodes in the system is not done through query to Kubernetes API server, instead through query to the service itself. Since the service is behind a round-robin load balancer, this approach will gradually discover all the nodes in the system.
* To keep the cluster up-to-date, there is a background consistency run every 5 seconds, this service pings all the nodes in the system and makes sure they pong back.
* So that I do not have to take care of data transfers when node comes online or goes offline, I deployed multi-layer storage approach. First layer is the local cache (Microsoft.Extension.Caching), whereas the second layer goes to a persistent CosmosDb deployment in Azure.
* Keyspace is partitioned using the SHA1 algorithm.
* The APIs always return information about which nodes successfuly performed the hash table operation, thus giving the possibility for the client to make sure the cluster works properly.
* Some of the metadata and ping responses are also cached through Microsoft.Extensions.Caching.

## Deployment
Everything is deployed to my Azure subscription. I will not share the production credentials here. (Although, I was lazy to properly take care of the CosmosDb conn string). The gRPC endpoints are accessible through https://104.45.73.230:5001.

## Plan
- [x]  Setup Docker
- [x]  Kubernetes multiple instances with a load balancer
- [x]  Single node with a backed file; both deletions, additions and retrievals.
- [x]  Write tests. 
- [x]  Get from Azure
- [x]  Caching on top.
- [x]  Figure out how nodes will jump to the system!
- [x]  Multiple nodes at start, with linear routing.
- [x]  Nodes can be added
- [x]  Nodes can be deleted
- [x]  Deploy to Azure
- [x]  Easy script to showcase
- [x]  Write up. (talk about named services)

- [ ]  Chord algorithm

## Write-Ups
* [Information](Containarization.md) about experience with k8s and docker.
* [Problems](GRPC.md) with gRPC on AspNet.Core. TBH, the project does not really seem production-ready. Especially in comparison with the other libraries from the AspNet.Core eco-system.
* I have also included some [handy cli commands](commands.md). 