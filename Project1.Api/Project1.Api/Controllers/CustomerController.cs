using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using Microsoft.Data.SqlClient;
using Project1.DB;
using Project1.Logic;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project1.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {

        private readonly IDBCommands dBCommands;
        private readonly ILogger<CustomerController> logger;
        public CustomerController(IDBCommands dBCommands, ILogger<CustomerController> logger)
        {
            this.dBCommands = dBCommands;
            this.logger = logger;
        }
        // GET: api/<CustomerController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> findCustomerAsync([FromQuery, Required] string firstName, string lastName)
        {
            IEnumerable<Customer> customers;
            try
            {
                customers = await dBCommands.findCustomerAsync(firstName, lastName);
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "SQL error while finding {firstName} {lastName}", firstName, lastName);
                return StatusCode(500);
            }

            return customers.ToList();

        }


        

        // PUT api/<CustomerController>/5
        [HttpPut]
        public async Task<StatusCodeResult> AddNewCustomerAsync([FromQuery, Required] string firstName, string lastName)
        {
            try
            {
                await dBCommands.AddNewCustomerAsync(firstName, lastName);
                return StatusCode(200);
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "SQL error while creating customer {firstName} {lastName}", firstName, lastName);
                return StatusCode(500);
            }
        }

        
    }
}
