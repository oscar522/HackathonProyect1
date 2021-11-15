using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using WebApi.Models;
using WebApi.Services;
using WebApplication1.Data;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private readonly Connection _context;

        public UsersController(IUserService userService, Connection context)
        {
            _userService = userService;
            _context = context;

        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            var response = _userService.Authenticate(model);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            try {

                var xxxx = _context.Orders.Where(x => x.id == 2637).ToList();
                var cc = "";
            } catch(Exception e ) {
                var error = e; 
            }
            

            var users = _userService.GetAll();
            return Ok(users);
        }
    }
}
