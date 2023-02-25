﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PlanTogetherDotNetAPI.Data;
using PlanTogetherDotNetAPI.DTOs;
using PlanTogetherDotNetAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace PlanTogetherDotNetAPI.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private readonly UserManager<AppUser> userManager;

        public AccountController(UserManager<AppUser> userManager) 
        {
            this.userManager = userManager;
        }

        [Route("user-count")]
        public async Task<IHttpActionResult> GetUserCountAsync()
        {
            var count = await userManager.Users.CountAsync();
            return Ok(count);
        }

        [Route("register")]
        public async Task<IHttpActionResult> PostRegisterAsync(RegisterDTO input)
        {
            var user = new AppUser
            {
                UserName = input.UserName,
                Email = input.Email,
                DisplayName = input.DisplayName
            };
            var result = await userManager.CreateAsync(user,input.Password);
            if(result.Succeeded)
            {
                return Ok(result);
            }
            return BadRequest();
        }

        [Route("login")]
        public async Task<IHttpActionResult> PostLoginAsync(LoginDTO lg)
        {
            var user = await userManager.FindByNameAsync(lg.UserName);
            var result = await userManager.CheckPasswordAsync(user,lg.Password);
            if(!result)
            {
                return Unauthorized();
            }
            return Ok();
        }
    }
}
