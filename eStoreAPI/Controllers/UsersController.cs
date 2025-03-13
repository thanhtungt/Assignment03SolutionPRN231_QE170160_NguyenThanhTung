/*using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess.Contexts;
using BusinessObject.Models;
using AutoMapper;
using Microsoft.Graph.Models;

namespace eStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UsersController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("alluser")]  // Lấy tất cả người dùng
        public async Task<ActionResult<IEnumerable<AspNetUsers>>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            var userDTOs = _mapper.Map<IEnumerable<AspNetUsers>>(users);
            return Ok(userDTOs);
        }

        [HttpPost]
        public async Task<ActionResult<AspNetUsersDTO>> PostUser(AspNetUsersDTO userDTO)
        {
            var user = _mapper.Map<AspNetUsers>(userDTO);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AspNetUsersDTO>> GetUser(string id)
        {
            var user = await _context.Users.Include(u => u.AspNetUserRoles).FirstOrDefaultAsync(u => u.Id == id);
            if(user == null)
            {
                return NotFound();
            }
            var aspNetUsersDTO = _mapper.Map<AspNetUsersDTO>(user);
            return Ok(aspNetUsersDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(string id, AspNetUsersDTO userDTO)
        {
            if (id != userDTO.Id)  // So sánh với Id thay vì UserId
            {
                return BadRequest("id khong hop le");
            }

            var user = await _context.Users.FindAsync(id);
            if(user == null)
            {
                return NotFound();
            }
            _mapper.Map(userDTO, user);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
            }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<AspNetUsersDTO>> DeleteUser(string id)
        {
            var user = await _context.AspNetUsers.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            var userDTO = _mapper.Map<AspNetUsersDTO>(user);
            return NoContent();
        }


    }
}
*/
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess.Contexts;
using BusinessObject.Models;
using AutoMapper;
using System.Security.Claims;

namespace eStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UsersController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("alluser")]
        [Authorize(Roles = "Admin")] // Chỉ admin mới được xem tất cả người dùng
        public async Task<ActionResult<IEnumerable<AspNetUsers>>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            var userDTOs = _mapper.Map<IEnumerable<AspNetUsersDTO>>(users); // Sửa để trả về DTO
            return Ok(userDTOs);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // Chỉ admin mới được thêm người dùng
        public async Task<ActionResult<AspNetUsersDTO>> PostUser(AspNetUsersDTO userDTO)
        {
            var user = _mapper.Map<AspNetUsers>(userDTO);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetUser", new { id = user.Id }, _mapper.Map<AspNetUsersDTO>(user));
        }

        [HttpGet("{id}")]
        [Authorize] // Yêu cầu đăng nhập
        public async Task<ActionResult<AspNetUsersDTO>> GetUser(string id)
        {
            var user = await _context.Users.Include(u => u.AspNetUserRoles)
                                          .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            // Chỉ cho phép người dùng xem thông tin của chính họ hoặc admin
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId != id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var aspNetUsersDTO = _mapper.Map<AspNetUsersDTO>(user);
            return Ok(aspNetUsersDTO);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutUser(string id, AspNetUsersDTO userDTO)
        {
            if (id != userDTO.Id)
            {
                return BadRequest("ID không hợp lệ");
            }

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId != id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Kiểm tra Email/UserName trùng (tùy chọn)
            if (userDTO.Email != user.Email && _context.Users.Any(u => u.Email == userDTO.Email && u.Id != id))
            {
                return BadRequest("Email đã được sử dụng.");
            }

            _mapper.Map(userDTO, user);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Chỉ admin mới được xóa
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}