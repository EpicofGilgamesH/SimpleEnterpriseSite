using System;
using System.Collections.Generic;
using System.Text;

namespace Ses.AspNetCore.Framework.Helper.Tree
{
    public class ZtreeModel
    {
        public Guid id { get; set; }
        public Guid pId { get; set; }
        public string name { get; set; }
        public bool open { get; set; }
        public bool @checked { get; set; }
    }
}
