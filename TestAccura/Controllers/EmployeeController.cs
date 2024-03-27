using Microsoft.AspNetCore.Mvc;
using TestAccura.Models;
using TestAccura.Repository;

namespace TestAccura.Controllers
{
    // Controllers/EmployeeController.cs
    [ApiController]
    [Route("api/employees")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _employeeRepository.GetAllEmployees();
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await _employeeRepository.GetEmployeeById(id);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        [HttpPost("create")]
        public async Task<IActionResult> AddEmployee([FromBody] Employee employee)
        {
            employee.CreatedDate = DateTime.Now;
            await _employeeRepository.AddEmployee(employee);
            return Ok(employee.EmployeeId); // Returning the newly created employee's id
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] Employee employee)
        {
            var existingEmployee = await _employeeRepository.GetEmployeeById(id);

            if (existingEmployee == null)
            {
                return NotFound();
            }

            existingEmployee.FirstName = employee.FirstName;
            existingEmployee.LastName = employee.LastName;
            existingEmployee.Gender = employee.Gender;
            existingEmployee.DateOfBirth = employee.DateOfBirth;
            existingEmployee.Address = employee.Address;
            existingEmployee.Department = employee.Department;
            existingEmployee.BasicSalary = employee.BasicSalary;

            await _employeeRepository.UpdateEmployee(existingEmployee);

            return Ok(existingEmployee);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var result = await _employeeRepository.DeleteEmployee(id);

            if (result == 0)
            {
                return NotFound();
            }

            return Ok(id);
        }
    }

}
