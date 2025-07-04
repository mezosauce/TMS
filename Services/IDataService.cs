﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Time_Managmeent_System.Models;

namespace Time_Managmeent_System.Services;
    public interface IDataService
    {
    Task<IEnumerable<Employee>> GetEmployees();
    Task CreateEmployee(Employee employee);
    Task DeleteEmployee(int id);
    Task UpdateEmployee(Employee employee);
    }
