using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Data;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;

namespace CodeChallenge.Repositories
{
    public class EmployeeRespository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRespository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Employee Add(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid().ToString();
            _employeeContext.Employees.Add(employee);
            return employee;
        }

        public Employee GetById(string id)
        {
            var employee = _employeeContext.Employees.SingleOrDefault(e => e.EmployeeId == id);
            LoadDirectReports(employee); // Direct reports aren't loaded because of lazy loading, force load here
            return employee;
        }

        private void LoadDirectReports(Employee employee)
        {
            if (employee == null)
                return;

            _employeeContext.Entry(employee).Collection(e => e.DirectReports).Load();

            // Instead of empty list that is created from Load() above, use null to match what GetAll() returns
            if(employee.DirectReports.Count == 0)
            {
                employee.DirectReports = null;
                return;
            }

            foreach(var report in employee.DirectReports)
            {
                LoadDirectReports(report);
            }
        }

        public List<Employee> GetAll()
        {
            return _employeeContext.Employees.ToList();
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }

        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }
    }
}
