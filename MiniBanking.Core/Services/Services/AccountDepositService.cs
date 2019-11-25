using Microsoft.EntityFrameworkCore;
using MiniBanking.Core.Helper.Entities;
using MiniBanking.Core.Models.DTOModels;
using MiniBanking.Entity;
using MiniBanking.Core.Models.SearchModels;
using System;
using System.Collections.Generic;
using System.Linq;
using MiniBanking.Core.Models.DTOModels.Inventory;
using MiniBanking.Core.Helper;

namespace MiniBanking.Core.Services
{
    public class AccountDepositService : GenericRepository<AccountDeposit>
    {
        private readonly MiniBankingDbContext _context;
        public AccountDepositService(MiniBankingDbContext context) : base(context)
        {
            _context = context;
        }

        public override AccountDeposit Get(Guid id)
        {
            return _context.AccountDeposit.AsNoTracking().Where(o => o.Id == id).FirstOrDefault();
        }       

        public override IQueryable<AccountDeposit> GetAll()
        {
            return _context.AccountDeposit.AsNoTracking();
        }

        public override PagedList<AccountDeposit> GetAll(PagingParams pagingParams)
        {
            var query = _context.AccountDeposit.AsNoTracking().AsNoTracking()
                        .Skip(pagingParams.PageSize * (pagingParams.PageNumber - 1))
                        .Take(pagingParams.PageSize);
            var count = _context.AccountDeposit.Count();
            return new PagedList<AccountDeposit>(query, count, 0, pagingParams.PageNumber, pagingParams.PageSize);
        }

        //public PagedList<CategoryDto> GetAll(PagingParams pagingParams, SearchCategory categorySearch, FilterObject filterObject)
        //{
        //    var query = _context.AccountDeposit.AsNoTracking().Where(x =>
        //                                                        (categorySearch.CategoryName.IsNullOrEmpty() || x.CategoryName.Contains(categorySearch.CategoryName))
        //                                                        && (categorySearch.RecordStatus == null || x.RecordStatus == categorySearch.RecordStatus)
        //                                                      ).Select(s => new CategoryDto
        //                                                      {
        //                                                          Id = s.Id,
        //                                                          CategoryName = s.CategoryName,
        //                                                          Description = s.Description,
        //                                                          RecordStatus = s.RecordStatus
        //                                                      });

        //    query = filterObject.FilterQuery<CategoryDto>(query);

        //    var queryResult = query.Skip(pagingParams.PageSize * (pagingParams.PageNumber - 1)).Take(pagingParams.PageSize);

        //    var count = query.Count();
        //    return new PagedList<CategoryDto>(queryResult, count, pagingParams.PageNumber, pagingParams.PageSize);
        //}
            
    }
}
