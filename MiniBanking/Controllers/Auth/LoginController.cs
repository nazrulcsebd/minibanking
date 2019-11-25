using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MiniBanking.Core.Helper;
using MiniBanking.Core.Helper.Entities;
using MiniBanking.Entity;
using MiniBanking.Core.Services.Auth;
using CodeBonds.Utility;
using MiniBanking.Core.Services;
using MiniBanking.Core.Models.DTOModels.Inventory;
using System.Transactions;

namespace CodeBonds.Controllers.Auth
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly MiniBankingDbContext _context;
        private AuthenticateService authService;
        private CustomerService customerService;
        private CustomerAccountService customerAccountService;
        private AccountDepositService depositService;
        private AccountTransactionService transactionService;

        public LoginController(MiniBankingDbContext context)
        {
            _context = context;
            authService = new AuthenticateService(_context);
            customerService = new CustomerService(_context);
            customerAccountService = new CustomerAccountService(_context);
            depositService = new AccountDepositService(_context);
            transactionService = new AccountTransactionService(_context, Constants.DefaultConnectionString);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody]LoginModel login)
        {
            ResponseModel rm = new ResponseModel();
            var users = authService.GetUsers(login);

            if (users != null && users.Any())
            {
                users = users.Where(s => s.Password.ToString().Equals(login.Password.Encrypt()));
                var customerAccount = customerAccountService.GetAccountByCustomer(users.FirstOrDefault().Id);
                var accountBalance = transactionService.GetAccountBalance(users.FirstOrDefault().Id, customerAccount.Id);

                if (users.Any())
                {
                    if (users.Count() == 1)
                    {

                        var tokenString = GenerateJSONWebToken(users.FirstOrDefault(), customerAccount);

                        rm.Notification = ApUtility.CreateNotification("Loggedin Successfully.", Enums.NotificationType.Success);
                        rm.Data = new
                        {
                            Token = tokenString,
                            IsLogin = true,
                            User = users.Select(o => new { o.Email, UserName = o.FirstName, o.LastName }).FirstOrDefault(),
                            AccountBalance = accountBalance
                        };

                        return Ok(rm);
                    }
                    else
                    {
                        rm.Notification = ApUtility.CreateNotification("Loggedin Successfully.", Enums.NotificationType.Success);
                        rm.Data = new { IsLogin = true, Options = users.Select(o => new { Id = o.Id, Email = o.Email, Name = o.PersonalCode }) };

                        return Ok(rm);
                    }
                }
                else
                {
                    rm.Notification = ApUtility.CreateNotification("Invalid username or password or your account is inactive", Enums.NotificationType.Error);
                    return BadRequest(rm);
                }
            }
            else
            {
                rm.Notification = ApUtility.CreateNotification("User Information Not Available.", Enums.NotificationType.Error);
                return BadRequest(rm);
            }
        }

        private string GenerateJSONWebToken(Customer user, CustomerAccount account)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Startup.StaticConfig["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>();
            claims.Add(new Claim("CustomerId", user.Id.ToString()));
            claims.Add(new Claim("Email", user.Email));
            claims.Add(new Claim("PersonalCode", user.PersonalCode.ToString()));
            claims.Add(new Claim("AccountId", account.Id.ToString()));

            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            var token = new JwtSecurityToken(Startup.StaticConfig["Jwt:Issuer"],
                Startup.StaticConfig["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetEncryptPassword(string password)
        {
            return Ok(Encription.Encrypt(password));
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult CustomerRegistration([FromBody]CustomerDto customerdto)
        {
            ResponseModel rm = new ResponseModel();

            if (!ModelState.IsValid)
            {
                rm.Notification = ApUtility.CreateNotification("Invalid information", Enums.NotificationType.Error);
                return StatusCode(500, rm);
            }

            try
            {
                var chekDuplicateEemail = customerService.GetByEmail(customerdto.Email);
                if (chekDuplicateEemail != null)
                {
                    rm.Notification = ApUtility.CreateNotification("Email Already Exists.", Enums.NotificationType.Error);
                    return StatusCode(500, rm);
                }

                var customer = ApUtility.ObjectMapToOther<CustomerDto, Customer>(customerdto);
                var customerAccount = ApUtility.ObjectMapToOther<CustomerDto, CustomerAccount>(customerdto);

                customer.Id = Guid.NewGuid();
                customer.Password = customer.Password.Encrypt();
                customer.CreatedOn = DateTime.UtcNow;

                customerAccount.Id = Guid.NewGuid();
                customerAccount.CustomerId = customer.Id;
                customerAccount.AccountNumber = customerAccountService.GetAccountNumberMax();
                customerAccount.RecordStatus = true;
                customerAccount.CreatedOn = DateTime.UtcNow;

                AccountDeposit deposit = new AccountDeposit()
                {
                    Id = new Guid(),
                    CustomerId = customer.Id,
                    AccountId = customerAccount.Id,
                    DepositDate = DateTime.UtcNow,
                    Amount = customerAccount.Amount
                };

                AccountTransaction accountTransaction = new AccountTransaction()
                {
                    Id = new Guid(),
                    CustomerId = customer.Id,
                    AccountId = customerAccount.Id,
                    TransactionDate = DateTime.UtcNow,
                    TransferType = (int)Enums.TransactionType.Deposit,
                    DrAmount = customerAccount.Amount,
                    CrAmount = 0
                };

                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {
                        customerService.Create(customer);
                        customerService.SaveChanges();

                        customerAccountService.Create(customerAccount);
                        customerAccountService.SaveChanges();

                        depositService.Create(deposit);
                        depositService.SaveChanges();

                        transactionService.Create(accountTransaction);
                        transactionService.SaveChanges();

                        ts.Complete();
                    }
                    catch (Exception ex)
                    {
                        ts.Dispose();
                        rm.Notification = ApUtility.CreateNotification(ex.Message, Enums.NotificationType.Error);
                        return StatusCode(500, rm);
                    }
                }

                rm.Notification = ApUtility.CreateNotification("Customer Registration is succeed.", Enums.NotificationType.Success);
                rm.Data = customerService.Get(customer.Id);

                return Created(string.Empty, rm);
            }
            catch (Exception ex)
            {
                rm.Notification = ApUtility.CreateNotification(ex.Message, Enums.NotificationType.Error);
                return StatusCode(500, rm);
            }
        }
    }
}