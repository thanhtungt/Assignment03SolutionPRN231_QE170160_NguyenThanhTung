using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess.Contexts;
using BusinessObject.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;

namespace eStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public OrderDetailsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/OrderDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDetail>>> GetOrderDetails()
        {
            var orderDetails = await _context.OrderDetails.Include(od => od.Product).ToListAsync();
            var orderDetailDTOs = _mapper.Map<IEnumerable<OrderDetail>>(orderDetails);
            return Ok(orderDetailDTOs);
        }

        // GET: api/OrderDetails/5
        /*[HttpGet("{id}")]
        public async Task<ActionResult<OrderDetailDTO>> GetOrderDetail(int id)
        {
            var orderDetail = await _context.OrderDetails.Include(od => od.Product)
                                                         .FirstOrDefaultAsync(od => od.OrderId == id);

            if (orderDetail == null)
            {
                return NotFound();
            }

            var orderDetailDTO = _mapper.Map<OrderDetailDTO>(orderDetail);
            return Ok(orderDetailDTO);
        }*/
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<OrderDetailDTO>>> GetOrderDetailsByOrderId(int id)
        {
            var orderDetails = await _context.OrderDetails
                .Where(od => od.OrderId == id)
                .Include(od => od.Product)
                .ToListAsync();

            if (!orderDetails.Any())
            {
                return NotFound();
            }

            var orderDetailDTOs = _mapper.Map<List<OrderDetailDTO>>(orderDetails);
            return Ok(orderDetailDTOs);
        }

        // POST: api/OrderDetails
        [HttpPost]
        public async Task<ActionResult<OrderDetailDTO>> PostOrderDetail(OrderDetailDTO orderDetailDTO)
        {
            var orderDetail = _mapper.Map<OrderDetail>(orderDetailDTO);
            _context.OrderDetails.Add(orderDetail);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetOrderDetail", new { id = orderDetail.OrderId }, orderDetailDTO);
        }


    }
}
