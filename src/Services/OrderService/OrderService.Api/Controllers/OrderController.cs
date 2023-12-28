using System;
using System.Diagnostics;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Features.Queries.GetOrderDetailById;

namespace OrderService.Api.Controllers
{
    //[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public class OrderController:ControllerBase
	{
        private readonly IMediator mediator;

        public OrderController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderDetailsById(Guid id)
        {
            var res = await mediator.Send(new GetOrderDetailsQuery(id));

            return Ok(res);
        }
    }
}

