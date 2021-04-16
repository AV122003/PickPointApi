using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PickPointApi.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace PickPointApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private IOrderRepository orderRepository;
        private IPostomatRepository postomatRepository;
        private IMemoryCache memoryCache;
        private readonly int timeSpanValue = 5;

        public OrderController(IMemoryCache memoryCache, IOrderRepository orderRepository, IPostomatRepository postomatRepository)
        {
            this.memoryCache = memoryCache;
            this.orderRepository = orderRepository;
            this.postomatRepository = postomatRepository;
        }


        [HttpPost("OrderAdd")]
        public async Task<IActionResult> OrderAdd([FromBody] ExternalOrder externalOrder)
        {
            if (externalOrder.Products.Count() > 10)
                return BadRequest();

            if (!PhoneNumberCheck(externalOrder.UserPhone))
                return BadRequest();
            
            Postomat postomat = postomatRepository.PostomatGet(externalOrder.PostomatNumber.ToString());
            if (postomat == null)
                return NotFound();

            if (!postomat.Status)
                return StatusCode(403);

            //дополнение: проверка наличия заказа с таким же номером
            if (orderRepository.OrderGet(externalOrder.Number) != null)
                return StatusCode(403);

            Order order = OrderMapper(externalOrder);
            if (!orderRepository.OrderAdd(order))
                return NotFound();
            else
                memoryCache.Set(order.Number, order, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(timeSpanValue) });

            return Ok(await Task.Run(() =>
            {
                return true;
            })); // 200 OK    
        }

        [HttpPost("OrderChange")]
        public async Task<IActionResult> OrderChange([FromBody] ExternalOrder externalOrder)
        {
            if (externalOrder.Products.Count() > 10)
                return BadRequest();

            if (!PhoneNumberCheck(externalOrder.UserPhone))
                return BadRequest();

            Order order = OrderMapper(externalOrder);
            if (!orderRepository.OrderChange(order))
                return NotFound();
            else
                memoryCache.Set(order.Number, order, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(timeSpanValue) });

            return Ok(await Task.Run(() =>
            {
                return true;
            })); // 200 OK    
        }

        [HttpGet("OrderGet")]
        public async Task<IActionResult> OrderGet(int orderNumber)
        {
            if (!memoryCache.TryGetValue(orderNumber, out Order order))
            {
                order = orderRepository.OrderGet(orderNumber);
                if (order != null)
                {
                    memoryCache.Set(order.Number, order, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(timeSpanValue)));
                }
            }

            if (order == null)
                return NotFound();

            return Ok(await Task.Run(() =>
            {
                return ExternalOrderMapper(order);
            })); // 200 OK    
        }

        [HttpPost("OrderCancel")]
        public async Task<IActionResult> OrderCancel([FromBody] int orderNumber)
        {
            if (!orderRepository.OrderСancel(orderNumber))
                return NotFound();
            else
                memoryCache.Remove(orderNumber);

            return Ok(await Task.Run(() =>
            {
                return true;
            })); // 200 OK    
        }

        private bool PhoneNumberCheck(string phoneMumber)
        {
            Regex rgx = new Regex(@"\+7\d{3}-\d{3}-\d{2}-\d{2}");
            return rgx.IsMatch(phoneMumber);
        }

        private Order OrderMapper(ExternalOrder order)
        {
            return new Order(order.Status)
            {
                Number = order.Number,
                Products = order.Products,
                Cost = order.Cost,
                PostomatNumber = order.PostomatNumber,
                UserPhone = order.UserPhone,
                UserFIO = order.UserFIO
            };
        }
        private ExternalOrder ExternalOrderMapper(Order order)
        {
            return new ExternalOrder()
            {
                Number = order.Number,
                Status = order.Status,
                Products = order.Products,
                Cost = order.Cost,
                PostomatNumber = order.PostomatNumber,
                UserPhone = order.UserPhone,
                UserFIO = order.UserFIO
            };
        }
    }
}
