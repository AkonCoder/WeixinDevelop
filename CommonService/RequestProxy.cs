using CommonLib;
using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace CommonService
{
    public class RequestProxy
    {
        #region 接口信息

        /// <summary>
        /// API接口密钥
        /// </summary>
        public static string AuthCode = "MQkMaxNHsSvDNsuiDlBH";

        /// <summary>
        /// 主站接口地址
        /// </summary>
        public static string ProxyUrl = string.Format("http://{0}/API/OpenData.ashx", ConfigurationManager.AppSettings["SysApiService"].ToString());

        /// <summary>
        /// 登录缓存信息
        /// </summary>
        public static Hashtable HtLoginUserInfo = new Hashtable();

        /// <summary>
        /// 日志记录委托
        /// </summary>
        /// <param name="logModel"></param>
        /// <returns></returns>
        delegate bool ResponseLogDelegate(ResponseLogModel logModel);

        #endregion


        #region CreateAuthCode 生成验证信息
        /// <summary>
        /// 生成验证信息
        /// </summary>
        /// <returns></returns>
        public ProxyRequestModel CreateAuthCode()
        {
            var requestMd = new ProxyRequestModel { Timestamp = Helper.GetTimeStamp(), Nonce = Helper.GetRandomNum() };

            var strSign = new StringBuilder();
            strSign.Append(AuthCode);
            strSign.Append(requestMd.Timestamp);
            strSign.Append(requestMd.Nonce);
            requestMd.Signature = Helper.SHA1_Encrypt(strSign.ToString());

            return requestMd;
        }
        #endregion

        #region BindUserUpdate 更新登录缓存信息
        /// <summary>
        /// 更新登录缓存信息
        /// </summary>
        /// <param name="wxOpenid">微信Openid</param>
        /// <param name="userModel"></param>
        public void BindUserUpdate(string wxOpenid, ManageUserLite userModel)
        {
            if (HtLoginUserInfo != null)
            {
                if (HtLoginUserInfo.ContainsKey(wxOpenid))
                {
                    lock (HtLoginUserInfo)
                    {
                        HtLoginUserInfo[wxOpenid] = userModel;
                    }
                }
                else
                {
                    lock (HtLoginUserInfo)
                    {
                        HtLoginUserInfo.Add(wxOpenid, userModel);
                    }
                }
            }
            else
            {
                HtLoginUserInfo = new Hashtable { { wxOpenid, userModel } };
            }
        }
        #endregion

        #region BindUserSearch 查询登录缓存信息
        /// <summary>
        /// 查询登录缓存信息
        /// </summary>
        /// <param name="wxOpenid"></param>
        /// <returns></returns>
        public OpenRequestModel BindUserSearch(string wxOpenid)
        {
            var model = new OpenRequestModel();
            model.WeixinOpenId = wxOpenid;

            if (HtLoginUserInfo != null)
            {
                if (HtLoginUserInfo.ContainsKey(wxOpenid))
                {
                    var objModel = (ManageUserLite)HtLoginUserInfo[wxOpenid];
                    TimeSpan ts = DateTime.Now - objModel.CacheTime;
                    if (ts.TotalMinutes > 10)
                    {
                        model.OperatorId = objModel.OperatorId;
                    }
                }
            }

            return model;
        }
        #endregion

        #region SendRequest 发送代理请求(Post)
        /// <summary>
        /// 发送代理请求(Post)
        /// </summary>
        /// <param name="accInfo">请求参数</param>
        /// <param name="requestName">请求名称</param>
        /// <param name="requestJson">请求参数(Json)</param>
        /// <returns></returns>
        public ProxyResponseModel SendRequest(OpenRequestModel userInfo, string requestName, string requestJson = "")
        {

            var requestMd = CreateAuthCode();
            requestMd.OperatorId = userInfo.OperatorId;
            requestMd.UserPower = userInfo.UserPower;
            requestMd.WeixinOpenId = userInfo.WeixinOpenId;
            requestMd.RequestName = requestName;
            requestMd.RequestJson = requestJson;

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("signature", requestMd.Signature);
            parameters.Add("timestamp", requestMd.Timestamp);
            parameters.Add("nonce", requestMd.Nonce);
            parameters.Add("operatorid", requestMd.OperatorId.ToString());
            parameters.Add("userpower", requestMd.UserPower.ToString());
            parameters.Add("openid", requestMd.WeixinOpenId);
            parameters.Add("requestname", requestMd.RequestName);
            parameters.Add("requestjson", requestMd.RequestJson);

            string strResult = Helper.SendHttpPost(ProxyUrl, parameters);

            var objResponse = Helper.JsonDeserializeObject<ProxyResponseModel>(strResult);

            //缓存关联信息
            if (userInfo.OperatorId == 0 && objResponse.OperatorId != 0)
            {
                var userModel = new ManageUserLite();
                userModel.OperatorId = objResponse.OperatorId;
                userModel.UserPower = 0;
                userModel.WeixinOpenid = userInfo.WeixinOpenId;
                userModel.CacheTime = DateTime.Now;

                BindUserUpdate(userInfo.WeixinOpenId, userModel);
            }

            //异步委托记录响应日志
            try
            {
                ResponseLogModel reModel = new ResponseLogModel();
                reModel.WxOpenid = userInfo.WeixinOpenId;
                reModel.OperatorId = objResponse.OperatorId;
                reModel.OrderName = requestName;
                reModel.OrderStatus = objResponse.Status;
                reModel.ObjectStr = objResponse.StrObj;
                reModel.ErrDesc = objResponse.ErrDesc;

                ResponseLogDelegate FnDelegate = new ResponseLogDelegate(SetResponseLog);
                IAsyncResult iResult = FnDelegate.BeginInvoke(reModel, new AsyncCallback(ar =>
                {
                    ResponseLogDelegate dele = (ResponseLogDelegate)((AsyncResult)ar).AsyncDelegate;
                    dele.EndInvoke(ar);
                }), null);
            }
            catch
            {
            }

            return objResponse;
        }
        #endregion

        #region SetResponseLog 写入响应日志信息
        /// <summary>
        /// 写入响应日志信息
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public bool SetResponseLog(ResponseLogModel response)
        {
            bool bResult = false;
            var strSql = new StringBuilder();
            strSql.Append("INSERT INTO T_Response_Sys(createTime, wxOpenid, operatorId, orderName, orderStatus, objectStr, errDesc)");
            strSql.Append(" VALUES (getdate(),@wxOpenid,@operatorId,@orderName,@orderStatus,@objectStr,@errDesc);");

            SqlParameter[] parameters = { 
                new SqlParameter("@wxOpenid", SqlDbType.NVarChar,200),
                new SqlParameter("@operatorId", SqlDbType.Int),
                new SqlParameter("@orderName", SqlDbType.NVarChar,200),
                new SqlParameter("@orderStatus", SqlDbType.Int),
                new SqlParameter("@objectStr", SqlDbType.NVarChar,200),
                new SqlParameter("@errDesc", SqlDbType.NVarChar,200)};
            parameters[0].Value = response.WxOpenid;
            parameters[1].Value = response.OperatorId;
            parameters[2].Value = response.OrderName;
            parameters[3].Value = response.OrderStatus;
            parameters[4].Value = response.ObjectStr;
            parameters[5].Value = response.ErrDesc;

            int iResult = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            if (iResult > 0)
            {
                bResult = true;
            }
            return bResult;
        }
        #endregion
    }
}
