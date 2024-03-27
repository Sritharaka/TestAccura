using Microsoft.EntityFrameworkCore;
using TestAccura.Models;

namespace TestAccura.Repository
{
    // Repositories/EmployeeRepository.cs
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AccuraContext _context;

        public EmployeeRepository(AccuraContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployees()
        {
            return await _context.Employees.ToListAsync();
        }

        public async Task<Employee> GetEmployeeById(int employeeId)
        {
            return await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        }

        public async Task<int> AddEmployee(Employee employee)
        {
            _context.Employees.Add(employee);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateEmployee(Employee employee)
        {
            _context.Employees.Update(employee);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteEmployee(int employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                return await _context.SaveChangesAsync();
            }
            return 0; // Employee not found
        }
    }

}
