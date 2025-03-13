using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess.Contexts;
using BusinessObject.Models;
using AutoMapper;

namespace eStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CategoriesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {
            var categories = await _context.Categories.Include(c => c.Products).ToListAsync();
            var categoryDTOs = _mapper.Map<IEnumerable<CategoryDTO>>(categories);
            return Ok(categoryDTOs);
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDTO>> PostCategory(CategoryDTO categoryDTO)
        {
            // Ánh xạ CategoryDTO thành thực thể Category
            var category = _mapper.Map<Category>(categoryDTO);

            // Không cần phải gán giá trị cho CategoryId, vì nó là identity column
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = category.CategoryId }, categoryDTO);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDTO>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            var categoryDTO = _mapper.Map<CategoryDTO>(category);
            return Ok(categoryDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, CategoryDTO categoryDTO)
        {
            if (id != categoryDTO.CategoryId)
            {
                return BadRequest("ID trong URL không khớp với ID trong dữ liệu.");
            }

            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound($"Danh mục với ID {id} không tồn tại.");
            }

            _mapper.Map(categoryDTO, category);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound($"Danh mục với ID {id} không tồn tại.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // Trả về 204 No Content sau khi cập nhật thành công
        }


        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CategoryDTO>> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            var categoryDTO = _mapper.Map<CategoryDTO>(category);
            return Ok(categoryDTO);
        }



    }
}
