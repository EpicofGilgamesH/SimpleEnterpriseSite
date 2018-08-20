using System;
using System.Collections.Generic;
using System.Text;

namespace Ses.AspNetCore.Framework.Helper.Tree
{
    public class SesTreeModel
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public bool HasChildren { get; set; }
        public int Level { get; set; }
        public object Data { get; set; }

        /// <summary>
        /// 在创建树形json数据时采用
        /// </summary>
        public string Text { get; set; }
    }
}
