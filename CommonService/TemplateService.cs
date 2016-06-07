using CommonLib;
using Model;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.CommonAPIs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using I200_Quartz.Helpers;
using User = Model.User;

namespace CommonService
{
    /// <summary>
    /// 模板处理
    /// </summary>
    public class TemplateService
    {
        private delegate void SendTemplateMessageDelegate(Dictionary<string, Model.TemplateMessage> list);

        private string AppId = System.Configuration.ConfigurationManager.AppSettings["AppId"].ToString();

        /// <summary>
        /// 推送信息
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public SendTemplateMessageResult SendTemplateMessage(TemplateMessage template)
        {
            if (!AccessTokenContainer.CheckRegistered(AppId))
            {
                AccessTokenContainer.Register(AppId,
                    System.Configuration.ConfigurationManager.AppSettings["AppSecret"].ToString());
            }
            var accessToken = AccessTokenContainer.GetToken(AppId);

            SendTemplateMessageResult smr = Senparc.Weixin.MP.AdvancedAPIs.Template.SendTemplateMessage(accessToken,
                template.touser, template.TemplateId, template.topcolor, template.url, template.data);
            return smr;
        }

        public void SendTemplate(string[] toUsers, Model.TemplateMessage templateModel)
        {
            if (toUsers != null && toUsers.Length > 0)
            {
                foreach (string toUser in toUsers)
                {
                    if (toUser.Length > 0)
                    {
                        templateModel.touser = toUser;
                        CommonService.TemplateService service = new CommonService.TemplateService();

                        SendTemplateMessageResult smr = service.SendTemplateMessage(templateModel);
                        if (smr.errmsg != "ok")
                        {
                            CommonLib.Logger.Info(CommonLib.Helper.JsonSerializeObject(smr));
                        }
                        Thread.Sleep(1000);
                    }
                }
            }
        }

        public void SendTemplate(Dictionary<string, Model.TemplateMessage> list)
        {
            foreach (KeyValuePair<string, Model.TemplateMessage> itemList in list)
            {
                if (itemList.Key.Length > 0)
                {
                    itemList.Value.touser = itemList.Key;
                    CommonService.TemplateService service = new CommonService.TemplateService();

                    SendTemplateMessageResult smr = service.SendTemplateMessage(itemList.Value);
                    if (smr.errmsg != "ok")
                    {
                        CommonLib.Logger.Info(CommonLib.Helper.JsonSerializeObject(smr));
                    }
                    //记录推送消息的日志
                    LoggingPushDatas(itemList.Key.ToString(), DateTime.Now, itemList.Value.data["remark"].value);
                    Thread.Sleep(1000);
                }
            }
        }

        /// <summary>
        /// 记录推送消息的信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="date"></param>
        /// <param name="tpMessage"></param>
        public void LoggingPushDatas(string userId, DateTime date, string tpMessage)
        {
            try
            {
                var strSql =
                    " INSERT INTO dbo.T_PushSalesInfo_Log (UserId, Detail, sendTime ) VALUES (@UserId,@Detail,@sendTime)";
                var result = DapperHelper.Execute(strSql, new {UserId = userId, Detail = tpMessage, sendTime = date});
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        /// <summary>
        /// 推送销售汇总信息
        /// </summary>
        /// <param name="activeNum"></param>
        /// <param name="everydayActive"></param>
        /// <param name="salesNum"></param>
        /// <param name="currentToday"></param>
        /// <param name="loginNum"></param>
        /// <param name="newAccountNum"></param>
        /// <param name="accountNum"></param>
        /// <param name="addGoodsNum"></param>
        /// <param name="smsNum"></param>
        /// <param name="orderNum"></param>
        /// <param name="orderMoney"></param>
        /// <param name="salesMoney"></param>
        /// <param name="salesCategoryNum"></param>
        /// <param name="outLayNum"></param>
        /// <param name="thisWeekDeduplicationActive"></param>
        /// <param name="thisMonthDeduplicationActive"></param>
        /// <param name="everyDayActiveRate"></param>
        /// <param name="thisWeekDeduplicationActiveRate"></param>
        /// <param name="thisMonthDeduplicationActiveRate"></param>
        /// <param name="sumAccNum"></param>
        /// <returns></returns>
        public int PushSalesReport(string currentToday, int loginNum, int newAccountNum, int accountNum, int addGoodsNum,
            int smsNum,
            int orderNum, decimal orderMoney, int activeNum, int everydayActive, int salesNum, decimal salesMoney,
            int salesCategoryNum, int outLayNum, int thisWeekDeduplicationActive, int thisMonthDeduplicationActive,
            double everyDayActiveRate, double thisWeekDeduplicationActiveRate, double thisMonthDeduplicationActiveRate, int sumAccNum)
        {
            Model.TemplateMessage templateModel = new Model.TemplateMessage();
            var nowhour = DateTime.Now.Hour;
            string[] toInnerUserS =
                System.Configuration.ConfigurationManager.AppSettings["PushInnerUsers"].ToString().Split(',');
            string[] toAssignUserS =
                System.Configuration.ConfigurationManager.AppSettings["PushRealUsers"].ToString().Split(',');

            templateModel.url = "";
            templateModel.topcolor = "";
            templateModel.data["first"] = new TemplateItem("");
            templateModel.data["currentToday"] = new TemplateItem((currentToday).ToString());
            templateModel.data["loginNum"] = new TemplateItem((loginNum).ToString());
            templateModel.data["newAccountNum"] = new TemplateItem((newAccountNum).ToString());
            templateModel.data["accountNum"] = new TemplateItem((accountNum).ToString());
            templateModel.data["addGoodsNum"] = new TemplateItem((addGoodsNum).ToString());
            templateModel.data["smsNum"] = new TemplateItem((smsNum).ToString());
            templateModel.data["orderNum"] = new TemplateItem((orderNum).ToString());
            templateModel.data["orderMoney"] = new TemplateItem((orderMoney).ToString());
            templateModel.data["activeNum"] = new TemplateItem((activeNum).ToString());
            templateModel.data["everydayActive"] = new TemplateItem((everydayActive).ToString());
            templateModel.data["outLayNum"] = new TemplateItem((outLayNum).ToString());
            templateModel.data["salesNum"] = new TemplateItem((salesNum).ToString());
            templateModel.data["salesMoney"] = new TemplateItem((salesMoney).ToString());
            templateModel.data["salesCategoryNum"] = new TemplateItem((salesCategoryNum).ToString());
            templateModel.data["thisWeekDeduplicationActive"] =
                new TemplateItem((thisWeekDeduplicationActive).ToString());
            templateModel.data["thisMonthDeduplicationActive"] =
                new TemplateItem((thisMonthDeduplicationActive).ToString());
            templateModel.data["everyDayActiveRate"] =
                new TemplateItem((everyDayActiveRate).ToString());
            templateModel.data["thisWeekDeduplicationActiveRate"] =
                new TemplateItem((thisWeekDeduplicationActiveRate).ToString());
            templateModel.data["thisMonthDeduplicationActiveRate"] =
                new TemplateItem((thisMonthDeduplicationActiveRate).ToString());
            templateModel.data["sumAccNum"] =
            new TemplateItem((sumAccNum).ToString());
            templateModel.data["keyword2"] = new TemplateItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            templateModel.data["keyword1"] = new TemplateItem("平台信息（昨日）");
            var strResult = new StringBuilder();
            var salesInfo = new ApiModel.SalesInfo();
            salesInfo.LoginNum = loginNum;
            salesInfo.NewAccNum = newAccountNum;
            salesInfo.UserNum = accountNum;
            salesInfo.AddGoodsNum = addGoodsNum;
            salesInfo.SmsNum = smsNum;
            salesInfo.OutLayNum = outLayNum;
            salesInfo.OrderNum = orderNum;
            salesInfo.OrderMoney = orderMoney;
            salesInfo.EveryDayActive = everydayActive;
            salesInfo.SalesNum = salesNum;
            salesInfo.SalesMoney = salesMoney;
            salesInfo.ThisWeekDeduplicationActive = thisWeekDeduplicationActive;
            salesInfo.ThisMonthDeduplicationActive = thisMonthDeduplicationActive;
            salesInfo.SumAccNum = sumAccNum;
            salesInfo.EveryDayActiveRate = everyDayActiveRate;
            salesInfo.ThisWeekDeduplicationActiveRate = thisWeekDeduplicationActiveRate;
            salesInfo.ThisMonthDeduplicationActiveRate = thisMonthDeduplicationActiveRate;
            var dateType = -1;
            strResult.Append(CommonLib.TemplateAssign.SalesInfo(salesInfo, dateType));
            templateModel.data["remark"] = new TemplateItem(strResult.ToString());
            Dictionary<string, Model.TemplateMessage> list = new Dictionary<string, TemplateMessage>();

            var timeNode = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["TimeNode"].ToString());
            if (nowhour < timeNode)
            {
                //八点执行推送的用户（内部用户）
                //推送用户的OpenId
                var innerStrSql =
                    "select UserOpenId,IsOpen,UserType,Remark from T_AttentionUsers where IsOpen=1 and UserType=0;";
                var userList = DapperHelper.Query<User>(innerStrSql).ToList();

                if (userList.Count > 0)
                {
                    foreach (var item in userList)
                    {
                        list.Add(item.UserOpenId, templateModel);
                    }
                }
            }
            else
            {
                //九点执行推送的用户（投资者）
                //推送用户的OpenId
                var outterStrSql =
                    "select UserOpenId,IsOpen,UserType,Remark from T_AttentionUsers where IsOpen=1 and UserType=1;";
                var userList = DapperHelper.Query<User>(outterStrSql).ToList();

                if (userList.Count > 0)
                {
                    foreach (var item in userList)
                    {
                        list.Add(item.UserOpenId, templateModel);
                    }
                }
            }

            if (list.Count > 0)
            {
                var tokenDelegate = new SendTemplateMessageDelegate(SendTemplate);
                tokenDelegate.BeginInvoke(list, null, null);
                return list.Count;
            }
            else
            {
                return 0;
            }
        }
    }
}