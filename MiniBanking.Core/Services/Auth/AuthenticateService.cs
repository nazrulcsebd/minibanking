using Microsoft.EntityFrameworkCore;
using MiniBanking.Core.Helper;
using MiniBanking.Core.Helper.Entities;
using MiniBanking.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniBanking.Core.Services.Auth
{
    public class AuthenticateService : GenericRepository<Customer>
    {
        private readonly MiniBankingDbContext _context;

        public AuthenticateService(MiniBankingDbContext context) : base(context)
        {
            _context = context;
        }
        public Customer GetUser(LoginModel loginModel)
        {
            return _context.Customer.AsNoTracking()
                .Where(o => o.Email.Equals(loginModel.Email) && o.PersonalCode.Equals(loginModel.PersonalCode) && o.Password == loginModel.Password.Encrypt()).FirstOrDefault();
        }
        public IEnumerable<Customer> GetUsers(LoginModel loginModel)
        {
            return _context.Customer.AsNoTracking()
                .Where(o => o.Email.Equals(loginModel.Email) && o.PersonalCode.Equals(loginModel.PersonalCode));
        }

    }
}
