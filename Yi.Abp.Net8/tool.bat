//安装
dotnet tool install yi-abp
dotnet tool install -g --add-source ./nupkg yi-abp
//卸载
dotnet tool uninstall -g yi-abp
//使用
yi-abp -v  [查看版本]
yi-abp -h  [查看帮助]
yi-abp new Acme.BookStore -csf  [创建Acme.BookStore项目，并创建解决方案文件夹]