# Build Server
```
docker run -it --rm --name sample --env ASPNETCORE_Kestrel__Certificates__Default__Path=/app/aspnetapp.pfx --env ASPNETCORE_Kestrel__Certificates__Default__Password=password -p 5001:5001 dht_server
```
# Kubernetes
```
# List out all the different objects
kubectl get --all-namespaces [service|pod|node]

# Sets namespace context
kubectl config set-context --current --namespace=dht
kubectl get pods -o wide --all-namespace

# Write outcurrent status of pod (including failure reasons)
kubectl describe pods --namespace=dht dht-5f55cf9c8d-mlb7z

# Write out logs (stdout) from a given pod
kubectl logs --namespace=dht dht-5f55cf9c8d-mlb7z

# Write out rolling logs (f) that matches selector app=
kubectl logs -f --namespace=dht -lapp=dht

# Restart and pull new image
kubectl rollout restart deployment/dht --namespace=dht

# Delete a pod
kubectl delete pod dnsutils

# Views current context
kubectl config view

# View if I can perform certain action
kubectl auth can-i get pods --as system:serviceaccount:dht:default
```

# Azure CLI
```
az login
az account set --subscription 1819469c-8e5b-4923-ab58-a1b8383164a4
az account show


... creating resources...

# Login to acr
az acr login --name dsvacr
# Retag existing image
docker tag dht_server dsvacr.azurecr.io/dht_server:latest
# Build an image to be pushed
docker build -f  .\DistributedHashTable\Dockerfile -t dsvacr.azurecr.io/dht_server:latest .
# Push to Azure Container Service
docker push dsvacr.azurecr.io/dht_server:latest
```

# Run Client
dotnet run -- 5001