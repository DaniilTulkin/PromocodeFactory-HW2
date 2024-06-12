using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryEmployeeRepository : IEmployeeRepository
    {
        protected IEnumerable<Employee> Data { get;set; }

        public InMemoryEmployeeRepository(IEnumerable<Employee> data)
        {
            Data = data;
        }

        public Task<Employee> CreateAsync(Employee employee)
        {
            if (!Data.Any(x => x.Email == employee.Email)) {
                employee.Id = new Guid();
                Data.Append(employee);

                return Task.FromResult(employee);
            }
            
            return null;
        }

        public Task<Employee> DeleteAsync(Guid id)
        {
            Employee employee = GetByIdAsync(id).Result;
            if (employee != null) {
                Data = Data.Where(x => x.Id != id);
            }

            return Task.FromResult(employee);
        }

        public Task<IEnumerable<Employee>> GetAllAsync() =>
            Task.FromResult(Data);

        public Task<Employee> GetByEmailAsync(string email) =>
            Task.FromResult(Data.FirstOrDefault(x => x.Email == email));

        public Task<Employee> GetByIdAsync(Guid id) =>
            Task.FromResult(Data.FirstOrDefault(x => x.Id == id));

        public Task<Employee> UpdateAsync(Guid id, Employee newEmployee)
        {
            Employee employee = GetByIdAsync(id).Result;
            if (employee != null) {
                Data = Data.Where(x => x.Id != id);
                
                newEmployee.Id = employee.Id;
                Data.Append(newEmployee);
            }

            return Task.FromResult(employee);
        }
    }
}