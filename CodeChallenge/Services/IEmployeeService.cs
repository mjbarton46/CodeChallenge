using System.Collections.Generic;
using CodeChallenge.Models;

namespace CodeChallenge.Services
{
    public interface IEmployeeService
    {
        Employee GetById(string id);
        List<Employee> GetAll();
        Employee Create(Employee employee);
        Employee Replace(Employee originalEmployee, Employee newEmployee);
        ReportingStructure GetDirectReports(string id);
        ApiResponse GetCompensations(string id);
    }
}
