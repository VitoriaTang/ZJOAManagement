using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Collections;

namespace ZJOASystem.Models
{
    public class Department
    {
        public int Id { get; set; }
        [Required]
        [StringLength(32, MinimumLength = 2)]
        public string Name { get; set; }

        [StringLength(64)]
        public string Telephone { get; set; }
        public virtual List<Employee> Employees { get; set; }

        public virtual int ParentId { get; set; }

        public virtual int ManagerId { get; set; }
    }

    public class Employee
    {
        private List<Department> _innerList = new List<Department>();
        public int Id { get; set; }
        [Required]
        [StringLength(32, MinimumLength = 2)]
        public string Name { get; set; }
        [StringLength(64)]
        public string Telephone { get; set; }
        [StringLength(64)]
        public string Encode { get; set; }
        [StringLength(128)]
        public string Email { get; set; }
        [StringLength(256)]
        public string Address { get; set; }
        public virtual List<Department> Departments
        {
            get { return _innerList; }
        }

        public string GetDepartmentTexts()
        {
            StringBuilder builder = new StringBuilder();
            if (_innerList != null && _innerList.Count > 0)
            {
                foreach (Department depart in _innerList)
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(",");
                    }
                    builder.Append(depart.Name);
                }
            }
            return builder.ToString();
        }

        public string Department
        {
            get
            {
                return this.GetDepartmentTexts();
            }
        }
        public bool Belongs(int departmentId)
        {
            if (this.Departments == null || Departments.Count == 0)
            {
                return departmentId == 0;
            }
            else
            {
                bool existed = false;
                foreach (Department department in Departments)
                {
                    if (department.Id == departmentId)
                    {
                        existed = true;
                        break;
                    }
                }
                return existed;
            }
        }
    }

    public class InnerEmployee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Encode { get; set; }
        public string NameEncode
        {
            get
            {
                return string.Join("_", Name, Encode);
            }
        }
    }
}