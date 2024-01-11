using Acme.BookStore.Domain.Entities;
using Acme.BookStore.Domain.Shared.Enums;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Acme.BookStore.SqlSugarCore.DataSeeds
{
    public class BookStoreDataSeed : IDataSeedContributor, ITransientDependency
    {
        private ISqlSugarRepository<BookAggregateRoot> _bookRepository;
        private IGuidGenerator _guidGenerator;
        public BookStoreDataSeed(ISqlSugarRepository<BookAggregateRoot> repository, IGuidGenerator guidGenerator)
        {
            _bookRepository = repository;
            _guidGenerator = guidGenerator;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (!await _bookRepository.IsAnyAsync(x => true))
            {
                await _bookRepository.InsertAsync(
                    new BookAggregateRoot
                    {
                        Name = "1984",
                        Type = BookTypeEnum.Dystopia,
                        PublishDate = new DateTime(1949, 6, 8),
                        Price = 19.84f
                    },
                    autoSave: true
                );

                await _bookRepository.InsertAsync(
                    new BookAggregateRoot
                    {
                        Name = "The Hitchhiker's Guide to the Galaxy",
                        Type = BookTypeEnum.ScienceFiction,
                        PublishDate = new DateTime(1995, 9, 27),
                        Price = 42.0f
                    },
                    autoSave: true
                );
            }
        }


    }
}
