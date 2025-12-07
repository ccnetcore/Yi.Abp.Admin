namespace Yi.Framework.Rbac.Domain.Shared.Caches;

public class FileCacheItem
{
    public Guid Id { get; set; }

    /// <summary>
    /// 文件大小 
    ///</summary>
    public decimal FileSize { get; set; }

    /// <summary>
    /// 文件名 
    ///</summary>
    public string FileName { get; set; }

    /// <summary>
    /// 文件路径 
    ///</summary>
    public string FilePath { get; set; }

    public DateTime CreationTime { get; set; }

    public Guid? CreatorId { get; set; }

    public Guid? LastModifierId { get; set; }

    public DateTime? LastModificationTime { get; set; }
}