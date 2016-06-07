using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.CommonAPIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Web.Controllers
{
    public class TemplateController : ApiController
    {
        // GET api/template
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/template/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/template
        public SendTemplateMessageResult Post(Model.TemplateMessage tm)
       {
            CommonService.TemplateService service = new CommonService.TemplateService();
            return service.SendTemplateMessage(tm);
        }

        // PUT api/template/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/template/5
        public void Delete(int id)
        {
        }
    }
}
