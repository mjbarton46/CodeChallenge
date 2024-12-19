
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

using CodeChallenge.Models;

using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeCodeChallenge.Tests.Integration
{
    [TestClass]
    public class EmployeeControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        // Attribute ClassInitialize requires this signature
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer();
            _httpClient = _testServer.NewClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void CreateEmployee_Returns_Created()
        {
            // Arrange
            var employee = new Employee()
            {
                Department = "Complaints",
                FirstName = "Debbie",
                LastName = "Downer",
                Position = "Receiver",
            };

            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/employees",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newEmployee = response.DeserializeContent<Employee>();
            Assert.IsNotNull(newEmployee.EmployeeId);
            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
            Assert.AreEqual(employee.Department, newEmployee.Department);
            Assert.AreEqual(employee.Position, newEmployee.Position);
        }

        [TestMethod]
        public void GetEmployeeById_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employees/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var employee = response.DeserializeContent<Employee>();
            Assert.AreEqual(expectedFirstName, employee.FirstName);
            Assert.AreEqual(expectedLastName, employee.LastName);

            Assert.IsNotNull(employee.DirectReports);
            Assert.AreEqual(2, employee.DirectReports.Count);

            var paulMcCartney = employee.DirectReports[0];
            Assert.IsNull(paulMcCartney.DirectReports);

            var ringoStarr = employee.DirectReports[1];
            Assert.IsNotNull(ringoStarr.DirectReports);
            Assert.AreEqual(2, ringoStarr.DirectReports.Count);

            var peteBest = ringoStarr.DirectReports[0];
            Assert.IsNull(peteBest.DirectReports);

            var georgeHarrison = ringoStarr.DirectReports[1];
            Assert.IsNull(georgeHarrison.DirectReports);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_Ok()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f",
                Department = "Engineering",
                FirstName = "Pete",
                LastName = "Best",
                Position = "Developer VI",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var putRequestTask = _httpClient.PutAsync($"api/employees/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var putResponse = putRequestTask.Result;
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, putResponse.StatusCode);
            var newEmployee = putResponse.DeserializeContent<Employee>();

            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_NotFound()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "Invalid_Id",
                Department = "Music",
                FirstName = "Sunny",
                LastName = "Bono",
                Position = "Singer/Song Writer",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PutAsync($"api/employees/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        [DataRow("16a596ae-edd3-4847-99fe-c4518e82c86f", 4)]
        [DataRow("b7839309-3348-463b-a7e3-5de1c168beb3", 0)]
        [DataRow("03aa1462-ffa9-4978-901b-7c001562cf6f", 2)]
        [DataRow("62c1084e-6e34-4630-93fd-9153afb65309", 0)]
        [DataRow("c0c2293d-16bd-4603-8e08-638a9d18b22c", 0)]
        public void GetEmployeeDirectReports_ValidEmployeeId_ReturnsCorrectNumberOfReports(string employeeId, int expectedDirectReports)
        {
            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employees/{employeeId}/reports");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var reportingStructure = response.DeserializeContent<ReportingStructure>();
            Assert.AreEqual(employeeId, reportingStructure.Employee.EmployeeId);
            Assert.AreEqual(expectedDirectReports, reportingStructure.NumberOfReports);
        }

        [TestMethod]
        public void GetEmployeeDirectReports_NonexistingEmployeeId_ReturnsNotFound()
        {
            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employees/doesnotexist/reports");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void GetEmployeeCompensations_EmployeeWithMultipleCompensations_ReturnsCompensations()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var exepectedCompensation1 = new Compensation { Salary = 100000, EffectiveDate = DateTime.Parse("01/01/2025") };
            var exepectedCompensation2 = new Compensation { Salary = 90000, EffectiveDate = DateTime.Parse("01/01/2024") };

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employees/{employeeId}/compensations");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var compensations = response.DeserializeContent<List<Compensation>>();
            Assert.AreEqual(2, compensations.Count);

            Assert.IsNotNull(compensations[1].Employee?.EmployeeId);
            Assert.AreEqual(employeeId, compensations[1].Employee.EmployeeId);
            Assert.AreEqual(exepectedCompensation1.Salary, compensations[1].Salary);
            Assert.AreEqual(exepectedCompensation1.EffectiveDate, compensations[1].EffectiveDate);
            // Could write private helpers to compare Compensation objects (and other classes too)

            Assert.IsNotNull(compensations[0].Employee?.EmployeeId);
            Assert.AreEqual(employeeId, compensations[0].Employee.EmployeeId);
            Assert.AreEqual(exepectedCompensation2.Salary, compensations[0].Salary);
            Assert.AreEqual(exepectedCompensation2.EffectiveDate, compensations[0].EffectiveDate);
        }

        [TestMethod]
        public void GetEmployeeCompensations_EmployeeWithSingleCompensation_ReturnsCompensation()
        {
            // Arrange
            var employeeId = "b7839309-3348-463b-a7e3-5de1c168beb3";
            var exepectedCompensation1 = new Compensation { Salary = 200000, EffectiveDate = DateTime.Parse("02/01/2025") };

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employees/{employeeId}/compensations");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var compensations = response.DeserializeContent<List<Compensation>>();
            Assert.AreEqual(1, compensations.Count);

            Assert.IsNotNull(compensations[0].Employee?.EmployeeId);
            Assert.AreEqual(employeeId, compensations[0].Employee.EmployeeId);
            Assert.AreEqual(exepectedCompensation1.Salary, compensations[0].Salary);
            Assert.AreEqual(exepectedCompensation1.EffectiveDate, compensations[0].EffectiveDate);
        }

        [TestMethod]
        public void GetEmployeeCompensations_EmployeeWithNoCompensations_ReturnsEmptyList()
        {
            // Arrange
            var employeeId = "c0c2293d-16bd-4603-8e08-638a9d18b22c";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employees/{employeeId}/compensations");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var compensations = response.DeserializeContent<List<Compensation>>();
            Assert.IsNotNull(compensations);
            Assert.AreEqual(0, compensations.Count);
        }

        [TestMethod]
        public void GetEmployeeCompensations_NonexistingEmployeeId_ReturnsNotFound()
        {
            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employees/doesnotexist/compensations");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
