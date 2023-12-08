using API;
using API.Controllers;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KursovaApp.Tests.ControllerTests
{
    public class AccountControllerTests
    {
        private readonly ITokenService _tokenService;
        private readonly IAccountRepository _accountRepository;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            _accountRepository = A.Fake<IAccountRepository>();
            _tokenService = A.Fake<ITokenService>();

            //SUT
            _controller = new AccountController(_accountRepository, _tokenService);
        }

        [Fact]
        public async Task AccountController_Register_ReturnUserDto()
        {
            //Arrange
            var registerDto = new RegisterDto
            {
                Email = "TestEmail@gmail.com",
                Username = "TestUser",
                Password = "password"
            };
            var user = A.Fake<AppUser>();
            A.CallTo(() => _accountRepository.Register(registerDto)).Returns(user);

            //Act
            var result = await _controller.Register(registerDto);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<UserDto>>();
            A.CallTo(() => _accountRepository.Register(registerDto)).MustHaveHappened();
        }

        [Fact]
        public async Task AccountController_Login_ReturnsUserDto()
        {
            //Arrange
            var loginDto = new LoginDto
            {
                Email = "TestEmail@gmail.com",
                Password = "password"
            };

            //Act
            var result = await _controller.Login(loginDto);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<UserDto>>();
        }

        [Fact]
        public async Task AccountController_Register_Returns400Status()
        {
            //Arrange
            string email = "TestEmail@gmail.com";
   
            var registerDto = new RegisterDto
            {
                Email = "TestEmail@gmail.com",
                Username = "TestUser",
                Password = "password"
            };

            A.CallTo(() => _accountRepository.UserExists(email)).Returns(true);           

            //Act
            var result = await _controller.Register(registerDto);

            //Assert
            result.Should().NotBeNull();
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]

        public async Task AccountController_Login_Returns401StatusInvalidPassword()
        {
            //Arrange
            var loginDto = new LoginDto
            {
                Email = "TestEmail@gmail.com",
                Password = "Password"
            };

            using var hmac = new HMACSHA512();

            A.CallTo(() => _accountRepository.GetUserByEmailAsync(loginDto.Email)).Returns(new AppUser
            {
                Email = loginDto.Email,
                IsEmailVerified = true,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pass")),
                PasswordSalt = hmac.Key
            });

            //Act
            var result = await _controller.Login(loginDto);

            //Assert
            result.Should().NotBeNull();
            result.Result.Should().BeOfType<UnauthorizedObjectResult>();
        }
    }
}
