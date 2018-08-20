using System;
using System.Collections.Generic;
using System.Text;

namespace Ses.AspNetCore.ViewModels.Icon
{
    public class IconViewModel
    {
        public Guid Id { get; set; }
        public string IconNo { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
