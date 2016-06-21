using System;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Senparc.Weixin.MP.Agent;
using Senparc.Weixin.Context;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Helpers;
using Senparc.Weixin.MP.MessageHandlers;
using System.Text;
using CommonLib;
using Model;
using System.Collections.Generic;
using System.IO;
using I200_Quartz.Helpers;

namespace CommonService.CustomMessageHandler
{
    /// <summary>
    /// 自定义MessageHandler
    /// </summary>
    public partial class CustomMessageHandler
    {
        private string GetWelcomeInfo()
        {
            return "欢迎关注我的账号！";
        }

        public override IResponseMessageBase OnTextOrEventRequest(RequestMessageText requestMessage)
        {
            // 预处理文字或事件类型请求。
            // 这个请求是一个比较特殊的请求，通常用于统一处理来自文字或菜单按钮的同一个执行逻辑，
            // 会在执行OnTextRequest或OnEventRequest之前触发，具有以下一些特征：
            // 1、如果返回null，则继续执行OnTextRequest或OnEventRequest
            // 2、如果返回不为null，则终止执行OnTextRequest或OnEventRequest，返回最终ResponseMessage
            // 3、如果是事件，则会将RequestMessageEvent自动转为RequestMessageText类型，其中RequestMessageText.Content就是RequestMessageEvent.EventKey

            return null;
        }

        public override IResponseMessageBase OnEvent_ClickRequest(RequestMessageEvent_Click requestMessage)
        {
            IResponseMessageBase reponseMessage = null;
            var dateType = 0;
            string responseContent = string.Empty;
            var userOpenId = requestMessage.FromUserName;
            
            StringBuilder strSql= new StringBuilder();
            strSql.Append("select  Id from T_AttentionUsers where UserOpenId=@UserOpenId and IsOpen=1");
            int affectedRows = 0;
            try
            {
                affectedRows = DapperHelper.GetModel<int>(strSql.ToString(), new { UserOpenId = userOpenId });
                PushMessageService.LoggingRequestInfo(affectedRows.ToString(), requestMessage.EventKey.ToString(), "", DateTime.Now);
            }
            catch (Exception ex)
            {
                PushMessageService.LoggingRequestInfo(requestMessage.FromUserName, requestMessage.EventKey.ToString(), ex.ToString(), DateTime.Now);
            }

            if (affectedRows < 1)
            {
                var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
                strongResponseMessage.Content = "该账号尚未绑定,请联系管理员";
                reponseMessage = strongResponseMessage;
                //记录请求日志信息
                PushMessageService.LoggingRequestInfo( requestMessage.FromUserName , requestMessage.EventKey.ToString(), "该用户无权限查询", DateTime.Now);
                return reponseMessage;
            }

            //菜单点击，需要跟创建菜单时的Key匹配
            switch (requestMessage.EventKey)
            {
                case "YesterdaySalesInfo":

                    #region 平台销售信息（昨日）
                    dateType = 0;
                    var yesterdaySalesInfo = PushMessageService.GetSalesInfoByType(dateType);
                    var yesterdaystrongResponseMessage = CreateResponseMessage<ResponseMessageNews>();
                    reponseMessage = yesterdaystrongResponseMessage;
                    var yesterdaySalesStr = new StringBuilder();
                    var yesterdaySalesResult = Helper.JsonDeserializeObject<ApiModel.SalesInfo>(yesterdaySalesInfo);
                    yesterdaySalesStr.Append(CommonLib.TemplateAssign.SalesInfo(yesterdaySalesResult,dateType));
                    yesterdaystrongResponseMessage.Articles.Add(new Article()
                    {
                        Title = "平台信息（昨日）",
                        Description = yesterdaySalesStr.ToString()
                    });
                    responseContent = yesterdaySalesStr.ToString();
                    #endregion
                    break;

                case "LastWeekSalesInfo":

                    #region 平台销售信息（上周）
                    dateType = 1;
                    var lastWeekSalesInfo = PushMessageService.GetSalesInfoByType(dateType);
                    var lastWeekstrongResponseMessage = CreateResponseMessage<ResponseMessageNews>();
                    reponseMessage = lastWeekstrongResponseMessage;
                    var lastWeekSalesStr = new StringBuilder();

                    var lastWeekSalesResult = Helper.JsonDeserializeObject<ApiModel.SalesInfo>(lastWeekSalesInfo);
                    lastWeekSalesStr.Append(CommonLib.TemplateAssign.SalesInfo(lastWeekSalesResult,dateType));
                    lastWeekstrongResponseMessage.Articles.Add(new Article()
                    {
                        Title = "平台信息（上周）",
                        Description = lastWeekSalesStr.ToString()
                    });
                    responseContent = lastWeekSalesStr.ToString();
                    #endregion
                    break;

                case "ThisMonthSalesInfo":

                    #region 平台销售信息（本月）

                    dateType = 2;
                    var thisMonthSalesInfo = PushMessageService.GetSalesInfoByType(dateType);
                    var thisMonthstrongResponseMessage = CreateResponseMessage<ResponseMessageNews>();
                    reponseMessage = thisMonthstrongResponseMessage;
                    var thisMonthSalesStr = new StringBuilder();
                    var thisMonthSalesResult = Helper.JsonDeserializeObject<ApiModel.SalesInfo>(thisMonthSalesInfo);
                    thisMonthSalesStr.Append(CommonLib.TemplateAssign.SalesInfo(thisMonthSalesResult,dateType));
                    thisMonthstrongResponseMessage.Articles.Add(new Article()
                    {
                        Title = "平台信息 (本月)",
                        Description = thisMonthSalesStr.ToString(),
                    });
                    responseContent = thisMonthSalesStr.ToString();
                    #endregion
                    break;
                case "ThisWeekSalesInfo":

                    #region 平台销售信息（本周）
                    dateType = 3;
                    var thisWeekSalesInfo = PushMessageService.GetSalesInfoByType(dateType);
                    var thisWeekstrongResponseMessage = CreateResponseMessage<ResponseMessageNews>();
                    reponseMessage = thisWeekstrongResponseMessage;
                    var thisWeekSalesStr = new StringBuilder();
                    var thisWeekSalesResult = Helper.JsonDeserializeObject<ApiModel.SalesInfo>(thisWeekSalesInfo);
                    thisWeekSalesStr.Append(CommonLib.TemplateAssign.SalesInfo(thisWeekSalesResult,dateType));
                    thisWeekstrongResponseMessage.Articles.Add(new Article()
                    {
                        Title = "平台信息（本周）",
                        Description = thisWeekSalesStr.ToString()
                    });
                    responseContent = thisWeekSalesStr.ToString();
                    #endregion
                    break;

                default:

                    #region

                {
                    var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
                    strongResponseMessage.Content = "尚未绑定";
                    reponseMessage = strongResponseMessage;
                    responseContent = "尚未绑定"; 
                }
                    break;

                    #endregion
            }

            //记录请求日志信息
            PushMessageService.LoggingRequestInfo(requestMessage.FromUserName, requestMessage.EventKey.ToString(), string.IsNullOrWhiteSpace(responseContent) ? "无返回数据" : responseContent, DateTime.Now);
             
            return reponseMessage;
        }

        public override IResponseMessageBase OnEvent_EnterRequest(RequestMessageEvent_Enter requestMessage)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            responseMessage.Content = "您刚才发送了ENTER事件请求。";
            return responseMessage;
        }

        public override IResponseMessageBase OnEvent_LocationRequest(RequestMessageEvent_Location requestMessage)
        {
            //这里是微信客户端（通过微信服务器）自动发送过来的位置信息
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "发现您的附近有家好吃的";
            return responseMessage; //这里也可以返回null（需要注意写日志时候null的问题）
        }

        public override IResponseMessageBase OnEvent_ScanRequest(RequestMessageEvent_Scan requestMessage)
        {
            //通过扫描关注
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "通过扫描关注。";
            return responseMessage;
        }

        public override IResponseMessageBase OnEvent_ViewRequest(RequestMessageEvent_View requestMessage)
        {
            //说明：这条消息只作为接收，下面的responseMessage到达不了客户端，类似OnEvent_UnsubscribeRequest
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "您点击了view按钮，将打开网页：" + requestMessage.EventKey;
            return responseMessage;
        }

        public override IResponseMessageBase OnEvent_MassSendJobFinishRequest(
            RequestMessageEvent_MassSendJobFinish requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "接收到了群发完成的信息。";
            return responseMessage;
        }

        /// <summary>
        /// 订阅（关注）事件
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            responseMessage.Content = GetWelcomeInfo();
            return responseMessage;
        }

        /// <summary>
        /// 退订
        /// 实际上用户无法收到非订阅账号的消息，所以这里可以随便写。
        /// unsubscribe事件的意义在于及时删除网站应用中已经记录的OpenID绑定，消除冗余数据。并且关注用户流失的情况。
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_UnsubscribeRequest(RequestMessageEvent_Unsubscribe requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "有空再来";
            return responseMessage;
        }

        /// <summary>
        /// 事件之扫码推事件(scancode_push)
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_ScancodePushRequest(
            RequestMessageEvent_Scancode_Push requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之扫码推事件";
            return responseMessage;
        }

        /// <summary>
        /// 事件之扫码推事件且弹出“消息接收中”提示框(scancode_waitmsg)
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_ScancodeWaitmsgRequest(
            RequestMessageEvent_Scancode_Waitmsg requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之扫码推事件且弹出“消息接收中”提示框";
            return responseMessage;
        }

        /// <summary>
        /// 事件之弹出拍照或者相册发图（pic_photo_or_album）
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_PicPhotoOrAlbumRequest(
            RequestMessageEvent_Pic_Photo_Or_Album requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之弹出拍照或者相册发图";
            return responseMessage;
        }

        /// <summary>
        /// 事件之弹出系统拍照发图(pic_sysphoto)
        /// 实际测试时发现微信并没有推送RequestMessageEvent_Pic_Sysphoto消息，只能接收到用户在微信中发送的图片消息。
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_PicSysphotoRequest(RequestMessageEvent_Pic_Sysphoto requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之弹出系统拍照发图";
            return responseMessage;
        }

        /// <summary>
        /// 事件之弹出微信相册发图器(pic_weixin)
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_PicWeixinRequest(RequestMessageEvent_Pic_Weixin requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之弹出微信相册发图器";
            return responseMessage;
        }

        /// <summary>
        /// 事件之弹出地理位置选择器（location_select）
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_LocationSelectRequest(
            RequestMessageEvent_Location_Select requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之弹出地理位置选择器";
            return responseMessage;
        }
    }
}