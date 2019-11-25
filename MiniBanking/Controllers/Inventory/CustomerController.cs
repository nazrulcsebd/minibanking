using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

namespace CodeBonds.Controllers.Inventory
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : BaseController
    {
        private CustomerService customerService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerController(MiniBankingDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
            customerService = new CustomerService(context);
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var itemType = customerService.Get(id);
            if (itemType != null)
            {
                return Ok(itemType);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetCustomer()
        {
            var itemType = customerService.Get(Guid.Parse(AppClaim.CustomerId));
            if (itemType != null)
            {
                return Ok(itemType);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]Customer customer)
        {
            if (!ModelState.IsValid)
            {
                ResponseModel.Notification = ApUtility.CreateNotification("Invalid information", Enums.NotificationType.Error);
                return StatusCode(500, ResponseModel);
            }

            try
            {
                var chekDuplicateEemail = customerService.GetByEmail(customer.Email);
                if (chekDuplicateEemail != null)
                {
                    ResponseModel.Notification = ApUtility.CreateNotification("Email Already Exists.", Enums.NotificationType.Error);
                    return StatusCode(500, ResponseModel);
                }

                customer.Id = Guid.NewGuid();
                customer.Password = customer.Password.Encrypt();
                customer.CreatedOn = DateTime.UtcNow;

                customerService.Create(customer);
                customerService.SaveChanges();

                customer = customerService.Get(customer.Id);

                ResponseModel.Notification = ApUtility.CreateNotification("Customer Registration is succeed.", Enums.NotificationType.Success);
                ResponseModel.Data = customer;

                return Created(string.Empty, ResponseModel);
            }
            catch (Exception ex)
            {
                ResponseModel.Notification = ApUtility.CreateNotification(ex.Message, Enums.NotificationType.Error);
                return StatusCode(500, ResponseModel);
            }
        }

        [HttpPut]
        public IActionResult Put(Customer customer)
        {
            try
            {
                customer.ModifiedOn = DateTime.UtcNow;
                customerService.Update(customer);
                customerService.SaveChanges();

                customer = customerService.Get(customer.Id);

                ResponseModel.Notification = ApUtility.CreateNotification("Customer Info is updated successfully", Enums.NotificationType.Success);
                ResponseModel.Data = customer;

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
                customerService.Delete(id);
                customerService.SaveChanges();

                ResponseModel.Notification = ApUtility.CreateNotification("Customer delete successfully", Enums.NotificationType.Success);
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
        public IActionResult CustomerSli()
        {
            return Ok(customerService.CustomerSli(Guid.Parse(AppClaim.CustomerId)));
        }
    }
}