using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vetis.Classes.Service;

namespace Vetis.Classes
{
    public abstract class baseClassApplication : baseClass
    {        
        public abstract Guid Save();
        public abstract void GetByIdOrApplicationId();
    }
}
