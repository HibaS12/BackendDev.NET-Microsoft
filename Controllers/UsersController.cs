using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Data;
using UserManagementAPI.Models;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserRepository _repo;

        public UsersController(UserRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult GetAllUsers(int page = 1, int pageSize = 20)
        {
            var users = _repo.GetAll(page, pageSize);
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = _repo.GetById(id);
            if (user == null)
                return NotFound(new { message = $"User with ID {id} not found." });

            return Ok(user);
        }

        [HttpPost("add")]
        public IActionResult CreateUser([FromBody] User user)
        {
            var success = _repo.Add(user);
            if (!success)
                return BadRequest(new { message = "Invalid user data or duplicate email." });

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        [HttpPut("update/{id}")]
        public IActionResult UpdateUser(int id, [FromBody] User user)
        {
            user.Id = id;
            var success = _repo.Update(user);

            if (!success)
                return BadRequest(new { message = $"User with ID {id} not found or invalid data." });

            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteUser(int id)
        {
            var success = _repo.Delete(id);
            if (!success)
                return NotFound(new { message = $"User with ID {id} not found." });

            return NoContent();
        }

        [HttpGet("error")]
        public IActionResult Error()
        {
            throw new Exception("Test exception");
        }
    }
}