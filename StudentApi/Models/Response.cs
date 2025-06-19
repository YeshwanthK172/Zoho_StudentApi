using System.Collections.Generic;

namespace StudentApi.Models 
{
    public class Response
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public Student Student { get; set; }
        public List<Student> ListStudent { get; set; }

        public Response() // Add this constructor to initialize ListStudent
        {
            ListStudent = new List<Student>();
        }
    }
}