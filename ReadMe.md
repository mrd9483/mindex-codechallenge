# Developers Notes

## Considerations and Assumptions
* Each task type could have been its own service, controller, and test suite. I opted to keep them together to keep the like domain items together. Both Compensation and ReportingStructure are weak entities, tied back to employee
* There was an assumption made that the compensation endpoint could return multiple results, because of the effective date. 
* Added two users to the data, to test circular dependencies with the direct reports
* I changed how the test suite behaves, I changed the set up and tear down methods to be changed on each test rather than class instansiation. This guaranteed all tests would run the same each time they ran.
  

## Potential Improvements
* Implement unit tests as well as integration tests. Would have been easier and more trivial to test some of the more naunced features like circular dependencies or constraints, using mocks and stubs.
* The foreign key checks written for EF do not work with in memory databases (https://github.com/aspnet/EntityFrameworkCore/issues/2166, https://docs.microsoft.com/en-us/ef/core/miscellaneous/testing/in-memory) it maybe advantageous to use a relational database such as Sqlite.
* Better understanding of the business requirements and goals of this project - Depending on the use cases, better attention can be given to efficiency, performance and code organization.
  

## New Endpoints

### Create Compensation
    HTTP Method: POST 
    URL: localhost:8080/api/employee/compensation
    PAYLOAD: Compensation
    RESPONSE: Compensation

### Get Compensations
    HTTP Method: GET
    URL: localhost:8080/api/employee/compensation/{employeeId}
    RESPONSE: List of Compensation

### Get Number of Reports
    HTTP Method: GET
    URL: localhost:8080/api/employee/numberOfReports/{employeeId}
    RESPONSE: ReportingStructure


## New Data Types

## Compensation
```json
{
  "type":"Compensation",
  "properties": {
    "compensationId": {
      "type": "string"
    },
    "employeeId": {
      "type": "string"
    },
    "salary": {
      "type": "decimal"
    },
    "effectiveDate": {
          "type": "datetime"
    },
    "employee": {
          "type": "Employee"
    }
  }
}
```

## ReportingStructure
```json
{
  "type":"ReportingStructure",
  "properties": {
    "Employee": {
      "type": "Employee"
    },
    "numberOfReports": {
      "type": "int"
    }
  }
}
```

