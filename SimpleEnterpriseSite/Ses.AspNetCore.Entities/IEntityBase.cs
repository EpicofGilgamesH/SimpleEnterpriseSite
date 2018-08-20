using System;
using System.Collections.Generic;
using System.Text;

namespace Ses.AspNetCore.Entities
{
    public interface IEntityBase<TKey>
    {
        TKey Id { get; set; }
    }
}
