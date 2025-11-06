using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using Volo.Abp.Auditing;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Yi.Framework.Core.Enums;
using Yi.Framework.Core.Helper;

namespace Yi.Framework.Rbac.Domain.Entities
{
    [SugarTable("File")]
    public class FileAggregateRoot : AggregateRoot<Guid>, IAuditedObject
    {
        public FileAggregateRoot()
        {
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="fileId">文件标识id</param>
        /// <param name="fileName">文件名</param>
        /// <param name="fileSize">文件大小</param>
        public FileAggregateRoot(Guid fileId, string fileName, decimal fileSize)
        {
            this.Id = fileId;
            this.FileSize = fileSize;
            this.FileName = fileName;

            var type = GetFileType();

            var savePath = GetSaveFilePath();
            this.FilePath = savePath;
        }

        /// <summary>
        /// 检测目录是否存在，不存在便创建
        /// </summary>
        public void CheckDirectoryOrCreate()
        {
            var savePath = GetSaveDirPath();
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
        }

        /// <summary>
        /// 文件类型
        /// </summary>
        /// <returns></returns>
        public FileTypeEnum GetFileType()
        {
            return MimeHelper.GetFileType(this.FileName);
        }

        /// <summary>
        /// 获取文件mime
        /// </summary>
        /// <returns></returns>
        public string GetMimeMapping()
        {
            return MimeHelper.GetMimeMapping(this.FileName)??@"text/plain";
        }

        /// <summary>
        /// 落库目录路径
        /// </summary>
        /// <returns></returns>
        public string GetSaveDirPath()
        {
            return $"wwwroot/{GetFileType()}";
        }

        /// <summary>
        /// 落库文件路径
        /// </summary>
        /// <returns></returns>
        public string GetSaveFilePath()
        {
            string savefileName = GetSaveFileName();
            return Path.Combine(GetSaveDirPath(), savefileName);
        }

        /// <summary>
        /// 获取保存的文件名
        /// </summary>
        /// <returns></returns>
        public string GetSaveFileName()
        {
            return this.Id.ToString() + Path.GetExtension(this.FileName);
        }

        /// <summary>
        /// 检测，并且返回缩略图的保存路径
        /// </summary>
        /// <param name="saveFileName"></param>
        /// <returns></returns>
        public string GetAndCheakThumbnailSavePath(bool isCheak=false)
        {
            string thumbnailPath = $"wwwroot/{FileTypeEnum.thumbnail}";
            if (isCheak)
            {
                if (!Directory.Exists(thumbnailPath))
                {
                    Directory.CreateDirectory(thumbnailPath);
                } 
            }
            return Path.Combine(thumbnailPath, GetSaveFileName());
        }

        
        /// <summary>
        /// 获取查询的的文件路径
        /// </summary>
        /// <param name="file"></param>
        /// <param name="isThumbnail"></param>
        /// <returns></returns>
        public  string? GetQueryFileSavePath(bool? isThumbnail)
        {
            string fileSavePath;
            //如果为缩略图，需要修改路径
            if (isThumbnail is true)
            {
                fileSavePath = this.GetAndCheakThumbnailSavePath();
            }
            else
            {
                fileSavePath = this.GetSaveFilePath();
            }
            return fileSavePath;
        }
        
        /// <summary>
        /// 文件大小 
        ///</summary>
        [SugarColumn(ColumnName = "FileSize")]
        public decimal FileSize { get; internal set; }

        /// <summary>
        /// 文件名 
        ///</summary>
        [SugarColumn(ColumnName = "FileName")]
        public string FileName { get; internal set; }

        /// <summary>
        /// 文件路径 
        ///</summary>
        [SugarColumn(ColumnName = "FilePath")]
        public string FilePath { get; internal set; }

        public DateTime CreationTime { get; set; }
        public Guid? CreatorId { get; set; }

        public Guid? LastModifierId { get; set; }

        public DateTime? LastModificationTime { get; set; }
    }
}