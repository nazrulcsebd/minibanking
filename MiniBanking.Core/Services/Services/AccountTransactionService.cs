using MiniBanking.Entity;
using Microsoft.EntityFrameworkCore;
using MiniBanking.Core.Models.DTOModels;
using System;
using System.Linq;
using MiniBanking.Core.Models.DTOModels.Inventory;
using Dapper;
using System.Data.SqlClient;
using MiniBanking.Core.Helper.Entities;
using MiniBanking.Core.Models.SearchModels;
using MiniBanking.Core.Helper;

namespace MiniBanking.Core.Services
{
    public class AccountTransactionService : GenericRepository<AccountTransaction>
    {
        private readonly MiniBankingDbContext _context;
        private readonly string _connectionString = string.Empty;
        public AccountTransactionService(MiniBankingDbContext context, string connectionString) : base(context)
        {
            _context = context;
            _connectionString = connectionString;
        }

        public override AccountTransaction Get(Guid id)
        {
            return _context.AccountTransaction.AsNoTracking().Where(o => o.Id == id).FirstOrDefault();
        }

        public AccountBalanceDto GetAccountBalance(Guid customerId, Guid accountId)
        {
            string query = $@"SELECT acc.CustomerId, acc.AccountId, ca.CreatedOn, 
                            CASE WHEN ca.AccountType = 1 THEN 'Saving' ELSE 'Current' END AccountType, 
                            ca.AccountNumber, BalanceAmount
                            FROM
                            (
	                            SELECT CustomerId, AccountId, SUM(DrAmount - CrAmount) BalanceAmount 
	                            FROM AccountTransaction 
	                            WHERE CustomerId = '{customerId.ToString()}' 
	                            and   AccountId = '{accountId.ToString()}'
	                            and   TransactionDate <= GETDATE()
	                            GROUP BY CustomerId, AccountId
                            )acc INNER JOIN CustomerAccount ca ON acc.CustomerId = ca.CustomerId and acc.AccountId = ca.AccountId";


            var v = new AccountBalanceDto();

            using (var con = new SqlConnection(_connectionString))
            {
                v = con.Query<AccountBalanceDto>(query).FirstOrDefault();
            }

            return v;
        }

        public PagedList<AccountTransactionDto> GetAll(PagingParams pagingParams, SearchTransaction search, FilterObject filterObject)
        {
            var query = _context.AccountTransaction.AsNoTracking().Where(x =>
                                                                (x.CustomerId == search.CustomerId)
                                                                && ((search.FromDate == null && search.ToDate == null) || (x.TransactionDate >= search.FromDate && x.TransactionDate <= search.ToDate))
                                                                && (search.TransactionType == 0 || x.TransferType == search.TransactionType)
                                                              ).Select(s => new AccountTransactionDto
                                                              {
                                                                  Id = s.Id,
                                                                  DrAmount = s.DrAmount,
                                                                  CrAmount = s.CrAmount,
                                                                  TransferText = ((Enums.TransactionType)s.TransferType).ToDescription(),
                                                                  TransactionDate = s.TransactionDate,
                                                                  Amount = (s.DrAmount - s.CrAmount) < 0 ? (s.DrAmount - s.CrAmount) * -1 : (s.DrAmount - s.CrAmount)
                                                              });

            query = filterObject.FilterQuery<AccountTransactionDto>(query);

            var queryResult = query.Skip(pagingParams.PageSize * (pagingParams.PageNumber - 1)).Take(pagingParams.PageSize);

            var count = query.Count();
            var sum = query.Sum(s => s.DrAmount - s.CrAmount);
            return new PagedList<AccountTransactionDto>(queryResult, count, sum, pagingParams.PageNumber, pagingParams.PageSize);
        }
    }
}
