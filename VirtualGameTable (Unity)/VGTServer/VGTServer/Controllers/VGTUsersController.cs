using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using VGTDataStore.Core;
using System.Security.Cryptography;
using VGTServer.Answers;
using VGTServer.Models;
using VGTDataStore.InMemory;
using Newtonsoft.Json;

namespace VGTServer.Controllers
{
    [Route("/api/users/")]
    [ApiController]
    public class VGTUsersController : ControllerBase
    {
        private IVGTUserDataStore _usersDataStore;

        public VGTUsersController(
            IVGTUserDataStore userDataStore)
        {
            _usersDataStore = userDataStore;
        }

        [HttpGet("count")]
        public IActionResult GetVGTUsersCount()
        {
            if (_usersDataStore.Users != null)
                return Ok(_usersDataStore.Users.Count);

            return NotFound();
        }

        [HttpGet("info")]
        public IActionResult GetVGTUsersInfo()
        {
            if (_usersDataStore.Users != null)
                return Ok(_usersDataStore.Users.Values);

            return NotFound();
        }

        [HttpGet("info/{id}")]
        public IActionResult GetVGTUsersInfo(Guid id)
        {
            if (_usersDataStore.Users != null)
                if (_usersDataStore.Users.FirstOrDefault(x => x.Value.UserId == id).Value != null)
                {
                    var user = _usersDataStore.Users.FirstOrDefault(x => x.Value.UserId == id).Value;

                    return Ok(new InfoAnswerModel(user.Login, user.Chips, user.Email, user.RoleId));
                }

            return NotFound();
        }

        [HttpGet("info/chips/{id}")]
        public IActionResult GetVGTUserInfo(Guid id)
        {
            if (_usersDataStore.Users != null)
            {
                if(_usersDataStore.Users.FirstOrDefault(x => x.Key == id).Value != null)
                    return Ok(_usersDataStore.Users.FirstOrDefault(x => x.Key == id).Value.Chips);

                return NotFound();
            }
            return NotFound();
        }

        [HttpGet("{id}")]
        public IActionResult GetVGTUser(Guid id)
        {
            if (_usersDataStore.Users != null)
                return Ok(_usersDataStore.Users.FirstOrDefault(x => x.Key == id).Value);

            return NotFound();
        }

        [HttpPost("auth/{login}")]
        public IActionResult GetVGTUser(string login, [FromBody] string password)
        {
            if (_usersDataStore.Users.FirstOrDefault(x => x.Value.Login == login && x.Value.Password == password).Value != null)
                return Ok(new Answer(_usersDataStore.Users.FirstOrDefault(x => x.Value.Login == login && x.Value.Password == password).Value.UserId, "All data is correct"));

            return BadRequest(new Answer(Guid.Empty, "Wrong data"));
        }

        [HttpPost("register")]
        public IActionResult RegisterVGTUser([FromBody] VGTUserCreateModel user)
        {
            if (user == null)
                return BadRequest(new Answer(Guid.Empty, "Wrong userdata"));

            foreach (var storedUser in _usersDataStore.Users.Values)
            {
                if (storedUser.Login == user.Login)
                    return BadRequest(new Answer(Guid.Empty, "User with that login already exists"));

                if (storedUser.Email == user.Email)
                    return BadRequest(new Answer(Guid.Empty, "User with that email already exists"));
            }

            var restrictedUser = user.ToVGTUser();
            Guid id = _usersDataStore.AddUser(restrictedUser);

            return Ok(new Answer(id, "Added successfully"));
        }

        [HttpPatch("changeChips/{userId}&{chips}")]
        public IActionResult ChangeChips(Guid userId, int chips)
        {
            _usersDataStore.ChangeUserChips(userId, chips);
            return Ok("Changed");
        }
    }
}
