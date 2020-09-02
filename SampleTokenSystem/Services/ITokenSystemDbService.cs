using TokenManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TokenManagementSystem.Services
{
    public interface ITokenSystemDbService
    {
        IEnumerable<CustomerDashboard> GetCustomersTokenDetails();
        IEnumerable<Token> GetTokenDetailsForBank();
        Customer GetCustomerById(string id);
        Task AddCustomerDetails(Customer customer);
        Task<bool> UpdateTokenStatusAsync(string id, string status);
        Task UpdateCustomerAsync(string id, Customer customer);
        Task DeleteCustomerAsync(string id);
    }
}
