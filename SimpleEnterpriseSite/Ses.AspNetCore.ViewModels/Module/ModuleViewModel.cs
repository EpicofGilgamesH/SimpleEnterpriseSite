using Ses.AspNetCore.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ses.AspNetCore.ViewModels.Module
{
    public class ModuleViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public Guid ParentId { get; set; }
        public string Url { get; set; }
        public int Sort { get; set; }
        public bool IsMenu { get; set; }
        public string Description { get; set; }
        public StateEnum State { get; set; }
    }
}
