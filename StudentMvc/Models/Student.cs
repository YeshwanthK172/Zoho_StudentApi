namespace StudentMvc.Models
{
    public class Student
    {
        public int Reg_no { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public int mark_10th { get; set; }
        public int mark_12th { get; set; }
        public int cutoff { get; set; }
    }
}
