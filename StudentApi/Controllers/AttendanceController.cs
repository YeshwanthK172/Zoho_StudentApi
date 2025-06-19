using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using StudentApi.Models; // or Student.Models, depending on your project


[ApiController]
[Route("api/[controller]")]
public class AttendanceController : ControllerBase
{
    private readonly IConfiguration _configuration;
    public AttendanceController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("CheckIn")]
    public IActionResult CheckIn([FromBody] int regNo)
    {
        using var connection = new MySqlConnection(_configuration.GetConnectionString("StudentDB"));
        connection.Open();

        var cmd = new MySqlCommand(
            "INSERT INTO attendance (reg_no, check_in, date) VALUES (@reg, CURTIME(), CURDATE())",
            connection);
        cmd.Parameters.AddWithValue("@reg", regNo);
        cmd.ExecuteNonQuery();

        return Ok("Checked in");
    }



    [HttpPost("CheckOut")]
    public IActionResult CheckOut([FromBody] int regNo)
    {
        using var connection = new MySqlConnection(_configuration.GetConnectionString("StudentDB"));
        connection.Open();

        // Find latest unmatched check-in
        var selectCmd = new MySqlCommand(
            "SELECT id FROM attendance WHERE reg_no=@reg AND date=CURDATE() AND check_out IS NULL ORDER BY id DESC LIMIT 1",
            connection);
        selectCmd.Parameters.AddWithValue("@reg", regNo);
        var result = selectCmd.ExecuteScalar();

        if (result == null)
            return BadRequest("No active check-in found for today.");

        int attendanceId = Convert.ToInt32(result);

        // Update that row with check-out
        var updateCmd = new MySqlCommand(
            "UPDATE attendance SET check_out=CURTIME(), hours=TIMESTAMPDIFF(HOUR, check_in, CURTIME()) WHERE id=@id",
            connection);
        updateCmd.Parameters.AddWithValue("@id", attendanceId);
        updateCmd.ExecuteNonQuery();

        return Ok("Checked out");
    }
    [HttpGet("GetLastAttendanceStatus/{regNo}")]
    public IActionResult GetLastAttendanceStatus(int regNo)
    {
        using var connection = new MySqlConnection(_configuration.GetConnectionString("StudentDB"));
        connection.Open();

        var cmd = new MySqlCommand(
            "SELECT check_in, check_out FROM attendance WHERE reg_no=@reg AND date=CURDATE() ORDER BY id DESC LIMIT 1",
            connection);
        cmd.Parameters.AddWithValue("@reg", regNo);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            var checkIn = reader["check_in"];
            var checkOut = reader["check_out"];

            if (checkIn != DBNull.Value && checkOut == DBNull.Value)
                return Ok("CheckedIn");

            return Ok("CheckedOut");
        }

        // No record for today — treat as not checked in
        return Ok("CheckedOut");
    }




    [HttpGet("GetAttendance/{regNo}")]
    public IActionResult GetAttendance(int regNo)
    {
        var list = new List<Attendance>();
        using var connection = new MySqlConnection(_configuration.GetConnectionString("StudentDB"));
        connection.Open();

        var cmd = new MySqlCommand("SELECT date, check_in, check_out, hours FROM attendance WHERE reg_no=@reg", connection);
        cmd.Parameters.AddWithValue("@reg", regNo);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            var attendance = new Attendance
            {
                Date = reader.GetDateTime(reader.GetOrdinal("date")),

                CheckIn = reader.IsDBNull(reader.GetOrdinal("check_in"))
                          ? (DateTime?)null
                          : reader.GetDateTime(reader.GetOrdinal("check_in")),

                CheckOut = reader.IsDBNull(reader.GetOrdinal("check_out"))
                           ? (DateTime?)null
                           : reader.GetDateTime(reader.GetOrdinal("check_out")),

                Hours = reader.IsDBNull(reader.GetOrdinal("hours"))
                        ? 0
                        : reader.GetInt32(reader.GetOrdinal("hours"))
            };

            list.Add(attendance);
        }

        return Ok(list);
    }
    [HttpGet("GetCheckedInStudents")]
    public IActionResult GetCheckedInStudents()
    {
        var list = new List<Student>();
        var connection = new MySqlConnection(_configuration.GetConnectionString("StudentDB"));
        connection.Open();

        // Fetch students who have a check-in today and have not yet checked out
        var cmd = new MySqlCommand(@"
        SELECT s.* FROM student_details s
        JOIN attendance a ON s.Reg_no = a.reg_no
        WHERE a.date = CURDATE() AND a.check_in IS NOT NULL AND a.check_out IS NULL", connection);

        var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var student = new Student
            {
                Reg_no = Convert.ToInt32(reader["Reg_no"]),
                Name = reader["Name"].ToString()!,
                Course = reader["Course"].ToString()!,
                Mark_10th = Convert.ToInt32(reader["mark_10th"]),
                Mark_12th = Convert.ToInt32(reader["mark_12th"]),
                Cutoff = Convert.ToInt32(reader["cutoff"])
            };

            list.Add(student);
        }

        return Ok(list);
    }





}
