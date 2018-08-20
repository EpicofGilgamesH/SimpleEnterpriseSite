using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Ses.AspNetCore.Framework.Base_Controller
{
    public abstract class BaseController : Controller
    {

        public ContentResult JsonContent(object obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            return base.Content(json);
        }

        public ContentResult SuccessData(object data = null)
        {
            SesJsonResult result = new SesJsonResult(JsonResultStatus.OK, data, null);
            return this.JsonContent(result);
        }
        public ContentResult SuccessMsg(string msg = null)
        {
            SesJsonResult result = new SesJsonResult(JsonResultStatus.OK, msg);
            return this.JsonContent(result);
        }


        public ContentResult AddSuccessData(object data, string msg = "添加成功")
        {
            SesJsonResult result = new SesJsonResult(JsonResultStatus.OK, data, msg);
            return this.JsonContent(result);
        }
        public ContentResult AddSuccessMsg(string msg = "添加成功")
        {
            return this.SuccessMsg(msg);
        }
        public ContentResult UpdateSuccessMsg(string msg = "更新成功")
        {
            return this.SuccessMsg(msg);
        }
        public ContentResult DeleteSuccessMsg(string msg = "删除成功")
        {
            return this.SuccessMsg(msg);
        }
        public ContentResult FailedMsg(string msg = null)
        {
            SesJsonResult retResult = new SesJsonResult(JsonResultStatus.Failed, msg);
            return this.JsonContent(retResult);
        }
    }
}
