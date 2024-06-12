using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee> GetByIdAsync(Guid id);
        Task<Employee> GetByEmailAsync(string email);
        Task<Employee> CreateAsync(Employee employee);
        Task<Employee> UpdateAsync(Guid id, Employee employee);
        Task<Employee> DeleteAsync(Guid id);
    }
}