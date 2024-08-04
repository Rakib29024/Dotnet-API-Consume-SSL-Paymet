using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WebApiConsumeDemo.Models;
using WebApiConsumeDemo.ViewModels;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;

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
            UserDynamicFormViewModel userForm = new UserDynamicFormViewModel();
            try
            {
                var response = await _httpClient.GetAsync(externalApiUrl);

                if (response.IsSuccessStatusCode)
                {
                    // user info fetch
                    var content = await response.Content.ReadAsStringAsync();
                    var user = JsonSerializer.Deserialize<UserViewModel>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    userForm.UserInfo = user;

                    // form fileds
                    List<DynamicFormFieldViewModel> formFields = new List<DynamicFormFieldViewModel>
                    {
                        new DynamicFormFieldViewModel
                        {
                            FieldType = "Text",
                            FieldName = "Name",
                            IsRequired = "No"
                        },
                        new DynamicFormFieldViewModel
                        {
                            FieldType = "Text",
                            FieldName = "Amount",
                            IsRequired = "Yes"
                        }
                    };

                    userForm.FormFileds = formFields;

                    return PartialView("_UserInfoView", userForm);
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
    // [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submited([FromBody] Dictionary<string, string> formData)
    {
            await Task.Delay(2000);
            DynamicFormDataRequest formDataModel = new DynamicFormDataRequest();

            try
            {
                // Filter out the __RequestVerificationToken
                var filteredData = formData.Where(kvp => kvp.Key != "__RequestVerificationToken").ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                formDataModel.Fields = filteredData;
                formDataModel.Fields["Extra"] = "Test";
            }
            catch (HttpRequestException e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }

            return View("_UserPaymentView");
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
