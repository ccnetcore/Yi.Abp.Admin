# 🍉Docker 构建说明

## 🍊后端
执行目录：Yi\Yi.Abp.Net8

#### 🍊启动
D:/code/csharp/source/Yi/Yi.Bbs.Vue3/yi-bbs.conf 为我的配置文件，内部带了默认的配置文件，根据自己配置进行更改

//不带配置文件
docker run -d --name yi.admin -p 19001:19001 jiftcc/yi.admin:1.0.0

//带配置文件
docker run -d --name yi.admin -p 19001:19001 -v D:/code/csharp/source/Yi/Yi.Abp.Net8/src/Yi.Abp.Web/appsettings.json:/app/appsettings.json jiftcc/yi.admin:1.0.0


#### 🍊完整代码编译
docker build -t jiftcc/yi.admin:1.0.0 -f Dockerfile .

#### 🍊快速产物编译
docker build -t jiftcc/yi.admin:1.0.0 -f DockerfileFast .

****

## 🍇前端
执行目录：Yi\Yi.Bbs.Vue3

#### 🍇启动
D:/code/csharp/source/Yi/Yi.Bbs.Vue3/yi-bbs.conf 为我的conf配置目录，默认反向代理到ccnetcore.com，根据自己后端地址进行修改配置

docker run -d --name yi.bbs -p 18001:18001 -v D:/code/csharp/source/Yi/Yi.Bbs.Vue3/yi-bbs.conf:/etc/nginx/conf.d/yi-bbs.conf jiftcc/yi.bbs:1.0.0

#### 🍇完整代码编译
docker build -t jiftcc/yi.bbs:1.0.0 -f Dockerfile .

#### 🍇快速产物编译
docker build -t jiftcc/yi.bbs:1.0.0 -f DockerfileFast .

