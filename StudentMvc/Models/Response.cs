// StudentMVC/Models/Response.cs
using System.Collections.Generic;

namespace StudentMvc.Models
{
    public class Response
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public Student Student { get; set; }
        public List<Student> ListStudent { get; set; }

        public Response()
        {
            ListStudent = new List<Student>(); // Initialize to prevent null reference
        }
    }
}