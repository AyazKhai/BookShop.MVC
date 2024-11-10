using Azure.Core;
using BookShop.Common.Models.Models;
using BookShop.MVC.Data;
using BookShop.MVC.Extensions;
using BookShop.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using NuGet.Configuration;
using System.Text.Json;

namespace BookShop.MVC.Controllers
{


    public class CartController : Controller
    {
        private readonly UserManager<ShopUser> _userManager;
        private readonly ApplicationDbContext _applicationDbContext;

        public CartController(UserManager<ShopUser> userManager, ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            _applicationDbContext = applicationDbContext;
        }

        //Метод для добавления товара в корзину
        [HttpPost]
        public IActionResult AddToCart([FromBody] CartItem request)
        {
            // Get the current cart from the session
            var cart = HttpContext.Session.GetCart() ?? new List<CartItem>();

            // Check if the item is already in the cart
            var cartItem = cart.FirstOrDefault(item => item.Title == request.Title);
            if (cartItem != null)
            {
                // Increase the quantity if the item is already in the cart
                cartItem.Quantity++;
            }
            else
            {
                // Add a new item to the cart
                cart.Add(new CartItem
                {
                    ProductId = cart.LastOrDefault()?.ProductId + 1 ?? 1,
                    BooktId = request.BooktId,
                    Title = request.Title,
                    AuthorName = request.AuthorName,
                    ImageStr = request.ImageStr,
                    Price = request.Price,
                    Quantity = 1
                });
                Console.WriteLine($"ProductId = {request.ProductId} " +
                    $"Title = {request.Title}  " +
                    $"Price = {request.Price}" +
                    $"AuthorName = {request.AuthorName}");
            }

            // Save the updated cart back to the session
            HttpContext.Session.SetCart(cart);

            // Return a success response
            return Json(new { success = true, cartItemCount = cart.Sum(item => item.Quantity) });
        }


        // Метод для отображения корзины
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetCart();
            return View(cart); // Передаем корзину в представление
        }

        // Метод для удаления товара из корзины
        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = HttpContext.Session.GetCart() ?? new List<CartItem>();
            var item = cart.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                cart.Remove(item);
                HttpContext.Session.SetCart(cart); // Обновляем корзину в сессии
            }

            // Возвращаем пустой ответ для AJAX-запроса
            return Ok();
        }

        // Метод для очистки всей корзины
        public IActionResult ClearCart()
        {
           
            HttpContext.Session.SetCart(new List<CartItem>());
            return Ok();
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int newQuantity)
        {
            var cart = HttpContext.Session.GetCart() ?? new List<CartItem>();
            // Логика для обновления количества товара
            var cartItem = cart.FirstOrDefault(i => i.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity = newQuantity;
                HttpContext.Session.SetCart(cart); // Обновляем корзину в сессии
            }

            // Рассчитываем общую стоимость всех товаров в корзине
            decimal totalAmount = cart.Sum(i => i.Price * i.Quantity);

            // Вернуть обновленную сумму для элемента и общую стоимость
            return Json(new
            {
                total = cartItem.Price * cartItem.Quantity,
                subtotal = totalAmount // Добавляем общую сумму
            });
        }

        [HttpPost]
        public IActionResult UpdateItemOptions(int productId, bool isWrapped, bool isStamped)
        {
            var cart = HttpContext.Session.GetCart() ?? new List<CartItem>();
            var cartItem = cart.FirstOrDefault(i => i.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.isWrapped = isWrapped;
                cartItem.isStamped = isStamped;
                HttpContext.Session.SetCart(cart); // Обновляем корзину в сессии
            }

            return Json(new { success = true });
        }



        [Authorize(Roles = "God,Admin")]
        public async Task<IActionResult> Order()
        {
            // Получаем текущего пользователя
            var user = await _userManager.GetUserAsync(User);

            // Загружаем корзину из сессии
            var cartItems = HttpContext.Session.GetCart() ?? new List<CartItem>();

            // Создаем модель OrderInfo
            var orderInfo = new OrderInfo
            {
                FirstName = user?.FirstName ?? "",
                LastName = user?.LastName ?? "",
                Email = user?.Email ??"",
                PaymentMethod = "", // можно задать по умолчанию
                CartItems = cartItems,
                TotalPrice = cartItems.Sum(item => item.Price * item.Quantity)
            };

            return View(orderInfo);
        }

        
        [Authorize(Roles = "God,Admin")]
        public async Task<IActionResult> SubmitOrder(OrderInfo orderInfo)
        {
            if (ModelState != null)
            {
                try
                {
                    var cart = HttpContext.Session.GetCart() ?? new List<CartItem>();
                    if (cart.Count == 0 )
                    {
                        ViewData["ErrorMessage"] = "Ваша корзина пуста. Пожалуйста, добавьте товары перед оформлением заказа.";
                        return RedirectToAction("Index", "Home");
                    }
                    // Сохранение заказа в базе данных
                    //orderInfo.TotalPrice = 0;
                    //orderInfo.OrderId = 12;
                    // Заполняем CartItems и сериализуем их в BooksJson
                    orderInfo.CartItems = cart;
                    orderInfo.SerializeCartItems();
                    

                    _applicationDbContext.OrderInfos.Add(orderInfo); // Добавляем заказ в таблицу
                    await _applicationDbContext.SaveChangesAsync(); // Сохраняем изменения в базе данных

                    // Очистка корзины в сессии после успешного оформления заказа
                    HttpContext.Session.SetCart(new List<CartItem>());

                    // Отправка сообщения об успешной отправке заказа
                    TempData["SuccessMessage"] = "Ваш заказ успешно оформлен!";



                    // Дополнительная обработка, если необходимо, например, отправка email или уведомлений
                    // await _emailService.SendOrderConfirmationEmail(orderInfo);

                    // Перенаправление на страницу с подтверждением заказа
                    return View();
                }
                catch (Exception ex)
                {
                    // Логирование ошибки
                    ViewData["ErrorMessage"] = "Произошла ошибка при оформлении заказа. Пожалуйста, попробуйте снова.";
                    return View();
                }
            }

            // Если модель невалидна, возвращаем обратно на страницу с ошибками
            return RedirectToAction("Index", "Home");

            //return RedirectToAction("OrderConfirmation");
        }

    }



}
