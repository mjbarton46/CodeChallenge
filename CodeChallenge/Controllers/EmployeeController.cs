using CodeChallenge.Models;
using CodeChallenge.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/employees")]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        [HttpPost]
        public IActionResult CreateEmployee([FromBody] Employee employee)
        {
            _logger.LogDebug("Received employee create request for '{firstName} {lastName}'", employee.FirstName, employee.LastName);

            _employeeService.Create(employee);

            return CreatedAtRoute("getEmployeeById", new { id = employee.EmployeeId }, employee);
        }

        [HttpGet("{id}", Name = "getEmployeeById")]
        public IActionResult GetEmployeeById(string id)
        {
            _logger.LogDebug("Received employee get request for '{id}'", id);

            var employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpGet()]
        public IActionResult GetAllEmployees()
        {
            _logger.LogDebug("Received employee get all request");

            var employees = _employeeService.GetAll();

            return Ok(employees);
        }

        [HttpPut("{id}")]
        public IActionResult ReplaceEmployee(string id, [FromBody]Employee newEmployee)
        {
            _logger.LogDebug("Recieved employee update request for '{id}'", id);

            var existingEmployee = _employeeService.GetById(id);
            if (existingEmployee == null)
                return NotFound();

            _employeeService.Replace(existingEmployee, newEmployee);

            return Ok(newEmployee);
        }
    }
}
