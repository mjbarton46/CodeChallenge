namespace CodeChallenge.Models
{
    public class ReportingStructure
    {
        public Employee Employee { get; set; } // Would do just EmployeeId in production API
        public int NumberOfReports { get; set; }
    }
}
