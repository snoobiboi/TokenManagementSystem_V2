using Microsoft.Azure.Cosmos;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TokenManagementSystem.Models;
using TokenManagementSystem.Models.Constants;
using TokenManagementSystem.Services;

namespace TokenManagementSystem.Tests.Services
{
    class TokenSystemDbServiceTest
    {
        [SetUp]
        public void Setup()
        {
            //var mockContainer = new Mock<Container>();
            //var mockCosmosClient = new Mock<CosmosClient>();

            //// mockContainer.Setup()
            //mockCosmosClient.Setup(x => x.GetContainer("DatabaseId", "ContainerId")).Returns();

        }

        [Test]
        public async Task TokenSystemDbService_AddCustomerDetails_ReturnsCreatedSuccess()
        {

            var mockContainer = new Mock<Container>();
            var mockCosmosClient = new Mock<CosmosClient>();
            var responseMock = new Mock<ItemResponse<Customer>>();
            Customer newCustomer = new Customer
            {
                FirstName = "New",
                Surname = "Customer",
                Age = 35,
                CustomerType = CustomerType.AccountHolder,
                AccountNumber = 34567,
                Token = new Token { ServiceType = ServiceType.Transaction }
            };

            var existingCusotmers = new List<Customer>
            {
                new Customer
                {
                    Id = "1111-11111-11",
                    FirstName = "FirstCustoer",
                    Surname = "SecondName",
                    Age = 35,
                    CustomerType = CustomerType.AccountHolder,
                    AccountNumber = 123456,
                    Token = new Token {ServiceType = ServiceType.Transaction, TokenNumber = 1}
                }
            }.AsQueryable();

            mockContainer.Setup(x => x.GetItemLinqQueryable<Customer>(true, null, null))
               .Returns((IOrderedQueryable<Customer>)existingCusotmers);

            mockCosmosClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(mockContainer.Object);

            var serviceUnderTest = new TokenSystemDbService(mockCosmosClient.Object, It.IsAny<string>(), It.IsAny<string>());

            var response = await serviceUnderTest.AddCustomerDetails(newCustomer);

            Assert.IsNotNull(response);
            Assert.AreEqual(2, response.TokenNumber);
            Assert.AreEqual(0, response.Counter);
            Assert.AreEqual(Status.InQueue, response.Status);
        }


    }
}
