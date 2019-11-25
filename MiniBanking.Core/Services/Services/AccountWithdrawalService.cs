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
    public class AccountWithdrawalService : GenericRepository<AccountWithdrawal>
    {
        private readonly MiniBankingDbContext _context;
        public AccountWithdrawalService(MiniBankingDbContext context) : base(context)
        {
            _context = context;
        }

        public override AccountWithdrawal Get(Guid id)
        {
            return _context.AccountWithdrawal.AsNoTracking().Where(o => o.Id == id).FirstOrDefault();
        }       

        public override IQueryable<AccountWithdrawal> GetAll()
        {
            return _context.AccountWithdrawal.AsNoTracking();
        }

        public override PagedList<AccountWithdrawal> GetAll(PagingParams pagingParams)
        {
            var query = _context.AccountWithdrawal.AsNoTracking().AsNoTracking()
                        .Skip(pagingParams.PageSize * (pagingParams.PageNumber - 1))
                        .Take(pagingParams.PageSize);
            var count = _context.AccountWithdrawal.Count();
            return new PagedList<AccountWithdrawal>(query, count, 0, pagingParams.PageNumber, pagingParams.PageSize);
        }

        //public PagedList<CategoryDto> GetAll(PagingParams pagingParams, SearchCategory categorySearch, FilterObject filterObject)
        //{
        //    var query = _context.AccountWithdrawal.AsNoTracking().Where(x =>
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
