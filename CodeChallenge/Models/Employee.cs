using System.Collections.Generic;

namespace CodeChallenge.Models
{
    public class Employee
    {
        public string EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public List<Employee> DirectReports { get; set; }
    }
}
