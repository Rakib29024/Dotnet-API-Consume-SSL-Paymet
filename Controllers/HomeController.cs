using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WebApiConsumeDemo.Models;
using WebApiConsumeDemo.ViewModels;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WebApiConsumeDemo.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly HttpClient _httpClient;

    public HomeController(ILogger<HomeController> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<IActionResult> Index()
    {
            var externalApiUrl = "https://jsonplaceholder.typicode.com/users";

            try
            {
                var response = await _httpClient.GetAsync(externalApiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var users = JsonSerializer.Deserialize<List<UserViewModel>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return View(users);
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error fetching data from external API");
                }
            }
            catch (HttpRequestException e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
    }

    [HttpGet("Info")]
    public async Task<IActionResult> Info(int id)
    {
            var externalApiUrl = "https://jsonplaceholder.typicode.com/users/"+ id;

            try
            {
                var response = await _httpClient.GetAsync(externalApiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var user = JsonSerializer.Deserialize<UserViewModel>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return PartialView("_UserInfoView", user);
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error fetching data from external API");
                }
            }
            catch (HttpRequestException e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirm(UserViewModel userModel)
    {
            var externalApiUrl = "https://jsonplaceholder.typicode.com/users/"+ userModel.Id;
            await Task.Delay(5000);

            try
            {
                var response = await _httpClient.GetAsync(externalApiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var user = JsonSerializer.Deserialize<UserViewModel>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return PartialView("_UserPaymentView", user);
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error fetching data from external API");
                }
            }
            catch (HttpRequestException e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
