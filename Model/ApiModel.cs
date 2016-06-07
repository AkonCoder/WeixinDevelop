using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class ApiModel
    {
        #region ManageUserModel 登录人员Model
        /// <summary>
        /// 登录人员Model
        /// </summary>
        public class ManageUserModel
        {
            /// <summary>
            /// 人员ID
            /// </summary>
            public int UserID { get; set; }

            /// <summary>
            /// 登录名
            /// </summary>
            public string UserName { get; set; }

            /// <summary>
            /// 登录权限
            /// </summary>
            public int PowerSession { get; set; }

            /// <summary>
            /// 登录次数
            /// </summary>
            public int LoginCnt { get; set; }

            /// <summary>
            /// 手机号码
            /// </summary>
            public string Phone { get; set; }

            /// <summary>
            /// 姓名
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 登录状态
            /// </summary>
            public bool LoginStatus { get; set; }

            /// <summary>
            /// 微信OpenId
            /// </summary>
            public string WeixinOpenid { get; set; }
        }
        #endregion

        #region IndexInfo 平台信息
        /// <summary>
        /// 平台信息
        /// </summary>
        public class IndexInfo
        {
            /// <summary>
            /// 店铺登录
            /// </summary>
            public int LoginNum { get; set; }

            /// <summary>
            /// 新增店铺
            /// </summary>
            public int RegShop { get; set; }

            /// <summary>
            /// 店铺会员
            /// </summary>
            public int ShopUser { get; set; }

            /// <summary>
            /// 新增商品
            /// </summary>
            public int GoodsNum { get; set; }

            /// <summary>
            /// 短信发送
            /// </summary>
            public int SendSms { get; set; }

            /// <summary>
            /// 销售笔数
            /// </summary>
            public int SaleNum { get; set; }

            /// <summary>
            /// 销售商品种类
            /// </summary>
            public int SaleKinds { get; set; }

            /// <summary>
            /// 销售金额
            /// </summary>
            public decimal SaleSum { get; set; }

            /// <summary>
            /// 支出笔数
            /// </summary>
            public int PayNum { get; set; }

            /// <summary>
            /// 支出金额
            /// </summary>
            public decimal PaySum { get; set; }

            /// <summary>
            /// 订单笔数
            /// </summary>
            public int OrderNum { get; set; }

            /// <summary>
            /// 订单金额
            /// </summary>
            public decimal OrderSum { get; set; }

            /// <summary>
            /// 客户反馈
            /// </summary>
            public int FeedbackNum { get; set; }

            /// <summary>
            /// 活跃用户
            /// </summary>
            public int ActiveNum { get; set; }

            /// <summary>
            /// 活跃率
            /// </summary>
            public decimal ActiveRate { get; set; }
            /// <summary>
            /// 安卓注册
            /// </summary>
            public int Android { get; set; }
            /// <summary>
            /// IOS 注册
            /// </summary>
            public int Ios { get; set; }
            /// <summary>
            /// DAU DD数据专用
            /// </summary>
            public int DAU { get; set; }
            /// <summary>
            /// 店铺总数
            /// </summary>
            public int AccountNum { get; set; }


        }

        public class IndexInfoToday : IndexInfo
        {
            /// <summary>
            /// 客户端登录数
            /// </summary>
            public int ClientNum { get; set; }

            /// <summary>
            /// 平台回访
            /// </summary>
            public int RevisitNum { get; set; }

            /// <summary>
            /// 微信关注
            /// </summary>
            public int WeixinFollow { get; set; }

        }
        #endregion

        #region TodayAccModel 今日注册店铺
        /// <summary>
        /// 今日注册店铺
        /// </summary>
        public class TodayAccModel
        {
            /// <summary>
            /// 店铺Id
            /// </summary>
            public int AccId { get; set; }

            /// <summary>
            /// 店铺名称
            /// </summary>
            public string AccName { get; set; }

            /// <summary>
            /// 店主姓名
            /// </summary>
            public string ManageName { get; set; }

            /// <summary>
            /// 手机号码
            /// </summary>
            public string Phone { get; set; }

            /// <summary>
            /// 电子邮箱
            /// </summary>
            public string Email { get; set; }

            /// <summary>
            /// 注册信息
            /// </summary>
            public DateTime RegDate { get; set; }
        }
        #endregion

        #region OrderInfo 订单信息
        /// <summary>
        /// 订单信息
        /// </summary>
        public class OrderInfo
        {
            /// <summary>
            /// Item数量
            /// </summary>
            public int RowNum { get; set; }

            /// <summary>
            /// 合计金额
            /// </summary>
            public decimal TotalMoney { get; set; }

            /// <summary>
            /// 订单列表
            /// </summary>
            public List<OrderInfoItem> OrderList { get; set; }
        }

        /// <summary>
        /// 订单信息Item
        /// </summary>
        public class OrderInfoItem
        {
            /// <summary>
            /// 订单Id
            /// </summary>
            public int Oid { get; set; }

            /// <summary>
            /// 订单编号
            /// </summary>
            public string OrderNo { get; set; }

            /// <summary>
            /// 产品名称
            /// </summary>
            public string ProjectName { get; set; }

            /// <summary>
            /// 业务Id
            /// </summary>
            public int BusId { get; set; }

            /// <summary>
            /// 业务数量
            /// </summary>
            public int BusQuantity { get; set; }

            /// <summary>
            /// 实收金额
            /// </summary>
            public decimal RealPayMoney { get; set; }

            /// <summary>
            /// 店铺id
            /// </summary>
            public int AccId { get; set; }

            /// <summary>
            /// 店铺名称
            /// </summary>
            public string AccName { get; set; }

            /// <summary>
            /// 下单时间
            /// </summary>
            public DateTime CreateDate { get; set; }

            /// <summary>
            /// 支付时间
            /// </summary>
            public DateTime TransactionDate { get; set; }

            /// <summary>
            /// 订单状态
            /// </summary>
            public int OrderStatus { get; set; }

            /// <summary>
            /// 支付方式
            /// </summary>
            public string PayDesc { get; set; }

            /// <summary>
            /// 发票Id
            /// </summary>
            public int InvoiceId { get; set; }
        }
        #endregion

        #region FeedbackItem 用户反馈
        /// <summary>
        /// 用户反馈
        /// </summary>
        public class FeedbackItem
        {
            /// <summary>
            /// 记录Id
            /// </summary>
            public int Id { get; set; }

            /// <summary>
            /// 店铺id
            /// </summary>
            public int AccId { get; set; }

            /// <summary>
            /// 店铺名称
            /// </summary>
            public string AccName { get; set; }

            /// <summary>
            /// 店铺手机号码
            /// </summary>
            public string AccPhone { get; set; }

            /// <summary>
            /// 反馈内容
            /// </summary>
            public string FdContent { get; set; }

            /// <summary>
            /// 反馈时间
            /// </summary>
            public DateTime InsertTime { get; set; }

            /// <summary>
            /// 处理状态
            /// </summary>
            public int Status { get; set; }

            /// <summary>
            /// 处理描述
            /// </summary>
            public string StatusDesc { get; set; }
        }
        #endregion

        #region Location 经纬度Model
        /// <summary>
        /// 经纬度Model
        /// </summary>
        public class Location
        {
            /// <summary>
            /// 纬度
            /// </summary>
            public double Latitude { get; set; }

            /// <summary>
            /// 经度
            /// </summary>
            public double Longitude { get; set; }
        }
        #endregion

        #region LocationModel 用户位置信息
        /// <summary>
        /// 用户位置信息
        /// </summary>
        public class LocationModel : Location
        {
            /// <summary>
            /// 位置精度
            /// </summary>
            public double Precision { get; set; }

            /// <summary>
            /// 更新时间
            /// </summary>
            public DateTime UpdateTime { get; set; }
        }
        #endregion


        #region 销售信息
        public class SalesInfo
        {
            /// <summary>
            /// 编号
            /// </summary>
            public int Id { get; set; }

            /// <summary>
            /// 当天日期
            /// </summary>
            public string CurrentToday { get; set; }

            /// <summary>
            /// 每日店铺登录次数
            /// </summary>
            public int LoginNum { get; set; }

            /// <summary>
            /// 每日新增店铺数量
            /// </summary>
            public int NewAccNum { get; set; }

            /// <summary>
            /// 店铺会员
            /// </summary>
            public int UserNum { get; set; }

            /// <summary>
            /// 新增商品
            /// </summary>
            public int AddGoodsNum { get; set; }

            /// <summary>
            /// 短信发送次数
            /// </summary>
            public int SmsNum { get; set; }

            /// <summary>
            /// 支出笔数
            /// </summary>
            public int OutLayNum { get; set; }

            /// <summary>
            /// 订单数量
            /// </summary>
            public int OrderNum { get; set; }

            /// <summary>
            /// 每日订单总金额
            /// </summary>
            public decimal OrderMoney { get; set; }

            /// <summary>
            /// 昨日活跃数量
            /// </summary>
            public int ActiveNum { get; set; }

            /// <summary>
            /// 昨日活跃占比
            /// </summary>
            public decimal YesterdayActiveRate { get; set; }


            /// <summary>
            /// 日活
            /// </summary>
            public int EveryDayActive { get; set; }

            /// <summary>
            /// 销售笔数
            /// </summary>
            public int SalesNum { get; set; }

            /// <summary>
            /// 销售金额
            /// </summary>
            public decimal SalesMoney { get; set; }

            /// <summary>
            /// 销售商品种类数量
            /// </summary>
            public int SaleKinds { get; set; }

            /// <summary>
            /// 每日去重日活
            /// </summary>
            public int EverydayDeduplicationActive { get; set; }

            /// <summary>
            /// 上周去重日活
            /// </summary>
            public int LastWeekDeduplicationActive { get; set; }

            /// <summary>
            /// 本周去重日活
            /// </summary>
            public int ThisWeekDeduplicationActive { get; set; }

            /// <summary>
            /// 本月去重日活
            /// </summary>
            public int ThisMonthDeduplicationActive { get; set; }

            /// <summary>
            /// 每日日活占比
            /// </summary>
            public double EveryDayActiveRate { get; set; }

            /// <summary>
            /// 本周活占比
            /// </summary>
            public double ThisWeekDeduplicationActiveRate { get; set; }

            /// <summary>
            /// 本月日活占比
            /// </summary>
            public double ThisMonthDeduplicationActiveRate { get; set; }

            /// <summary>
            /// 店铺总注册数量
            /// </summary>
            public int SumAccNum { get; set; }
            
            /// <summary>
            /// 是否发送
            /// </summary>
            public int IsSend { get; set; }
        }      
        #endregion

    }
}
