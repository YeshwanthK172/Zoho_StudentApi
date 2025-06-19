using StudentApi.Models;
public class LoginRequest
{
    public int Reg_no { get; set; } // Add this field
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

