# Server
docker run -it --rm --name sample --env ASPNETCORE_Kestrel__Certificates__Default__Path=/app/aspnetapp.pfx --env ASPNETCORE_Kestrel__Certificates__Default__Password=password -p 5001:5001 dht_server


# Client
dotnet run -- 5001