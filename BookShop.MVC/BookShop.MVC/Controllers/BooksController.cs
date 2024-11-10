using Azure;
using BookShop.Common.Models.Models;
using BookShop.MVC.Models;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookShop.MVC.Controllers
{
    public class BooksController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<BooksController> _logger;
        public BooksController(ILogger<BooksController> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> Index( int? offset,  string? sortname, string? order, int? genreid)
        {
            try
            {

                ViewData["genreid"] = genreid;

                HttpClient client = _clientFactory.CreateClient(name: "BookShop.WebAPI");
                string requestUri = "api/books/array?limit=160";//&offset=14&sort=price&order=asd&genreid=5
                if (offset != null) 
                {
                    requestUri += $"&offset={offset}";
                }
                if (sortname != null) 
                {
                    requestUri += $"&sort={sortname}";
                }
                if (order != null)
                {
                    requestUri += $"&order={order}";//asc or desc
                }
                if (genreid != null) 
                {
                    requestUri += $"&genreid={genreid}";
                }

                // Sending the GET request to the API
                HttpResponseMessage response = await client.GetAsync(requestUri);

                // Check if the response was successful
                if (response.IsSuccessStatusCode)
                {
                    ViewData["books"] = await response.Content.ReadFromJsonAsync<Book[]>();
                }
                else
                {
                    _logger.LogWarning($"Failed to retrieve books from the API. Status Code: {response.StatusCode}");
                    ViewData["books"] = Enumerable.Empty<Book>().ToArray();
                }


                string requestgenreurl = "api/genre";
                // Sending the GET request to the API
                HttpResponseMessage responsegenre = await client.GetAsync(requestgenreurl);

                // Check if the response was successful
                if (responsegenre.IsSuccessStatusCode)
                {
                    ViewData["genres"] = await responsegenre.Content.ReadFromJsonAsync<Genre[]>();
                }
                else
                {
                    _logger.LogWarning($"Failed to retrieve genres from the API. Status Code: {responsegenre.StatusCode}");
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

        //public async Task<IActionResult> Index(int? id)
        //{
        //    try
        //    {
        //        HttpClient client = _clientFactory.CreateClient(name: "BookShop.WebAPI");
        //        string requestUri = id.HasValue ? $"api/bookgenre?genreid={id}" : "api/bookgenre";

        //        // Sending the GET request to the API
        //        HttpResponseMessage response = await client.GetAsync(requestUri);

        //        // Check if the response was successful
        //        if (response.IsSuccessStatusCode)
        //        {
        //            ViewData["books"] = await response.Content.ReadFromJsonAsync<Book[]>();
        //        }
        //        else
        //        {
        //            _logger.LogWarning($"Failed to retrieve books from the API. Status Code: {response.StatusCode}");
        //            ViewData["books"] = Enumerable.Empty<Book>().ToArray();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogWarning($"The web API service is not responding. Exception: {ex.Message}");
        //        ViewData["books"] = Enumerable.Empty<Book>().ToArray();
        //    }

        //    return View();
        //}


        public async Task<IActionResult> BookPage(int? id)
        {
            string uri;
            if (!id.HasValue)
            {
                return View();
            }
            else
            {
                //https://localhost:7299/api/books/bybookid?bookid=7
                //ViewData["Title"] = $"Customers in {id}";
                uri = $"api/books/bybookid?bookid={id}";

            }

            try
            {

                //HttpRequestMessage request = new(method: HttpMethod.Get, requestUri: uri);
                //HttpResponseMessage response = await client.SendAsync(request);
                HttpClient client = _clientFactory.CreateClient(name: "BookShop.WebAPI");



                HttpResponseMessage response = await client.GetAsync(uri);

                if (!response.IsSuccessStatusCode)
                {
                    ViewData["Title"] = "Exception!";
                    // Логирование ошибки
                    throw new Exception($"Error fetching data from API: {response.StatusCode}");
                }
                List<Book> books = new List<Book>();
                var firstBook = await response.Content.ReadFromJsonAsync<Book>();
                books.Add(firstBook);
                var model = firstBook;

                if (model.Authors != null)
                {
                    if (model.Authors.Count == 1)
                    {
                        response = await client.GetAsync($"api/bookauthor?authorid={model.Authors.First().AuthorId}");
                        if (response.IsSuccessStatusCode)
                        {
                            var authbooks = await response.Content.ReadFromJsonAsync<Book[]>();
                            if (authbooks != null)
                            {
                                var uniqueBooks = authbooks
                                    .GroupBy(book => book.BookId)
                                    .Select(group => group.First()) // Получаем первый уникальный элемент в каждой группе
                                    .Take(4)
                                    .ToList();

                                // Используем AddRange для добавления уникальных книг в список
                                books.AddRange(uniqueBooks);
                            }
                        }
                        else
                        {
                            _logger.LogWarning($"Failed to retrieve books from the API. Status Code: {response.StatusCode}");
                        }
                    }
                }

                response = await client.GetAsync(uri);
                ViewData["Title"] = $"Book - {model.Title}";
                return View(books);
            }
            catch (Exception ex)
            {
                // Логирование и обработка исключения
                // Например, записать в журнал или показать сообщение об ошибке
                Console.WriteLine($"Exception: {ex.Message}");
                return View("Error", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
            }
            return View();
        }

        public async Task<IActionResult> AuthorPage(int? id, string? name)
        {
            string uri;

            if (!id.HasValue)
            {
                return View();
            }
            else
            {

                //ViewData["Title"] = $"Customers in {id}";
                uri = $"api/bookauthor?authorid={id}";

            }


            try
            {

                //HttpRequestMessage request = new(method: HttpMethod.Get, requestUri: uri);
                //HttpResponseMessage response = await client.SendAsync(request);
                HttpClient client = _clientFactory.CreateClient(name: "BookShop.WebAPI");



                HttpResponseMessage response = await client.GetAsync(uri);

                if (!response.IsSuccessStatusCode)
                {
                    ViewData["Title"] = "Exception!";
                    // Логирование ошибки
                    throw new Exception($"Error fetching data from API: {response.StatusCode}");
                }

                var books = await response.Content.ReadFromJsonAsync<Book[]>();

                ViewData["Title"] = name;
                return View(books);
            }
            catch (Exception ex)
            {
                // Логирование и обработка исключения
                // Например, записать в журнал или показать сообщение об ошибке
                Console.WriteLine($"Exception: {ex.Message} {ex.Data}");
                return View("Error", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
            }
            return View();
        }


    }
}
