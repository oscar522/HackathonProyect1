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
    public class RoutesController : ControllerBase
    {
        private IUserService _userService;
        private readonly Connection _context;

        public RoutesController(IUserService userService, Connection context)
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

        
        [HttpGet]
        public IActionResult CreateRoutes(string DateRoutes )
        {
            try {
                    RoutesService RoutesService_ = new RoutesService();
                    var StatusResult = RoutesService_.RoutesCreate(_context, Convert.ToDateTime( DateRoutes));
                return Ok(StatusResult);

            } catch(Exception e ) {
                return UnprocessableEntity(e); 
            }
           
        }
    }
}
