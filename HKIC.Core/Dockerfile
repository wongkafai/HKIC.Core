FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS publish
COPY PriceBookService.Core /src/PriceBookService.Core
WORKDIR /src/PriceBookService.Core
RUN dotnet restore "PriceBookService.Core.csproj"
RUN dotnet publish "PriceBookService.Core.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim
ENV TZ=Asia/Hong_Kong
EXPOSE 8080
WORKDIR /app
COPY --from=publish /app .
RUN ln -s /lib/x86_64-linux-gnu/libdl.so.2 /lib/x86_64-linux-gnu/libdl.so
RUN apt-get update -q && \
    apt-get upgrade -qy && \
    apt-get install -qy libgdiplus && \
    apt-get install -qy gss-ntlmssp
RUN ln -s /usr/lib/libgdiplus.so /lib/x86_64-linux-gnu/libgdiplus.so
RUN apt-get install -qy cifs-utils
COPY ["PriceBookService.Core/ca_bundle.crt", "/usr/local/share/ca-certificates/macroviewhk-ca.crt"]
RUN update-ca-certificates
ENTRYPOINT ["dotnet", "PriceBookService.Core.dll"]
