using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TaskList.Models;
using TaskList.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TaskList.Services;
using Microsoft.Extensions.Primitives;
using System;
using Microsoft.Net.Http.Headers;

namespace TaskList.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        IUserService UserService;
        public UserController(IUserService UserService)
        {
            this.UserService = UserService;
        }

        [HttpPost]
        [Route("[action]")]
        public ActionResult<String> Login([FromBody] User user)
        {
            List<User> users = UserService.GetAll();
            User myUser = users.FirstOrDefault(u => u.Name.Equals(user.Name) && u.Password.Equals(user.Password));
            if (myUser == null)
                return Unauthorized();

            var claims = new List<Claim>();

            if (myUser.IsAdmin)
                claims.Add(new Claim("type", "Admin"));
            else
                claims.Add(new Claim("type", "User"));
            claims.Add(new Claim("userid", myUser.Id.ToString()));

            var token = TokenService.GetToken(claims);

            return new OkObjectResult(TokenService.WriteToken(token));

        }

        [HttpGet]
        [Route("[action]")]
        [Authorize(Policy = "Admin")]
        public ActionResult<List<User>> GetAll() =>
            UserService.GetAll();


        [HttpGet("{id}")]
        [Authorize(Policy = "User")]
        public ActionResult<User> Get(int id)
        {
            var user = UserService.Get(id);
            if (user == null)
                return NotFound();

            return user;
        }

        [HttpGet]
        [Route("[action]")]
        [Authorize(Policy = "User")]
        public ActionResult<User> GetCurrentUser()
        {
            var token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            var user = UserService.GetCurrentUser(token);
            if (user == null)
                return NotFound();
            return user;
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public IActionResult Create(User user)
        {
            UserService.Add(user);
            return CreatedAtAction(nameof(Create), new { id = user.Id }, user);

        }
        
        [HttpDelete("{id}")]
        [Authorize(Policy = "Admin")]
        public IActionResult Delete(int id)
        {            
            var user = UserService.Get(id);
            if (user is null)
                return NotFound();
            if( user.IsAdmin)
                return Forbid();
            UserService.Delete(id);

            return Content(UserService.Count.ToString());
        }
    }
}