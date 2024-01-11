using Acme.BookStore.Domain.Shared.Enums;
using Volo.Abp.Application.Dtos;

namespace Acme.BookStore.Application.Contracts.Dtos.Book
{
    public class BookDto : AuditedEntityDto<Guid>
    {
        public string Name { get; set; }

        public BookTypeEnum Type { get; set; }

        public DateTime PublishDate { get; set; }

        public float Price { get; set; }
    }
}
