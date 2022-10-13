using Akka.Actor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyOnlineStore.Messages.Catalogs;
using MyOnlineStoreAPI.Refs;

namespace MyOnlineStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> Get()
        {

            return Ok(await ActorDirectory.CatalogActorRouter.Ask(new QueryStoreCatalog()));
        }

    }
}
