using System;

namespace CodeChallenge.Models
{
    public class Compensation
    {
        // Just doing CompensationId to give EF a primary key
        // Not actually used right now but could be used to get specific compensation entry for an employee if desired
        public int CompensationId { get; set; }
        public string EmployeeId { get; set; } // FK to Employee (rather than full Employee object here)
        public int Salary { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}
