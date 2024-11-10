using BookShop.Common.Models.Models;
using BookShop.Common.Models.Postgress.Models;
using BookShop.MVC.Controllers;
using BookShop.MVC.Data;
using BookShop.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Text;
using static System.Console;

namespace WebApplication2.Controllers
{
    [Authorize(Roles = "God,Admin")]
    public class AdministrationController : Controller
    {


        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ShopUser> userManager;
        private readonly ILogger<AdministrationController> _logger;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ApplicationDbContext _applicationDbContext;

        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<ShopUser> userManager, IHttpClientFactory clientFactory, ILogger<AdministrationController> logger, ApplicationDbContext applicationDbContext)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            _clientFactory = clientFactory;
            _logger = logger;
            _applicationDbContext = applicationDbContext;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }


        [Authorize(Roles = "God")]
        public async Task<IActionResult> AssignAdminRoleIndex()
        {
            return View();
        }
        [Authorize(Roles = "God")]
        [HttpPost]
        public async Task<IActionResult> AssignAdminRole(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["Message"] = "Email is required.";
                return RedirectToAction(nameof(AssignAdminRoleIndex));
            }

            // Найти пользователя по электронной почте
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                TempData["Message"] = $"Could not find a user with the email '{email}'.";
                return RedirectToAction(nameof(AssignAdminRoleIndex));
            }

            // Проверка, есть ли роль "Administrators"
            var roleExists = await roleManager.RoleExistsAsync("Admin");
            if (!roleExists)
            {
                TempData["Message"] = "The 'Admin' role does not exist. Please create the role first.";
                return RedirectToAction(nameof(AssignAdminRoleIndex));
            }

            // Проверка, есть ли у пользователя уже роль Admin
            if (await userManager.IsInRoleAsync(user, "Admin"))
            {
                TempData["Message"] = $"User {user.UserName} is already in the Admin role.";
                return RedirectToAction(nameof(AssignAdminRoleIndex));
            }

            // Добавление пользователя к роли Admin
            var result = await userManager.AddToRoleAsync(user, "Admin");
            if (result.Succeeded)
            {
                TempData["Message"] = $"User {user.UserName} has been successfully assigned the Admin role.";
                return RedirectToAction(nameof(AssignAdminRoleIndex));
            }

            // Обработка ошибок при добавлении роли
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            // Оставляем пользователя на той же странице с ошибками
            return View("AssignAdminRoleIndex");
        }



        public async Task<IActionResult> AddBook()
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.ErrorMessage = TempData["ErrorMessage"];
            try
            {
                HttpClient client = _clientFactory.CreateClient(name: "BookShop.WebAPI");
                string requestUriAuthor = "api/author";
                string requestUriGenre = "api/genre";
                string requestUriPublisher = "api/publisher";
                string requestUriBook = "api/books";

                // Отправка первого запроса для получения авторов
                HttpResponseMessage authorResponse = await client.GetAsync(requestUriAuthor);

                // Проверка успешности ответа
                if (authorResponse.IsSuccessStatusCode)
                {
                    ViewData["authors"] = await authorResponse.Content.ReadFromJsonAsync<Author[]>();
                }
                else
                {
                    _logger.LogWarning($"Failed to retrieve authors from the API. Status Code: {authorResponse.StatusCode}");
                    ViewData["authors"] = Enumerable.Empty<Author>().ToArray();
                }

                // Отправка второго запроса для получения жанров
                HttpResponseMessage genreResponse = await client.GetAsync(requestUriGenre);

                // Проверка успешности ответа
                if (genreResponse.IsSuccessStatusCode)
                {
                    ViewData["genres"] = await genreResponse.Content.ReadFromJsonAsync<Genre[]>();
                }
                else
                {
                    _logger.LogWarning($"Failed to retrieve genres from the API. Status Code: {genreResponse.StatusCode}");
                    ViewData["genres"] = Enumerable.Empty<Genre>().ToArray();
                }

                // Отправка третьего запроса для получения жанров
                HttpResponseMessage publisherResponse = await client.GetAsync(requestUriPublisher);

                // Проверка успешности ответа
                if (publisherResponse.IsSuccessStatusCode)
                {
                    ViewData["publishers"] = await publisherResponse.Content.ReadFromJsonAsync<Publisher[]>();
                }
                else
                {
                    _logger.LogWarning($"Failed to retrieve genres from the API. Status Code: {genreResponse.StatusCode}");
                    ViewData["publishers"] = Enumerable.Empty<Genre>().ToArray();
                }

                HttpResponseMessage booksResponse = await client.GetAsync(requestUriBook);

                // Проверка успешности ответа
                if (booksResponse.IsSuccessStatusCode)
                {
                    ViewData["books"] = await booksResponse.Content.ReadFromJsonAsync<Book[]>();
                }
                else
                {
                    _logger.LogWarning($"Failed to retrieve authors from the API. Status Code: {booksResponse.StatusCode}");
                    ViewData["books"] = Enumerable.Empty<Book>().ToArray();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"The web API service is not responding. Exception: {ex.Message}");
                ViewData["authors"] = Enumerable.Empty<Author>().ToArray();
                ViewData["genres"] = Enumerable.Empty<Genre>().ToArray();
                ViewData["publishers"] = Enumerable.Empty<Genre>().ToArray();
                ViewData["books"] = Enumerable.Empty<Book>().ToArray();
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateBook(Book book, int? genreid, int? authorid)
        {

            book.BookId = 1;

            if (!genreid.HasValue)
            {
                book.Bookgenres = null;
            }

            if (!authorid.HasValue)
            {
                book.Authors = null;
                // Обработка случая, когда authorid не указан
            }

            if (!book.PublisherId.HasValue)
            {
                book.PublisherId = null;

                // Обработка случая, когда authorid не указан
            }

            if (ModelState.IsValid)
            {
                var images = new List<ImageLink>();

                if (book.ImageFiles != null && book.ImageFiles.Any())
                {
                    var directoryPath = Path.Combine("wwwroot/img/boksCards");
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    foreach (var file in book.ImageFiles)
                    {

                        if (file.Length > 0)
                        {
                            var filePath = Path.Combine(directoryPath, Path.GetFileName(file.FileName));
                            // Использование FileStream для сохранения файла
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            // Создание объекта ImageLink и добавление его в список
                            var imageLink = new ImageLink
                            {
                                Url = $"{Path.GetFileName(file.FileName)}", // Путь к изображению
                                Format = Path.GetExtension(file.FileName), // Формат изображения
                                Size = (int)file.Length // Размер изображения в байтах
                            };

                            images.Add(imageLink);
                        }
                    }
                }

                HttpClient client = _clientFactory.CreateClient(name: "BookShop.WebAPI");
                string requestUrl = "api/books";
                string genreUrl = "api/bookgenre";
                string authorUrl = "api/bookauthor";

                book.Imagelinks = images;
                // Сериализация объекта Book в JSON
                var bookJson = JsonConvert.SerializeObject(book);

                // Создание HTTP-содержимого для отправки JSON
                var httpContent = new StringContent(bookJson, Encoding.UTF8, "application/json");

                // Отправка POST-запроса
                HttpResponseMessage response = await client.PostAsync(requestUrl, httpContent);


                int bookid = 0;
                // Проверка успешности ответа
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();

                    // Если ожидаемое содержимое - это число, то мы можем преобразовать его в int или другой числовой тип
                    if (int.TryParse(responseBody, out int result))
                    {
                        bookid = result;
                        TempData["SuccessMessage"] += "Book created successfully!";
                    }

                    //return RedirectToAction("AddBook");
                }
                else
                {
                    TempData["ErrorMessage"] += "Error occurred while creating the book.";
                    //return RedirectToAction("AddBook");
                }

                if (bookid > 0)
                {

                    //var author = book.Authors.FirstOrDefault();
                    if (authorid > 0) // Проверяем, что authorid больше нуля
                    {
                        var bookauthor = new
                        {
                            bookauthorId = 0,
                            bookId = bookid,
                            authorId = authorid,
                        };

                        // Сериализация безымянного объекта в JSON
                        var authorJson = JsonConvert.SerializeObject(bookauthor);

                        // Создание HTTP-содержимого для отправки JSON
                        var authorHttpContent = new StringContent(authorJson, Encoding.UTF8, "application/json");

                        // Создание HttpRequestMessage
                        var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7299/api/bookauthor")
                        {
                            Content = authorHttpContent
                        };

                        // Установка заголовка accept
                        request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/plain"));

                        // Отправка POST-запроса с использованием HttpClient
                        HttpResponseMessage authorResponse = await client.SendAsync(request);

                        // Проверка успешности ответа
                        if (authorResponse.IsSuccessStatusCode)
                        {
                            TempData["SuccessMessage"] += "Author set successfully!";
                            // Верните на нужную страницу или выполните другие действия
                        }
                        else
                        {
                            var errorContent = await authorResponse.Content.ReadAsStringAsync();
                            TempData["ErrorMessage"] += $"Error occurred while creating the author: {errorContent}";
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] += "Author wasn't set";
                    }


                    //var genre = book.Bookgenres.FirstOrDefault();
                    if (genreid > 0) // Проверяем, что genre не равен null
                    {
                        var genreObj = new
                        {
                            bookId = bookid,
                            genreId = genreid // Используем genreId только если genre не равен null
                        };

                        // Сериализация безымянного объекта в JSON
                        var genreJson = JsonConvert.SerializeObject(genreObj);

                        // Создание HTTP-содержимого для отправки JSON
                        var genreHttpContent = new StringContent(genreJson, Encoding.UTF8, "application/json");

                        // Отправка POST-запроса
                        HttpResponseMessage genreResponse = await client.PostAsync(genreUrl, genreHttpContent);

                        // Проверка успешности ответа
                        if (genreResponse.IsSuccessStatusCode)
                        {
                            TempData["SuccessMessage"] += "Genre set successfully!";
                            // Возврат на страницу AddBook или другую
                        }
                        else
                        {
                            var errorContent = await genreResponse.Content.ReadAsStringAsync();
                            TempData["ErrorMessage"] += $"Error occurred while creating the genre: {errorContent}";
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] += "Genre wasnt set";
                    }


                }





            }
            else
            {
                TempData["ErrorMessage"] += "Fail";
            }

            // Если модель не валидна, вернитесь к форме с сообщениями об ошибках
            return RedirectToAction("AddBook");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteBook(int bookid)
        {
            if (bookid > 0)
            {
                HttpClient client = _clientFactory.CreateClient(name: "BookShop.WebAPI");
                string bookUrl = $"api/books?bookid={bookid}";

                // Отправка POST-запроса
                HttpResponseMessage response = await client.DeleteAsync(bookUrl);

                // Проверка успешности ответа
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] += "Book deleted";
                    // Возврат на страницу AddBook или другую
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] += $"Error occurred while deleting the book: {errorContent}";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Couldnt delete the book";
            }
            return RedirectToAction("AddBook");
        }

        public async Task<IActionResult> UpdateBookView(int bookid)
        {
            if (bookid > 0)
            {
                HttpClient client = _clientFactory.CreateClient(name: "BookShop.WebAPI");
                string bookUrl = $"api/books/bybookid?bookid={bookid}";

                HttpResponseMessage response = await client.GetAsync(bookUrl);

                // Проверка успешности ответа
                if (response.IsSuccessStatusCode)
                {
                    var book = await response.Content.ReadFromJsonAsync<Book>();
                    return View(book);
                    // Возврат на страницу AddBook или другую
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] += $"Error updating while deleting the book: {errorContent}";
                }
            }
            return RedirectToAction("AddBook");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBook(int id, Book book)
        {
            if (id > 0)
            {
                HttpClient client = _clientFactory.CreateClient(name: "BookShop.WebAPI");

                string checkBookUrl = $"api/books/bybookid?bookid={id}";
                var existingBook = await client.GetAsync(checkBookUrl);
                if (existingBook == null)
                {
                    return View();
                }


                string bookUrl = $"api/books?id={id}";
                // Сериализация объекта Book в JSON
                var bookJson = JsonConvert.SerializeObject(book);

                // Создание HTTP-содержимого для отправки JSON
                var httpContent = new StringContent(bookJson, Encoding.UTF8, "application/json");

                // Отправка POST-запроса
                HttpResponseMessage response = await client.PutAsync(bookUrl, httpContent);

                // Проверка успешности ответа
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] += "Book changed";
                    // Возврат на страницу AddBook или другую
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] += $"Error occurred while changing the book: {errorContent}";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Couldnt change the book";
            }
            return RedirectToAction("AddBook");
        }



        public async Task<IActionResult> AddAuthor()
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.ErrorMessage = TempData["ErrorMessage"];
            try
            {
                HttpClient client = _clientFactory.CreateClient(name: "BookShop.WebAPI");
                string requestUriAuthor = "api/author";

                // Отправка первого запроса для получения авторов
                HttpResponseMessage authorResponse = await client.GetAsync(requestUriAuthor);

                // Проверка успешности ответа
                if (authorResponse.IsSuccessStatusCode)
                {
                    ViewData["authors"] = await authorResponse.Content.ReadFromJsonAsync<Author[]>();
                }
                else
                {
                    _logger.LogWarning($"Failed to retrieve authors from the API. Status Code: {authorResponse.StatusCode}");
                    ViewData["authors"] = Enumerable.Empty<Author>().ToArray();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"The web API service is not responding. Exception: {ex.Message}");
                ViewData["authors"] = Enumerable.Empty<Author>().ToArray();
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateAuthor(Author author)
        {
            if (ModelState.IsValid)
            {

                HttpClient client = _clientFactory.CreateClient(name: "BookShop.WebAPI");
                string authorUrl = "api/author";

                // Сериализация объекта Book в JSON
                var authorJson = JsonConvert.SerializeObject(author);

                // Создание HTTP-содержимого для отправки JSON
                var httpContent = new StringContent(authorJson, Encoding.UTF8, "application/json");

                // Отправка POST-запроса
                HttpResponseMessage response = await client.PostAsync(authorUrl, httpContent);

                // Проверка успешности ответа
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] += "Author created";
                    // Возврат на страницу AddBook или другую
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] += $"Error occurred while creating author: {errorContent}";
                }

            }
            else
            {
                TempData["ErrorMessage"] += "Fail";
            }
            return RedirectToAction("AddAuthor");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAuthor(int authorid)
        {
            if (authorid > 0)
            {
                HttpClient client = _clientFactory.CreateClient(name: "BookShop.WebAPI");
                string authorUrl = $"api/author?authorid={authorid}";



                // Отправка POST-запроса
                HttpResponseMessage response = await client.DeleteAsync(authorUrl);

                // Проверка успешности ответа
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] += "Author deleted";
                    // Возврат на страницу AddBook или другую
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] += $"Error occurred while deleting the author: {errorContent}";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Couldn delete author";
            }
            return RedirectToAction("AddAuthor");
        }




        public async Task<IActionResult> AddGenre()
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.ErrorMessage = TempData["ErrorMessage"];
            try
            {
                HttpClient client = _clientFactory.CreateClient(name: "BookShop.WebAPI");
                string requestUriGenre = "api/genre";

                // Отправка первого запроса для получения авторов
                HttpResponseMessage genreResponse = await client.GetAsync(requestUriGenre);

                // Проверка успешности ответа
                if (genreResponse.IsSuccessStatusCode)
                {
                    //ViewData["genres"] = await genreResponse.Content.ReadFromJsonAsync<Genre[]>();

                    var gg = await genreResponse.Content.ReadFromJsonAsync<Genre[]>();
                    ViewData["genres"] = gg;

                }
                else
                {
                    _logger.LogWarning($"Failed to retrieve genres from the API. Status Code: {genreResponse.StatusCode}");
                    ViewData["genres"] = Enumerable.Empty<Genre>().ToArray();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"The web API service is not responding. Exception: {ex.Message}");
                ViewData["genres"] = Enumerable.Empty<Genre>().ToArray();
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateGenre(Genre author)
        {
            if (ModelState.IsValid)
            {

                HttpClient client = _clientFactory.CreateClient(name: "BookShop.WebAPI");
                string genreUrl = "api/genre";

                // Сериализация объекта Book в JSON
                var genreJson = JsonConvert.SerializeObject(author);

                // Создание HTTP-содержимого для отправки JSON
                var httpContent = new StringContent(genreJson, Encoding.UTF8, "application/json");

                // Отправка POST-запроса
                HttpResponseMessage response = await client.PostAsync(genreUrl, httpContent);

                // Проверка успешности ответа
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] += "Genre was created";
                    // Возврат на страницу AddBook или другую
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] += $"Error occurred while creating genre: {errorContent}";
                }

            }
            else
            {
                TempData["ErrorMessage"] += "Fail";
            }
            return RedirectToAction("AddGenre");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteGenre(int genreid)
        {
            if (genreid > 0)
            {
                HttpClient client = _clientFactory.CreateClient(name: "BookShop.WebAPI");
                string genreUrl = $"api/genre?genreid={genreid}";



                // Отправка POST-запроса
                HttpResponseMessage response = await client.DeleteAsync(genreUrl);

                // Проверка успешности ответа
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] += "Genre was deleted";
                    // Возврат на страницу AddBook или другую
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] += $"Error occurred while deleting Genre: {errorContent}";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Couldnt delete genre";
            }
            return RedirectToAction("AddGenre");
        }



        public async Task<IActionResult> SalesHistory(string sortField = "date", string sortOrder = "desc")
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.ErrorMessage = TempData["ErrorMessage"];

            try
            {
                // Запрос для выборки заказов
                var ordersQuery = _applicationDbContext.OrderInfos
                    .AsQueryable();

                // Применяем сортировку на основе выбранного поля и порядка
                switch (sortField)
                {
                    case "date":
                        ordersQuery = sortOrder == "asc" ? ordersQuery.OrderBy(o => o.OrderDate) : ordersQuery.OrderByDescending(o => o.OrderDate);
                        break;
                    case "price":
                        ordersQuery = sortOrder == "asc" ? ordersQuery.OrderBy(o => o.TotalPrice) : ordersQuery.OrderByDescending(o => o.TotalPrice);
                        break;
                    case "name":
                        ordersQuery = sortOrder == "asc" ? ordersQuery.OrderBy(o => o.FirstName) : ordersQuery.OrderByDescending(o => o.FirstName);
                        break;
                    case "surname":
                        ordersQuery = sortOrder == "asc" ? ordersQuery.OrderBy(o => o.LastName) : ordersQuery.OrderByDescending(o => o.LastName);
                        break;
                    default:
                        ordersQuery = ordersQuery.OrderByDescending(o => o.OrderDate); // По умолчанию сортируем по дате по убыванию
                        break;
                }

                // Получаем список заказов
                var orderHistory = await ordersQuery.ToListAsync();

                // Десериализуем BooksJson в CartItems для каждого заказа
                foreach (var order in orderHistory)
                {
                    order.DeserializeCartItems();
                }

                ViewBag.CurrentSortField = sortField; // Передаем текущий сортируемый параметр
                ViewBag.CurrentSortOrder = sortOrder; // Передаем текущий порядок сортировки

                return View(orderHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при получении истории заказов: {ex.Message}");
                ViewBag.ErrorMessage = "Произошла ошибка при загрузке истории заказов.";
            }

            return View();
        }
    }
}

        //public class RolesController : Controller
        //{
        //    private string AdminRole = "God";
        //    private string UserEmail = "admintest@gmail.com";


        //    private readonly RoleManager<IdentityRole> roleManager;
        //    private readonly UserManager<ShopUser> userManager;

        //    public RolesController(RoleManager<IdentityRole> roleManager, UserManager<ShopUser> userManager)
        //    {
        //        this.roleManager = roleManager;
        //        this.userManager = userManager;
        //    }

        //    public async Task<IActionResult> Index()
        //    {
        //        if (!(await roleManager.RoleExistsAsync(AdminRole)))
        //        {
        //            await roleManager.CreateAsync(new IdentityRole(AdminRole));
        //        }

        //        ShopUser user = await userManager.FindByEmailAsync(UserEmail);

        //        if (user == null)
        //        {
        //            user = new();
        //            user.UserName = UserEmail;
        //            user.Email = UserEmail;
        //            IdentityResult result = await userManager.CreateAsync(user, "Rn_Vbwef$7cZJB7");

        //            if (result.Succeeded)
        //            {
        //                WriteLine($"User {user.UserName} created succesfully");
        //            }
        //            else
        //            {
        //                foreach (IdentityError error in result.Errors)
        //                {
        //                    WriteLine(error.Description);
        //                }
        //            }
        //        }

        //        if (!user.EmailConfirmed)
        //        {
        //            string token = await userManager
        //                .GenerateEmailConfirmationTokenAsync(user);
        //            IdentityResult result = await userManager
        //                .ConfirmEmailAsync(user, token);

        //            if (result.Succeeded)
        //            {
        //                WriteLine($"User {user.UserName} email confirmed succesfully.");
        //            }
        //            else
        //            {
        //                foreach (IdentityError error in result.Errors)
        //                {
        //                    WriteLine(error.Description);
        //                }
        //            }
        //        }

        //        if (!(await userManager.IsInRoleAsync(user, AdminRole)))
        //        {
        //            IdentityResult result = await userManager
        //                .AddToRoleAsync(user, AdminRole);

        //            if (result.Succeeded)
        //            {
        //                WriteLine($"User {user.UserName} added to {AdminRole} succesfully.");
        //            }
        //            else
        //            {
        //                foreach (IdentityError error in result.Errors)
        //                {
        //                    WriteLine(error.Description);
        //                }
        //            }
        //        }

        //        return Redirect("/");
        //    }
        //}
  