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
    public class CustomerService : GenericRepository<Customer>
    {
        private readonly MiniBankingDbContext _context;

        public CustomerService(MiniBankingDbContext context) : base(context)
        {
            _context = context;
        }

        public override Customer Get(Guid id)
        {
            return _context.Customer.AsNoTracking().Where(o => o.Id == id).FirstOrDefault();
        }

        public Customer GetByEmail(string email)
        {
            return _context.Customer.AsNoTracking().Where(o => o.Email == email).FirstOrDefault();
        }       
        public IEnumerable<SelectListItem> CustomerSli(Guid customerId)
        {
            var sli = new List<SelectListItem>();
            sli.Add(new SelectListItem
            {
                Text = "Please Select an Account",
                Value = ""
            });

            sli.AddRange(_context.Customer.Where(w=> w.Id != customerId).Select(x => new SelectListItem
            {
                Text = x.FirstName + " " + x.LastName,
                Value = x.Id.ToString().ToLower()
            }).AsNoTracking().ToList());

            return sli;
        }

        public override IQueryable<Customer> GetAll()
        {
            return _context.Customer.AsNoTracking();
        }

        public override PagedList<Customer> GetAll(PagingParams pagingParams)
        {
            var query = _context.Customer.AsNoTracking().AsNoTracking()
                        .Skip(pagingParams.PageSize * (pagingParams.PageNumber - 1))
                        .Take(pagingParams.PageSize);
            var count = _context.Customer.Count();
            return new PagedList<Customer>(query, count, 0, pagingParams.PageNumber, pagingParams.PageSize);
        }

        
    }
}
