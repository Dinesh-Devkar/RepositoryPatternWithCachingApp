using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController:ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository=customerRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Customer customer)
        {
            await _customerRepository.AddAsync(customer);
            return Ok("Customer Created Successfully");
        }

        [HttpPost("MultipleCustomers")]
        public async Task<IActionResult> PostCustomers()
        {
            await _customerRepository.AddCustomerRangeAsync();
            return Ok("Customers Created Successfully");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers= await _customerRepository.GetAllAsync();
            return Ok(customers);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id,Customer customerDto)
        {
            var customer= await _customerRepository.GetByIdAsync(id);
            if (customer==null)
            {
                return BadRequest("Customer Not Found");
            }
            await _customerRepository.UpdateAsync(customerDto);
            return Ok("Customer Updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var customer= await _customerRepository.GetByIdAsync(id);
            if (customer==null)
            {
                return BadRequest("Customer Not Found");
            }
            await _customerRepository.DeleteAsync(customer);
            return Ok("Customer Deleted successfully");
        }

    }
}