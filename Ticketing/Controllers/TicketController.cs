using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedModels;
using System;
using System.Threading.Tasks;

namespace Ticketing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly IBus _bus;
        public TicketController(IBus bus)
        {
            _bus = bus;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateTicket(Ticket ticket)
        {
            if (ticket != null)
            {
                ticket.BookedOn = DateTime.Now;
                Uri uri = new Uri("rabbitmq://localhost/ticketQueue");
                var endPoint = await _bus.GetSendEndpoint(uri);
                await endPoint.Send(ticket);
                return Ok(new { success = true, message = "Ticket Created Successfully" });
            }
            return BadRequest(new { success = false, message = "Failed to create ticket" });
        }
    }
}
