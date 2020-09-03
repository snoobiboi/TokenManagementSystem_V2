using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenManagementSystem.Controllers;
using TokenManagementSystem.Models;
using TokenManagementSystem.Models.Constants;
using TokenManagementSystem.Services;

namespace TokenManagementSystem.Tests.Controllers
{
    class CustomersControllerTest
    {
        Mock<ITokenSystemDbService> mockService;
        CustomersController underTest;
        List<CustomerDashboard> customerDashBoardDetails;
        List<Customer> customers;

        public double Customers { get; private set; }

        [SetUp]
        public void SetUp()
        {
            mockService = new Mock<ITokenSystemDbService>();
            underTest = new CustomersController(mockService.Object);

            customerDashBoardDetails = new List<CustomerDashboard>
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

            customers = new List<Customer>
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
                };
        }

        [Test]
        public void Get_ReturnsCustomerDashboardDetails_AsExpected()
        {
            // Arrange
            mockService.Setup(x => x.GetCustomersTokenDetails()).Returns(customerDashBoardDetails);

           // Act
           var response =  underTest.Get();

            // Assert
            Assert.AreEqual(customerDashBoardDetails.Count(), response.Count());
            Assert.AreEqual(customerDashBoardDetails[0].Counter, response.ToList()[0].Counter);
            Assert.AreEqual(customerDashBoardDetails[0].EstimatedWaitingTime, response.ToList()[0].EstimatedWaitingTime);
            Assert.AreEqual(customerDashBoardDetails[0].TokenNumber, response.ToList()[0].TokenNumber);
            Assert.AreEqual(customerDashBoardDetails[1].Counter, response.ToList()[1].Counter);
            Assert.AreEqual(customerDashBoardDetails[1].EstimatedWaitingTime, response.ToList()[1].EstimatedWaitingTime);
            Assert.AreEqual(customerDashBoardDetails[1].TokenNumber, response.ToList()[1].TokenNumber);
        }

        [Test]
        public void GetById_ReturnsSingleCustomerDetails_AsExpected()
        {
            // Arrrange
            mockService.Setup(x => x.GetCustomerById(customers.LastOrDefault().Id)).Returns(customers.LastOrDefault());

            // Act
            var response = underTest.GetById(customers.LastOrDefault().Id) as ObjectResult;

            // Assert
            Assert.AreEqual(StatusCodes.Status200OK, response.StatusCode);
            Assert.AreEqual(customers.LastOrDefault(), response.Value);
        }

        [Test]
        public async Task Post_CreatesCustomer_ReturnToken_AsExpected()
        {
            // Arrange
            mockService.Setup(x => x.AddCustomerDetails(It.IsAny<Customer>())).Returns(Task.FromResult(customers[0].Token));

            // Act
            var response = await underTest.Post(customers[0]) as ObjectResult;

            // Asset
            Assert.AreEqual(StatusCodes.Status201Created, response.StatusCode);
        }

        [Test]
        public async Task Post_CreatesCustomer_ReturnBadRequestErrorForInvalidModel_AsExpected()
        {
            // Arrange
            mockService.Setup(x => x.AddCustomerDetails(It.IsAny<Customer>())).Returns(Task.FromResult(customers[0].Token));
            var customer = new Customer { FirstName = "Invalid Model" };
            underTest.ModelState.AddModelError("SurName", "Required");

            // Act
            var response = await underTest.Post(customer) as ObjectResult;

            // Asset
            Assert.AreEqual(StatusCodes.Status400BadRequest, response.StatusCode);
        }
    }
}
