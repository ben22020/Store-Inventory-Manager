using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Project1.DB;
using Project1.Logic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project1.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {



        private readonly IDBCommands dBCommands;
        private readonly ILogger<OrderController> logger;
        public OrderController(IDBCommands dBCommands, ILogger<OrderController> logger)
        {
            this.dBCommands = dBCommands;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> listOrderDetailsOfCustomerAsync([FromQuery, Required] string customer)
        {

            List<Order> orders = new();

            try
            {
                orders = (List<Order>)await dBCommands.listOrderDetailsOfCustomerAsync(customer);
                Order order = orders.ElementAt(0);
                Console.WriteLine(order.storeID);
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "SQL error while getting orders of customer ID {customer}", customer);
                return StatusCode(500);
            }

            return new JsonResult(orders);
        }

        // POST api/<OrderController>
        [HttpPut]
        public async Task<StatusCodeResult> PlaceOrderAsync([FromQuery, Required] string customerID, string locationID, string date, string productID, string quantity)
        {
            try
            {
                await dBCommands.PlaceOrderAsync(customerID, locationID, date, productID, quantity);
                return StatusCode(200);
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "SQL error while creating order for customer #{customerID}", customerID);
                return StatusCode(500);
            }
        }

   
        


    }
}



