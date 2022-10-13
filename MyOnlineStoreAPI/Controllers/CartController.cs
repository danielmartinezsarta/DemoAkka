using Akka.Actor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyOnlineStore.Messages.Billing;
using MyOnlineStore.Messages.Billing.Commands;
using MyOnlineStoreAPI.Refs;

namespace MyOnlineStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get(string product, int quantity, string user)
        {
            
            return Ok(new { Tick = await ActorDirectory.CheckOutActor.Ask(new AddProductToCart(user, product, quantity, 0M)) });
        }


        [HttpGet("cart-status")]
        public async Task<IActionResult> Get(string user)
        {
            return Ok(new { Tick = await ActorDirectory.CheckOutActor.Ask(new GetUserCartCommand(user)) });
        }
    }
}
