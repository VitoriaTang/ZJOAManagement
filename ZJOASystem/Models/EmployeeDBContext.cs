using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace ZJOASystem.Models
{
    public class EmployeeDBContext: DbContext
    {
        public EmployeeDBContext()
            : base("name=DefaultConnection")
        {
            System.Data.Entity.Database.SetInitializer(new MySqlEmployeeInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Department>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Employee>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }

        public override int SaveChanges()
        {
            int result = 0;
            var entities = ChangeTracker.Entries<Employee>();
            result += SaveEmployee(entities);

            var depEntities = ChangeTracker.Entries<Department>();
            result += SaveDepartment(depEntities);
            return result;
        }

        public List<Department> GetEmployeeDepartments(int employeeId)
        {
            List<Department> departments = null;
            string command = "SELECT Department_Id FROM EmployeeDepartments WHERE Employee_Id={0}";

            List<int> result = this.Database.SqlQuery<int>(command, employeeId).ToList<int>();
            if (result != null && result.Count > 0)
            {
                departments = new List<Department>();
                foreach (int departmentId in result)
                {
                    departments.Add(GetDepartment(departmentId));
                }
            }

            return departments;
        }
        private int SaveEmployee(IEnumerable<System.Data.Entity.Infrastructure.DbEntityEntry<Employee>> entities)
        {
            int result = 0;

            foreach (var entry in entities)
            {
                if (entry.State == EntityState.Added)
                {

                    string insertcmd = String.Format(
                       "INSERT INTO Employees (Name, Encode, Telephone, Email, Address) VALUES ('{0}','{1}', '{2}','{3}','{4}')",
                       entry.Entity.Name, entry.Entity.Encode, entry.Entity.Telephone, entry.Entity.Email, entry.Entity.Address);
                    this.Database.ExecuteSqlCommand(insertcmd);

                    List<Department> departments = entry.Entity.Departments;
                    foreach (Department department in departments)
                    {
                        string insertCommand = String.Format(
                       "INSERT INTO EmployeeDepartments (Employee_Id, Department_Id) SELECT Id,{0} FROM Employees WHERE Employees.Encode='{1}'",  department.Id, entry.Entity.Encode);
                        this.Database.ExecuteSqlCommand(insertCommand);
                    }


                    result += 1;
                    entry.State = EntityState.Unchanged;
                }
                else if (entry.State == EntityState.Modified)
                {
                    using (var tran = this.Database.BeginTransaction())
                    {
                        try
                        {
                            List<Department> departments = entry.Entity.Departments;
                            string updatecmd = String.Format(
                               "UPDATE Employees SET Name='{0}', Encode='{1}', Telephone='{2}', Email='{3}', Address='{4}' WHERE Id={5}",
                               entry.Entity.Name, entry.Entity.Encode, entry.Entity.Telephone, entry.Entity.Email, entry.Entity.Address, entry.Entity.Id);
                            this.Database.ExecuteSqlCommand(updatecmd);


                            string deleteCommand = String.Format("DELETE FROM EmployeeDepartments WHERE Employee_Id={0}", entry.Entity.Id);
                            this.Database.ExecuteSqlCommand(deleteCommand);
                            foreach (Department department in departments)
                            {
                                string insertCommand = String.Format(
                               "INSERT INTO EmployeeDepartments (Employee_Id, Department_Id) VALUES ({0},{1})", entry.Entity.Id, department.Id);
                                this.Database.ExecuteSqlCommand(insertCommand);
                            }


                            tran.Commit();
                            result += 1;
                        }
                        catch
                        {
                            tran.Rollback();
                        }
                    }

                    entry.State = EntityState.Unchanged;
                }
                else if (entry.State == EntityState.Deleted)
                {
                    using (var tran = this.Database.BeginTransaction())
                    {
                        try
                        {
                            List<Department> departments = entry.Entity.Departments;
                            string deleteCommand = String.Format("DELETE FROM EmployeeDepartments WHERE Employee_Id={0}", entry.Entity.Id);
                            this.Database.ExecuteSqlCommand(deleteCommand);

                            string deleteCmd = String.Format(
                               "DELETE FROM Employees WHERE Id={0}", entry.Entity.Id);
                            this.Database.ExecuteSqlCommand(deleteCmd);

                            tran.Commit();
                            result += 1;
                        }
                        catch
                        {
                            tran.Rollback();
                        }
                    }
                }
            }

            return result;
        }
        private int SaveDepartment(IEnumerable<System.Data.Entity.Infrastructure.DbEntityEntry<Department>> entities)
        {
            int result = 0;

            foreach (var entry in entities)
            {
                if (entry.State == EntityState.Added)
                {

                    string insertcmd = String.Format(
                       "INSERT INTO Departments (Name, Telephone, ParentId, ManagerId) VALUES ('{0}','{1}', {2},{3})",
                       entry.Entity.Name, entry.Entity.Telephone, entry.Entity.ParentId, entry.Entity.ManagerId);
                    this.Database.ExecuteSqlCommand(insertcmd);

                    result += 1;
                    entry.State = EntityState.Unchanged;
                }
                else if (entry.State == EntityState.Modified)
                {
                    using (var tran = this.Database.BeginTransaction())
                    {
                        try
                        {
                            string updatecmd = String.Format(
                               "UPDATE Departments SET Name='{0}', Telephone='{1}', ParentId={2}, ManagerId={3} WHERE Id={4}",
                               entry.Entity.Name, entry.Entity.Telephone, entry.Entity.ParentId, entry.Entity.ManagerId,  entry.Entity.Id);
                            this.Database.ExecuteSqlCommand(updatecmd);

                            tran.Commit();
                            result += 1;
                        }
                        catch
                        {
                            tran.Rollback();
                        }
                    }

                    entry.State = EntityState.Unchanged;
                }
                else if (entry.State == EntityState.Deleted)
                {
                    using (var tran = this.Database.BeginTransaction())
                    {
                        try
                        {
                            string deleteCommand = String.Format("DELETE FROM Departments WHERE Id={0}", entry.Entity.Id);
                            this.Database.ExecuteSqlCommand(deleteCommand);
                           
                            tran.Commit();
                            result += 1;
                        }
                        catch
                        {
                            tran.Rollback();
                        }
                    }
                }
            }

            return result;
        }
        
        public System.Data.Entity.DbSet<ZJOASystem.Models.Employee> Employees { get; set; }
        public System.Data.Entity.DbSet<ZJOASystem.Models.Department> Departments { get; set; }


        public Department GetDepartment(int departmentId)
        {
            string sqlcmdText = "SELECT * FROM Departments WHERE Id={0}";

            List<Department> departments = this.Database.SqlQuery<Department>(sqlcmdText, departmentId).ToList<Department>();

            if (departments != null && departments.Count > 0)
            {
                return departments[0];
            }
            else
            {
                return null;
            }
        }

        public List<Employee> GetDepartmentEmployees(int departmentId)
        {
            string sqlcmdText = "SELECT Employee_Id FROM EmployeeDepartments WHERE Department_Id={0}";

            List<int> employeeIds = this.Database.SqlQuery<int>(sqlcmdText, departmentId).ToList<int>();

            if (employeeIds != null && employeeIds.Count > 0)
            {
                List<Employee> result = new List<Employee>();
                
                foreach (int id in employeeIds)
                {
                    Employee employee = this.Employees.Find(id);
                    if (employee != null)
                    {
                        result.Add(employee);
                    }
                }

                return result;
            }
            else
            {
                return null;
            }
        }
        public void SetEmployeeDepartments(Employee employee)
        {
            if(employee == null)
            {
                return;
            }

            string command = "SELECT Department_Id FROM EmployeeDepartments WHERE Employee_Id={0}";

            List<int> result = this.Database.SqlQuery<int>(command, employee.Id).ToList<int>();
            if (result != null && result.Count > 0)
            {
                foreach (int departmentId in result)
                {
                    employee.Departments.Add(GetDepartment(departmentId));
                }
            }
        }

        public List<InnerEmployee> GetInnerEmployees(string encode)
        {
            string command = "SELECT Id, Name, Encode FROM employees WHERE Encode is not null";
            if (!string.IsNullOrEmpty(encode))
            {
                command = command + string.Format(" and Encode='{0}'", encode);
            }
            List<InnerEmployee> result = this.Database.SqlQuery<InnerEmployee>(command).ToList<InnerEmployee>();
            return result;
        }

        
        public int GetEmployeeId(string encode)
        {
            string command = string.Format("SELECT Id FROM employees WHERE Encode='{0}'", encode);
            
            List<int> result = this.Database.SqlQuery<int>(command).ToList<int>();
            if (result != null && result.Count > 0)
            {
                return result[0];
            }
            else
            {
                return 0;
            }
        }
    }
}