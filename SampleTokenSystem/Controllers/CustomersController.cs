using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TokenManagementSystem.Models;
using TokenManagementSystem.Services;

namespace TokenManagementSystem.Controllers
{
    [Produces("application/json")]
    [Route("api/customers")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ITokenSystemDbService tokenDbService;

        public CustomersController(ITokenSystemDbService dbService) => tokenDbService = dbService;

        public IEnumerable<CustomerDashboard> Get()
        {
            return tokenDbService.GetCustomersTokenDetails();
        }

        [HttpGet("{Id}")]
        public IActionResult GetById(string id)
        {
            return Ok(tokenDbService.GetCustomerById(id));
        }

        [HttpPost]
        public async Task<IActionResult> Post(Customer customer)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var token = await tokenDbService.AddCustomerDetails(customer);
                return StatusCode(StatusCodes.Status201Created, token);
            }
            catch (Exception e)
            {
                return Problem("Ooops... Not able to add the customer. " + e.Message);
            }           
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Put(string id, Customer customer)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customer.Id)
            {
                return BadRequest("Customer Id's to update do not match");
            }

            try
            {
                await tokenDbService.UpdateCustomerAsync(id, customer);
            }
            catch (Exception e)
            { 
                return NotFound("No record found against the Id. " + e.Message);
            }
            
            return Ok("Customer " + id + " is updated successfuly");
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                // customerRepository.DeleteCustomer(id);
                await tokenDbService.DeleteCustomerAsync(id);
            }
            catch (Exception e)
            {

                return Problem("we encountered a problem. " + e.Message);
            }

            return Ok("Customer details has been deleted");
        }
    }
}
