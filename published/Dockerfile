FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY published/FruityNET.dll ./
ENTRYPOINT ["dotnet", "FruityNET.dll"]
