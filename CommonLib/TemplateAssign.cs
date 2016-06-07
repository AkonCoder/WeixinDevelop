using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLib
{
   public static class TemplateAssign
    {
       /// <summary>
       /// 销售信息
       /// </summary>
       /// <param name="oResult"></param>
       /// <param name="type"></param>
       /// <returns></returns>
       public static string SalesInfo(ApiModel.SalesInfo oResult,int type)
       {
           var strResult = new StringBuilder();

           strResult.Append(string.Format("注册数：{0}个\r\n", oResult.SumAccNum));
           strResult.Append(string.Format("新增店铺：{0}个\r\n", oResult.NewAccNum));
           strResult.Append(string.Format("新增会员：{0}个\r\n", oResult.UserNum));
           strResult.Append(string.Format("新增商品：{0}种\r\n", oResult.AddGoodsNum));
           strResult.Append(string.Format("短信：{0}条\r\n", oResult.SmsNum));
           strResult.Append(string.Format("订单：{0}个(¥{1})\r\n", oResult.OrderNum, oResult.OrderMoney));
           //strResult.Append(string.Format("订单金额：¥{0}\r\n", oResult.OrderMoney));
           strResult.Append(string.Format("昨日活跃： {0}家({1}%)\r\n", oResult.EveryDayActive, oResult.EveryDayActiveRate));
           strResult.Append(string.Format("7天活跃： {0}家({1}%)\r\n", oResult.ThisWeekDeduplicationActive, oResult.ThisWeekDeduplicationActiveRate));
           strResult.Append(string.Format("30天活跃： {0}家({1}%)\r\n", oResult.ThisMonthDeduplicationActive, oResult.ThisMonthDeduplicationActiveRate));
           strResult.Append(string.Format("销售笔数：{0}笔\r\n", oResult.SalesNum));
           strResult.Append(string.Format("销售金额：¥{0}\r\n", oResult.SalesMoney));
           //strResult.Append(string.Format("店铺登录：{0}个\r\n", oResult.LoginNum));
           //strResult.Append(string.Format("支出信息：{0}笔\r\n", oResult.OutLayNum));
           return strResult.ToString();
       }

    }
}
