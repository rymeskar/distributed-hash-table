# Server
```
docker run -it --rm --name sample --env ASPNETCORE_Kestrel__Certificates__Default__Path=/app/aspnetapp.pfx --env ASPNETCORE_Kestrel__Certificates__Default__Password=password -p 5001:5001 dht_server
```
## Kubernetes
```
# List out all the different objects
kubectl get --all-namespaces [service|pod|node]

# Sets namespace context
kubectl config set-context --current --namespace=dht

# Write outcurrent status of pod (including failure reasons)
kubectl describe pods --namespace=dht dht-5f55cf9c8d-mlb7z

# Write out logs (stdout) from a given pod
kubectl logs --namespace=dht dht-5f55cf9c8d-mlb7z

# Write out rolling logs (f) that matches selector app=
kubectl logs -f --namespace=dht -lapp=dht
```

# Client
dotnet run -- 5001