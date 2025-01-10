FROM mcr.microsoft.com/dotnet/aspnet:8.0.1 AS base
RUN apt-get update && apt-get install -y fonts-dejavu
WORKDIR /app

EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0.101 AS build
WORKDIR /src
COPY ["ASChurchManager.API.Membro/ASChurchManager.API.Membro.csproj", "ASChurchManager.API.Membro/"]
COPY ["ASChurchManager.Infra.CrossCutting.IoC/ASChurchManager.Infra.CrossCutting.IoC.csproj", "ASChurchManager.Infra.CrossCutting.IoC/"]
COPY ["ASChurchManager.Domain/ASChurchManager.Domain.csproj", "ASChurchManager.Domain/"]
COPY ["ASChurchManager.Application/ASChurchManager.Application.csproj", "ASChurchManager.Application/"]
COPY ["ASChurchManager.WebApi.Oauth.Client/ASChurchManager.WebApi.Oauth.Client.csproj", "ASChurchManager.WebApi.Oauth.Client/"]
COPY ["ASChurchManager.Infra.Data/ASChurchManager.Infra.Data.csproj", "ASChurchManager.Infra.Data/"]
COPY ASChurchManager.API.Membro/Resources /src/Resources

RUN dotnet restore "ASChurchManager.API.Membro/ASChurchManager.API.Membro.csproj"
COPY . .

WORKDIR "/src/ASChurchManager.API.Membro"
RUN dotnet build "ASChurchManager.API.Membro.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ASChurchManager.API.Membro.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /src/Resources ./Resources

ENV TZ=America/Sao_Paulo

ENTRYPOINT ["dotnet", "ASChurchManager.API.Membro.dll", "--environment=Production"]