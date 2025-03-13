using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess.Contexts;
using BusinessObject.Models;
using AutoMapper;

namespace eStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProductsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            var productDTOs = _mapper.Map<IEnumerable<ProductDTO>>(products);
            return Ok(productDTOs);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var product = await _context.Products.Include(p => p.Category)
                                                 .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            var productDTO = _mapper.Map<ProductDTO>(product);
            return Ok(productDTO);
        }

        // POST: api/Products
        [HttpPost]
        public async Task<ActionResult<ProductDTO>> PostProduct(ProductDTO productDTO)
        {
            // Chuyển ProductDTO thành Product thực thể
            var product = _mapper.Map<Product>(productDTO);

            // Không cần phải gán giá trị cho ProductId vì nó là identity column, sẽ tự động tăng
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.ProductId }, productDTO);
        }


        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, ProductDTO productDTO)
        {
            if (id != productDTO.ProductId)
            {
                return BadRequest("Id khong hop le");
            }
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            _mapper.Map(productDTO, product);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ProductDTO>> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            var productDTO = _mapper.Map<ProductDTO>(product);
            return Ok(productDTO);
        }
    }
}
