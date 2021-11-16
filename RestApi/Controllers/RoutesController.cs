using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        //private readonly Connection _context;
        private readonly IConfiguration _configuration;
        public IConfiguration Configuration { get; }

        public RoutesController(IUserService userService, IConfiguration iConfig)
        {
            _userService = userService;
            _configuration = iConfig;
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
        public IActionResult CreateRoutes(string DateRoutes)
        {

            try
            {
                RoutesService RoutesService_ = new RoutesService();
                var StatusResult = RoutesService_.RoutesCreate( Convert.ToDateTime( DateRoutes));
                return Ok(StatusResult);

            } catch(Exception e ) {
                return UnprocessableEntity(e); 
            }
           
        }
    }
}
