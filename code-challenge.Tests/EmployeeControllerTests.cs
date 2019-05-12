using challenge.Controllers;
using challenge.Data;
using challenge.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using code_challenge.Tests.Integration.Extensions;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using code_challenge.Tests.Integration.Helpers;
using System.Text;
using challenge.Domain;
using System.Collections.Generic;

namespace code_challenge.Tests.Integration
{
    [TestClass]
    public class EmployeeControllerTests
    {
        private HttpClient _httpClient;
        private TestServer _testServer;

        [TestInitialize]
        public void InitializeTest()
        {
            _testServer = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<TestServerStartup>()
                .UseEnvironment("Development"));

            _httpClient = _testServer.CreateClient();
        }
        
        [TestCleanup]
        public void CleanUpTest()
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
            var postRequestTask = _httpClient.PostAsync("api/employee",
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
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var employee = response.DeserializeContent<Employee>();
            Assert.AreEqual(expectedFirstName, employee.FirstName);
            Assert.AreEqual(expectedLastName, employee.LastName);
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
            var putRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
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
            var postRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void NumberOfReports_Returns_Ok()
        { 
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";
            var expectedNumberOfReports = 4;

            var getRequestTask = _httpClient.GetAsync($"api/employee/numberOfReports/{employeeId}");
            var response = getRequestTask.Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var numOfReports = response.DeserializeContent<ReportingStructure>();
            Assert.AreEqual(expectedFirstName, numOfReports.Employee.FirstName);
            Assert.AreEqual(expectedLastName, numOfReports.Employee.LastName);
            Assert.AreEqual(expectedNumberOfReports, numOfReports.NumberOfReports);
        }

        [TestMethod]
        public void NumberOfReports_ThrowsCircularDependency_InternalServerError()
        {
            // Arrange
            // Arrange
            var employeeId = "c0c2293d-16bd-4603-8e08-9897542ac12";

            var getRequestTask = _httpClient.GetAsync($"api/employee/numberOfReports/{employeeId}");
            var response = getRequestTask.Result;

            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [TestMethod]
        public void CreateCompensation_Returns_Created()
        {
            // Arrange
            var compensation = new Compensation()
            {
                EmployeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f",
                Salary = 70000.00M,
                EffectiveDate = new DateTime(2019, 1, 1)
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/employee/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newCompensation = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(newCompensation.CompensationId);
            Assert.AreEqual(compensation.EmployeeId, newCompensation.EmployeeId);
            Assert.AreEqual(compensation.Salary, newCompensation.Salary);
            Assert.AreEqual(compensation.EffectiveDate, newCompensation.EffectiveDate);
        }

        [TestMethod]
        public void CreateCompensation_EmployeeDoesNotExist_InternalError()
        {
            // Arrange
            var compensation = new Compensation()
            {
                EmployeeId = "badbadbad-bad-bad-bad-badbadbad",
                Salary = 70000.00M,
                EffectiveDate = new DateTime(2019, 1, 1)
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/employee/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [TestMethod]
        public void GetCompensationsById_Returns_Ok()
        {
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";

            var c1 = CreateCompensations(employeeId, 40000M, new DateTime(2017, 1, 1));
            var c2 = CreateCompensations(employeeId, 70000M, new DateTime(2019, 1, 1));
            var c3 = CreateCompensations(employeeId, 50000M, new DateTime(2018, 1, 1));

            var getRequestTask = _httpClient.GetAsync($"api/employee/compensation/{employeeId}");
            var response = getRequestTask.Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var compensations = response.DeserializeContent<List<Compensation>>();
            Assert.AreEqual(3, compensations.Count);
            Assert.AreEqual(compensations[2].EmployeeId, c1.EmployeeId);
            Assert.AreEqual(compensations[1].Salary, c3.Salary);
            Assert.AreEqual(compensations[0].EffectiveDate, c2.EffectiveDate);
        }

        /// <summary>
        /// Helper method to create multiple compensations. No asserts as this method has been tested already
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="salary"></param>
        /// <param name="effectiveDate"></param>
        private Compensation CreateCompensations(string employeeId, decimal salary, DateTime effectiveDate)
        {
            // Arrange
            var compensation = new Compensation()
            {
                EmployeeId = employeeId,
                Salary = salary,
                EffectiveDate = effectiveDate
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/employee/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            return response.DeserializeContent<Compensation>();
        }
    }
}
