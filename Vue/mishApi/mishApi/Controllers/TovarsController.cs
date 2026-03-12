using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mishApi.Models;

namespace mishApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TovarsController : ControllerBase
    {
        private readonly StorePinkBearDbContext _context;

        public TovarsController(StorePinkBearDbContext context)
        {
            _context = context;
        }

        // GET: api/Tovars/GetAll
        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<Tovar>>> GetAll(
        [FromQuery] string? sortBy = "default",
        [FromQuery] string? search = null,
        [FromQuery] int? categoryId = null)
        {
            try
            {
                var goods = _context.Tovars
                    .Include(t => t.TovarCategoryNavigation)
                    .Include(t => t.TovarManufacturNavigation)
                    .AsNoTracking()
                    .AsQueryable();

                if (categoryId.HasValue && categoryId > 0)
                {
                    goods = goods.Where(t => t.TovarCategory == categoryId.Value);
                }

                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.Trim().ToLower();
                    goods = goods.Where(t =>
                        t.TovarTitle.ToLower().Contains(search) ||
                        t.TovarDescription.ToLower().Contains(search) ||
                        (t.TovarCategoryNavigation != null && t.TovarCategoryNavigation.CategoryTitle.ToLower().Contains(search)) ||
                        (t.TovarManufacturNavigation != null && t.TovarManufacturNavigation.ManufacturTitle.ToLower().Contains(search))
                    );
                }

                goods = sortBy?.ToLower() switch
                {
                    "price_asc" => goods.OrderBy(t => t.TovarCost),
                    "price_desc" => goods.OrderByDescending(t => t.TovarCost),
                    "name_asc" => goods.OrderBy(t => t.TovarTitle),
                    "count_asc" => goods.OrderBy(t => t.TovarCount),
                    _ => goods.OrderByDescending(t => t.TovarId)
                };

                var products = await goods.ToListAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка: {ex.Message}");
            }
        }

        // GET: api/Tovars/GetSuppliers
        [HttpGet("GetSuppliers")]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetSuppliers()
        {
            return Ok(await _context.Suppliers.ToListAsync());
        }

        // GET: api/Tovars/GetManufacturers
        [HttpGet("GetManufacturers")]
        public async Task<ActionResult<IEnumerable<Manufactur>>> GetManufacturers()
        {
            return Ok(await _context.Manufacturs.ToListAsync());
        }

        // GET: api/Tovars/GetCategories
        [HttpGet("GetCategories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return Ok(await _context.Categories.ToListAsync());
        }

        // GET: api/Tovars/GetTovarId/{id}
        [HttpGet("GetTovarId/{id}")]
        public async Task<ActionResult<Tovar>> GetTovarId(int id)
        {
            var product = await _context.Tovars
                .Include(t => t.TovarManufacturNavigation)
                .Include(t => t.TovarSupplierNavigation)
                .Include(t => t.TovarCategoryNavigation)
                .FirstOrDefaultAsync(x => x.TovarId == id);

            return product == null
                ? NotFound("Товар не найден.")
                : Ok(product);
        }

        // GET: api/Tovars/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tovar>> GetTovar(int id)
        {
            var tovar = await _context.Tovars.FindAsync(id);

            if (tovar == null) return NotFound(); 

            return tovar;
        }

        // PUT: api/Tovars/
        [HttpPut]
        public async Task<IActionResult> PutTovar([FromBody] Tovar tovar)
        {
            _context.Entry(tovar).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TovarExists(tovar.TovarId))
                    return NotFound();
                else throw; 
            }
            return NoContent();
        }

        // PUT: api/Tovars/RedactTovar
        [HttpPut("RedactTovar")]
        public async Task<ActionResult> RedactTovar([FromBody] Tovar tovar)
        {
            try
            {
                if (tovar == null)
                    return BadRequest("Данные товара не предоставлены.");

                var existingGood = await _context.Tovars.FindAsync(tovar.TovarId);

                if (existingGood == null)
                    return NotFound($"Товар с ID {tovar.TovarId} не найден.");

                if (string.IsNullOrWhiteSpace(tovar.TovarTitle))
                    return BadRequest("Название товара является обязательным полем.");

                if (tovar.TovarTitle.Length > 45)
                    return BadRequest("Название товара не может превышать 45 символов.");

                if (tovar.TovarCost <= 0)
                    return BadRequest("Цена товара должна быть положительным числом.");

                if (tovar.TovarCount <= 0)
                    return BadRequest("Количество должно быть положительным числом.");

                if (string.IsNullOrWhiteSpace(tovar.TovarDescription))
                    return BadRequest("Описание товара является обязательным полем.");

                if (string.IsNullOrWhiteSpace(tovar.TovarSupplier.ToString()))
                    return BadRequest("Поставщик товара является обязательным полем.");

                if (string.IsNullOrWhiteSpace(tovar.TovarManufactur.ToString()))
                    return BadRequest("Производитель товара является обязательным полем.");

                if (string.IsNullOrWhiteSpace(tovar.TovarCategory.ToString()))
                    return BadRequest("Категория товара является обязательным полем.");

                if (tovar.TovarDescription.Length > 65)
                    return BadRequest("Описание товара не может превышать 65 символов.");

                existingGood.TovarTitle = tovar.TovarTitle.Trim();
                existingGood.TovarCost = tovar.TovarCost;
                existingGood.TovarSupplier = tovar.TovarSupplier;
                existingGood.TovarManufactur = tovar.TovarManufactur;
                existingGood.TovarCategory = tovar.TovarCategory;
                existingGood.TovarDescription = tovar.TovarDescription.Trim();
                existingGood.TovarCount = tovar.TovarCount;

                if (!string.IsNullOrEmpty(tovar.TovarPhoto))
                {
                    if (!string.IsNullOrEmpty(existingGood.TovarPhoto) &&
                        existingGood.TovarPhoto != tovar.TovarPhoto)
                    {
                        DeleteOldImage(existingGood.TovarPhoto);
                    }
                    existingGood.TovarPhoto = tovar.TovarPhoto;
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Товар успешно обновлен!",
                    id = existingGood.TovarId,
                    name = existingGood.TovarTitle
                });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"Ошибка базы данных: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        // POST: api/Tovars
        [HttpPost]
        public async Task<ActionResult<Tovar>> PostTovar(Tovar tovar)
        {
            _context.Tovars.Add(tovar);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTovar", new { id = tovar.TovarId }, tovar);
        }

        // POST: api/Tovars/AddTovar
        [HttpPost("AddTovar")]
        public async Task<ActionResult> AddTovar([FromBody] Tovar tovar)
        {
            try
            {
                if (tovar == null)
                    return BadRequest("Данные товара не предоставлены.");

                if (string.IsNullOrWhiteSpace(tovar.TovarTitle))
                    return BadRequest("Название товара является обязательным полем.");

                if (tovar.TovarTitle.Length > 45)
                    return BadRequest("Название товара не может превышать 45 символов.");

                if (tovar.TovarCost <= 0)
                    return BadRequest("Цена товара должна быть положительным числом.");

                if (tovar.TovarCount <= 0)
                    return BadRequest("Количество товара должно быть положительным числом.");

                if (string.IsNullOrWhiteSpace(tovar.TovarDescription))
                    return BadRequest("Описание товара является обязательным полем.");

                if (string.IsNullOrWhiteSpace(tovar.TovarSupplier.ToString()))
                    return BadRequest("Поставщик товара является обязательным полем.");

                if (string.IsNullOrWhiteSpace(tovar.TovarManufactur.ToString()))
                    return BadRequest("Производитель товара является обязательным полем.");

                if (string.IsNullOrWhiteSpace(tovar.TovarCategory.ToString()))
                    return BadRequest("Категория товара является обязательным полем.");

                if (tovar.TovarDescription.Length > 65)
                    return BadRequest("Описание товара не может превышать 65 символов.");

                var newTovar = new Tovar
                {
                    TovarTitle = tovar.TovarTitle.Trim(),
                    TovarCost = tovar.TovarCost,
                    TovarSupplier = tovar.TovarSupplier,
                    TovarManufactur = tovar.TovarManufactur,
                    TovarCategory = tovar.TovarCategory,
                    TovarDescription = tovar.TovarDescription.Trim(),
                    TovarPhoto = tovar.TovarPhoto,
                    TovarCount = tovar.TovarCount
                };

                _context.Tovars.Add(newTovar);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Товар успешно добавлен!",
                    id = newTovar.TovarId,
                    name = newTovar.TovarTitle
                });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"Ошибка базы данных: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        // DELETE: api/Tovars/5
        [HttpDelete("DeleteTovar/{id}")]
        public async Task<IActionResult> DeleteTovar(int id)
        {
            var tovar = await _context.Tovars.FindAsync(id);
            if (tovar == null)  return NotFound("Товар не найден."); 

            var isInOrder = await _context.Items.AnyAsync(i => i.ItemId == id);

            if (isInOrder)
                return BadRequest($"Товар '{tovar.TovarTitle}' присутствует в заказах и не может быть удален.");

            if (!string.IsNullOrEmpty(tovar.TovarPhoto))
            {
                DeleteOldImage(tovar.TovarPhoto);
            }

            _context.Tovars.Remove(tovar);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Товар успешно удален!",
                id = id,
                name = tovar.TovarTitle
            });
        }

        // POST: api/Tovars/UploadImage
        [HttpPost("UploadImage")]
        public async Task<ActionResult> UploadImage(IFormFile image)
        {
            try
            {
                if (image == null || image.Length == 0)
                    return BadRequest("Файл не выбран.");

                const int imageMaxSize = 5 * 1024 * 1024;
                if (image.Length > imageMaxSize)
                    return BadRequest($"Файл слишком большой.");

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(image.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                    return BadRequest("Недопустимый формат файла. Разрешены: JPG, PNG, GIF, WebP.");

                var fileName = $"product_{Guid.NewGuid():N}{fileExtension}";

                var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                if (!Directory.Exists(webRootPath)) 
                    Directory.CreateDirectory(webRootPath);

                var filePath = Path.Combine(webRootPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                return Ok(new
                {
                    success = true,
                    message = "Изображение успешно загружено",
                    fileName = fileName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при загрузке изображения: {ex.Message}");
            }
        }

        // GET: api/Tovars/GetBasketItems
        [HttpGet("GetBasketItems")]
        public async Task<ActionResult> GetBasketItems(int userId)
        {
            var basketItems = await _context.Baskets
                .Where(b => b.UserId == userId)
                .Include(b => b.Tovar)
                    .ThenInclude(t => t.TovarManufacturNavigation)
                .Include(b => b.Tovar)
                    .ThenInclude(t => t.TovarSupplierNavigation)
                .Include(b => b.Tovar)
                    .ThenInclude(t => t.TovarCategoryNavigation)
                .Select(b => new
                {
                    b.UserId,
                    b.TovarId,
                    Quantity = b.BasketCount,
                    Product = b.Tovar,
                    MaxQuantity = b.Tovar.TovarCount
                })
                .ToListAsync();

            return Ok(basketItems);
        }

        //POST: api/Tovars/AddToBasket
        [HttpPost("AddToBasket")]
        public async Task<ActionResult> AddToBasket(int userId, int tovarId, int quantity)
        {
            try
            {
                var product = await _context.Tovars
                    .FirstOrDefaultAsync(t => t.TovarId == tovarId);

                if (product == null)
                    return NotFound("Товар не найден.");

                if (product.TovarCount <= 0)
                    return BadRequest("Товар отсутствует на складе.");

                var existingItem = await _context.Baskets
                    .FirstOrDefaultAsync(o => o.UserId == userId &&
                                            o.TovarId == tovarId );

                if (existingItem != null)
                {
                    int newQuantity = (int)existingItem.BasketCount + quantity;
                    if (newQuantity > product.TovarCount)
                        return BadRequest($"Нельзя добавить больше {product.TovarCount} шт. (доступно на складе)");

                    existingItem.BasketCount = newQuantity;
                }
                else
                {
                    if (quantity > product.TovarCount)
                        return BadRequest($"Нельзя добавить больше {product.TovarCount} шт. (доступно на складе)");

                    var basketItem = new Basket
                    {
                        UserId = userId,
                        TovarId = tovarId,
                        BasketCount = quantity,
                    };
                    _context.Baskets.Add(basketItem);
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Товар добавлен в корзину",
                    success = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        // PUT: api/Tovars/UpdateBasketQuantity
        [HttpPut("UpdateBasketQuantity")]
        public async Task<ActionResult> UpdateBasketQuantity(int UserId, int TovarId, int quantity)
        {
            try
            {
                var basketItem = await _context.Baskets
                    .FirstOrDefaultAsync(o => o.UserId == UserId);

                if (basketItem == null)
                    return NotFound("Элемент корзины не найден.");

                var product = await _context.Tovars
                    .FirstOrDefaultAsync(t => t.TovarId == basketItem.TovarId);

                if (product == null)
                    return NotFound("Товар не найден.");

                if (quantity > product.TovarCount)
                {
                    return BadRequest($"Нельзя установить количество больше {product.TovarCount} шт. (доступно на складе)");
                }

                if (quantity <= 0)
                    _context.Baskets.Remove(basketItem);
                else
                    basketItem.BasketCount = quantity;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Количество обновлено",
                    success = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        // DELETE: api/Tovars/RemoveFromBasket
        [HttpDelete("RemoveFromBasket")]
        public async Task<ActionResult> RemoveFromBasket(int UserId, int TovarId)
        {
            try
            {
                var basketItem = await _context.Baskets
                    .FirstOrDefaultAsync(o => o.UserId == UserId
                                        && o.TovarId == TovarId);

                if (basketItem == null)
                    return NotFound("Элемент корзины не найден.");

                _context.Baskets.Remove(basketItem);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Товар удален из корзины",
                    success = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        // DELETE: api/Tovars/ClearBasket/{userId}
        [HttpDelete("ClearBasket/{userId}")]
        public async Task<ActionResult> ClearBasket(int userId)
        {
            try
            {
                var basketItems = await _context.Baskets
                    .Where(o => o.UserId == userId)
                    .ToListAsync();

                if (basketItems.Any())
                {
                    _context.Baskets.RemoveRange(basketItems);
                    await _context.SaveChangesAsync();
                }

                return Ok(new
                {
                    message = "Корзина очищена",
                    success = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        // POST: api/Tovars/CreateOrder
        [HttpPost("CreateOrder")]
        public async Task<ActionResult> CreateOrder([FromBody] OrderRequest request)
        {
            try
            {
                var userId = request.UserId;
                var pickUpPointId = request.PickUpPointId;
                var items = request.Items;

                if (items == null || !items.Any())
                    return BadRequest("Нет товаров для оформления заказа");

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return NotFound("Пользователь не найден");

                var pickUpPoint = await _context.Points.FindAsync(pickUpPointId);
                if (pickUpPoint == null)
                    return NotFound("Пункт выдачи не найден");

                foreach (var item in items)
                {
                    var product = await _context.Tovars.FindAsync(item.TovarId);
                    if (product == null)
                        return BadRequest($"Товар с ID {item.TovarId} не найден");

                    if (product.TovarCount < item.Quantity)
                        return BadRequest($"Недостаточно товара {product.TovarTitle} на складе. Доступно: {product.TovarCount}");
                }

                Random random = new Random();
                int orderCode;
                do
                {
                    orderCode = random.Next(100, 1000);
                } while (await _context.Orders.AnyAsync(o => o.OrderCode == orderCode));

                var order = new Order
                {
                    OrderDate = DateOnly.FromDateTime(DateTime.Now),
                    OrderDateDelivery = DateOnly.FromDateTime(DateTime.Now.AddDays(14)),
                    OrderPoint = pickUpPointId,
                    OrderUser = userId,
                    OrderCode = orderCode,
                    OrderStatus = "Новый"
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var item in items)
                {
                    var product = await _context.Tovars.FindAsync(item.TovarId);

                    var orderComposition = new Item
                    {
                        OrderId = order.OrderId,
                        ItemId = item.TovarId,
                        ItemCount = item.Quantity
                    };

                    _context.Items.Add(orderComposition);

                    product.TovarCount -= item.Quantity;
                }

                var basketItems = await _context.Baskets
                    .Where(o => o.UserId == userId)
                    .ToListAsync();
                _context.Baskets.RemoveRange(basketItems);

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Заказ успешно оформлен",
                    orderId = order.OrderId,
                    orderCode = order.OrderCode,
                    success = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при оформлении заказа: {ex.Message}");
            }
        }

        public class OrderRequest
        {
            public int UserId { get; set; }
            public int PickUpPointId { get; set; }
            public List<OrderItem> Items { get; set; }
        }

        // GET: api/Tovars/GetPickUpPoints
        [HttpGet("GetPickUpPoints")]
        public async Task<ActionResult> GetPickUpPoints()
        {
            try
            {
                var points = await _context.Points
                    .Select(p => new
                    {
                        p.PointId,
                        FullAddress = $"{p.PointIndex}, {p.PointCity}, ул.{p.PointStreet}, д.{p.PointHouse}"
                    })
                    .ToListAsync();

                return Ok(points);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при загрузке пунктов выдачи: {ex.Message}");
            }
        }

        // GET: api/Tovars/GetUserOrders
        [HttpGet("GetUserOrders")]
        public async Task<ActionResult> GetUserOrders(int userId)
        {
            try
            {
                var orders = await _context.Orders
                    .Where(o => o.OrderUser == userId)
                    .Include(o => o.Items)
                    .Include(o => o.OrderUserNavigation)
                    .Include(o => o.OrderPointNavigation)
                    .OrderBy(o => o.OrderId)
                    .Select(o => new
                    {
                        o.OrderId,
                        OrderDate = o.OrderDate,
                        OrderDateDelivery = o.OrderDateDelivery,
                        o.OrderPoint,
                        PickUpPointAddress = $"{o.OrderPointNavigation.PointIndex}, {o.OrderPointNavigation.PointCity}, ул.{o.OrderPointNavigation.PointStreet}, д.{o.OrderPointNavigation.PointHouse}",
                        o.OrderUser,
                        o.OrderCode,
                        o.OrderStatus,
                        Items = o.Items.Select(i => new
                        {
                            i.OrderId,
                            i.ItemId,
                            i.ItemCount,
                            Product = new
                            {
                                i.ItemNavigation.TovarTitle,
                                i.ItemNavigation.TovarCost,
                                i.ItemNavigation.TovarPhoto,
                                ManufacturerName = i.ItemNavigation.TovarManufacturNavigation != null ? i.ItemNavigation.TovarManufacturNavigation.ManufacturTitle : "",
                                CategoryName = i.ItemNavigation.TovarCategoryNavigation != null ? i.ItemNavigation.TovarCategoryNavigation.CategoryTitle : "",
                                SupplierName = i.ItemNavigation.TovarSupplierNavigation != null ? i.ItemNavigation.TovarSupplierNavigation.SupplierTitle : ""
                            },
                            TotalPrice = (i.ItemNavigation.TovarSale > 0
                                    ? i.ItemNavigation.TovarCost * (1 - (decimal)i.ItemNavigation.TovarSale / 100)
                                    : i.ItemNavigation.TovarCost) * i.ItemCount
                        }).ToList(),
                        TotalSum = o.Items.Sum(i => 
                            (i.ItemNavigation.TovarSale > 0
                            ? i.ItemNavigation.TovarCost * (1 - (decimal)i.ItemNavigation.TovarSale / 100)
                            : i.ItemNavigation.TovarCost) * i.ItemCount)
                    }).ToListAsync();

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при загрузке заказов: {ex.Message}");
            }
        }

        // GET: api/Tovars/GetAllOrders
        [HttpGet("GetAllOrders")]
        public async Task<ActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _context.Orders
                    .Where(o => o.OrderStatus != "В корзине")
                    .Include(o => o.Items)
                    .Include(o => o.OrderPointNavigation)
                    .Include(o => o.OrderUserNavigation)
                    .OrderByDescending(o => o.OrderId)
                    .Select(o => new
                    {
                        o.OrderId,
                        OrderDate = o.OrderDate,
                        OrderDateDelivery = o.OrderDateDelivery,
                        o.OrderPoint,
                        PickUpPointAddress = $"{o.OrderPointNavigation.PointIndex}, {o.OrderPointNavigation.PointCity}, ул.{o.OrderPointNavigation.PointStreet}, д.{o.OrderPointNavigation.PointHouse}",
                        o.OrderUser,
                        UserFull = $"{o.OrderUserNavigation.UserLastname} {o.OrderUserNavigation.UserName} {o.OrderUserNavigation.UserFirstname}",
                        o.OrderCode,
                        o.OrderStatus,
                        Items = o.Items.Select(i => new
                        {
                            i.OrderId,
                            i.ItemId,
                            i.ItemCount,
                            Product = new
                            {
                                i.ItemNavigation.TovarTitle,
                                i.ItemNavigation.TovarCost,
                                i.ItemNavigation.TovarPhoto,
                                ManufacturerName = i.ItemNavigation.TovarManufacturNavigation != null ? i.ItemNavigation.TovarManufacturNavigation.ManufacturTitle : "",
                                CategoryName = i.ItemNavigation.TovarCategoryNavigation != null ? i.ItemNavigation.TovarCategoryNavigation.CategoryTitle : "",
                                SupplierName = i.ItemNavigation.TovarSupplierNavigation != null ? i.ItemNavigation.TovarSupplierNavigation.SupplierTitle : ""
                            },
                            TotalPrice = (i.ItemNavigation.TovarSale > 0
                                    ? i.ItemNavigation.TovarCost * (1 - (decimal)i.ItemNavigation.TovarSale / 100)
                                    : i.ItemNavigation.TovarCost) * i.ItemCount
                        }).ToList(),
                        TotalSum = o.Items.Sum(i =>
                            (i.ItemNavigation.TovarSale > 0
                            ? i.ItemNavigation.TovarCost * (1 - (decimal)i.ItemNavigation.TovarSale / 100)
                            : i.ItemNavigation.TovarCost) * i.ItemCount)
                    })
                    .ToListAsync();

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при загрузке заказов: {ex.Message}");
            }
        }

        // PUT: api/Tovars/UpdateOrderStatus/{orderId}
        [HttpPut("UpdateOrderStatus/{orderId}")]
        public async Task<ActionResult> UpdateOrderStatus(int orderId, string orderStatus)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);

                if (order == null)
                    return NotFound("Заказ не найден");

                if (order.OrderStatus == "Завершен")
                    return BadRequest("Нельзя изменить статус завершенного заказа");

                order.OrderStatus = orderStatus;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Статус заказа успешно обновлен",
                    success = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при обновлении статуса: {ex.Message}");
            }
        }

        private void DeleteOldImage(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return;

            var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            var filePath = Path.Combine(webRootPath, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        private bool TovarExists(int id)
        {
            return _context.Tovars.Any(e => e.TovarId == id);
        }

        public class OrderItem
        {
            public int TovarId { get; set; }
            public int Quantity { get; set; }
        }






        // PUT: api/Tovars/UpdateOrder/{orderId}
        [HttpPut("UpdateOrder/{orderId}")]
        public async Task<ActionResult> UpdateOrder(int orderId, [FromBody] UpdateOrderRequest request)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Items)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null)
                    return NotFound("Заказ не найден");

                if (order.OrderStatus == "Завершен")
                    return BadRequest("Нельзя редактировать завершенный заказ");

                if (order.OrderStatus == "В корзине")
                    return BadRequest("Нельзя редактировать заказ, который еще в корзине");

                if (request.OrderDateDelivery.HasValue)
                    order.OrderDateDelivery = request.OrderDateDelivery.Value;

                if (request.OrderPoint.HasValue)
                {
                    var point = await _context.Points.FindAsync(request.OrderPoint.Value);
                    if (point == null)
                        return NotFound("Пункт выдачи не найден");

                    order.OrderPoint = request.OrderPoint.Value;
                }

                if (!string.IsNullOrWhiteSpace(request.OrderStatus))
                {
                    var validStatuses = new[] { "Новый", "В пути", "Готов к выдаче", "Завершен" };
                    if (!validStatuses.Contains(request.OrderStatus))
                        return BadRequest($"Недопустимый статус. Допустимые значения: {string.Join(", ", validStatuses)}");

                    order.OrderStatus = request.OrderStatus;
                }

                if (request.Items != null && request.Items.Any())
                {
                    foreach (var item in request.Items)
                    {
                        var product = await _context.Tovars.FindAsync(item.TovarId);
                        if (product == null)
                            return BadRequest($"Товар с ID {item.TovarId} не найден");

                        if (product.TovarCount < item.Quantity)
                            return BadRequest($"Недостаточно товара '{product.TovarTitle}' на складе. Доступно: {product.TovarCount}");
                    }

                    foreach (var oldItem in order.Items)
                    {
                        var oldProduct = await _context.Tovars.FindAsync(oldItem.ItemId);
                        if (oldProduct != null)
                        {
                            oldProduct.TovarCount += oldItem.ItemCount;
                        }
                    }

                    _context.Items.RemoveRange(order.Items);

                    foreach (var item in request.Items)
                    {
                        var product = await _context.Tovars.FindAsync(item.TovarId);

                        product.TovarCount -= item.Quantity;

                        _context.Items.Add(new Item
                        {
                            OrderId = orderId,
                            ItemId = item.TovarId,
                            ItemCount = item.Quantity
                        });
                    }
                }

                await _context.SaveChangesAsync();

                var updatedOrder = await _context.Orders
                    .Where(o => o.OrderId == orderId)
                    .Include(o => o.Items)
                        .ThenInclude(i => i.ItemNavigation)
                            .ThenInclude(t => t.TovarManufacturNavigation)
                    .Include(o => o.Items)
                        .ThenInclude(i => i.ItemNavigation)
                            .ThenInclude(t => t.TovarSupplierNavigation)
                    .Include(o => o.Items)
                        .ThenInclude(i => i.ItemNavigation)
                            .ThenInclude(t => t.TovarCategoryNavigation)
                    .Include(o => o.OrderPointNavigation)
                    .Include(o => o.OrderUserNavigation)
                    .Select(o => new
                    {
                        o.OrderId,
                        OrderDate = o.OrderDate,
                        OrderDateDelivery = o.OrderDateDelivery,
                        o.OrderPoint,
                        PickUpPointAddress = $"{o.OrderPointNavigation.PointIndex}, {o.OrderPointNavigation.PointCity}, ул.{o.OrderPointNavigation.PointStreet}, д.{o.OrderPointNavigation.PointHouse}",
                        o.OrderUser,
                        UserFull = $"{o.OrderUserNavigation.UserLastname} {o.OrderUserNavigation.UserName} {o.OrderUserNavigation.UserFirstname}",
                        o.OrderCode,
                        o.OrderStatus,
                        Items = o.Items.Select(i => new
                        {
                            i.OrderId,
                            i.ItemId,
                            i.ItemCount,
                            Product = new
                            {
                                i.ItemNavigation.TovarTitle,
                                i.ItemNavigation.TovarCost,
                                i.ItemNavigation.TovarPhoto,
                                i.ItemNavigation.TovarSale,
                                ManufacturerName = i.ItemNavigation.TovarManufacturNavigation != null ? i.ItemNavigation.TovarManufacturNavigation.ManufacturTitle : "",
                                CategoryName = i.ItemNavigation.TovarCategoryNavigation != null ? i.ItemNavigation.TovarCategoryNavigation.CategoryTitle : "",
                                SupplierName = i.ItemNavigation.TovarSupplierNavigation != null ? i.ItemNavigation.TovarSupplierNavigation.SupplierTitle : ""
                            },
                            TotalPrice = (i.ItemNavigation.TovarSale > 0
                                    ? i.ItemNavigation.TovarCost * (1 - (decimal)i.ItemNavigation.TovarSale / 100)
                                    : i.ItemNavigation.TovarCost) * i.ItemCount
                        }).ToList(),
                        TotalSum = o.Items.Sum(i =>
                            (i.ItemNavigation.TovarSale > 0
                            ? i.ItemNavigation.TovarCost * (1 - (decimal)i.ItemNavigation.TovarSale / 100)
                            : i.ItemNavigation.TovarCost) * i.ItemCount)
                    })
                    .FirstOrDefaultAsync();

                return Ok(new
                {
                    message = "Заказ успешно обновлен",
                    success = true,
                    order = updatedOrder
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при обновлении заказа: {ex.Message}");
            }
        }

        public class UpdateOrderRequest
        {
            public DateOnly? OrderDateDelivery { get; set; }
            public int? OrderPoint { get; set; }
            public string? OrderStatus { get; set; }
            public List<OrderItem>? Items { get; set; }
        }


        [HttpDelete("DeleteOrder/{orderId}")]
        public async Task<ActionResult> DeleteOrder(int orderId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Items)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null)
                    return NotFound("Заказ не найден");

                if (order.OrderStatus != "Завершен")
                    return BadRequest("Можно удалять только завершенные заказы");

                _context.Items.RemoveRange(order.Items);
                _context.Orders.Remove(order);

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Заказ успешно удален",
                    success = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при удалении заказа: {ex.Message}");
            }
        }
    }
}