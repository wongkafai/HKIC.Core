FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS publish
COPY HKIC.Core /HKIC/HKIC.Core
WORKDIR /HKIC/HKIC.Core
RUN dotnet restore "HKIC.Core.csproj"
RUN dotnet publish "HKIC.Core.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim
ENV TZ=Asia/Hong_Kong
EXPOSE 8080
WORKDIR /app
COPY --from=publish /app .

ENTRYPOINT ["dotnet", "HKIC.Core.dll"]
