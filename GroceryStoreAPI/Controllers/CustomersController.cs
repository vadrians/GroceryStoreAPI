using GroceryStoreAPI.Contracts.Interfaces;
using GroceryStoreAPI.Contracts.Models;
using GroceryStoreAPI.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace GroceryStoreAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllCustomers()
        {
            try
            {
                var response = await _customerService.GetAllCustomersAsync();

                return response.Succeeded()
                    ? Ok(response.Content)
                    : StatusCode((int)response.Status, response.ErrorMessage);
            }
            catch (ArgumentException e)
            {
                _logger.LogWarning(e.Message);
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error when {nameof(GetAllCustomers)}");
                return StatusCode((int) HttpStatusCode.InternalServerError, "Error getting customers");
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetAllCustomers(int id)
        {
            try
            {
                CustomerValidator.Validate(id);

                var response = await _customerService.GetCustomerAsync(id);

                return response.Succeeded()
                    ? Ok(response.Content)
                    : StatusCode((int)response.Status, response.ErrorMessage);
            }
            catch (ArgumentException e)
            {
                _logger.LogWarning(e.Message);
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error when {nameof(GetAllCustomers)}", new { id });
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error getting customer");
            }
        }

        [HttpPost]
        [Route("{name}")]
        public async Task<IActionResult> AddCustomer(string name)
        {
            try
            {
                CustomerValidator.Validate(name);

                var response = await _customerService.AddCustomer(name);

                return response.Succeeded()
                    ? Ok(response.Content)
                    : StatusCode((int)response.Status, response.ErrorMessage);
            }
            catch (ArgumentException e)
            {
                _logger.LogWarning(e.Message);
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error when {nameof(AddCustomer)}", new { name });
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error adding customers");
            }
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateCustomer(Customer customer)
        {
            try
            {
                CustomerValidator.Validate(customer);

                var response = await _customerService.UpdateCustomer(customer);

                if (response.Succeeded()) return Ok();

                return StatusCode((int)response.Status, response.ErrorMessage);
            }
            catch (ArgumentException e)
            {
                _logger.LogWarning(e.Message);
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error when {nameof(UpdateCustomer)}", new { customer });
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error updating customer");
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> RemoveCustomer(int id)
        {
            try
            {
                CustomerValidator.Validate(id);

                var response = await _customerService.RemoveCustomerAsync(id);

                if (response.Succeeded()) return Ok();

                return StatusCode((int) response.Status, response.ErrorMessage);
            }
            catch (ArgumentException e)
            {
                _logger.LogWarning(e.Message);
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error when {nameof(RemoveCustomer)}", new { id });
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error remove customer");
            }
        }
    }
}