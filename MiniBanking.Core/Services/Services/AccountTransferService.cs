using Microsoft.EntityFrameworkCore;
using MiniBanking.Core.Helper.Entities;
using MiniBanking.Core.Models.DTOModels;
using MiniBanking.Entity;
using MiniBanking.Core.Models.SearchModels;
using System;
using System.Collections.Generic;
using System.Linq;
using MiniBanking.Core.Helper;
using MiniBanking.Core.Models.DTOModels.Inventory;

namespace MiniBanking.Core.Services
{
    public class AccountTransferService : GenericRepository<AccountTransfer>
    {
        private readonly MiniBankingDbContext _context;

        public AccountTransferService(MiniBankingDbContext context) : base(context)
        {
            _context = context;
        }

        public override AccountTransfer Get(Guid id)
        {
            return _context.AccountTransfer.AsNoTracking().Where(o => o.Id == id).FirstOrDefault();
        }        

        public override IQueryable<AccountTransfer> GetAll()
        {
            return _context.AccountTransfer.AsNoTracking();
        }

        public override PagedList<AccountTransfer> GetAll(PagingParams pagingParams)
        {
            var query = _context.AccountTransfer.AsNoTracking().AsNoTracking()
                        .Skip(pagingParams.PageSize * (pagingParams.PageNumber - 1))
                        .Take(pagingParams.PageSize);
            var count = _context.AccountTransfer.Count();
            return new PagedList<AccountTransfer>(query, count, 0, pagingParams.PageNumber, pagingParams.PageSize);
        }

        //public PagedList<ItemDto> GetAll(PagingParams pagingParams, SearchItem searchItem, FilterObject filterObject)
        //{
        //    var query = _context.AccountTransfer.AsNoTracking().Where(x =>
        //                                                        (searchItem.ItemName.IsNullOrEmpty() || x.ItemName.Contains(searchItem.ItemName))
        //                                                        && (searchItem.RecordStatus == null || x.RecordStatus == searchItem.RecordStatus)
        //                                                      ).Select(s => new ItemDto
        //                                                      {
        //                                                          Id = s.Id,
        //                                                          ItemName = s.ItemName,
        //                                                          Description = s.Description,
        //                                                          RecordStatus = s.RecordStatus
        //                                                      });

        //    query = filterObject.FilterQuery<ItemDto>(query);

        //    var queryResult = query.Skip(pagingParams.PageSize * (pagingParams.PageNumber - 1)).Take(pagingParams.PageSize);

        //    var count = query.Count();
        //    return new PagedList<ItemDto>(queryResult, count, pagingParams.PageNumber, pagingParams.PageSize);
        //}
    }
}
