using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EnterpriseOrderSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {

        [HttpGet("public")]
        public IActionResult Public()
        {
            return Ok("This is public endpoint");
        }


        [HttpGet]
        [Authorize]
        public IActionResult GetProducts()
        {
            return Ok("You are authenticated!");
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateProduct()
        {
            return Ok("Only admin can create product");
        }

        // 👤 GET CURRENT USER FROM TOKEN
        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new
            {
                userId,
                email,
                role
            });
        }
    }
}
