using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TokenManagementSystem.Models;
using TokenManagementSystem.Models.Constants;
using TokenManagementSystem.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TokenManagementSystem.Controllers
{
    [Produces("application/json")]
    [Route("api/bank")]
    [ApiController]
    public class BankController : ControllerBase
    {
        private readonly ITokenSystemDbService tokenDbService;

        public BankController(ITokenSystemDbService dbService) => tokenDbService = dbService;

        public IEnumerable<Token> Get()
        {
            return tokenDbService.GetTokenDetailsForBank();
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            throw new NotImplementedException();
        }

        // POST api/<BankController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
            throw new NotImplementedException();
        }

        // PUT api/<BankController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] string status)
        {
            if (!String.IsNullOrEmpty(status) && (status == Status.InCounter || status == Status.Served))
            {
                try
                {
                    var response = await tokenDbService.UpdateTokenStatusAsync(id, status);
                    if (!response)
                    {
                        return NotFound("Error occured while updating.");
                    }
                    return Ok("Customer's token status is updated successfuly");
                }
                catch (Exception e)
                {
                    return Problem("Error occured while updating. " + e.Message);
                }
            }

            return BadRequest("Status not valid for updating.");
        }

        // DELETE api/<BankController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
