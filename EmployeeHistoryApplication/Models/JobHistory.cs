namespace EmployeeHistoryApplication.Models
{
    public class JobHistory
    {
        public int Id { get; set; }
        public int? BlogId { get; set; } //opcionalen foreign key stavam oti sho ako ne rabotel do sea nigde
        public Employee? Employee { get; set; }
    }
}
