#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build

ARG NUGET_REPO_KEY
ARG NUGET_REPO_URL

WORKDIR /src
COPY . .
WORKDIR "/src/Logging"
RUN dotnet restore "Logging.csproj"

RUN dotnet pack -c Release -o /app/build
RUN dotnet nuget push /app/build/*.nupkg --api-key $NUGET_REPO_KEY -s $NUGET_REPO_URL
