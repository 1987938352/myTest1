using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Attributes
{
    [AttributeUsage(AttributeTargets.Method,AllowMultiple =true)]//指定只能在方法上用 而且可以调用多个
    public class CheckPermissionAttribute: Attribute
    {
          public string Role { get; set; }

        public CheckPermissionAttribute(string Role)
        {
            this.Role = Role;
        }
    }
}
