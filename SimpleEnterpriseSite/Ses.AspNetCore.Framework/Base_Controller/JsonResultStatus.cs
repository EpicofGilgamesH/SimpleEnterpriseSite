using System;
using System.Collections.Generic;
using System.Text;

namespace Ses.AspNetCore.Framework.Base_Controller
{
    public enum JsonResultStatus
    {
        OK = 100,
        Failed = 101,
        NotLogin = 102,
        Unauthorized = 103,
    }
}
