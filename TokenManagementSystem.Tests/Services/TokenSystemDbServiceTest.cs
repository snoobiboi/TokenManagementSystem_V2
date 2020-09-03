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
            // Arrange
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

            // Act
            var serviceUnderTest = new TokenSystemDbService(mockCosmosClient.Object, It.IsAny<string>(), It.IsAny<string>());

            var response = await serviceUnderTest.AddCustomerDetails(newCustomer);


            // Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(2, response.TokenNumber);
            Assert.AreEqual(0, response.Counter);
            Assert.AreEqual(Status.InQueue, response.Status);
        }

        [Test]
        public void TokenSystemDbService_GetCustomerById_ReturnsSingleCustomerSuccess()
        {
            // Arrange
            var mockContainer = new Mock<Container>();
            var mockCosmosClient = new Mock<CosmosClient>();
            var responseMock = new Mock<ItemResponse<Customer>>();

            var existingCusotmers = new List<Customer>
            {
                new Customer
                {
                    Id = "1111-11-111",
                    FirstName = "First",
                    Surname = "SecondName",
                    Age = 35,
                    CustomerType = CustomerType.AccountHolder,
                    AccountNumber = 123456,
                    Token = new Token {ServiceType = ServiceType.Transaction, TokenNumber = 1, Counter = 0, Status = Status.InQueue}
                },
                new Customer {
                    Id = "2222-222-222",
                    FirstName = "Second",
                    Surname = "Customer",
                    Age = 35,
                    CustomerType = CustomerType.Guest,
                    SocialNumber = 76543,
                    Token = new Token { ServiceType = ServiceType.Service, TokenNumber = 2, Counter = 0, Status = Status.InQueue }
                }
            }.AsQueryable();

            mockContainer.Setup(x => x.GetItemLinqQueryable<Customer>(true, null, null))
               .Returns((IOrderedQueryable<Customer>)existingCusotmers);

            mockCosmosClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(mockContainer.Object);
      
            var serviceUnderTest = new TokenSystemDbService(mockCosmosClient.Object, It.IsAny<string>(), It.IsAny<string>());
            
            // Act
            var response = serviceUnderTest.GetCustomerById(existingCusotmers.Last().Id);

            // Assert
            Assert.AreEqual(existingCusotmers.Last(), response);
        }

        [Test]
        public void TokenSystemDbService_GetCustomersTokenDetails_ReturnsCustomerDashboardDetails()
        {
            // Arrange
            var mockContainer = new Mock<Container>();
            var mockCosmosClient = new Mock<CosmosClient>();
            var responseMock = new Mock<ItemResponse<Customer>>();

            var existingCusotmers = new List<Customer>
            {
                new Customer
                {
                    Id = "1111-11-111",
                    FirstName = "First",
                    Surname = "SecondName",
                    Age = 35,
                    CustomerType = CustomerType.AccountHolder,
                    AccountNumber = 123456,
                    Token = new Token {ServiceType = ServiceType.Transaction, TokenNumber = 1, Counter = 0, Status = Status.InQueue}
                },
                new Customer {
                    Id = "2222-222-222",
                    FirstName = "Second",
                    Surname = "Customer",
                    Age = 35,
                    CustomerType = CustomerType.Guest,
                    SocialNumber = 76543,
                    Token = new Token { ServiceType = ServiceType.Service, TokenNumber = 2, Counter = 2, Status = Status.InCounter }
                },
                new Customer {
                    Id = "3333-333-333",
                    FirstName = "Third",
                    Surname = "Customer",
                    Age = 34,
                    CustomerType = CustomerType.Guest,
                    SocialNumber = 1234322,
                    Token = new Token { ServiceType = ServiceType.Service, TokenNumber = 3, Counter = 0, Status = Status.InQueue }
                }
            }.AsQueryable();

            var expected = new List<CustomerDashboard>
            {
                new CustomerDashboard
                {
                    TokenNumber = 1,
                    EstimatedWaitingTime = 5,
                    Counter = 0
                },
                new CustomerDashboard
                {
                    TokenNumber = 3,
                    EstimatedWaitingTime = 25,
                    Counter = 0
                }
            };

            mockContainer.Setup(x => x.GetItemLinqQueryable<Customer>(true, null, null))
               .Returns((IOrderedQueryable<Customer>)existingCusotmers);

            mockCosmosClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(mockContainer.Object);
            
            var serviceUnderTest = new TokenSystemDbService(mockCosmosClient.Object, It.IsAny<string>(), It.IsAny<string>());

            // Act
            var response = serviceUnderTest.GetCustomersTokenDetails();

            // Assert
            Assert.AreEqual(expected.Count(), response.Count());
            Assert.AreEqual(expected[0].Counter, response.ToList()[0].Counter);
            Assert.AreEqual(expected[0].EstimatedWaitingTime, response.ToList()[0].EstimatedWaitingTime);
            Assert.AreEqual(expected[0].TokenNumber, response.ToList()[0].TokenNumber);
            Assert.AreEqual(expected[1].Counter, response.ToList()[1].Counter);
            Assert.AreEqual(expected[1].EstimatedWaitingTime, response.ToList()[1].EstimatedWaitingTime);
            Assert.AreEqual(expected[1].TokenNumber, response.ToList()[1].TokenNumber);
        }
    
        [Test]
        public void TokenSystemDbService_GetTokenDetailsForBank_ReturnsBankDashboardDetails()
        {
            // Arrange
            var mockContainer = new Mock<Container>();
            var mockCosmosClient = new Mock<CosmosClient>();
            var responseMock = new Mock<ItemResponse<Customer>>();

            var existingCusotmers = new List<Customer>
            {
                new Customer
                {
                    Id = "1111-11-111",
                    FirstName = "First",
                    Surname = "SecondName",
                    Age = 35,
                    CustomerType = CustomerType.AccountHolder,
                    AccountNumber = 123456,
                    Token = new Token {ServiceType = ServiceType.Transaction, TokenNumber = 1, Counter = 0, Status = Status.InQueue}
                },
                new Customer {
                    Id = "2222-222-222",
                    FirstName = "Second",
                    Surname = "Customer",
                    Age = 35,
                    CustomerType = CustomerType.Guest,
                    SocialNumber = 76543,
                    Token = new Token { ServiceType = ServiceType.Service, TokenNumber = 2, Counter = 2, Status = Status.InCounter }
                }
            }.AsQueryable();

            var expected = new List<Token>
            {
                new Token
                {
                    TokenNumber = 1,
                    Status = Status.InQueue,
                    Counter = 0,
                    ServiceType = ServiceType.Transaction
                },
                new Token
                {
                    TokenNumber = 2,
                    Status = Status.InCounter,
                    Counter = 2,
                    ServiceType = ServiceType.Service
                }
            };

            mockContainer.Setup(x => x.GetItemLinqQueryable<Customer>(true, null, null))
               .Returns((IOrderedQueryable<Customer>)existingCusotmers);

            mockCosmosClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(mockContainer.Object);

            var serviceUnderTest = new TokenSystemDbService(mockCosmosClient.Object, It.IsAny<string>(), It.IsAny<string>());

            // Act
            var response = serviceUnderTest.GetTokenDetailsForBank();

            // Assert
            Assert.AreEqual(expected.Count(), response.Count());
            Assert.AreEqual(expected[0].Counter, response.ToList()[0].Counter);
            Assert.AreEqual(expected[0].Status, response.ToList()[0].Status);
            Assert.AreEqual(expected[0].TokenNumber, response.ToList()[0].TokenNumber);
            Assert.AreEqual(expected[0].ServiceType, response.ToList()[0].ServiceType);
            Assert.AreEqual(expected[1].Counter, response.ToList()[1].Counter);
            Assert.AreEqual(expected[1].Status, response.ToList()[1].Status);
            Assert.AreEqual(expected[1].TokenNumber, response.ToList()[1].TokenNumber);
            Assert.AreEqual(expected[1].ServiceType, response.ToList()[1].ServiceType);
        }

        [Test]
        public void UpdateTokenStatusAsync_TokenSystemDbService_UpdateTokenStatusWithSucess()
        {
            // Arrange
            var mockContainer = new Mock<Container>();
            var mockCosmosClient = new Mock<CosmosClient>();
            var responseMock = new Mock<ItemResponse<Customer>>();

            var existingCusotmers = new List<Customer>
            {
                new Customer
                {
                    Id = "1111-11-111",
                    FirstName = "First",
                    Surname = "SecondName",
                    Age = 35,
                    CustomerType = CustomerType.AccountHolder,
                    AccountNumber = 123456,
                    Token = new Token {ServiceType = ServiceType.Transaction, TokenNumber = 1, Counter = 0, Status = Status.InQueue}
                }
            }.AsQueryable();

            mockContainer.Setup(x => x.GetItemLinqQueryable<Customer>(true, null, null))
               .Returns((IOrderedQueryable<Customer>)existingCusotmers);

            mockCosmosClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(mockContainer.Object);

            var serviceUnderTest = new TokenSystemDbService(mockCosmosClient.Object, It.IsAny<string>(), It.IsAny<string>());

            // Act
            var response = serviceUnderTest.UpdateTokenStatusAsync(existingCusotmers.FirstOrDefault().Id, Status.Served);

            // Assert
            Assert.IsTrue(response.Result);
        }

        [Test]
        public void UpdateTokenStatusAsync_TokenSystemDbService_UpdateTokenStatusFails()
        {
            // Arrange
            var mockContainer = new Mock<Container>();
            var mockCosmosClient = new Mock<CosmosClient>();
            var responseMock = new Mock<ItemResponse<Customer>>();

            var existingCusotmers = new List<Customer>
            {
                new Customer
                {
                    Id = "1111-11-111",
                    FirstName = "First",
                    Surname = "SecondName",
                    Age = 35,
                    CustomerType = CustomerType.AccountHolder,
                    AccountNumber = 123456,
                    Token = new Token {ServiceType = ServiceType.Transaction, TokenNumber = 1, Counter = 0, Status = Status.InQueue}
                }
            }.AsQueryable();

            mockContainer.Setup(x => x.GetItemLinqQueryable<Customer>(true, null, null))
               .Returns((IOrderedQueryable<Customer>)existingCusotmers);

            mockCosmosClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(mockContainer.Object);

            var serviceUnderTest = new TokenSystemDbService(mockCosmosClient.Object, It.IsAny<string>(), It.IsAny<string>());

            // Act
            var response = serviceUnderTest.UpdateTokenStatusAsync("abcd-124", Status.Served);

            // Assert
            Assert.IsFalse(response.Result);
        }
    }
}
