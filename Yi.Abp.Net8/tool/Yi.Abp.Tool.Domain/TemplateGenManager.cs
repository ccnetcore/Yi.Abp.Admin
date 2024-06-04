using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using Yi.Abp.Tool.Domain.Shared.Dtos;
using Yi.Abp.Tool.Domain.Shared.Options;

namespace Yi.Abp.Tool.Domain
{
    public class TemplateGenManager : ITransientDependency
    {
        public readonly ToolOptions _toolOptions;
        public TemplateGenManager(IOptionsMonitor<ToolOptions> toolOptions) { _toolOptions = toolOptions.CurrentValue; }
        public async Task<string> CreateTemplateAsync(TemplateGenCreateDto input)
        {
            if (string.IsNullOrEmpty(input.TemplateFilePath))
            {
                throw new UserFriendlyException($"模板路径无法找到，请检查,[{input.TemplateFilePath}]路径");
            }
            if (string.IsNullOrEmpty(_toolOptions.TempDirPath))
            {
                throw new UserFriendlyException($"临时目录路径无法找到，请检查,[{_toolOptions.TempDirPath}]路径");
            }

            var id = Guid.NewGuid().ToString("N");
            var tempFileDirPath = Path.Combine(_toolOptions.TempDirPath, $"{id}");
            if (!Directory.Exists(tempFileDirPath))
            {
                Directory.CreateDirectory(tempFileDirPath);
            }

            //文件解压覆盖
            ZipFile.ExtractToDirectory(input.TemplateFilePath, tempFileDirPath, true);

            await ReplaceContentAsync(tempFileDirPath, input.ReplaceStrData);
            var tempFilePath = Path.Combine(_toolOptions.TempDirPath, $"{id}.zip");

            ZipFile.CreateFromDirectory(tempFileDirPath, tempFilePath);
            //创建压缩包后删除临时目录
            Directory.Delete(tempFileDirPath, true);
            return tempFilePath;
        }

        /// <summary>
        /// 替换内容,key为要替换的内容，value为替换成的内容
        /// </summary>
        /// <returns></returns>
        private async Task ReplaceContentAsync(string rootDirectory, Dictionary<string, string> dic)
        {

            foreach (var dicEntry in dic)
            {
                await ReplaceInDirectory(rootDirectory, dicEntry.Key, dicEntry.Value);
            }

            //替换目录名
            static async Task ReplaceInDirectory(string directoryPath, string searchString, string replaceString)
            {
                // 替换当前目录下的文件和文件夹名称
                var newDirPath = await ReplaceInFiles(directoryPath, searchString, replaceString);

                // 递归遍历子目录
                string[] subDirectories = Directory.GetDirectories(newDirPath);
                foreach (string subDirectory in subDirectories)
                {
                    await ReplaceInDirectory(subDirectory, searchString, replaceString);
                }
            }

            //替换文件名
            static async Task<string> ReplaceInFiles(string directoryPath, string searchString, string replaceString)
            {
                // 替换目录名
                string directoryName = new DirectoryInfo(directoryPath).Name;
                string newDirectoryName = directoryName.Replace(searchString, replaceString);
                if (directoryName != newDirectoryName)
                {
                    string parentDirectory = Path.GetDirectoryName(directoryPath);
                    string newDirectoryPath = Path.Combine(parentDirectory, newDirectoryName);
                    Directory.Move(directoryPath, newDirectoryPath);
                    directoryPath = newDirectoryPath;
                }


                // 替换文件名
                string[] files = Directory.GetFiles(directoryPath);
                foreach (string file in files)
                {
                    string newFileName = file.Replace(searchString, replaceString);
                    if (file != newFileName)
                    {
                        File.Move(file, newFileName);
                    }
                }

                files = Directory.GetFiles(directoryPath);
                // 替换文件内容
                foreach (string file in files)
                {
                    string fileContent = await File.ReadAllTextAsync(file);
                    string newFileContent = fileContent.Replace(searchString, replaceString);
                    await File.WriteAllTextAsync(file, newFileContent);
                }


                return directoryPath;
            }
        }
    }
}
