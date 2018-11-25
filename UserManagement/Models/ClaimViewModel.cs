using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace UserManagement.Models
{
    public class ClaimModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
