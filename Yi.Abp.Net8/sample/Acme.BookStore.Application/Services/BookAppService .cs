using Acme.BookStore.Application.Contracts.Dtos.Book;
using Acme.BookStore.Application.Contracts.IServices;
using Acme.BookStore.Domain.Entities;
using SqlSugar;
using Volo.Abp.Application.Dtos;
using Yi.Framework.Ddd.Application;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Acme.BookStore.Application.Services
{
    public class BookAppService :
           YiCrudAppService<
               BookAggregateRoot, //The Book entity
               BookDto, //Used to show books
               Guid, //Primary key of the book entity
               PagedAndSortedResultRequestDto, //Used for paging/sorting
               BookCreateUpdateDto>, //Used to create/update a book
           IBookAppService //implement the IBookAppService
    {
        private ISqlSugarRepository<BookAggregateRoot, Guid> _repository;
        public BookAppService(ISqlSugarRepository<BookAggregateRoot, Guid> repository)
            : base(repository)
        {
            _repository = repository;
        }

        public override async Task<PagedResultDto<BookDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            {
                RefAsync<int> total = 0;

                //由于直接查询接口基本上都是有包含查询条件的，默认内置的查询接口将无法满足业务的需求，所以基本上多查询都是有进行重写的
                var entities = await _repository._DbQueryable
                              //.WhereIF(!string.IsNullOrEmpty(input.ConfigKey), x => x.ConfigKey.Contains(input.ConfigKey!))
                              //          .WhereIF(!string.IsNullOrEmpty(input.ConfigName), x => x.ConfigName!.Contains(input.ConfigName!))
                              //          .WhereIF(input.StartTime is not null && input.EndTime is not null, x => x.CreationTime >= input.StartTime && x.CreationTime <= input.EndTime)
                              .ToPageListAsync(input.SkipCount, input.MaxResultCount, total);
                return new PagedResultDto<BookDto>(total, await MapToGetListOutputDtosAsync(entities));
            }
        }
    }
}
