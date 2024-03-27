using TestAccura.Models;

namespace TestAccura.Repository
{
    // Repositories/IEmployeeRepository.cs
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllEmployees();
        Task<Employee> GetEmployeeById(int employeeId);
        Task<int> AddEmployee(Employee employee);
        Task<int> UpdateEmployee(Employee employee);
        Task<int> DeleteEmployee(int employeeId);
    }

}
