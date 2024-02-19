using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Services;
using Volo.Abp.Uow;
using Yi.Framework.CodeGen.Application.Contracts.IServices;
using Yi.Framework.CodeGen.Domain.Entities;
using Yi.Framework.CodeGen.Domain.Managers;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.CodeGen.Application.Services
{
    /// <summary>
    /// CodeGen
    /// </summary>
    public class CodeGenService : ApplicationService, ICodeGenService
    {
        private ISqlSugarRepository<TableAggregateRoot, Guid> _tableRepository;
        private CodeFileManager _codeFileManager;
        private WebTemplateManager _webTemplateManager;
        public CodeGenService(ISqlSugarRepository<TableAggregateRoot, Guid> tableRepository, CodeFileManager codeFileManager, WebTemplateManager webTemplateManager)
        {
            _tableRepository = tableRepository;
            _codeFileManager = codeFileManager;
            _webTemplateManager = webTemplateManager;
        }

        /// <summary>
        /// Web To Code
        /// </summary>
        /// <returns></returns>
        public async Task PostWebBuildCodeAsync(List<Guid> ids)
        {
            //获取全部表
            var tables = await _tableRepository._DbQueryable.Where(x => ids.Contains(x.Id)).Includes(x => x.Fields).ToListAsync();
            foreach (var table in tables)
            {
                await _codeFileManager.BuildWebToCodeAsync(table);
            }

        }


        /// <summary>
        /// Web To Db
        /// </summary>
        /// <returns></returns>
        public async Task PostWebBuildDbAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Code To Web
        /// </summary>
        /// <returns></returns>
        [UnitOfWork]
        public async Task PostCodeBuildWebAsync()
        {
            var tableAggregateRoots = await _webTemplateManager.BuildCodeToWebAsync();
            //覆盖数据库，将聚合根保存到数据库
            _tableRepository._Db.DbMaintenance.TruncateTable<TableAggregateRoot>();
            _tableRepository._Db.DbMaintenance.TruncateTable<FieldEntity>();

            //导航插入即可
            await _tableRepository._Db.InsertNav(tableAggregateRoots).Include(x => x.Fields).ExecuteCommandAsync();
        }


        /// <summary>
        /// Code To Db
        /// </summary>
        /// <returns></returns>
        public async Task PostCodeBuildDbAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 打开目录
        /// </summary>
        /// <returns></returns>
        [HttpPost("code-gen/dir/{**path}")]
        public async Task PostDir([FromRoute] string path)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                path = Uri.UnescapeDataString(path);
                //去除包含@的目录
                path = string.Join("\\", path.Split("\\").Where(x => !x.Contains("@")).ToList());
                Process.Start("explorer.exe", path);
            }
            else
            {
                throw new UserFriendlyException("当前操作系统不支持打开目录");
            }
        }
    }
}
