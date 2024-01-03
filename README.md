<h1 align="center"><img align="left" height="150px" src="https://ccnetcore.com/prod-api/wwwroot/logo.png"> Yi框架</h1>
<h4 align="center">一套以用户体验出发的.Net8 Web开源框架</h4>
<h5 align="center">支持Abp.vNext 版本原生版本、Furion版本，前端后台接入Ruoyi Vue3.0</h5>
<h2 align="center">集大成者，终究轮子</h2>

[English](README-en.md) | 简体中文

![sdk](https://img.shields.io/badge/sdk-8.0.0-d.svg)![License MIT](https://img.shields.io/badge/license-Apache-blue.svg?style=flat-square)

****
### 简介:
YiFramework是一个基于.Net8+Abp.vNext+SqlSugar的DDD领域驱动设计后端开源框架

谁说Abp复杂？谁说DDD难？`打破常规，化繁为简`，新人入门，项目二开，最佳方式之一

**中文：意框架**（和他的名字一样“简易”，同时接入Java的Ruoyi Vue3.0前端）

模块化，可根据业务自行引用或抛弃，集大成者，大而全乎，也许你能从中学习到一些独特见解

**英文：YiFramework**

Yi框架-一套与SqlSugar一样爽的.Net8开源框架。
与Sqlsugar理念一致，以用户体验出发。
适合.Net8学习、Sqlsugar学习 、项目二次开发。
集大成者，终究轮子

（项目与Sqlsugar同步更新，但这作者老杰哥代码天天爆肝到凌晨两点，我们也尽量会跟上他的脚步。更新频繁，所以可watching持续关注。）

————这不仅仅是一个程序，更是一个艺术品，面向艺术的开发！

> 核心特点：简单好用，框架不以打包形式引用，而是直接以项目附带源码给出，自由度拉满，遵循Mit协议，允许随意修改（请注明来源即可）

**分支：**

- (推荐) **Abp**: 基于Abp.vNext分支，DDD领域驱动设计,回归开发本质，极度简单，用起来贼爽

-  **Furion**: 基于Furion分支

****

### 官网及演示地址：

废话少说直接上地址

Yi社区官网网址：[ccnetcore.com](https://ccnetcore.com)  (已上线，欢迎加入)

Rbac后台管理系统：已上线，暂不提供演示地址，可本地部署访问

App移动端系统：已上线，暂不提供演示地址，可本地部署访问

Rbac演示地址：https://ccnetcore.com:1000  （用户cc、密码123456）


### 支持:

- [x] 完全支持单体应用架构
- [x] 完全支持分布式应用架构
- [x] 完全支持微服务架构

****
### 详细到爆炸的Yi框架教程导航：
（既然选择开源，就本该怀揣着最纯粹分享的内心，我们全套打包分享，开源包括且不仅仅是：文档、框架代码、模块代码、运维CICD等，希望能够帮助到您）

1. [框架快速开始教程](https://ccnetcore.com/article/aaa00329-7f35-d3fe-d258-3a0f8380b742)(已完成)
2. [框架功能模块教程](https://ccnetcore.com/article/8c464ab3-8ba5-2761-a4b0-3a0f83a9f312)(已完成)
3. [实战演练开发教程](https://ccnetcore.com/article/e89c9593-f337-ada7-d108-3a0f83ae48e6)
4. [橙子运维CICD教程](https://ccnetcore.com/article/6b80ed42-50cd-db15-c073-3a0fa8f7fd77)(已完成)

****
### 它的理念:
谁说Abp复杂？谁说DDD难？打破常规，化繁为简，新人入门，项目二开，最佳方式之一

> 一百个人，就有一百种DDD，Yi框架不一定是极度严格的DDD，而是站在巨人的肩膀上，经过极多项目的提炼，摸索出一种最佳实践


优雅的进行快速开发，通常，简单程度与优雅程度不可兼得，Yi框架并不一昧的追求极致的解耦，会站在用户使用角度上，在使用难易度进行考虑衡量

> 一个面向用户的快速开发后端框架

在真正的使用这，你会明白这一点，极致的简单，也是优雅的一种体现。
****

## 特点
- 面向用户的后端框架，使用简单，适合小型、中型、企业级项目
- 项目直接内置源码，不打包，非常适合进行二开改造
- 内置包含大量通用场景模块
- 优雅支持分布式及微服务架构
- 等等

## 基础设施简介

以下全部功能可直接使用：

- [Abp.vNext官网](https://docs.abp.io/zh-Hans/abp/latest/)

- [SqlSugar官网](https://www.donet5.com/home/doc)

## 内置模块简介
- Rbac权限管理系统（已上线）
- Bbs论坛社区系统（已上线）

> 重复的东西，无需再写一遍，这也是优雅的体现之一

****
### 核心技术
#### 后端
C# Asp.NetCore 8.0
- [x] 动态Api：Abp.vNext
- [x] 鉴权授权：Jwt
- [x] 日志：Serilog
- [x] 模块化：Abp.vNext
- [x] 依赖注入：Autofac
- [x] 对象映射：Mapster
- [x] ORM: SqlsugarCore
- [x] 多租户：Abp.vNext
- [x] 后台任务：Quartz.Net
- [x] 本地缓存：Abp.vNext
- [x] 分布式缓存：Abp.vNext
- [x] 事件总线：Abp.vNext

#### 前端
js Vue3.2
- [x] 异步请求：axios
- [x] 图表：echarts
- [x] ui：element-plus
- [x] 存储：pinia
- [x] 路由：vue-router
- [x] 打包：vite

#### 运维
- [x] 部署：nginx
- [x] CICD：gitlab+Jenkins
- [x] Docker：harbor


****
### 业务支持模块：  

RABC权限管理系统（正在更新）
（采用ruoyi前端）
- 用户管理
- 角色管理
- 菜单管理
- 部门管理
- 岗位管理
- 字典管理
- 参数管理
- 用户在线
- 操作日志
- 登录日志
- 定时任务
- 缓存列表
- 服务监控
- WebFirst代码生成工具

 **演示截图：** 
![输入图片说明](readme/1.png)
![输入图片说明](readme/2.png)
![输入图片说明](readme/3.png)
![输入图片说明](readme/4.png)
![输入图片说明](readme/5.png)
![输入图片说明](readme/6.png)
![输入图片说明](readme/7.png)
![输入图片说明](readme/8.png)
![输入图片说明](readme/9.png)
![输入图片说明](readme/10.png)
![输入图片说明](readme/1696760969217.jpg)
![输入图片说明](readme/1696761014270.jpg)

****
### 感谢：

[橙子]https://ccnetcore.com

[XWen]https://gitee.com/on-wensil

[朝夕教育]https://www.zhaoxiedu.net

[Sqlsugar老杰哥]https://www.donet5.com/Home/Doc

[车神]微信公众号搜索Dotnet技术进阶

[RuYiAdmin如意老兄]https://gitee.com/pang-mingjun/RuYiAdmin

[ZrAdminNetCore字母老哥]https://gitee.com/izory/ZrAdminNetCore

[Admin.NET]https://gitee.com/zuohuaijun/Admin.NET

[Furion百小僧]https://furion.baiqian.ltd/

****
### 联系我们：

作者QQ：`454313500`，2029年之前作者24小时在线，时刻保持活跃更新。

QQ交流群：官方一群（已满）、官方二群（已满）、官方三群：`786308927`（基本已满）、官方四群:`498310311`（新群）

联系作者，这里人人都是顾问

官方网址留言区：[ccnetcore.com](https://ccnetcore.com) 

****
### FQA:

前往官网查看留言区

[留言区](https://ccnetcore.com/discuss/1641030787056930818)