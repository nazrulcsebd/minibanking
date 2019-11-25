using Microsoft.AspNetCore.Mvc;
using System;
using MiniBanking.Core.Helper;
using MiniBanking.Core.Helper.Entities;
using MiniBanking.Core.Models.DTOModels;
using MiniBanking.Core.Models.DTOModels.Inventory;
using MiniBanking.Core.Models.SearchModels;
using MiniBanking.Core.Services;
using MiniBanking.Entity;
using CodeBonds.Utility;
using Microsoft.AspNetCore.Http;
using System.Transactions;

namespace CodeBonds.Controllers.Inventory
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountTransactionsController : BaseController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private AccountTransactionService transactionService;
        private AccountDepositService depositService;
        private AccountWithdrawalService withdrawalService;
        private CustomerAccountService customerAccountService;
        private AccountTransferService transferService;
        public AccountTransactionsController(MiniBankingDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
            transactionService = new AccountTransactionService(context, Constants.DefaultConnectionString);
            depositService = new AccountDepositService(context);
            withdrawalService = new AccountWithdrawalService(context);
            customerAccountService = new CustomerAccountService(context);
            transferService = new AccountTransferService(context);
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var dosageForm = transactionService.Get(id);
            if (dosageForm != null)
            {
                return Ok(dosageForm);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetWithPagination([FromQuery]PagingParams pagingParams, [FromQuery]SearchTransaction search, [FromQuery] FilterObject filterObject)
        {
            search.CustomerId = Guid.Parse(AppClaim.CustomerId);
            var transaction = transactionService.GetAll(pagingParams, search, filterObject);

            if (transaction != null)
            {
                Response.Headers.Add("X-Pagination", transaction.GetHeader().ToJson());

                var request = _httpContextAccessor.HttpContext.Request;

                var response = new PaginationResponseModel<AccountTransactionDto>
                {
                    Paging = transaction.GetHeader(),
                    Links = PaginationResponseModel<AccountTransactionDto>.GetLinks(transaction, request),
                    Lists = transaction.List,
                    TotalSum = transaction.TotaSum
                };

                return Ok(response);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult PostForDeposit([FromBody]AccountDeposit deposit)
        {
            if (!ModelState.IsValid)
            {
                ResponseModel.Notification = ApUtility.CreateNotification("Invalid information", Enums.NotificationType.Error);
                return StatusCode(500, ResponseModel);
            }

            try
            {
                deposit.Id = Guid.NewGuid();
                deposit.DepositDate = DateTime.UtcNow;
                deposit.CustomerId = Guid.Parse(AppClaim.CustomerId);
                deposit.AccountId = Guid.Parse(AppClaim.AccountId);

                AccountTransaction accountTransaction = new AccountTransaction()
                {
                    Id = new Guid(),
                    CustomerId = Guid.Parse(AppClaim.CustomerId),
                    AccountId = Guid.Parse(AppClaim.AccountId),
                    TransactionDate = DateTime.UtcNow,
                    TransferType = (int)Enums.TransactionType.Deposit,
                    DrAmount = deposit.Amount,
                    CrAmount = 0
                };

                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {
                        depositService.Create(deposit);
                        depositService.SaveChanges();

                        transactionService.Create(accountTransaction);
                        transactionService.SaveChanges();

                        ts.Complete();
                    }
                    catch (Exception ex)
                    {
                        ts.Dispose();
                        ResponseModel.Notification = ApUtility.CreateNotification(ex.Message, Enums.NotificationType.Error);
                        return StatusCode(500, ResponseModel);
                    }
                }

                ResponseModel.Notification = ApUtility.CreateNotification("Account Desposit is succeed", Enums.NotificationType.Success);
                ResponseModel.Data = transactionService.GetAccountBalance(Guid.Parse(AppClaim.CustomerId), Guid.Parse(AppClaim.AccountId));

                return Created(string.Empty, ResponseModel);
            }
            catch (Exception ex)
            {
                ResponseModel.Notification = ApUtility.CreateNotification(ex.Message, Enums.NotificationType.Error);
                return StatusCode(500, ResponseModel);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult PostForWithdrawal([FromBody]AccountWithdrawal withdrawal)
        {
            if (!ModelState.IsValid)
            {
                ResponseModel.Notification = ApUtility.CreateNotification("Invalid information", Enums.NotificationType.Error);
                return StatusCode(500, ResponseModel);
            }

            try
            {
                withdrawal.Id = Guid.NewGuid();
                withdrawal.WithdrawalDate = DateTime.UtcNow;
                withdrawal.CustomerId = Guid.Parse(AppClaim.CustomerId);
                withdrawal.AccountId = Guid.Parse(AppClaim.AccountId);

                AccountTransaction accountTransaction = new AccountTransaction()
                {
                    Id = new Guid(),
                    CustomerId = Guid.Parse(AppClaim.CustomerId),
                    AccountId = Guid.Parse(AppClaim.AccountId),
                    TransactionDate = DateTime.UtcNow,
                    TransferType = (int)Enums.TransactionType.Witwdrawal,
                    DrAmount = 0,
                    CrAmount = withdrawal.Amount
                };

                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {
                        withdrawalService.Create(withdrawal);
                        withdrawalService.SaveChanges();

                        transactionService.Create(accountTransaction);
                        transactionService.SaveChanges();

                        ts.Complete();
                    }
                    catch (Exception ex)
                    {
                        ts.Dispose();
                        ResponseModel.Notification = ApUtility.CreateNotification(ex.Message, Enums.NotificationType.Error);
                        return StatusCode(500, ResponseModel);
                    }
                }

                ResponseModel.Notification = ApUtility.CreateNotification("Account Withdrawal is succeed", Enums.NotificationType.Success);
                ResponseModel.Data = transactionService.GetAccountBalance(Guid.Parse(AppClaim.CustomerId), Guid.Parse(AppClaim.AccountId));

                return Created(string.Empty, ResponseModel);
            }
            catch (Exception ex)
            {
                ResponseModel.Notification = ApUtility.CreateNotification(ex.Message, Enums.NotificationType.Error);
                return StatusCode(500, ResponseModel);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult PostForTransfer([FromBody]AccountTransfer transfer)
        {
            if (!ModelState.IsValid)
            {
                ResponseModel.Notification = ApUtility.CreateNotification("Invalid information", Enums.NotificationType.Error);
                return StatusCode(500, ResponseModel);
            }

            try
            {
                var customerAccount = customerAccountService.GetAccountByCustomer(transfer.ToCustomerId);

                transfer.Id = Guid.NewGuid();
                transfer.TransferType = 1;
                transfer.TransferDate = DateTime.UtcNow;
                transfer.FromCustomerId = Guid.Parse(AppClaim.CustomerId);
                transfer.FromAccountId = Guid.Parse(AppClaim.AccountId);
                transfer.ToAccountId = customerAccount.Id;

                AccountTransaction transactionCr = new AccountTransaction()
                {
                    Id = new Guid(),
                    CustomerId = Guid.Parse(AppClaim.CustomerId),
                    AccountId = Guid.Parse(AppClaim.AccountId),
                    TransactionDate = DateTime.UtcNow,
                    TransferType = (int)Enums.TransactionType.Transfer,
                    DrAmount = 0,
                    CrAmount = transfer.Amount
                };

                AccountTransaction transactionDr = new AccountTransaction()
                {
                    Id = new Guid(),
                    CustomerId = transfer.ToCustomerId,
                    AccountId = transfer.ToAccountId,
                    TransactionDate = DateTime.UtcNow,
                    TransferType = (int)Enums.TransactionType.Deposit,
                    DrAmount = transfer.Amount,
                    CrAmount = 0
                };

                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {
                        transferService.Create(transfer);
                        transferService.SaveChanges();

                        transactionService.Create(transactionCr);
                        transactionService.Create(transactionDr);
                        transactionService.SaveChanges();

                        ts.Complete();
                    }
                    catch (Exception ex)
                    {
                        ts.Dispose();
                        ResponseModel.Notification = ApUtility.CreateNotification(ex.Message, Enums.NotificationType.Error);
                        return StatusCode(500, ResponseModel);
                    }
                }

                ResponseModel.Notification = ApUtility.CreateNotification("Account Transfer is succeed", Enums.NotificationType.Success);
                ResponseModel.Data = transactionService.GetAccountBalance(Guid.Parse(AppClaim.CustomerId), Guid.Parse(AppClaim.AccountId));

                return Created(string.Empty, ResponseModel);
            }
            catch (Exception ex)
            {
                ResponseModel.Notification = ApUtility.CreateNotification(ex.Message, Enums.NotificationType.Error);
                return StatusCode(500, ResponseModel);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Post([FromBody]AccountTransaction transaction)
        {
            if (!ModelState.IsValid)
            {
                ResponseModel.Notification = ApUtility.CreateNotification("Invalid information", Enums.NotificationType.Error);
                return StatusCode(500, ResponseModel);
            }

            try
            {
                transaction.Id = Guid.NewGuid();
                transaction.CreatedOn = DateTime.UtcNow;

                transactionService.Create(transaction);
                transactionService.SaveChanges();

                transaction = transactionService.Get(transaction.Id);

                ResponseModel.Notification = ApUtility.CreateNotification("Account Transaction created successfully", Enums.NotificationType.Success);
                ResponseModel.Data = transaction;

                return Created(string.Empty, ResponseModel);
            }
            catch (Exception ex)
            {
                ResponseModel.Notification = ApUtility.CreateNotification(ex.Message, Enums.NotificationType.Error);
                return StatusCode(500, ResponseModel);
            }
        }

        [HttpPut]
        public IActionResult Put(AccountTransaction transaction)
        {
            try
            {
                transactionService.Update(transaction);
                transactionService.SaveChanges();

                transaction = transactionService.Get(transaction.Id);

                ResponseModel.Notification = ApUtility.CreateNotification("Account Transaction updated successfully", Enums.NotificationType.Success);
                ResponseModel.Data = transaction;

                return Accepted(string.Empty, ResponseModel);
            }
            catch (Exception ex)
            {
                ResponseModel.Notification = ApUtility.CreateNotification(ex.Message, Enums.NotificationType.Error);
                return StatusCode(500, ResponseModel);
            }
        }

        [HttpDelete]
        public IActionResult Delete(Guid id)
        {
            try
            {
                transactionService.Delete(id);
                transactionService.SaveChanges();

                ResponseModel.Notification = ApUtility.CreateNotification("Account Transaction delete successfully", Enums.NotificationType.Success);
                return Accepted(ResponseModel);

            }
            catch (Exception ex)
            {
                ResponseModel.Notification = ApUtility.CreateNotification(ex.Message, Enums.NotificationType.Error);
                return StatusCode(500, ResponseModel);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetAccountBalance()
        {
            var accountBalance = transactionService.GetAccountBalance(Guid.Parse(AppClaim.CustomerId), Guid.Parse(AppClaim.AccountId));

            if (accountBalance != null)
            {
                return Ok(accountBalance);
            }
            else
            {
                return NotFound();
            }
        }

    }
}
