using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.DataTableViewModels
{
    public class UserDataTableViewModel
    {
        public string ID { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set;  }
        public string Roles { get; set; }
        public string Email { get; set;  }
    }
}
