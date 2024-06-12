using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Сотрудники
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeesController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<EmployeeShortResponse>> GetEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();

            var employeesModelList = employees.Select(x =>
                new EmployeeShortResponse()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FullName = x.FullName,
                }).ToList();

            return employeesModelList;
        }

        /// <summary>
        /// Получить данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            var employeeModel = new EmployeeResponse()
            {
                Id = employee.Id,
                Email = employee.Email,
                Roles = employee.Roles.Select(x => new RoleItemResponse()
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };

            return employeeModel;
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeResponse>> CreateEmployee(
            [FromBody] EmployeeCreateRequest newEmployee) 
        {
            Employee employee = new()
            {
                Email = newEmployee.Email,
                FirstName = newEmployee.FirstName,
                LastName = newEmployee.LastName
            };
            
            Employee responseEmployee = await _employeeRepository.CreateAsync(employee);

            if (responseEmployee != null) 
            {
                var employeeModel = new EmployeeResponse()
                {
                    Id = responseEmployee.Id,
                    Email = responseEmployee.Email,
                    Roles = responseEmployee.Roles.Select(x => new RoleItemResponse()
                    {
                        Name = x.Name,
                        Description = x.Description
                    }).ToList(),
                    FullName = responseEmployee.FullName,
                    AppliedPromocodesCount = responseEmployee.AppliedPromocodesCount
                };

                return employeeModel;
            }

            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EmployeeResponse>> UpdateEmployee(
            Guid id,
            [FromBody] EmployeeUpdateRequest newEmployee) 
        {
            Employee employee = new()
            {
                Email = newEmployee.Email,
                FirstName = newEmployee.FirstName,
                LastName = newEmployee.LastName
            };
            
            Employee responseEmployee = 
                await _employeeRepository.UpdateAsync(id, employee);

            if (responseEmployee != null) 
            {
                var employeeModel = new EmployeeResponse()
                {
                    Id = responseEmployee.Id,
                    Email = responseEmployee.Email,
                    Roles = responseEmployee.Roles.Select(x => new RoleItemResponse()
                    {
                        Name = x.Name,
                        Description = x.Description
                    }).ToList(),
                    FullName = responseEmployee.FullName,
                    AppliedPromocodesCount = responseEmployee.AppliedPromocodesCount
                };

                return employeeModel;
            }

            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<EmployeeResponse>> DeleteEmployee(Guid id) 
        {
            Employee responseEmployee = await _employeeRepository.DeleteAsync(id);

            if (responseEmployee != null) 
            {
                var employeeModel = new EmployeeResponse()
                {
                    Id = responseEmployee.Id,
                    Email = responseEmployee.Email,
                    Roles = responseEmployee.Roles.Select(x => new RoleItemResponse()
                    {
                        Name = x.Name,
                        Description = x.Description
                    }).ToList(),
                    FullName = responseEmployee.FullName,
                    AppliedPromocodesCount = responseEmployee.AppliedPromocodesCount
                };

                return employeeModel;
            }

            return BadRequest();
        }
    }
}