using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.CommandLineUtils;
using Yi.Abp.Tool.Application.Contracts;
using Yi.Abp.Tool.Application.Contracts.Dtos;

namespace Yi.Abp.Tool.Commands
{
    public class NewCommand : ICommand
    {
        private readonly ITemplateGenService _templateGenService;

        public NewCommand(ITemplateGenService templateGenService)
        {
            _templateGenService = templateGenService;
        }


        public string Command => "new";
        public string? Description => "创建项目模板` yi-abp new <name> -csf `";

        public void CommandLineApplication(CommandLineApplication application)
        {
            application.HelpOption("-h|--help");
            
            var templateTypeOption = application.Option("-t|--template", "模板类型:`module`|`porject`",
                CommandOptionType.SingleValue);
            var pathOption = application.Option("-p|--path", "创建路径", CommandOptionType.SingleValue);
            var csfOption = application.Option("-csf", "是否创建解决方案文件夹", CommandOptionType.NoValue);
            
            var soureOption = application.Option("-s|--soure", "模板来源，gitee模板库分支名称: 默认值`default`",
                CommandOptionType.SingleValue);
            
            var dbmsOption = application.Option("-dbms|--dataBaseMs", "数据库类型，支持目前主流数据库",
                CommandOptionType.SingleValue);
            
            var moduleNameArgument = application.Argument("moduleName", "模块名", (_) => { });
            
            //子命令，new list
            application.Command("list",(applicationlist) =>
            {
                applicationlist.OnExecute(() =>
                {
                    Console.WriteLine("正在远程搜索中...");
                   var list=_templateGenService.GetAllTemplatesAsync().Result;
                    var tip = $"""
                              全部模板包括:
                              模板名称
                              ----------------
                              {list.JoinAsString("\n")}
                              """;
                    Console.WriteLine(tip);
                    return 0;
                });
            });
            
            application.OnExecute(() =>
            {
                if (dbmsOption.HasValue())
                {
                    Console.WriteLine($"检测到使用数据库类型-{dbmsOption.Value()}，请在生成后，只需在配置文件中，更改DbConnOptions:Url及DbType即可，支持目前主流数据库20+");
                }
                
                var path = string.Empty;
                if (pathOption.HasValue())
                {
                    path = pathOption.Value();
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                        return 0;
                    }
                    
                }
       
                
                
                #region 处理生成类型

                var id = Guid.NewGuid().ToString("N");
                var zipPath = string.Empty;
                byte[] fileByteArray;

                var soure= soureOption.HasValue() ? soureOption.Value() : "default";
                
                var templateType = templateTypeOption.HasValue() ? templateTypeOption.Value() : "module";
                if (templateType == "module")
                {
                    //代表模块生成
                    fileByteArray = (_templateGenService.CreateModuleAsync(new TemplateGenCreateInputDto
                    {
                        Name = moduleNameArgument.Value,
                        ModuleSoure = soure
                    }).Result);
                }
                else
                {
                    //还是代表模块生成
                    fileByteArray = _templateGenService.CreateModuleAsync(new TemplateGenCreateInputDto
                    {
                        Name = moduleNameArgument.Value,
                    }).Result;
                }



                zipPath = Path.Combine(path, $"{id}.zip");
                File.WriteAllBytes(zipPath, fileByteArray);

                #endregion

                #region 处理解决方案文件夹

                //默认是当前目录
                var unzipDirPath = "./";
                //如果创建解决方案文件夹
                if (csfOption.HasValue())
                {
                    var moduleName = moduleNameArgument.Value.ToLower().Replace(".", "-");

                    unzipDirPath = Path.Combine(path, moduleName);
                    if (Directory.Exists(unzipDirPath))
                    {
                        throw new UserFriendlyException($"文件夹[{unzipDirPath}]已存在，请删除后重试");
                    }

                    Directory.CreateDirectory(unzipDirPath);
                }

                #endregion

        
                ZipFile.ExtractToDirectory(zipPath, unzipDirPath);
                //创建压缩包后删除临时目录
                File.Delete(zipPath);

                Console.WriteLine("恭喜~模块已生成！");
                return 0;
            });
        }
    }
}