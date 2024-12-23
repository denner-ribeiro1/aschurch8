FROM mcr.microsoft.com/dotnet/aspnet:8.0.1 AS base
RUN apt-get update && apt-get install -y libgdiplus
COPY ["ASChurchManager.Web/deps/libssl1.1_1.1.1f-1ubuntu2_amd64.deb", "./"]
RUN set -xe \
    && apt-get update -qq \
    && dpkg -i libssl1.1_1.1.1f-1ubuntu2_amd64.deb
WORKDIR /app

EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0.101 AS build
WORKDIR /src
COPY ["ASChurchManager.Web/ASChurchManager.Web.csproj", "ASChurchManager.Web/"]
COPY ["ASChurchManager.Infra.CrossCutting.IoC/ASChurchManager.Infra.CrossCutting.IoC.csproj", "ASChurchManager.Infra.CrossCutting.IoC/"]
COPY ["ASChurchManager.Domain/ASChurchManager.Domain.csproj", "ASChurchManager.Domain/"]
COPY ["ASChurchManager.Application/ASChurchManager.Application.csproj", "ASChurchManager.Application/"]
COPY ["ASChurchManager.WebApi.Oauth.Client/ASChurchManager.WebApi.Oauth.Client.csproj", "ASChurchManager.WebApi.Oauth.Client/"]
COPY ["ASChurchManager.Infra.Data/ASChurchManager.Infra.Data.csproj", "ASChurchManager.Infra.Data/"]
RUN dotnet restore "ASChurchManager.Web/ASChurchManager.Web.csproj"
COPY . .
WORKDIR "/src/ASChurchManager.Web"
RUN dotnet build "ASChurchManager.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ASChurchManager.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV TZ=America/Sao_Paulo
RUN chmod 755 /app/Rotativa/Linux/wkhtmltopdf

ENTRYPOINT ["dotnet", "ASChurchManager.Web.dll", "--environment=Production"]