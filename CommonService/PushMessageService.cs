using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using CommonLib;
using I200_Quartz.Helpers;
using Model;
using Senparc.Weixin.MP.Entities;

namespace CommonService
{
    public static class PushMessageService
    {
        /// <summary>
        /// 更新推送销售信息的状态（数据异常处理）
        /// </summary>
        /// <returns></returns>
        public static int UpdateSalesInfoStatus(string date)
        {
            var strSql = "update [dbo].[T_SalesInfo] set IsSend=1 where datediff(day,CurrentToday,@date )= 0";
            var result = DapperHelper.Execute(strSql.ToString(), new {date = date});
            return result;
        }


        /// <summary>
        /// 记录请求日志
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="requestData"></param>
        /// <param name="responseData"></param>
        /// <param name="date"></param>
        public static void LoggingRequestInfo(string userId, string requestData, string responseData, DateTime createTime)
        {
            var strSql =
                "  INSERT INTO T_RequestData_Log(UserId,RequestData ,ResponseData,createTime) VALUES(@UserId,@RequestData,'" + responseData + "',@CreateTime);";
            var result = DapperHelper.Execute(strSql.ToString(),
                new { UserId = userId, RequestData = requestData, CreateTime = createTime });
        }


        /// <summary>
        /// 根据类型获取销售数据（本月、本周）
        /// </summary>
        /// <param name="type"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public static string GetSalesInfoByType(int type)
        {
            var salesInfo = new ApiModel.SalesInfo();
            var nowDate = DateTime.Now;
            //var startTime = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            //往后延迟9小时，防止数据未同步带来的问题（例如：22号9点之前，只能查到20号的数据，过了9点才能查询到21号的数据）
            var   factTime = DateTime.Now.AddDays(-1).AddHours(-9).ToString("yyyy-MM-dd");
            var   endTime = DateTime.Now.ToString("yyyy-MM-dd");

            if (type==0)
            {
                #region 昨日数据
                var strSql = new StringBuilder();
                strSql.Append("SELECT EveryDayActiveRate,ThisWeekDeduplicationActiveRate,ThisMonthDeduplicationActiveRate,SumAccNum,EveryDayActive,ThisWeekDeduplicationActive,ThisMonthDeduplicationActive, NewAccNum, AccountNum as UserNum, AddGoodsNum , SmsNum, outLayNum, OrderNum , OrderMoney, ActiveNum, OldSalesNum, OldSalesMoney, SalesNum, SalesMoney,SalesCategoryNum as SaleKinds,IsSend from [T_SalesInfo] where CONVERT(varchar(10),CurrentToday, 23)=@CurrentToday;");
                salesInfo = DapperHelper.GetModel<ApiModel.SalesInfo>(strSql.ToString(), new { StartTime = factTime, CurrentToday = factTime });
                if (salesInfo.IsSend==1)
                {
                    //如果数据存在异常,则取前一天的数据
                    var visualDate = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd");
                    salesInfo = DapperHelper.GetModel<ApiModel.SalesInfo>(strSql.ToString(), new { StartTime = visualDate, CurrentToday = visualDate });
                }
                #endregion
            }
            else if (type==1)
            {
                #region 上周数据

                int today = Convert.ToInt32(DateTime.Now.DayOfWeek);
                var startTime = DateTime.Now.AddDays(-today - 6).ToString("yyyy-MM-dd");
                endTime = DateTime.Now.AddDays(-today).ToString("yyyy-MM-dd");
                var strSql=new StringBuilder();
                var tempStrSql=new StringBuilder();
                tempStrSql.Append("SELECT  IsSend from [T_SalesInfo] where CONVERT(varchar(10),CurrentToday, 23)= @FactTime;");
                salesInfo = DapperHelper.GetModel<ApiModel.SalesInfo>(tempStrSql.ToString(), new { FactTime = factTime });
                if (salesInfo.IsSend==1)
                {
                    factTime = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd");
                }
                strSql.Append("DECLARE @EveryDayActive int;");
                strSql.Append("DECLARE @ThisWeekDeduplicationActive int;");
                strSql.Append("DECLARE @ThisMonthDeduplicationActive int;");
                strSql.Append("DECLARE @EveryDayActiveRate DECIMAL(18,2);");
                strSql.Append("DECLARE @ThisWeekDeduplicationActiveRate DECIMAL(18,2);");
                strSql.Append("DECLARE @ThisMonthDeduplicationActiveRate DECIMAL(18,2);");
                strSql.Append("DECLARE @SumAccNum int;");
                strSql.Append("set @EveryDayActive=(select EveryDayActive from T_SalesInfo where datediff(day,CurrentToday,@FactTime )=0); ");
                strSql.Append("set @ThisWeekDeduplicationActive=(select ThisWeekDeduplicationActive from T_SalesInfo where datediff(day,CurrentToday,@FactTime )=0); ");
                strSql.Append("set @ThisMonthDeduplicationActive=(select ThisMonthDeduplicationActive from T_SalesInfo where datediff(day,CurrentToday,@FactTime )=0); ");
                strSql.Append("set @EveryDayActiveRate=(select EveryDayActiveRate from T_SalesInfo where datediff(day,CurrentToday,@FactTime)=0); ");
                strSql.Append("set @ThisWeekDeduplicationActiveRate=(select ThisWeekDeduplicationActiveRate from T_SalesInfo where datediff(day,CurrentToday,@FactTime)=0); ");
                strSql.Append("set @ThisMonthDeduplicationActiveRate=(select ThisMonthDeduplicationActiveRate from T_SalesInfo where datediff(day,CurrentToday,@FactTime)=0); ");
                strSql.Append("set @SumAccNum=(select SumAccNum from T_SalesInfo where datediff(day,CurrentToday,@FactTime)=0); ");
                strSql.Append("SELECT  @SumAccNum as SumAccNum,@EveryDayActiveRate as EveryDayActiveRate,@ThisWeekDeduplicationActiveRate as ThisWeekDeduplicationActiveRate,@ThisMonthDeduplicationActiveRate as ThisMonthDeduplicationActiveRate,@EveryDayActive as EveryDayActive,@ThisWeekDeduplicationActive as ThisWeekDeduplicationActive,@ThisMonthDeduplicationActive as ThisMonthDeduplicationActive,sum( NewAccNum) as NewAccNum,sum( AccountNum) as UserNum , SUM( AddGoodsNum) as AddGoodsNum , sum(SmsNum) as SmsNum,sum( outLayNum) as outLayNum, sum(OrderNum) as OrderNum ,sum( OrderMoney) as OrderMoney,sum( ActiveNum) as ActiveNum,sum( OldSalesNum) as OldSalesNum,sum( OldSalesMoney) as OldSalesMoney,sum( SalesNum) as SalesNum,sum( SalesMoney) as SalesMoney,sum( SalesCategoryNum) as SaleKinds from [T_SalesInfo] where CONVERT(varchar(10),CurrentToday, 23)>=@StartTime and CONVERT(varchar(10),CurrentToday, 23)<=@EndTime;");
                salesInfo = DapperHelper.GetModel<ApiModel.SalesInfo>(strSql.ToString(), new { StartTime = startTime, EndTime = endTime, FactTime = factTime });

                #endregion
                
            }
            else if (type==2)
            {
                #region 本月销售数据

                var startTime = new DateTime(nowDate.Year, nowDate.Month, 1).ToString("yyyy-MM-dd");//本月一号
                var strSql=new StringBuilder();
                var dayOfMonth = DateTime.Now.Day;
                var tempStrSql = new StringBuilder();
                tempStrSql.Append("SELECT  IsSend from [T_SalesInfo] where CONVERT(varchar(10),CurrentToday, 23)= @FactTime;");
                salesInfo = DapperHelper.GetModel<ApiModel.SalesInfo>(tempStrSql.ToString(), new { FactTime = factTime });
               //如果昨日数据异常，则取前日的数据，如果是本月一号，由于数据未同步，所以取上月最后一天的数据。
                if (salesInfo.IsSend == 1)
                {
                    factTime = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd");
                    if (dayOfMonth == 1)
                    {
                        startTime = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd");
                        endTime = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                    }
                }
                else
                {
                    if (dayOfMonth == 1)
                    {
                        startTime = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                    }
                }
                strSql.Append("DECLARE @EveryDayActive int;");
                strSql.Append("DECLARE @ThisWeekDeduplicationActive int;");
                strSql.Append("DECLARE @ThisMonthDeduplicationActive int;");
                strSql.Append("DECLARE @EveryDayActiveRate DECIMAL(18,2);");
                strSql.Append("DECLARE @ThisWeekDeduplicationActiveRate DECIMAL(18,2);");
                strSql.Append("DECLARE @ThisMonthDeduplicationActiveRate DECIMAL(18,2);");
                strSql.Append("DECLARE @SumAccNum int;");
                strSql.Append("set @EveryDayActive=(select EveryDayActive from T_SalesInfo where datediff(day,CurrentToday,@FactTime )=0); ");
                strSql.Append("set @ThisWeekDeduplicationActive=(select ThisWeekDeduplicationActive from T_SalesInfo where datediff(day,CurrentToday,@FactTime )=0); ");
                strSql.Append("set @ThisMonthDeduplicationActive=(select ThisMonthDeduplicationActive from T_SalesInfo where datediff(day,CurrentToday,@FactTime )=0); ");
                strSql.Append("set @EveryDayActiveRate=(select EveryDayActiveRate from T_SalesInfo where datediff(day,CurrentToday,@FactTime)=0); ");
                strSql.Append("set @ThisWeekDeduplicationActiveRate=(select ThisWeekDeduplicationActiveRate from T_SalesInfo where datediff(day,CurrentToday,@FactTime)=0); ");
                strSql.Append("set @ThisMonthDeduplicationActiveRate=(select ThisMonthDeduplicationActiveRate from T_SalesInfo where datediff(day,CurrentToday,@FactTime)=0); ");
                strSql.Append("set @SumAccNum=(select SumAccNum from T_SalesInfo where datediff(day,CurrentToday,@FactTime)=0); ");
                strSql.Append("SELECT @SumAccNum as SumAccNum,@EveryDayActiveRate as EveryDayActiveRate,@ThisWeekDeduplicationActiveRate as ThisWeekDeduplicationActiveRate,@ThisMonthDeduplicationActiveRate as ThisMonthDeduplicationActiveRate,@EveryDayActive as EveryDayActive,@ThisWeekDeduplicationActive as ThisWeekDeduplicationActive,@ThisMonthDeduplicationActive as ThisMonthDeduplicationActive,sum( NewAccNum) as NewAccNum,sum( AccountNum) as UserNum ,sum( AddGoodsNum) as AddGoodsNum , sum(SmsNum) as SmsNum,sum( outLayNum) as outLayNum, sum(OrderNum) as OrderNum ,sum( OrderMoney) as OrderMoney,sum( ActiveNum) as ActiveNum,sum( OldSalesNum) as OldSalesNum,sum( OldSalesMoney) as OldSalesMoney,sum( SalesNum) as SalesNum,sum( SalesMoney) as SalesMoney,sum( SalesCategoryNum) as SaleKinds from [T_SalesInfo] where CONVERT(varchar(10),CurrentToday, 23)>=@StartTime and CONVERT(varchar(10),CurrentToday, 23)<@EndTime;");
                salesInfo = DapperHelper.GetModel<ApiModel.SalesInfo>(strSql.ToString(), new { StartTime = startTime, EndTime = endTime, FactTime = factTime });

                #endregion
            }
            else if (type==3)
            {
                #region 本周销售数据

                var days = Convert.ToInt32(nowDate.DayOfWeek);
                var startTime = nowDate.AddDays(-days + 1).ToString("yyyy-MM-dd"); //本周第一天
                var strSql=new StringBuilder();
                var tempStrSql = new StringBuilder();
                var visualDate = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd");
                tempStrSql.Append("SELECT  IsSend from [T_SalesInfo] where CONVERT(varchar(10),CurrentToday, 23)= @FactTime;");
                salesInfo = DapperHelper.GetModel<ApiModel.SalesInfo>(tempStrSql.ToString(), new { FactTime = factTime });
                if (salesInfo.IsSend == 1)
                {
                    factTime = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd");
                    if (days == 1)
                    {
                        startTime = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd");
                    }
                }
                else
                {
                    if (days == 1)
                    {
                        startTime = nowDate.AddDays(-1).ToString("yyyy-MM-dd");
                        endTime = DateTime.Now.ToString("yyyy-MM-dd");
                    }
                }
                strSql.Append("DECLARE @EveryDayActive int;");
                strSql.Append("DECLARE @ThisWeekDeduplicationActive int;");
                strSql.Append("DECLARE @ThisMonthDeduplicationActive int;");
                strSql.Append("DECLARE @EveryDayActiveRate DECIMAL(18,2);");
                strSql.Append("DECLARE @ThisWeekDeduplicationActiveRate DECIMAL(18,2);");
                strSql.Append("DECLARE @ThisMonthDeduplicationActiveRate DECIMAL(18,2);");
                strSql.Append("DECLARE @SumAccNum int;");
                strSql.Append("set @EveryDayActive=(select EveryDayActive from T_SalesInfo where datediff(day,CurrentToday,@FactTime )=0); ");
                strSql.Append("set @ThisWeekDeduplicationActive=(select ThisWeekDeduplicationActive from T_SalesInfo where datediff(day,CurrentToday,@FactTime )=0); ");
                strSql.Append("set @ThisMonthDeduplicationActive=(select ThisMonthDeduplicationActive from T_SalesInfo where datediff(day,CurrentToday,@FactTime )=0); ");
                strSql.Append("set @EveryDayActiveRate=(select EveryDayActiveRate from T_SalesInfo where datediff(day,CurrentToday,@FactTime)=0); ");
                strSql.Append("set @ThisWeekDeduplicationActiveRate=(select ThisWeekDeduplicationActiveRate from T_SalesInfo where datediff(day,CurrentToday,@FactTime)=0); ");
                strSql.Append("set @ThisMonthDeduplicationActiveRate=(select ThisMonthDeduplicationActiveRate from T_SalesInfo where datediff(day,CurrentToday,@FactTime)=0); ");
                strSql.Append("set @SumAccNum=(select SumAccNum from T_SalesInfo where datediff(day,CurrentToday,@FactTime)=0); ");
                strSql.Append("SELECT @SumAccNum as SumAccNum,@EveryDayActiveRate as EveryDayActiveRate,@ThisWeekDeduplicationActiveRate as ThisWeekDeduplicationActiveRate,@ThisMonthDeduplicationActiveRate as ThisMonthDeduplicationActiveRate,@EveryDayActive as EveryDayActive,@ThisWeekDeduplicationActive as ThisWeekDeduplicationActive,@ThisMonthDeduplicationActive as ThisMonthDeduplicationActive,sum( NewAccNum) as NewAccNum,sum( AccountNum) as UserNum ,sum( AddGoodsNum) as AddGoodsNum , sum(SmsNum) as SmsNum,sum( outLayNum) as outLayNum, sum(OrderNum) as OrderNum ,sum( OrderMoney) as OrderMoney,sum( ActiveNum) as ActiveNum,sum( OldSalesNum) as OldSalesNum,sum( OldSalesMoney) as OldSalesMoney,sum( SalesNum) as SalesNum,sum( SalesMoney) as SalesMoney,sum( SalesCategoryNum) as SaleKinds from [T_SalesInfo] where CONVERT(varchar(10),CurrentToday, 23)>=@StartTime and CONVERT(varchar(10),CurrentToday, 23)<@EndTime");  
                salesInfo = DapperHelper.GetModel<ApiModel.SalesInfo>(strSql.ToString(), new { StartTime = startTime, EndTime = endTime, FactTime = factTime, CurrentToday= visualDate });
                #endregion
            }
            return Helper.JsonSerializeObject(salesInfo);
        }
    }
}