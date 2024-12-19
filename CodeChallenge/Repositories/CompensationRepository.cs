using System.Collections.Generic;
using System.Linq;
using CodeChallenge.Data;
using CodeChallenge.Models;

namespace CodeChallenge.Repositories
{
    // Did Compensation in its own repository since it's its own "table"
    // Realistically could keep it in EmployeeRepository too
    // Just depends on specific project/team and how division/organization is done
    public class CompensationRepository : ICompensationRepository
    {
        private readonly EmployeeContext _employeeContext;
        public CompensationRepository(EmployeeContext employeeContext) { 
            _employeeContext = employeeContext;
        }

        public List<Compensation> GetCompensations(string id)
        {
            return _employeeContext.Compensations.Where(c => string.Equals(c.Employee.EmployeeId, id)).ToList();
        }
    }
}
