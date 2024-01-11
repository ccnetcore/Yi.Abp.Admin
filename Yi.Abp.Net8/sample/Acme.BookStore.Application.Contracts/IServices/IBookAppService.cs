using Acme.BookStore.Application.Contracts.Dtos.Book;
using Volo.Abp.Application.Dtos;
using Yi.Framework.Ddd.Application.Contracts;

namespace Acme.BookStore.Application.Contracts.IServices
{
    public interface IBookAppService :
      IYiCrudAppService< //Defines CRUD methods
          BookDto, //Used to show books
          Guid, //Primary key of the book entity
          PagedAndSortedResultRequestDto, //Used for paging/sorting
          BookCreateUpdateDto> //Used to create/update a book
    {

    }
}
