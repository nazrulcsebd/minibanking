using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniBanking.Core.Helper;
using MiniBanking.Core.Helper.Entities;
using MiniBanking.Entity;

namespace CodeBonds.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BaseController : ControllerBase
    {
        public ResponseModel ResponseModel;
        public AppClaim AppClaim;
        public readonly MiniBankingDbContext Context;

        public BaseController(MiniBankingDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            ResponseModel = new ResponseModel();
            Context = context;

            var user = httpContextAccessor.HttpContext.User;
            if (user.Identity.IsAuthenticated)
            {
                var claimsIdentity = (System.Security.Claims.ClaimsIdentity)user.Identity;
                AppClaim = claimsIdentity.ParseClaim();
            }
            else
            {
                AppClaim = null;
            }
        }
    }
}