namespace StudentApi.Models
{
    public class Attendance
    {
        public DateTime Date { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public int Hours { get; set; }
    }

}
