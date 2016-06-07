using CommonLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;


namespace Web.Fiters
{
    public class WebApiAuthAttribute : AuthorizationFilterAttribute
    {

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            //签名校验
            var request = actionContext.Request.Headers;
            var oSignature = request.SingleOrDefault(x => x.Key == "Signature");
            var oTimestamp = request.SingleOrDefault(x => x.Key == "Timestamp");
            var oNonce = request.SingleOrDefault(x => x.Key == "Nonce");

            string strSignature = (oSignature.Key == null ? "" : oSignature.Value.FirstOrDefault());
            string strTimestamp = (oTimestamp.Key == null ? "0" : oTimestamp.Value.FirstOrDefault());
            string strNonce = (oNonce.Key == null ? "" : oNonce.Value.FirstOrDefault());
            string strAppKey = "XKCE9P34TsqemfITS0W18RX6ewsxPK07MALZJ7Y";

            StringBuilder strSign = new StringBuilder();
            strSign.Append(strAppKey);
            strSign.Append(strTimestamp);
            strSign.Append(strNonce);

            long timeSpan = Convert.ToInt64(Helper.GetTimeStamp()) - Convert.ToInt64(strTimestamp);
            string strAuthCode = Helper.Md5Hash(strSign.ToString());
            if (strAuthCode.ToUpper() != strSignature.ToUpper() || timeSpan > (3 * 60*1000))
            {
                //签名未通过
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                actionContext.Response.Headers.Add("Authenticate", "Unauthorized");
                return;
            }
        }

    }
}