## **简介**

基于 [ruoyi-plus-vben & vben5 & ant-design-vue  ](https://gitee.com/dapppp/ruoyi-plus-vben.git) 的  前端项目

**完全兼容意框架[Yi.Admin](https://gitee.com/ccnetcore/Yi) rbac模块**

| 组件/框架      | 版本   |
| :------------- | :----- |
| vben           | 5.5.6  |
| ant-design-vue | 4.2.6  |
| vue            | 3.5.13 |

[![license](https://img.shields.io/github/license/anncwb/vue-vben-admin.svg)](LICENSE)

## 提示

该仓库使用vben最新版本v5开发

v5版本采用分仓(包)目录结构, 具体开发路径为: `根目录/apps/web-antd`

**后端需要开启”furion格式的规范化api“**：路径在Yi.Abp.Net8/src/Yi.Abp.Web/YiAbpWebModule.cs

## 预览

[预览地址点这里](https://yi.wjys.top/)


## 文档

[原ruoyi-plus-vben 框架文档](https://dapdap.top/)

[Vben V5 文档地址](https://doc.vben.pro/)

[后端 Yi 框架 文档地址](https://gitee.com/ccnetcore/Yi)

## 🚀系统截图

![image-20260101175759249](/resource/image-20260101175759249.png)

![image-20260101175912025](/resource/image-20260101175912025.png)

![image-20260101180006771](/resource/image-20260101180006771.png)


## 安装使用

前置准备环境(只能用pnpm)

```json
"packageManager": "pnpm",
"engines": {
  "node": ">=20.15.0",
  "pnpm": "latest"
},
```

- 获取项目代码

```bash
git clone -b only-front --single-branch https://gitee.com/vichen2021/yiabp-mini.git
```

2. 安装依赖

```bash
cd yiabp-mini

pnpm install
```

- 菜单图标替换

参考 [菜单图标替换](https://dapdap.top/guide/quick-start.html#%E8%8F%9C%E5%8D%95%E5%9B%BE%E6%A0%87%E5%AF%BC%E5%85%A5)


2. **推荐** 使用菜单自行配置 (跟 cloud 版本打开方式一致)

![图片](https://gitee.com/dapppp/ruoyi-plus-vben/raw/main/preview/菜单修改.png)

使用内嵌 iframe 方式需要解决跨域问题 可参考[nginx.conf](https://gitee.com/dromara/RuoYi-Vue-Plus/blob/5.X/script/docker/nginx/conf/nginx.conf#LC87)配置

- 运行

```bash
pnpm dev:antd
```

4. 打包

```bash
pnpm build:antd
```

## 这是一个特性 而不是一个bug!

1. 菜单管理可分配 但只有`admin`/`superadmin`角色能访问 其他角色访问会到403页面
2. 租户相关菜单可分配 但只有`superadmin`角色能访问 其他角色访问会到403页面
3. 分配的租户管理员无法修改自己的角色的菜单(即管理员角色的菜单) 防止自己把自己权限弄没了

## Git 贡献提交规范

参考 [vue](https://github.com/vuejs/vue/blob/dev/.github/COMMIT_CONVENTION.md) 规范 ([Angular](https://github.com/conventional-changelog/conventional-changelog/tree/master/packages/conventional-changelog-angular))

- `feat` 增加新功能
- `fix` 修复问题/BUG
- `style` 代码风格相关无影响运行结果的
- `perf` 优化/性能提升
- `refactor` 重构
- `revert` 撤销修改
- `test` 测试相关
- `docs` 文档/注释
- `chore` 依赖更新/脚手架配置修改等
- `workflow` 工作流改进
- `ci` 持续集成
- `types` 类型定义文件更改
- `wip` 开发中

## 浏览器支持

最低适配应该为`Chrome 88+`以上浏览器 详见 [css - where](https://developer.mozilla.org/en-US/docs/Web/CSS/:where#browser_compatibility)

本地开发推荐使用`Chrome` 最新版本浏览器

支持现代浏览器，不支持 IE

| [<img src="https://raw.githubusercontent.com/alrra/browser-logos/master/src/edge/edge_48x48.png" alt=" Edge" width="24px" height="24px" />](http://godban.github.io/browsers-support-badges/)</br>IE | [<img src="https://raw.githubusercontent.com/alrra/browser-logos/master/src/edge/edge_48x48.png" alt=" Edge" width="24px" height="24px" />](http://godban.github.io/browsers-support-badges/)</br>Edge | [<img src="https://raw.githubusercontent.com/alrra/browser-logos/master/src/firefox/firefox_48x48.png" alt="Firefox" width="24px" height="24px" />](http://godban.github.io/browsers-support-badges/)</br>Firefox | [<img src="https://raw.githubusercontent.com/alrra/browser-logos/master/src/chrome/chrome_48x48.png" alt="Chrome" width="24px" height="24px" />](http://godban.github.io/browsers-support-badges/)</br>Chrome | [<img src="https://raw.githubusercontent.com/alrra/browser-logos/master/src/safari/safari_48x48.png" alt="Safari" width="24px" height="24px" />](http://godban.github.io/browsers-support-badges/)</br>Safari |
| :-: | :-: | :-: | :-: | :-: |
| not support | last 2 versions | last 2 versions | last 2 versions | last 2 versions |
