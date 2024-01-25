#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Yi.Abp.Web/Yi.Abp.Web.csproj", "src/Yi.Abp.Web/"]
COPY ["framework/Yi.Framework.AspNetCore.Authentication.OAuth/Yi.Framework.AspNetCore.Authentication.OAuth.csproj", "framework/Yi.Framework.AspNetCore.Authentication.OAuth/"]
COPY ["framework/Yi.Framework.AspNetCore/Yi.Framework.AspNetCore.csproj", "framework/Yi.Framework.AspNetCore/"]
COPY ["framework/Yi.Framework.Core/Yi.Framework.Core.csproj", "framework/Yi.Framework.Core/"]
COPY ["src/Yi.Abp.Application/Yi.Abp.Application.csproj", "src/Yi.Abp.Application/"]
COPY ["framework/Yi.Framework.Ddd.Application/Yi.Framework.Ddd.Application.csproj", "framework/Yi.Framework.Ddd.Application/"]
COPY ["framework/Yi.Framework.Ddd.Application.Contracts/Yi.Framework.Ddd.Application.Contracts.csproj", "framework/Yi.Framework.Ddd.Application.Contracts/"]
COPY ["module/bbs/Yi.Framework.Bbs.Application/Yi.Framework.Bbs.Application.csproj", "module/bbs/Yi.Framework.Bbs.Application/"]
COPY ["module/rbac/Yi.Framework.Rbac.Application/Yi.Framework.Rbac.Application.csproj", "module/rbac/Yi.Framework.Rbac.Application/"]
COPY ["module/rbac/Yi.Framework.Rbac.Application.Contracts/Yi.Framework.Rbac.Application.Contracts.csproj", "module/rbac/Yi.Framework.Rbac.Application.Contracts/"]
COPY ["module/rbac/Yi.Framework.Rbac.Domain.Shared/Yi.Framework.Rbac.Domain.Shared.csproj", "module/rbac/Yi.Framework.Rbac.Domain.Shared/"]
COPY ["framework/Yi.Framework.Mapster/Yi.Framework.Mapster.csproj", "framework/Yi.Framework.Mapster/"]
COPY ["framework/Yi.Framework.SqlSugarCore.Abstractions/Yi.Framework.SqlSugarCore.Abstractions.csproj", "framework/Yi.Framework.SqlSugarCore.Abstractions/"]
COPY ["module/rbac/Yi.Framework.Rbac.Domain/Yi.Framework.Rbac.Domain.csproj", "module/rbac/Yi.Framework.Rbac.Domain/"]
COPY ["module/bbs/Yi.Framework.Bbs.Application.Contracts/Yi.Framework.Bbs.Application.Contracts.csproj", "module/bbs/Yi.Framework.Bbs.Application.Contracts/"]
COPY ["module/bbs/Yi.Framework.Bbs.Domain.Shared/Yi.Framework.Bbs.Domain.Shared.csproj", "module/bbs/Yi.Framework.Bbs.Domain.Shared/"]
COPY ["module/bbs/Yi.Framework.Bbs.Domain/Yi.Framework.Bbs.Domain.csproj", "module/bbs/Yi.Framework.Bbs.Domain/"]
COPY ["src/Yi.Abp.Application.Contracts/Yi.Abp.Application.Contracts.csproj", "src/Yi.Abp.Application.Contracts/"]
COPY ["src/Yi.Abp.Domain.Shared/Yi.Abp.Domain.Shared.csproj", "src/Yi.Abp.Domain.Shared/"]
COPY ["src/Yi.Abp.Domain/Yi.Abp.Domain.csproj", "src/Yi.Abp.Domain/"]
COPY ["src/Yi.Abp.SqlSugarCore/Yi.Abp.SqlSugarCore.csproj", "src/Yi.Abp.SqlSugarCore/"]
COPY ["framework/Yi.Framework.SqlSugarCore/Yi.Framework.SqlSugarCore.csproj", "framework/Yi.Framework.SqlSugarCore/"]
COPY ["module/bbs/Yi.Framework.Bbs.SqlSugarCore/Yi.Framework.Bbs.SqlSugarCore.csproj", "module/bbs/Yi.Framework.Bbs.SqlSugarCore/"]
COPY ["module/rbac/Yi.Framework.Rbac.SqlSugarCore/Yi.Framework.Rbac.SqlSugarCore.csproj", "module/rbac/Yi.Framework.Rbac.SqlSugarCore/"]
RUN dotnet restore "./src/Yi.Abp.Web/./Yi.Abp.Web.csproj"
COPY . .
WORKDIR "/src/src/Yi.Abp.Web"
RUN dotnet build "./Yi.Abp.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Yi.Abp.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Yi.Abp.Web.dll"]