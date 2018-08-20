using System;
using System.Collections.Generic;
using System.Text;

namespace Ses.AspNetCore.Framework.Base_Controller
{
    public class SesJsonResult
    {
        public JsonResultStatus Status { get; set; }
        public string Message { get; set; }

        public object Data { get; set; }

        public SesJsonResult(JsonResultStatus status)
        {
            Status = status;
        }

        public SesJsonResult(JsonResultStatus status, string message)
        {
            Status = status;
            Message = message;
        }

        public SesJsonResult(JsonResultStatus status, object data,string message)
        {
            Status = status;
            Message = message;
            Data = data;
        }
    }
}
