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
    class BankControllerTest
    {
        Mock<ITokenSystemDbService> mockService;
        BankController underTest;
        List<Token> bankDashBoardDetails;
        List<Customer> customers;

        [SetUp]
        public void Setup()
        {
            mockService = new Mock<ITokenSystemDbService>();
            underTest = new BankController(mockService.Object);

            bankDashBoardDetails = new List<Token>
                {
                    new Token
                    {
                        TokenNumber = 1,
                        Status = Status.InCounter,
                        Counter = 1,
                        ServiceType = ServiceType.Transaction
                    },
                    new Token
                    {
                        TokenNumber = 3,
                        Status = Status.InQueue,
                        Counter = 0,
                        ServiceType = ServiceType.Service
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
            mockService.Setup(x => x.GetTokenDetailsForBank()).Returns(bankDashBoardDetails);

            // Act
            var response = underTest.Get();

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual(bankDashBoardDetails.Count(), response.Count());
        }

        [Test]
        public async Task Put_UpdatesCustomerToken_AsExpected()
        {
            // Arrange
            mockService.Setup(x => x.UpdateTokenStatusAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            // Act
            var response = await underTest.Put("1111-111-11", Status.InCounter) as ObjectResult;

            // Assert
            Assert.AreEqual(StatusCodes.Status200OK, response.StatusCode);
            Assert.AreEqual("Customer's token status is updated successfuly", response.Value);
        }

        [Test]
        public async Task Put_UpdatesCustomerToken_ForInValidStatus_Fails()
        {
            // Arrange
            mockService.Setup(x => x.UpdateTokenStatusAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            // Act
            var response = await underTest.Put("1111-111-11", "Invalid Status") as ObjectResult;

            // Assert
            Assert.AreEqual(StatusCodes.Status400BadRequest, response.StatusCode);
            Assert.AreEqual("Status not valid for updating.", response.Value);
        }

        [Test]
        public async Task Put_UpdatesCustomerToken_ThrowsNotFoundOnInValidCustomer_AsExpected()
        {
            // Arrange
            mockService.Setup(x => x.UpdateTokenStatusAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(false));

            // Act
            var response = await underTest.Put("1111-111-11", Status.InCounter) as ObjectResult;

            // Assert
            Assert.AreEqual(StatusCodes.Status404NotFound, response.StatusCode);
            Assert.AreEqual("Invalid customer Id.", response.Value);
        }

    }
}
