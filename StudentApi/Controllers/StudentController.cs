using Microsoft.AspNetCore.Mvc;
using StudentApi.Models; 
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace StudentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase 
    {
        private readonly IConfiguration _configuration;

        public StudentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("GetAllStudents")]
        public Response GetAllStudents()
        {
            Response response = new();
            MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("StudentDB"));
            DAL dal = new DAL();
            response = dal.GetAllStudents(connection);
            return response;
        }

        [HttpPost]
        [Route("AddStudent")]
        public Response AddStudents([FromBody] Student student)
        {
            Response response = new();
            MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("StudentDB"));
            DAL dal = new DAL();
            response = dal.AddStudents(connection, student);
            return response;
        }
        [HttpPut]
        [Route("UpdateStudent")]
        
        public Response UpdateStudent([FromBody] Student student)
        {
            Response response = new();
            MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("StudentDB"));
            DAL dal = new DAL();
            response = dal.UpdateStudent(connection, student);
            return response;
        }



        [HttpDelete("DeleteStudent/{regNo}")]
        public IActionResult DeleteStudent(int regNo)
        {
            using var conn = new MySqlConnection(_configuration.GetConnectionString("StudentDB"));
            conn.Open();

            // Delete related records first to satisfy foreign key constraints
            var deleteAttendance = new MySqlCommand("DELETE FROM attendance WHERE reg_no = @reg", conn);
            deleteAttendance.Parameters.AddWithValue("@reg", regNo);
            deleteAttendance.ExecuteNonQuery();

            var deleteUsers = new MySqlCommand("DELETE FROM users WHERE reg_no = @reg", conn);
            deleteUsers.Parameters.AddWithValue("@reg", regNo);
            deleteUsers.ExecuteNonQuery();

            var deleteMarks = new MySqlCommand("DELETE FROM marks WHERE reg_no = @reg", conn);
            deleteMarks.Parameters.AddWithValue("@reg", regNo);
            deleteMarks.ExecuteNonQuery();

            // Finally, delete from student_details
            var deleteStudent = new MySqlCommand("DELETE FROM student_details WHERE Reg_no = @reg", conn);
            deleteStudent.Parameters.AddWithValue("@reg", regNo);
            int rows = deleteStudent.ExecuteNonQuery();

            return rows > 0 ? Ok("Deleted") : NotFound("Student not found");
        }






        [HttpGet("GetStudent/{regNo}")]
        public IActionResult GetStudent(int regNo)
        {
            using var connection = new MySqlConnection(_configuration.GetConnectionString("StudentDB"));
            connection.Open();

            var cmd = new MySqlCommand("SELECT * FROM student_details WHERE Reg_no=@reg", connection);
            cmd.Parameters.AddWithValue("@reg", regNo);
            var reader = cmd.ExecuteReader();

            if (reader.Read())
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

                return Ok(student);
            }

            return NotFound();
        }
        [HttpGet("GetCheckedInStudents")]
        public IActionResult GetCheckedInStudents()
        {
            var list = new List<Student>();
            using var connection = new MySqlConnection(_configuration.GetConnectionString("StudentDB"));
            connection.Open();

            var cmd = new MySqlCommand(
                "SELECT s.* FROM student_details s " +
                "JOIN attendance a ON s.Reg_no = a.reg_no " +
                "WHERE a.date = CURDATE() AND a.check_in IS NOT NULL AND a.check_out IS NULL",
                connection
            );

            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Student
                {
                    Reg_no = Convert.ToInt32(reader["Reg_no"]),
                    Name = reader["Name"].ToString()!,
                    Course = reader["Course"].ToString()!,
                    Mark_10th = Convert.ToInt32(reader["mark_10th"]),
                    Mark_12th = Convert.ToInt32(reader["mark_12th"]),
                    Cutoff = Convert.ToInt32(reader["cutoff"])
                });
            }

            return Ok(list); // ✅ Must return raw List<Student>
        }




    }
}