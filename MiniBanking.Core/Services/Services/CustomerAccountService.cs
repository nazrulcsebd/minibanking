using MiniBanking.Core.Helper;
using MiniBanking.Core.Helper.Entities;
using MiniBanking.Core.Models.DTOModels;
using MiniBanking.Core.Models.DTOModels.Inventory;
using MiniBanking.Core.Models.SearchModels;
using MiniBanking.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniBanking.Core.Services
{
    public class CustomerAccountService : GenericRepository<CustomerAccount>
    {
        private readonly MiniBankingDbContext _context;

        public CustomerAccountService(MiniBankingDbContext context) : base(context)
        {
            _context = context;
        }

        public override CustomerAccount Get(Guid id)
        {
            return _context.CustomerAccount.AsNoTracking().Where(o => o.Id == id).FirstOrDefault();
        }

        public override IQueryable<CustomerAccount> GetAll()
        {
            return _context.CustomerAccount.AsNoTracking();
        }
        public CustomerAccount GetAccountByCustomer(Guid customerId)
        {
            return _context.CustomerAccount.Where(w => w.CustomerId == customerId).FirstOrDefault();
        }

        public long GetAccountNumberMax()
        {
            var maxNumber = _context.CustomerAccount.Select(m => m.AccountNumber).DefaultIfEmpty(0).Max();
            return ++maxNumber;
        }

        public override PagedList<CustomerAccount> GetAll(PagingParams pagingParams)
        {
            var query = _context.CustomerAccount.AsNoTracking().AsNoTracking()
                        .Skip(pagingParams.PageSize * (pagingParams.PageNumber - 1))
                        .Take(pagingParams.PageSize);
            var count = _context.CustomerAccount.Count();
            return new PagedList<CustomerAccount>(query, count, 0, pagingParams.PageNumber, pagingParams.PageSize);
        }
        public List<SelectListItem> AccountSli(Guid customerId)
        {
            var query = _context.CustomerAccount.Where(c => c.CustomerId == customerId).Select(s => new SelectListItem
            {
                Label = s.AccountNumber.ToString(),
                Text = s.AccountNumber.ToString(),
                Value = s.Id.ToString()
            }
            ).AsNoTracking().ToList();

            return query;
        }


        //public PagedList<ManufacturarDto> GetAll(PagingParams pagingParams, SearchManufacturar searchManufacturar, FilterObject filterObject)
        //{
        //    var query = _context.CustomerAccount.AsNoTracking().Where(x =>
        //                                                        (searchManufacturar.ManufacturarName.IsNullOrEmpty() || x.ManufacturarName.Contains(searchManufacturar.ManufacturarName))
        //                                                      //&& (x.RecordStatus == true)
        //                                                      ).Select(s => new ManufacturarDto
        //                                                      {
        //                                                          Id = s.Id,
        //                                                          ManufacturarName = s.ManufacturarName,
        //                                                          ManufacturarLocalAgentName = s.ManufacturarLocalAgentName,
        //                                                          Description = s.Description,
        //                                                          ContactNumber = s.ConatctEmail,
        //                                                          ConatctEmail = s.ConatctEmail,
        //                                                          Address = s.Address,
        //                                                          RecordStatus = s.RecordStatus
        //                                                      });

        //    query = filterObject.FilterQuery<ManufacturarDto>(query);

        //    var queryResult = query.Skip(pagingParams.PageSize * (pagingParams.PageNumber - 1)).Take(pagingParams.PageSize);

        //    var count = query.Count();
        //    return new PagedList<ManufacturarDto>(queryResult, count, pagingParams.PageNumber, pagingParams.PageSize);
        //}
    }
}
