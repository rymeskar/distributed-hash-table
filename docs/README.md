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

* Paraphrasing of wiki!

## Algorithm Choice
I decided to use the Chord algorithm. Nodes and keys are assigned an {\displaystyle m}m-bit identifier using consistent hashing.
## Approach
* All store locally. But 
## Implementation Considerations
I have decided to use the following technologies for building this project:
* C# language as that is the language I am most proficient in
* .Net Core framework as that is the modern multi-platform runtime provided by Microsoft
* AspNet.Core application framework
* gRPC endpoints and clients because that was one of the topics of DSV
* Docker containers
* Kubernetes instrumentation so as to learn more about this technology
* Deployment to Azure so as to mimic production environment.
* Sending logs and telemetry to Azure App Insights

## Plan
* Kubernetes multiple instances with a load balancer
* Single node with a backed file; both deletions, additions and retrievals.
* Write tests. 
* Caching on top.
* Figure out how nodes will jump to the system!
* Multiple nodes at start, with linear routing.
* Nodes can be added
* Nodes can be deleted
* Improved routing (fingerprint)
* Deploy to Azure
* Easy script to showcase
* Write up.

hash (ip/host+port  )