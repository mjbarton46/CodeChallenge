﻿using System.Collections.Generic;
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

        public Compensation Add(Compensation compensation)
        {
            _employeeContext.Compensations.Add(compensation);
            _employeeContext.SaveChanges(); // In production, would add error handling to catch primary key violations, etc.
            return compensation;
        }

        public List<Compensation> GetByEmployeeId(string id)
        {
            return _employeeContext.Compensations.Where(c => string.Equals(c.EmployeeId, id)).ToList();
        }
    }
}
