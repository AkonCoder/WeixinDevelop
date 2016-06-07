using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonService
{
    public class RequestControl
    {
        #region 全局用户位置缓存
        /// <summary>
        /// 全局用户位置缓存
        /// </summary>
        public static Dictionary<string, ApiModel.LocationModel> UserLocation = new Dictionary<string, ApiModel.LocationModel>();
        #endregion

        #region BindUserSearch 查询登录缓存信息
        /// <summary>
        /// 查询登录缓存信息
        /// </summary>
        /// <param name="wxOpenid"></param>
        /// <returns></returns>
        public OpenRequestModel BindUserSearch(string wxOpenid)
        {
            RequestProxy fnProxy = new RequestProxy();

            return fnProxy.BindUserSearch(wxOpenid);
        }
        #endregion

        #region GetIndexInfoToday 平台信息(今日)
        /// <summary>
        /// 平台信息(今日)
        /// </summary>
        /// <param name="strObj"></param>
        /// <returns></returns>
        public WeixinResponse GetIndexInfoToday(OpenRequestModel oToken)
        {
            var wxResponse = new WeixinResponse();
            RequestProxy fnProxy = new RequestProxy();
            var response = fnProxy.SendRequest(oToken, "indextoday", "");

            if (response.Status == 0)
            {
                wxResponse = ReturnModel.Success(response.StrObj);
            }
            else
            {
                wxResponse = ReturnModel.NoBind();
            }

            return wxResponse;
        }
        #endregion

        #region GetIndexInfoYesterday 平台信息(昨日)
        /// <summary>
        /// 平台信息(昨日)
        /// </summary>
        /// <param name="strObj"></param>
        /// <returns></returns>
        public WeixinResponse GetIndexInfoYesterday(OpenRequestModel oToken)
        {
            var wxResponse = new WeixinResponse();
            RequestProxy fnProxy = new RequestProxy();
            var response = fnProxy.SendRequest(oToken, "indexyesterday", "");

            if (response.Status == 0)
            {
                wxResponse = ReturnModel.Success(response.StrObj);
            }
            else
            {
                wxResponse = ReturnModel.NoBind();
            }

            return wxResponse;
        }
        #endregion

        #region GetIndexInfoMonth 平台信息(本月)
        /// <summary>
        /// 平台信息(本月)
        /// </summary>
        /// <param name="strObj"></param>
        /// <returns></returns>
        public WeixinResponse GetIndexInfoMonth(OpenRequestModel oToken)
        {
            var wxResponse = new WeixinResponse();
            RequestProxy fnProxy = new RequestProxy();
            var response = fnProxy.SendRequest(oToken, "indexmonth", "");

            if (response.Status == 0)
            {
                wxResponse = ReturnModel.Success(response.StrObj);
            }
            else
            {
                wxResponse = ReturnModel.NoBind();
            }

            return wxResponse;
        }
        #endregion

        #region GetIndexInfoTotal 平台信息(全部)
        /// <summary>
        /// 平台信息(全部)
        /// </summary>
        /// <param name="strObj"></param>
        /// <returns></returns>
        public WeixinResponse GetIndexInfoTotal(OpenRequestModel oToken)
        {
            var wxResponse = new WeixinResponse();
            RequestProxy fnProxy = new RequestProxy();
            var response = fnProxy.SendRequest(oToken, "indextotal", "");

            if (response.Status == 0)
            {
                wxResponse = ReturnModel.Success(response.StrObj);
            }
            else
            {
                wxResponse = ReturnModel.NoBind();
            }

            return wxResponse;
        }
        #endregion

        #region GetTodayRegShop 今日注册店铺信息(最后15个)
        /// <summary>
        /// 今日注册店铺信息(最后15个)
        /// </summary>
        /// <param name="strObj"></param>
        /// <returns></returns>
        public WeixinResponse GetTodayRegShop(OpenRequestModel oToken)
        {
            var wxResponse = new WeixinResponse();
            RequestProxy fnProxy = new RequestProxy();
            var response = fnProxy.SendRequest(oToken, "todayregshop", "");

            if (response.Status == 0)
            {
                wxResponse = ReturnModel.Success(response.StrObj);
            }
            else
            {
                wxResponse = ReturnModel.NoBind();
            }

            return wxResponse;
        }
        #endregion

        #region GetTodayOrderInfo 今日订单详情(最新20个)
        /// <summary>
        /// 今日订单详情(最新20个)
        /// </summary>
        /// <param name="strObj"></param>
        /// <returns></returns>
        public WeixinResponse GetTodayOrderInfo(OpenRequestModel oToken)
        {
            var wxResponse = new WeixinResponse();
            RequestProxy fnProxy = new RequestProxy();
            var response = fnProxy.SendRequest(oToken, "todayorderinfo", "");

            if (response.Status == 0)
            {
                wxResponse = ReturnModel.Success(response.StrObj);
            }
            else
            {
                wxResponse = ReturnModel.NoBind();
            }

            return wxResponse;
        }
        #endregion

        #region GetTodayFeedback 今日客户反馈
        /// <summary>
        /// 今日客户反馈
        /// </summary>
        /// <param name="strObj"></param>
        /// <returns></returns>
        public WeixinResponse GetTodayFeedback(OpenRequestModel oToken)
        {
            var wxResponse = new WeixinResponse();
            RequestProxy fnProxy = new RequestProxy();
            var response = fnProxy.SendRequest(oToken, "todayfeedback", "");

            if (response.Status == 0)
            {
                wxResponse = ReturnModel.Success(response.StrObj);
            }
            else
            {
                wxResponse = ReturnModel.NoBind();
            }

            return wxResponse;
        }
        #endregion

        #region LocationUpdate 更新位置缓存信息
        /// <summary>
        /// 更新位置缓存信息
        /// </summary>
        /// <param name="wxOpenid"></param>
        /// <param name="locationModel"></param>
        public void LocationUpdate(string wxOpenid, ApiModel.LocationModel locationModel)
        {
            if (UserLocation != null)
            {
                if (UserLocation.ContainsKey(wxOpenid))
                {
                    lock (UserLocation)
                    {
                        UserLocation[wxOpenid] = locationModel;
                    }
                }
                else
                {
                    lock (UserLocation)
                    {
                        UserLocation.Add(wxOpenid, locationModel);
                    }
                }
            }
            else
            {
                UserLocation = new Dictionary<string, ApiModel.LocationModel> { { wxOpenid, locationModel } };
            }
        }
        #endregion

        #region LocationSearch 查询用户位置信息
        /// <summary>
        /// 查询用户位置信息
        /// </summary>
        /// <param name="wxOpenid"></param>
        /// <returns></returns>
        public ApiModel.LocationModel LocationSearch(string wxOpenid)
        {
            var model = new ApiModel.LocationModel();

            if (UserLocation != null)
            {
                if (UserLocation.ContainsKey(wxOpenid))
                {
                    model = UserLocation[wxOpenid];
                }
            }

            return model;
        }
        #endregion

        #region GetDistance 计算两个点之间距离(m)
        /// <summary>
        /// 计算两个点之间距离(m)
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public double GetDistance(ApiModel.Location start, ApiModel.Location end)
        {
            double lat1 = (Math.PI / 180) * start.Latitude;
            double lat2 = (Math.PI / 180) * end.Latitude;

            double lon1 = (Math.PI / 180) * start.Longitude;
            double lon2 = (Math.PI / 180) * end.Longitude;

            //地球半径
            double R = 6371;

            //两点间距离 km，如果想要米的话，结果*1000就可以了
            double d = Math.Acos(Math.Sin(lat1) * Math.Sin(lat2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(lon2 - lon1)) * R;

            return d * 1000;
        }
        #endregion


        #region GetWeiXinOpenIds 平台信息(本月)
        /// <summary>
        /// 得到openid列表
        /// </summary>
        /// <param name="strObj"></param>
        /// <returns></returns>
        public string[] GetWeiXinOpenIds(OpenRequestModel oToken,int weiXinType)
        {
            List<string> openIds = new List<string>();
            RequestProxy fnProxy = new RequestProxy();

            WeiXinOpenIdQueryRequest wxQR = new WeiXinOpenIdQueryRequest();
            wxQR.OpenIdType = weiXinType;

            var response = fnProxy.SendRequest(oToken, "pushopenid", CommonLib.Helper.JsonSerializeObject(wxQR));

            if (response.Status == 0)
            {
                openIds = CommonLib.Helper.JsonDeserializeObject<List<string>>(response.StrObj);
            }

            return openIds.ToArray() ;
        }
        #endregion

    }
}
