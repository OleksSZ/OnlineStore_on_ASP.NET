FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
# сначала только csproj: слой с restore кэшируется и не пересобирается без причины
COPY OnlineStore.Domain/*.csproj OnlineStore.Domain/
COPY OnlineStore.Application/*.csproj OnlineStore.Application/
COPY OnlineStore.Infrastructure/*.csproj OnlineStore.Infrastructure/
COPY OnlineStore.Api/*.csproj OnlineStore.Api/
RUN dotnet restore OnlineStore.Api/OnlineStore.Api.csproj
COPY . .
RUN dotnet publish OnlineStore.Api/OnlineStore.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
RUN apt-get update && apt-get install -y --no-install-recommends libgssapi-krb5-2 \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "OnlineStore.Api.dll"]