using System;
using System.Collections.Generic;
using System.Text;

namespace Ses.AspNetCore.ViewModels.Department
{
    public class DepartmentPageListViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Manager { get; set; }
        public string PName { get; set; }
        public string IsParent { get; set; }
        public string State { get; set; }
        public string CreationTime { get; set; }
        public string Description { get; set; }
    }
}
