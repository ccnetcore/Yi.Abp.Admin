# Docker 构建说明

## 执行命令

```shell
# 在Yi.Abp.Net8 目录下执行
docker build -t admin-server:${BUILD_NUMBER} -f ./src/Yi.Abp.Web/Dockerfile . 

```

## 注意

NuGet 源国内访问有时候会报错，可以考虑切换成华为源，加上参数

```shell
RUN dotnet restore --source https://repo.huaweicloud.com/repository/nuget/v3/index.json "./src/Yi.Abp.Web/./Yi.Abp.Web.csproj"

RUN dotnet build --source https://repo.huaweicloud.com/repository/nuget/v3/index.json "./Yi.Abp.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

RUN dotnet publish --source https://repo.huaweicloud.com/repository/nuget/v3/index.json "./Yi.Abp.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

```