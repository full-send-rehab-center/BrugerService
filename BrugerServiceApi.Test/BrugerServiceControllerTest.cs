using BrugerServiceApi.Controllers;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using BrugerServiceApi.Services;
using BrugerServiceApi.Models;

namespace BrugerServiceApi.Test;

public class MyServiceControllerTests
{
    private ILogger<BrugerServiceController> _logger = null!;
    private IConfiguration _configuration = null!;

    [SetUp]
    public void Setup()
    {
        _logger = new Mock<ILogger<BrugerServiceController>>().Object;

        // _configuration = new ConfigurationBuilder()
        //     .AddInMemoryCollection(myConfiguration)
        //     .Build();
    }

    [Test]
    public async Task TestUser_CreateUser_valid_DTO()
    {
        // Arrange
        // Forbered de nødvendige objekter og data til testen
        var userDTO = CreateUser();
        var stubRepo = new Mock<IUsersService>();
        stubRepo.Setup(svc => svc.CreateAsync(userDTO))
            .Returns(Task.FromResult<User?>(userDTO));
        var controller = new BrugerServiceController(_logger, stubRepo.Object);

        // Act
        // Udfør den handling, du vil teste
        var actionResult = await controller.Post(userDTO);
        var result = actionResult as CreatedAtActionResult;

        // Assert
        // Verificer resultatet af handlingen
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<User>(result.Value);
    }

    [Test]
    public async Task TestUser_GetUser_valid_return()
    {
        var stubRepo = new Mock<IUsersService>();
        var expectedUserId = "jH2h3yDhgeW8li7jH3nUed4J"; // Sample user ID
        var userDTO = CreateUser();
        stubRepo.Setup(service => service.GetAsync(expectedUserId))
                       .ReturnsAsync(userDTO);
        var controller = new BrugerServiceController(_logger, stubRepo.Object);

        // Act
        var result = await controller.Get(expectedUserId);

        // Assert
        var okResult = result as CreatedAtActionResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(nameof(controller.Get), okResult.ActionName);
        Assert.AreEqual(expectedUserId, okResult.RouteValues["id"]);

        var user = okResult.Value as User;
        Assert.IsNotNull(user);
        Assert.AreEqual(expectedUserId, userDTO.userID);
        Assert.AreEqual("Test User", user.username);
    }

    private User CreateUser()
    {
        var userDTO = new User()
        {
            userID = "jH2h3yDhgeW8li7jH3nUed4J",
            username = "Test User",
            password = "Test Password",
            salt = "Test Salt",
            role = "Test role",
            givenName = "Test Name",
            address = "Test Address",
            email = "Test Email",
            telephone = "Test Telephone"
        };
        return userDTO;
    }
}

