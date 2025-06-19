using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StudentMvc.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace StudentMvc.Controllers
{
    public class StudentController : Controller
    {
        private readonly HttpClient _httpClient;

        public StudentController(IConfiguration configuration)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"])
            };
        }

        [HttpGet]
        public IActionResult AddStudent() => View();

        [HttpPost]
        public async Task<IActionResult> AddStudent(StudentMvc.Models.Student student)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("Student/AddStudent", student);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("ViewStudent");

            ViewBag.Error = "Failed to add student.";
            return View(student);
        }
        [HttpGet]
        public IActionResult AdminDashboard()
        {
            // Optional: Add session check if necessary
            return View();
        }
        [HttpGet]

        [HttpPost]
        public async Task<IActionResult> CheckIn(int regNo)
        {
            var response = await _httpClient.PostAsJsonAsync("Attendance/CheckIn", regNo);
            TempData["Status"] = "Checked in successfully!";
            return RedirectToAction("StudentDashboard");
        }
        [HttpGet]
        public async Task<IActionResult> ViewAttendance(int regNo)
        {
            var response = await _httpClient.GetAsync($"Attendance/GetAttendance/{regNo}");
            var data = await response.Content.ReadFromJsonAsync<List<Attendance>>();
            return View("Attendance", data);
        }


        [HttpPost]
        public async Task<IActionResult> CheckOut(int regNo)
        {
            var response = await _httpClient.PostAsJsonAsync("Attendance/CheckOut", regNo);
            TempData["Status"] = "Checked out successfully!";
            return RedirectToAction("StudentDashboard");
        }

        public async Task<IActionResult> StudentDashboard()
        {
            if (HttpContext.Session.GetInt32("RegNo") is not int regNo)
                return RedirectToAction("Login", "Login");

            var response = await _httpClient.GetAsync($"Attendance/GetLastAttendanceStatus/{regNo}");
            string status = await response.Content.ReadAsStringAsync();

            ViewBag.Status = status;
            return View(regNo); // Pass regNo to view
        }

        [HttpGet]
        public async Task<IActionResult> ViewResults()
        {
            int? regNo = HttpContext.Session.GetInt32("RegNo");
            if (regNo == null)
                return RedirectToAction("Login", "Login");

            var response = await _httpClient.GetAsync($"Marks/ByStudent/{regNo}");
            var data = await response.Content.ReadFromJsonAsync<List<Marks>>();
            return View("Results", data);  // 👈 view name = Results.cshtml
        }

        [HttpGet]
        public IActionResult AddMarks()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AllResults()
        {
            var response = await _httpClient.GetAsync("Marks/GetAll");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to load results";
                return View("AllResults", new List<Marks>());
            }

            var data = await response.Content.ReadFromJsonAsync<List<Marks>>();
            return View("AllResults", data);
        }




        [HttpPost]
        public async Task<IActionResult> AddMarks(Marks marks)
        {
            // Validate each mark
            if (marks.Tamil > 100 || marks.English > 100 || marks.Maths > 100 ||
                marks.Science > 100 || marks.Social > 100)
            {
                ViewBag.Error = "Invalid marks: Each subject mark must be ≤ 100.";
                return View(marks);
            }

            var response = await _httpClient.PostAsJsonAsync("Marks/Add", marks);
            if (response.IsSuccessStatusCode)
            {
                TempData["Status"] = "Marks uploaded successfully.";
                return RedirectToAction("AddMarks");
            }
            else
            {
                TempData["Error"] = "Failed to upload marks.";
                return RedirectToAction("AddMarks");
            }
        }







        public async Task<IActionResult> ViewStudent()
        {
            List<StudentMvc.Models.Student> students = new();
            HttpResponseMessage response = await _httpClient.GetAsync("Student/GetAllStudents");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Response>();
                students = result?.ListStudent ?? new List<StudentMvc.Models.Student>();
            }

            return View("ViewStudents", students);
        }

        public async Task<IActionResult> DeleteStudent(int regNo)
        {
            await _httpClient.DeleteAsync($"Student/DeleteStudent/{regNo}");
            return RedirectToAction("ViewStudent");
        }

        [HttpGet]
        public async Task<IActionResult> EditStudent(int regNo)
        {
            HttpResponseMessage response = await _httpClient.GetAsync("Student/GetAllStudents");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Response>();
                var student = result?.ListStudent?.FirstOrDefault(s => s.Reg_no == regNo);
                return View(student);
            }

            return RedirectToAction("ViewStudent");
        }

        [HttpPost]
        public async Task<IActionResult> EditStudent(StudentMvc.Models.Student student)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync("Student/UpdateStudent", student);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("ViewStudent");

            ViewBag.Error = "Failed to update student.";
            return View(student);
        }
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            int? regNo = HttpContext.Session.GetInt32("RegNo");
            if (regNo == null) return RedirectToAction("Login", "Login");

            var response = await _httpClient.GetAsync($"Student/GetStudent/{regNo}");
            var student = await response.Content.ReadFromJsonAsync<Student>();

            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(Student student)
        {
            var response = await _httpClient.PutAsJsonAsync("Student/UpdateStudent", student);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Profile updated successfully.";
            }
            else
            {
                TempData["Error"] = " Failed to update profile.";
            }

            return RedirectToAction("StudentDashboard");
        }

        [HttpGet]
        public async Task<IActionResult> CheckedInStudents()
        {
            var response = await _httpClient.GetAsync("Student/GetCheckedInStudents");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Failed to fetch checked-in students.";
                return View("CheckedInStudents", new List<Student>());
            }

            var students = await response.Content.ReadFromJsonAsync<List<Student>>();
            return View("CheckedInStudents", students);
        }





        [HttpGet]
        public IActionResult Login() => View();

    }
}
