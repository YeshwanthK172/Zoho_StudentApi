using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using StudentApi.Models;

[ApiController]
[Route("api/[controller]")]
public class MarksController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public MarksController(IConfiguration config)
    {
        _configuration = config;
    }

    
    [HttpPost("Add")]
    public IActionResult Add([FromBody] Marks marks)
    {
        if (marks.Tamil > 100 || marks.English > 100 || marks.Maths > 100 || marks.Science > 100 || marks.Social > 100)
            return BadRequest("Each subject mark must be ≤ 100");

        marks.TotalMarks = marks.Tamil + marks.English + marks.Maths + marks.Science + marks.Social;
        marks.Result = (marks.Tamil >= 40 && marks.English >= 40 && marks.Maths >= 40 &&
                        marks.Science >= 40 && marks.Social >= 40) ? "Pass" : "Fail";

        using var conn = new MySqlConnection(_configuration.GetConnectionString("StudentDB"));
        conn.Open();
        var cmd = new MySqlCommand(@"INSERT INTO marks (reg_no, exam_name, Tamil, English, Maths, Science, Social, TotalMarks, Result) 
                                 VALUES (@reg, @exam, @ta, @en, @ma, @sc, @so, @tot, @res)", conn);
        cmd.Parameters.AddWithValue("@reg", marks.Reg_no);
        cmd.Parameters.AddWithValue("@exam", marks.ExamName);
        cmd.Parameters.AddWithValue("@ta", marks.Tamil);
        cmd.Parameters.AddWithValue("@en", marks.English);
        cmd.Parameters.AddWithValue("@ma", marks.Maths);
        cmd.Parameters.AddWithValue("@sc", marks.Science);
        cmd.Parameters.AddWithValue("@so", marks.Social);
        cmd.Parameters.AddWithValue("@tot", marks.TotalMarks);
        cmd.Parameters.AddWithValue("@res", marks.Result);
        cmd.ExecuteNonQuery();

        return Ok("Marks added");
    }

    [HttpGet("GetAll")]
    public IActionResult GetAllMarks()
    {
        var list = new List<Marks>();
        using var conn = new MySqlConnection(_configuration.GetConnectionString("StudentDB"));
        conn.Open();

        var cmd = new MySqlCommand("SELECT * FROM marks", conn);
        var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new Marks
            {
                Id = Convert.ToInt32(reader["id"]),
                Reg_no = Convert.ToInt32(reader["reg_no"]),
                ExamName = reader["exam_name"].ToString(),
                Tamil = Convert.ToInt32(reader["Tamil"]),
                English = Convert.ToInt32(reader["English"]),
                Maths = Convert.ToInt32(reader["Maths"]),
                Science = Convert.ToInt32(reader["Science"]),
                Social = Convert.ToInt32(reader["Social"]),
                TotalMarks = Convert.ToInt32(reader["TotalMarks"]),
                Result = reader["Result"].ToString()
            });
        }
        return Ok(list);
    }



    [HttpPut("Edit")]
    public IActionResult Edit([FromBody] Marks marks)
    {
        if (marks.Tamil > 100 || marks.English > 100 || marks.Maths > 100 ||
            marks.Science > 100 || marks.Social > 100)
            return BadRequest("Each subject mark must be ≤ 100");

        marks.TotalMarks = marks.Tamil + marks.English + marks.Maths + marks.Science + marks.Social;
        marks.Result = (marks.Tamil >= 40 && marks.English >= 40 && marks.Maths >= 40 &&
                        marks.Science >= 40 && marks.Social >= 40) ? "Pass" : "Fail";

        using var conn = new MySqlConnection(_configuration.GetConnectionString("StudentDB"));
        conn.Open();

        var cmd = new MySqlCommand(@"
        UPDATE marks 
        SET exam_name = @exam, 
            Tamil = @ta, 
            English = @en, 
            Maths = @ma, 
            Science = @sc, 
            Social = @so, 
            TotalMarks = @tot, 
            Result = @res 
        WHERE id = @id", conn);

        cmd.Parameters.AddWithValue("@id", marks.Id);
        cmd.Parameters.AddWithValue("@exam", marks.ExamName);
        cmd.Parameters.AddWithValue("@ta", marks.Tamil);
        cmd.Parameters.AddWithValue("@en", marks.English);
        cmd.Parameters.AddWithValue("@ma", marks.Maths);
        cmd.Parameters.AddWithValue("@sc", marks.Science);
        cmd.Parameters.AddWithValue("@so", marks.Social);
        cmd.Parameters.AddWithValue("@tot", marks.TotalMarks);
        cmd.Parameters.AddWithValue("@res", marks.Result);

        cmd.ExecuteNonQuery();

        return Ok("Marks updated successfully");
    }


    [HttpGet("ByStudent/{regNo}")]
    public IActionResult GetByStudent(int regNo)
    {
        var list = new List<Marks>();
        using var conn = new MySqlConnection(_configuration.GetConnectionString("StudentDB"));
        conn.Open();

        var cmd = new MySqlCommand("SELECT * FROM marks WHERE reg_no = @reg", conn);
        cmd.Parameters.AddWithValue("@reg", regNo);
        var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new Marks
            {
                Id = Convert.ToInt32(reader["id"]),
                Reg_no = Convert.ToInt32(reader["reg_no"]),
                ExamName = reader["exam_name"].ToString(),
                Tamil = Convert.ToInt32(reader["Tamil"]),
                English = Convert.ToInt32(reader["English"]),
                Maths = Convert.ToInt32(reader["Maths"]),
                Science = Convert.ToInt32(reader["Science"]),
                Social = Convert.ToInt32(reader["Social"]),
                TotalMarks = Convert.ToInt32(reader["TotalMarks"]),
                Result = reader["Result"].ToString() ?? ""
            });
        }

        return Ok(list);
    }


}
