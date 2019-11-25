using MiniBanking.Core.Helper;
using MiniBanking.Core.Helper.Entities;
using MiniBanking.Core.Models.DTOModels;
using MiniBanking.Core.Models.DTOModels.Inventory;
using MiniBanking.Core.Models.SearchModels;
using MiniBanking.Core.Services;
using MiniBanking.Entity;
using CodeBonds.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CodeBonds.Controllers.Inventory
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerAccountController : BaseController
    {
        private CustomerAccountService accountService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerAccountController(MiniBankingDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
            accountService = new CustomerAccountService(context);
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var customerAccount = accountService.Get(id);
            if (customerAccount != null)
            {
                return Ok(customerAccount);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]CustomerAccount account)
        {
            if (!ModelState.IsValid)
            {
                ResponseModel.Notification = ApUtility.CreateNotification("Invalid information", Enums.NotificationType.Error);
                return StatusCode(500, ResponseModel);
            }

            try
            {
                account.Id = Guid.NewGuid();
                account.CreatedOn = DateTime.UtcNow;

                accountService.Create(account);
                accountService.SaveChanges();

                account = accountService.Get(account.Id);

                ResponseModel.Notification = ApUtility.CreateNotification("Account created successfully", Enums.NotificationType.Success);
                ResponseModel.Data = account;

                return Created(string.Empty, ResponseModel);
            }
            catch (Exception ex)
            {
                ResponseModel.Notification = ApUtility.CreateNotification(ex.Message, Enums.NotificationType.Error);
                return StatusCode(500, ResponseModel);
            }
        }

        [HttpPut]
        public IActionResult Put(CustomerAccount account)
        {
            try
            {
                account.ModifiedOn = DateTime.UtcNow;

                accountService.Update(account);
                accountService.SaveChanges();

                account = accountService.Get(account.Id);

                ResponseModel.Notification = ApUtility.CreateNotification("Account information updated successfully", Enums.NotificationType.Success);
                ResponseModel.Data = account;

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
                accountService.Delete(id);
                accountService.SaveChanges();

                ResponseModel.Notification = ApUtility.CreateNotification("Account information delete successfully", Enums.NotificationType.Success);
                return Accepted(ResponseModel);

            }
            catch (Exception ex)
            {
                ResponseModel.Notification = ApUtility.CreateNotification(ex.Message, Enums.NotificationType.Error);
                return StatusCode(500, ResponseModel);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult SoftDelete(Guid id)
        {
            try
            {
                accountService.Update(
                            new CustomerAccount()
                            {
                                Id = id,
                                ModifiedOn = DateTime.UtcNow,
                                RecordStatus = false
                            },
                            p => p.RecordStatus, p => p.ModifiedOn
                            );

                accountService.SaveChanges();

                ResponseModel.Notification = ApUtility.CreateNotification("Manufacturar delete successfully", Enums.NotificationType.Success);
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
        public IActionResult GetAccountSli(Guid customerId)
        {
            return Ok(accountService.AccountSli(customerId));
        }
    }
}