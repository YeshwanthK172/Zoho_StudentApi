using Microsoft.AspNetCore.Mvc;
using StudentMvc.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace StudentMvc.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpClient _httpClient;

        public LoginController(IConfiguration config)
        {
            // Initialize HTTP client with the base URL of your Web API (defined in appsettings.json)
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(config["ApiSettings:BaseUrl"]!)
            };
        }

        // Displays the login form (GET /Login/Login)
        [HttpGet]
        public IActionResult Login()
        {
            return View(); // Views/Login/Login.cshtml
        }

        // Handles login form submission
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest req)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Auth/Login", req);

                if (response.IsSuccessStatusCode)
                {
                    var user = await response.Content.ReadFromJsonAsync<User>();

                    if (user == null)
                    {
                        TempData["Error"] = "Invalid login data received.";
                        return View();
                    }

                    HttpContext.Session.SetString("Role", user.Role);
                    HttpContext.Session.SetInt32("RegNo", user.Reg_no);

                    if (user.Role == "admin")
                    {
                        return RedirectToAction("AdminDashboard", "Student");
                    }
                    else if (user.Role == "student")
                    {
                        return RedirectToAction("StudentDashboard", "Student");
                    }

                    TempData["Error"] = "Unknown role.";
                    return View();
                }

                TempData["Error"] = "Invalid credentials.";
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                return View();
            }
        }




        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // clear everything
            return RedirectToAction("Login");
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(LoginRequest req)
        {
            var response = await _httpClient.PostAsJsonAsync("Auth/Register", req);

            if (response.IsSuccessStatusCode)
            {
                
                TempData["Success"] = "Registration successful. Please log in.";
                return RedirectToAction("Login");
            }

            
            ViewBag.Error = await response.Content.ReadAsStringAsync();
            return View(req);
        }



    }
}
