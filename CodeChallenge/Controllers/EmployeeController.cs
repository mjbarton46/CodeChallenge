using System.Collections.Generic;
using CodeChallenge.Models;
using CodeChallenge.Services;
using Microsoft.AspNetCore.Http;
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
        public IActionResult GetEmployeeById([FromRoute] string id)
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
        public IActionResult ReplaceEmployee([FromRoute] string id, [FromBody] Employee newEmployee)
        {
            _logger.LogDebug("Recieved employee update request for '{id}'", id);

            var existingEmployee = _employeeService.GetById(id);
            if (existingEmployee == null)
                return NotFound();

            _employeeService.Replace(existingEmployee, newEmployee);

            return Ok(newEmployee);
        }

        [HttpGet("{id}/reports")]
        public IActionResult GetEmployeeDirectReports([FromRoute] string id)
        {
            _logger.LogDebug("Received employee reports get request for '{id}'", id);

            var employeeDirectReports = _employeeService.GetDirectReports(id);

            if (employeeDirectReports == null)
                return NotFound();

            return Ok(employeeDirectReports);
        }

        [HttpPost("{id}/compensations")]
        public ActionResult<Compensation> CreateEmployeeCompensation([FromRoute] string id, [FromBody] Compensation compensation)
        {
            _logger.LogDebug("Received compensation create request for id '{id}'", id);

            compensation.EmployeeId = id;
            ApiResponse response = _employeeService.CreateCompensation(compensation);
            return ParseApiResponse<Compensation>(response);

            // Just returning simple Ok status code for now to avoid creating another endpoint...
            // To do the CreatedAtRoute, I would create another endpoint GET /api/employees/{employeeId}/compensations/{compensationId}
            //return CreatedAtRoute("getCompensationByEmployeeAndCompensationId", new { employeeId = id, compensationId = compensation.Id }, compensation);
        }

        [HttpGet("{id}/compensations")]
        public ActionResult<List<Compensation>> GetEmployeeCompensations([FromRoute] string id)
        {
            _logger.LogDebug("Received employee compensation get request for '{id}'", id);

            ApiResponse response = _employeeService.GetCompensations(id);
            return ParseApiResponse<List<Compensation>>(response);
        }

        // Allows service layer to inform controller what type of status code to return.
        // This way I can know from the controller's perspective whether null or empty means the resource (the employee)
        // could not be found (returns a 404) or that there was no Compensation found for the employee (returns an empty 200)
        //
        // Main downside to this approach is that service layer returns ApiResponse which doesn't make it
        // immediately obvious what the content return type would be (e.g. List<Compensation>).
        //
        // Just showing that I have comfort/experience with higher level decisions/design beyond just adding a barebones endpoint with basic logic
        private ActionResult<T> ParseApiResponse<T>(ApiResponse response)
        {
            if (response.StatusCode == StatusCodes.Status200OK && response is ApiResponse<T> converted)
                return Ok(converted.Content);

            return response.StatusCode switch // Would add other status codes as needed
            {
                StatusCodes.Status404NotFound => NotFound(),
                _ => Problem(),
            };
        }
    }
}
