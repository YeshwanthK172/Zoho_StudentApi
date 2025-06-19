// StudentAPI/Models/DAL.cs
using System.Data;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System;

namespace StudentApi.Models
{
    public class DAL
    {
        [Obsolete]
        public Response GetAllStudents(MySqlConnection connection)
        {
            Response response = new Response();
            MySqlDataAdapter adapter = new MySqlDataAdapter("SELECT * FROM student_details", connection);
            DataTable dt = new DataTable();
            List<Student> Liststudents = new List<Student>();

            adapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Student student = new Student();
                    student.Name = Convert.ToString(dt.Rows[i]["Name"]);
                    student.Course = Convert.ToString(dt.Rows[i]["Course"]);
                    student.Reg_no = Convert.ToInt32(dt.Rows[i]["Reg_no"]);
                    student.Mark_10th = Convert.ToInt32(dt.Rows[i]["mark_10th"]);
                    student.Mark_12th = Convert.ToInt32(dt.Rows[i]["mark_12th"]);
                    student.Cutoff = Convert.ToInt32(dt.Rows[i]["cutoff"]);
                    Liststudents.Add(student);
                }
            }
            response.ListStudent = Liststudents;
            if (Liststudents.Count > 0)
            {
                response.StatusCode = 200;
                response.StatusMessage = "Students data retrieved successfully";
            }
            else
            {
                response.StatusCode = 200; // Still success, just no data
                response.StatusMessage = "No students found";
            }
            return response;
        }

        public Response AddStudents(MySqlConnection connection, Student student)
        {
            Response response = new Response();
            MySqlCommand cmd = new MySqlCommand(
                "INSERT INTO student_details (Name, Reg_no, Course, mark_10th, mark_12th, cutoff) VALUES (@Name, @RegNo, @Course, @Mark10, @Mark12, @Cutoff)",
                connection);

            cmd.Parameters.AddWithValue("@Name", student.Name);
            cmd.Parameters.AddWithValue("@RegNo", student.Reg_no);
            cmd.Parameters.AddWithValue("@Course", student.Course);
            cmd.Parameters.AddWithValue("@Mark10", student.Mark_10th);
            cmd.Parameters.AddWithValue("@Mark12", student.Mark_12th);
            cmd.Parameters.AddWithValue("@Cutoff", student.Cutoff);

            connection.Open();
            int i = cmd.ExecuteNonQuery(); // no of rows attected  (1)
            connection.Close();

            if (i > 0)
            {
                response.StatusCode = 200;
                response.StatusMessage = "Student Added Successfully";
                response.Student = student;
            }
            else
            {
                response.StatusCode = 400;
                response.StatusMessage = "Failed to Add Student";
                response.Student = null;
            }
            return response;
        }

        public Response UpdateStudent(MySqlConnection connection, Student student)
        {
            Response response = new Response();
            MySqlCommand cmd = new MySqlCommand("UPDATE student_details SET Name=@Name, Course=@Course, mark_10th=@Mark10, mark_12th=@Mark12, cutoff=@Cutoff WHERE Reg_no=@RegNo", connection);
            cmd.Parameters.AddWithValue("@Name", student.Name);
            cmd.Parameters.AddWithValue("@Course", student.Course);
            cmd.Parameters.AddWithValue("@Mark10", student.Mark_10th);
            cmd.Parameters.AddWithValue("@Mark12", student.Mark_12th);
            cmd.Parameters.AddWithValue("@Cutoff", student.Cutoff);
            cmd.Parameters.AddWithValue("@RegNo", student.Reg_no);

            connection.Open();
            int i = cmd.ExecuteNonQuery();
            connection.Close();

            response.StatusCode = i > 0 ? 200 : 400;
            response.StatusMessage = i > 0 ? "Updated successfully" : "Update failed";
            return response;
        }

        public Response DeleteStudent(MySqlConnection connection, int regNo)
        {
            Response response = new Response();
            string query = "DELETE FROM student_details WHERE Reg_no = @RegNo";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@RegNo", regNo);

            connection.Open();
            int i = cmd.ExecuteNonQuery();
            connection.Close();

            response.StatusCode = i > 0 ? 200 : 400;
            response.StatusMessage = i > 0 ? "Student Deleted" : "Delete Failed";
            return response;
        }


    }
}