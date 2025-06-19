using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using StudentApi.Models; // ✅ Make sure you import your models

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // ✅ Login API
    [HttpPost("Login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        using var connection = new MySqlConnection(_configuration.GetConnectionString("StudentDB"));
        connection.Open();

        var command = new MySqlCommand("SELECT * FROM users WHERE username=@u AND password=@p", connection);
        command.Parameters.AddWithValue("@u", request.Username);
        command.Parameters.AddWithValue("@p", request.Password);
        var reader = command.ExecuteReader();

        if (reader.Read())
        {
            var user = new User
            {
                Username = reader["username"].ToString()!,
                Role = reader["role"].ToString()!,
                Reg_no = Convert.ToInt32(reader["reg_no"])
            };
            return Ok(user);

        }

        return Unauthorized("Invalid credentials");
    }

    // ✅ Student Self-Registration API
    [HttpPost("Register")]
    public IActionResult Register([FromBody] LoginRequest req)
    {
        using var connection = new MySqlConnection(_configuration.GetConnectionString("StudentDB"));
        connection.Open();

        // 1. Check if the student exists in student_details
        var checkStudentCmd = new MySqlCommand("SELECT COUNT(*) FROM student_details WHERE Reg_no = @reg", connection);
        checkStudentCmd.Parameters.AddWithValue("@reg", req.Reg_no);
        int studentExists = Convert.ToInt32(checkStudentCmd.ExecuteScalar());

        if (studentExists == 0)
        {
            return BadRequest("❌ You are not registered by the admin.");
        }

        // 2. Check if username already taken
        var checkUserCmd = new MySqlCommand("SELECT COUNT(*) FROM users WHERE username = @username", connection);
        checkUserCmd.Parameters.AddWithValue("@username", req.Username);
        int userExists = Convert.ToInt32(checkUserCmd.ExecuteScalar());

        if (userExists > 0)
        {
            return BadRequest("❌ Username already taken.");
        }

        // 3. Register new student user
        var insertCmd = new MySqlCommand("INSERT INTO users (username, password, role, reg_no) VALUES (@username, @password, 'Student', @reg)", connection);
        insertCmd.Parameters.AddWithValue("@username", req.Username);
        insertCmd.Parameters.AddWithValue("@password", req.Password);
        insertCmd.Parameters.AddWithValue("@reg", req.Reg_no);
        insertCmd.ExecuteNonQuery();

        return Ok("✅ Registration successful.");
    }
}
