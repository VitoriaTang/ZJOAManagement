using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using ZJOASystem.Controllers;

namespace ZJOASystem.Models
{
    public class ProductBase
    {
        public ProductBase()
        {
        }
        public ProductBase(string number, string name, string parentNumber)
            : base()
        {
            this.Name = name;
            this.Number = number;
            ParentNumber = parentNumber;
        }
        public string Number { get; set; }
        public string Name { get; set; }
        public string ParentNumber { get; set; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{");
            builder.AppendLine(string.Format("  \"Name\":\"{0}\" ,", Name));
            builder.AppendLine(string.Format("  \"Number\":\"{0}\" ,", Number));
            builder.AppendLine(string.Format("  \"ParentNumber\":\"{0}\" ,", ParentNumber));
            builder.Append("}");

            return builder.ToString();
        }
        public override bool Equals(object obj)
        {
            ProductBase objValue = (ProductBase)obj;
            if (objValue == null)
            {
                return false;
            }
            else
            {
                return obj.ToString().Equals(this.ToString(), StringComparison.CurrentCultureIgnoreCase);
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    internal class InnerProductBase
    {
        public string Number { get; set; }
        public string Name { get; set; }
        public string ParentNumber { get; set; }
    }
    public class Product
    {
        public int Id { get; set; }
        public Guid ProductGuid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductBaseNumber { get; set; }
        public string YearNumber { get; set; }
        public string BatchNumber { get; set; }
        public string SerialNumber { get; set; }
        public string Number
        {
            get
            {
                string numberText = string.Join("", ProductBaseNumber, YearNumber, BatchNumber, SerialNumber);
                return numberText;
            }
        }
        public ProductStatus Status { get; set; }

        private List<Action> _innerActions;
        public List<Action> Actions
        {
            get
            {
                if (_innerActions == null)
                {
                    _innerActions = new List<Action>();
                }
                return _innerActions;
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{");
            builder.AppendLine(string.Format("  \"name\":\"{0}\" ,", Name));
            builder.AppendLine(string.Format("  \"number\":\"{0}\" ,", Number));
            builder.AppendLine(string.Format("  \"description\":\"{0}\" ,", Description));
            builder.AppendLine(string.Format("  \"status\":{0} ,", Convert.ToInt32(Status)));
            builder.AppendLine(string.Format("  \"productguid\":'{0}' ,", Convert.ToString(ProductGuid)));
            builder.AppendLine("  \"actions\":");
            builder.AppendLine("  [");
            for (int i = 0; i < Actions.Count; i++)
            {
                builder.AppendLine("  "+ Actions[i].ToString());
            }
            builder.AppendLine("  ]");
            builder.Append("}");
            
            return builder.ToString();
        }

        public override bool Equals(object obj)
        {
            Product objValue = (Product)obj;
            if (objValue == null)
            {
                return false;
            }
            else
            {
                return obj.ToString().Equals(this.ToString(), StringComparison.CurrentCultureIgnoreCase);
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class InnerProduct
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public ProductStatus Status { get; set; }
        public Guid ProductGuid { get; set; }
        public Guid ParentGuid { get; set; }
        public InnerProduct(string number, string name, 
            string description, ProductStatus status, Guid productGuid, Guid parentGuid)
        {
            this.Name = name;
            this.Number = number;
            this.Description = description;
            this.Status = status;
            this.ProductGuid = productGuid;
            this.ParentGuid = parentGuid;
        }

        public string StatusText
        {
            get
            {
                switch (Status)
                {
                    case ProductStatus.Setup:
                        return ResourceReader.GetString("STATUS_SETUP");
                    case ProductStatus.Fixed:
                        return ResourceReader.GetString("STATUS_FIXED");
                    case ProductStatus.Qualified:
                        return ResourceReader.GetString("STATUS_QUALIFIED");
                    case ProductStatus.Unqualified:
                        return ResourceReader.GetString("STATUS_UNQUALIFIED");
                    case ProductStatus.Packaged:
                        return ResourceReader.GetString("STATUS_PACKAGE");
                    case ProductStatus.Delievered:
                        return ResourceReader.GetString("STATUS_DELIEVERED");
                    case ProductStatus.Disabled:
                        return ResourceReader.GetString("STATUS_DISABLED");
                    case ProductStatus.Unknown:
                    default:
                        return "";
                }
            }
        }
    }

    public class InnerProductAction
    {
        public Guid ProductGuid { get; set; }
        public ActionType ActionType { get; set; }
        public string ActionEmployees { get; set; }
        public ProductStatus Status { get; set; }
        public string Number { get; set; }
        public string ActionComments { get; set; }
        public List<string> GetOperators()
        {
            List<string> result = new List<string>();
            if (!string.IsNullOrEmpty(ActionEmployees))
            {
                string[] items = ActionEmployees.Split(',');
                foreach (string item in items)
                {
                    result.Add(item);
                }
            }
            return result;
        }
    }
    public class InnerComplexProduct 
    {
        public Guid ProductGuid { get; set; }
        public string ChildItems { get; set; }
        public string ActionEmployees { get; set; }
        public List<Guid> GetChildrenGuidList()
        {
            List<Guid> result = new List<Guid>();
            if (!string.IsNullOrEmpty(ChildItems))
            {
                string[] items = ChildItems.Split(',');
                foreach (string item in items)
                {
                    result.Add(Guid.Parse(item));
                }
            }
            return result;
        }
        public List<string> GetOperators()
        {
            List<string> result = new List<string>();
            if (!string.IsNullOrEmpty(ActionEmployees))
            {
                string[] items = ActionEmployees.Split(',');
                foreach (string item in items)
                {
                    result.Add(item);
                }
            }
            return result;
        }
    }
    public class InnerProductActions
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public ActionType ActionType { get; set; }
        public string ActionEmployees { get; set; }
        private List<String> _employees = null;
        
        public List<String> GetEmployeeEncodes()
        {
            if (_employees == null)
            {
                _employees = new List<string>();
            }

            if (!string.IsNullOrEmpty(ActionEmployees))
            {
                string[] items = ActionEmployees.Split(',');
                _employees = new List<string>(items);
            }
            return _employees;
        }
        public InnerProductActions()
        {

        }
        public InnerProductActions(string number, string name, string description, ActionType actionType, string employeeEncodes)
        {
            this.Name = name;
            this.Number = number;
            this.Description = description;
            this.ActionType = actionType;
            this.ActionEmployees = employeeEncodes;
        }
    }
    public class ProductAction
    {
        public string Name { get; set; }
        public string Encode { get; set; }
        public string NameEncode
        {
            get{
                return string.Join("_", Name, Encode);
            }
            
        }
        public Guid ProductGuid { get; set; }
        public Guid ParentGuid { get; set; }

        public ActionType ActionType { get; set; }
        public DateTime ActionTime { get; set; }
        public string ActionEmployee { get; set; }
        public ProductStatus Status { get; set; }
        public string Comments { get; set; }
    }
    public enum ActionType{
        Create = 1, //创建
        Setup = 2, //组装
        Test = 3, //测试
        Fix = 4, //维修
        Package = 5, //打包
        Deliever= 6, //发货
        UnSetup = 7, //卸载
    }
    public enum ProductStatus
    {
        Unknown = 0,
        Setup = 1,
        Unqualified = 2,
        Qualified = 3,
        Fixed = 4,
        Packaged = 5,
        Delievered = 6,
        Disabled = 7
        
    }
    public class Action
    {
        public int Id { get; set; }
        
        public ActionType ActionType { get; set; }

        public DateTime ActionTime { get; set; }

        public Guid ActionGuId { get; set; }

        public string Comments { get; set; }

        public string ProductNumber { get; set; }

        private List<string> _operators = null;
        public List<string> Operators
        {
            get
            {
                if (_operators == null)
                {
                    _operators = new List<string>();
                }
                return _operators;
            }
            set
            {
                _operators = value;
            }
        }
    }

    public class ProductAddition
    {
        public int Id { get; set; }
        public Guid AdditionGuid { get; set; }
        public string TrackNumber { get; set; }
        public string Sender { get; set; }
        public string SenderTelephone { get; set; }
        public string Receiver { get; set; }
        public string ReceiverTelephone { get; set; }
        public string Departure { get; set; }
        public string Destination { get; set; }
        public string Comments { get; set; }
        public Guid ProductGuid { get; set; }
    }
    public class InnerProductDelieverInfo
    {
        public string Name { get; set; }
        public string Encode { get; set; }
        public ProductStatus Status { get; set; }
        public string ActionEmployee { get; set; }
        public DateTime ActionTime { get; set; }
        public string ActionComments { get; set; }
        
        public Guid AdditionGuid { get; set; }
        public Guid ProductGuid { get; set; }
        public Guid ParentGuid { get; set; }
        
        public string TrackNumber { get; set; }
        public string Sender { get; set; }
        public string SenderTelephone { get; set; }
        public string Receiver { get; set; }
        public string ReceiverTelephone { get; set; }
        public string Departure { get; set; }
        public string Destination { get; set; }
        public string Comments { get; set; }
        public string NameEncode
        {
            get
            {
                return string.Join("_", Name, Encode);
            }
        }
    }
    
    public class InnerTestProduct
    {
        public Guid ProductGuid { get; set; }
        public string NameEncode { get; set; }
        public string ActionComments { get; set; }
        public ProductStatus Status { get; set; }
        public string ActionEmployee { get; set; }
    }
}