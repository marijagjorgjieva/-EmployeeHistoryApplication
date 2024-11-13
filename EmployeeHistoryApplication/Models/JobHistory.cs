namespace EmployeeHistoryApplication.Models
{
    public class JobHistory
    {
        public int Id { get; set; } 
        public required int EmployeeId { get; set; }
        public required Employee Employee { get; set; } = null!;
        public required string CompanyName { get; set; }
        public required string JobPostition { get; set; }


    }
}
