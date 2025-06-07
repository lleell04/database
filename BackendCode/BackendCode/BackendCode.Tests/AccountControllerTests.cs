using Xunit;
using Microsoft.AspNetCore.Mvc;
using BackendCode.Controllers;
using BackendCode.Data;
using BackendCode.DTOs.LoginModel;
using BackendCode.DTOs.UserInfo;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BackendCode.Tests
{
    public class AccountControllerTests
    {
        private readonly YourDbContext _context;
        private readonly AccountController _controller;
        private readonly Mock<ILogger<AccountController>> _loggerMock;

        public AccountControllerTests()
        {
            // 设置内存数据库
            var options = new DbContextOptionsBuilder<YourDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            _context = new YourDbContext(options);
            _loggerMock = new Mock<ILogger<AccountController>>();
            _controller = new AccountController(_context, _loggerMock.Object);
        }

        [Fact]
        public async Task Login_WithInvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var model = new LoginModel { Username = "", Password = "" };

            // Act
            var result = await _controller.Login(model);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_WithNonExistentUser_ReturnsNotFound()
        {
            // Arrange
            var model = new LoginModel { Username = "nonexistent@test.com", Password = "password123" };

            // Act
            var result = await _controller.Login(model);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Register_WithValidData_ReturnsOk()
        {
            // Arrange
            var model = new RegisterModel
            {
                Email = "test@example.com",
                Password = "Test123!",
                UserName = "TestUser"
            };

            // Act
            var result = _controller.UserRegister(model);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Register_WithExistingEmail_ReturnsBadRequest()
        {
            // Arrange
            var model = new RegisterModel
            {
                Email = "existing@example.com",
                Password = "Test123!",
                UserName = "TestUser"
            };

            // 先注册一个用户
            _controller.UserRegister(model);

            // Act
            var result = _controller.UserRegister(model);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task PasswordReset_WithValidData_ReturnsOk()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                Email = "reset@example.com",
                Password = "OldPass123!",
                UserName = "ResetUser"
            };

            // 先注册用户
            _controller.UserRegister(registerModel);

            var resetModel = new LoginModel
            {
                Username = "reset@example.com",
                Password = "NewPass123!"
            };

            // Act
            var result = _controller.PasswordReset(resetModel);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task CheckRegister_WithNewEmail_ReturnsOk()
        {
            // Arrange
            var email = "new@example.com";

            // Act
            var result = _controller.CheckRegister(email);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task CheckRegister_WithExistingEmail_ReturnsBadRequest()
        {
            // Arrange
            var model = new RegisterModel
            {
                Email = "existing@example.com",
                Password = "Test123!",
                UserName = "TestUser"
            };

            // 先注册用户
            _controller.UserRegister(model);

            // Act
            var result = _controller.CheckRegister(model.Email);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
} 