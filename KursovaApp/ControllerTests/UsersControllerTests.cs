using API;
using API.Controllers;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace KursovaApp.Tests.ControllerTests
{
    public class UsersControllerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserExtensions _userExtensions;
        private readonly IMemoryCache _cache;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _userRepository = A.Fake<IUserRepository>();
            _userExtensions = A.Fake<IUserExtensions>();
            _cache = A.Fake<IMemoryCache>();

            //SUT
            _controller = new UsersController(_userRepository, _userExtensions, _cache);
        }

        [Fact]
        public async Task UsersController_GetUsers_ReturnsOK()
        {
            //Arrange

            var users = A.Fake<IEnumerable<MemberDto>>();
            A.CallTo(() => _userRepository.GetMembersAsync(A<string>.That.Matches(email => true))).Returns(users);

            //Act
            var result = await _controller.GetUsers();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<IEnumerable<MemberDto>>>();
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]

        public async Task UsersController_UpdateRole_ReturnsNoContent()
        {
            //Arrange
            var memberUpdateDto = new MemberUpdateDto
            {
                Email = "TestEmail@gmail.com",
                RoleId = 1
            };

            var user = A.Fake<AppUser>();
            A.CallTo(() => _userRepository.GetUserByEmailAsync(memberUpdateDto.Email)).Returns(user);
            A.CallTo(() => _userRepository.SaveAllAsync()).Returns(true);

            //Act
            var result = await _controller.UpdateRole(memberUpdateDto);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact] 
        public async Task UsersController_DeleteUser_ReturnsOk()
        {
            //Arrange
            string email = "TestEmail@gmail.com";
            var user = A.Fake<AppUser>();
            A.CallTo(() => _userRepository.GetUserByEmailAsync(email)).Returns(user);
            A.CallTo(() => _userRepository.DeleteUser(user));
            A.CallTo(() => _userRepository.SaveAllAsync()).Returns(true);

            //Act
            var result = await _controller.DeleteUser(email);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task UsersController_UpdateRole_Returns400Status()
        {
            //Arrange
            var memberUpdateDto = new MemberUpdateDto
            {
                Email = "TestEmail@gmail.com",
                RoleId = 4
            };

            //Act
            var result = await _controller.UpdateRole(memberUpdateDto);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task UsersController_DeleteUser_Returns404Status()
        {
            //Arrange
            string email = "TestEmail@gmail.com";
            A.CallTo(() => _userRepository.GetUserByEmailAsync(email)).Returns((AppUser)null);
            //Act
            var result = await _controller.DeleteUser(email);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
