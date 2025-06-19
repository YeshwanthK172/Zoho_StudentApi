namespace StudentApi.Models
{
    public class Marks
    {
        public int Id { get; set; }
        public int Reg_no { get; set; }
        public string ExamName { get; set; } = string.Empty;

        public int Tamil { get; set; }
        public int English { get; set; }
        public int Maths { get; set; }
        public int Science { get; set; }
        public int Social { get; set; }

        public int TotalMarks { get; set; }
        public string Result { get; set; } = string.Empty;
    }
}
