using System.Collections.Generic;
using CodeChallenge.Models;
using CodeChallenge.Repositories;
using Microsoft.Extensions.Logging;

namespace CodeChallenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICompensationRepository _compensationRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository, ICompensationRepository compensationRepository)
        {
            _employeeRepository = employeeRepository;
            _compensationRepository = compensationRepository;
            _logger = logger;
        }

        public Employee Create(Employee employee)
        {
            if(employee != null)
            {
                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetById(string id)
        {
            if(!string.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id);
            }

            return null;
        }

        public List<Employee> GetAll()
        {
            return _employeeRepository.GetAll();
        }

        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if(originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }

        public ReportingStructure GetDirectReports(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var employee = _employeeRepository.GetById(id);
                if (employee == null)
                    return null;

                return new ReportingStructure
                {
                    Employee = employee,
                    NumberOfReports = GetNumberOfReports(employee)
                };
            }

            return null;
        }

        public ApiResponse CreateCompensation(Compensation compensation)
        {
            // Realistically EmployeeId should never be null since it was passed in route, being overly cautious in that first part of the OR
            if (string.IsNullOrEmpty(compensation.EmployeeId) || GetById(compensation.EmployeeId) == null)
                return ApiResponse.NotFound;

            var createdCompensation = _compensationRepository.Add(compensation);
            return ApiResponse.Ok(createdCompensation);
        }

        public ApiResponse GetCompensations(string id)
        {
            if (string.IsNullOrEmpty(id) || GetById(id) == null)
                return ApiResponse.NotFound;

            var compensations = _compensationRepository.GetByEmployeeId(id);
            return ApiResponse.Ok(compensations ?? new List<Compensation>());
        }

        private static int GetNumberOfReports(Employee employee)
        {
            if (employee.DirectReports == null)
                return 0;

            int numberOfReports = employee.DirectReports.Count;
            foreach(var report in employee.DirectReports)
            {
                numberOfReports += GetNumberOfReports(report);
            }
            return numberOfReports;
        }
    }
}
