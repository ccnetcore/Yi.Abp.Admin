FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root
RUN ln -sf /usr/share/zoneinfo/Asia/Shanghai /etc/localtime
RUN echo "Asia/Shanghai" > /etc/timezone
WORKDIR /app
EXPOSE 19001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /main
COPY . .
WORKDIR "/main/src/Yi.Abp.Web"
RUN dotnet restore "Yi.Abp.Web.csproj"

FROM build AS publish
WORKDIR "/main/src/Yi.Abp.Web"
RUN dotnet publish "Yi.Abp.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Yi.Abp.Web.dll"]