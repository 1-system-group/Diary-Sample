#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS publish
WORKDIR "/src/Diary-Sample"
COPY . .
RUN dotnet build "Diary-Sample.csproj" -c Release -o /app/build

RUN dotnet publish "Diary-Sample.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV TZ=Asia/Tokyo
CMD ASPNETCORE_URLS=http://*:$PORT dotnet Diary-Sample.dll