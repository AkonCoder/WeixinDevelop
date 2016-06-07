using CommonLib;
using CommonService;
using Model;
using Senparc.Weixin.MP.AdvancedAPIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web.Http;

namespace Web.Controllers
{
    public class PushReportController : ApiController
    {
        public int Get(string currentToday, int loginNum, int newAccountNum, int accountNum, int addGoodsNum, int smsNum,
            int orderNum, decimal orderMoney, int activeNum,int everydayActive, int salesNum, decimal salesMoney,
            int salesCategoryNum, int outLayNum, int thisWeekDeduplicationActive, int thisMonthDeduplicationActive,
            double everyDayActiveRate, double thisWeekDeduplicationActiveRate, double thisMonthDeduplicationActiveRate,int sumAccNum)
        {

            CommonService.TemplateService templateServ = new TemplateService();
            return templateServ.PushSalesReport(currentToday, loginNum, newAccountNum, accountNum, addGoodsNum, smsNum,
                orderNum, orderMoney, activeNum, everydayActive, salesNum, salesMoney, salesCategoryNum, outLayNum,
                 thisWeekDeduplicationActive, thisMonthDeduplicationActive,everyDayActiveRate,thisWeekDeduplicationActiveRate,thisMonthDeduplicationActiveRate,sumAccNum)
            ;
        }
    }
}
