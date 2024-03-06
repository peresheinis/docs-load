FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
COPY ./src ./src
WORKDIR "/src/DocumentService.Web"
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish -c Release /p:UseAppHost=false --use-current-runtime --self-contained false -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine AS final
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DocumentService.Web.dll"]