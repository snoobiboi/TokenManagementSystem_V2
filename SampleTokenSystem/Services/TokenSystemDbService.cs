using Microsoft.Azure.Cosmos;
using TokenManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TokenManagementSystem.Models.Constants;
using Microsoft.Extensions.Configuration;

namespace TokenManagementSystem.Services
{
    public class TokenSystemDbService : ITokenSystemDbService
    {
        private Container container;

        public TokenSystemDbService(
            CosmosClient dbClient,
            string databaseName,
            string contianerName)
        {
            this.container = dbClient.GetContainer(databaseName, contianerName);
        }
        public async Task<Token> AddCustomerDetails(Customer customer)
        {
            var customerId = Guid.NewGuid().ToString();
            var customerCount = this.container.GetItemLinqQueryable<Customer>(true).Count();
            customer.Id = customerId;
            customer.Token.TokenNumber = ++customerCount;
            customer.Token.Status = Status.InQueue;
            await this.container.CreateItemAsync(customer, new PartitionKey(customer.Token.ServiceType));
            return customer.Token;
        }

        public async Task DeleteCustomerAsync(string id)
        {
            var customerToBeDeleted = this.container.GetItemLinqQueryable<Customer>(true)
                   .Where(x => x.Id.Equals(id)).AsEnumerable().FirstOrDefault();
            await this.container.DeleteItemAsync<Customer>(id, new PartitionKey(customerToBeDeleted.Token.ServiceType));
        }

        public Customer GetCustomerById(string id)
        {
            try
            {
                var response = this.container.GetItemLinqQueryable<Customer>(true)
                    .Where(x => x.Id.Equals(id)).AsEnumerable().FirstOrDefault();
                return response;
            }
            catch (CosmosException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }      
        }

        public IEnumerable<CustomerDashboard> GetCustomersTokenDetails()
        {
            var customers = this.container.GetItemLinqQueryable<Customer>(true).Where(x => x.Token.Status.Equals(Status.InQueue)).AsEnumerable();
            int tokensInTransaction = 0;
            int tokensInService = 0;
            int transactionWaitingTime = 5;
            int serviceWaitingTime = 25;

            List<CustomerDashboard> customerDashboard = new List<CustomerDashboard>();

            foreach (var customer in customers)
            {
                var customerTokenDetails = new CustomerDashboard
                {
                    TokenNumber = customer.Token.TokenNumber,
                    Counter = customer.Token.Counter
                };

                if (customer.Token.ServiceType.Equals(ServiceType.Transaction))
                {
                    ++tokensInTransaction;
                    customerTokenDetails.EstimatedWaitingTime = transactionWaitingTime * tokensInTransaction;

                } else if(customer.Token.ServiceType.Equals(ServiceType.Service))
                {
                    ++tokensInService;
                    customerTokenDetails.EstimatedWaitingTime = serviceWaitingTime * tokensInService;
                }

                customerDashboard.Add(customerTokenDetails);
            }

            return customerDashboard;
        }

        public IEnumerable<Token> GetTokenDetailsForBank()
        {
            return this.container.GetItemLinqQueryable<Customer>(true).AsEnumerable().Select(x => x.Token);
        }
        
        public async Task UpdateCustomerAsync(string id, Customer customer)
        {
            await this.container.UpsertItemAsync<Customer>(customer, new PartitionKey(customer.Token.ServiceType));
        }

        public async Task<bool> UpdateTokenStatusAsync(string id, string status)
        {
            var customer = this.container.GetItemLinqQueryable<Customer>(true).Where(x => x.Id.Equals(id)).AsEnumerable().FirstOrDefault();

            if (customer == null)
            {
                return false;
            }

            customer.Token.Status = status;
            customer.Token.Counter = customer.Token.ServiceType == ServiceType.Transaction.ToString() ? 1 : 2;
            await this.container.UpsertItemAsync<Customer>(customer, new PartitionKey(customer.Token.ServiceType));
            return true;
        }
    }
}
