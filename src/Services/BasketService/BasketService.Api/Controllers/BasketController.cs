using System.Net;
using BasketService.Api.Core.Application.Repository;
using BasketService.Api.Core.Application.Services;
using BasketService.Api.Core.Domain.Models;
using BasketService.Api.IntegrationEvents.Events;
using EventBus.Base.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasketService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BasketController : ControllerBase
    {

        private readonly IBasketRepository basketRepository;
        private readonly IIdentityService identityService;
        private readonly IEventBus eventBus;
        private readonly ILogger<BasketController> logger;

        public BasketController(IBasketRepository basketRepository, IIdentityService identityService, IEventBus eventBus, ILogger<BasketController> logger)
        {
            this.basketRepository = basketRepository;
            this.identityService = identityService;
            this.eventBus = eventBus;
            this.logger = logger;
        }

        // GET: api/basket
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Basket Service is Up and Running");
        }

        // GET api/basket/id
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CustomerBasket),(int)HttpStatusCode.OK)]
        public async Task<ActionResult<CustomerBasket>> GetBasketById(string id)
        {
            var basket = await basketRepository.GetBasket(id);
            return Ok(basket??new CustomerBasket(id));
        }

        // PUT api/basket/5
        [HttpPut]
        [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket([FromBody] CustomerBasket basket)
        {
            return Ok(await basketRepository.UpdateBasket(basket));
        }


        // POST api/basket/AddItemToBasket
        [HttpPost]
        [Route("additem")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddItemToBasket([FromBody]BasketItem basketItem)
        {
            var userId = identityService.GetUserName().ToString();

            var basket = await basketRepository.GetBasket(userId);

            if (basket == null)
            {
                basket = new CustomerBasket(userId);
            }

            basket.Items.Add(basketItem);

            await basketRepository.UpdateBasket(basket);

            return Ok();
        }

        [Route("checkout")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            var userId = basketCheckout.Buyer;

            var basket = await basketRepository.GetBasket(userId);

            if (basket == null)
            {
                return BadRequest();
            }

            var userName = identityService.GetUserName();

            var eventMessage = new OrderCreatedIntegrationEvent(userId,userName,basketCheckout.City,basketCheckout.Street,basketCheckout.State,basketCheckout.Country,basketCheckout.ZipCode,basketCheckout.CardNumber,basketCheckout.CardHolderName,basketCheckout.CardExpiration,basketCheckout.CardSecurityNumber,basketCheckout.CardTypeId,basketCheckout.Buyer,basket);

            try
            {
                eventBus.Publish(eventMessage);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Publishing integration event : {IntegrationEventId} from {BasketService.Api}", eventMessage.Id);

                return BadRequest();
            }
            return Accepted();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void),(int) HttpStatusCode.OK)]
        public async Task Delete(string id)
        {
            await basketRepository.DeleteBasket(id);
        }
    }
}

