using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web.Configuration;
using CommonLib;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MessageHandlers;

namespace CommonService.CustomMessageHandler
{
    /// <summary>
    /// 自定义MessageHandler
    /// 把MessageHandler作为基类，重写对应请求的处理方法
    /// </summary>
    public partial class CustomMessageHandler : MessageHandler<CustomMessageContext>
    {
        /*
         * 重要提示：v1.5起，MessageHandler提供了一个DefaultResponseMessage的抽象方法，
         * DefaultResponseMessage必须在子类中重写，用于返回没有处理过的消息类型（也可以用于默认消息，如帮助信息等）；
         * 其中所有原OnXX的抽象方法已经都改为虚方法，可以不必每个都重写。若不重写，默认返回DefaultResponseMessage方法中的结果。
         */


#if DEBUG
        private string agentUrl = "http://localhost:12222/App/Weixin/4";
        private string agentToken = "27C455F496044A87";
        private string wiweihiKey = "CNadjJuWzyX5bz5Gn+/XoyqiqMa5DjXQ";
#else
    //下面的Url和Token可以用其他平台的消息，或者到www.weiweihi.com注册微信用户，将自动在“微信营销工具”下得到
        private string agentUrl = WebConfigurationManager.AppSettings["WeixinAgentUrl"]; //这里使用了www.weiweihi.com微信自动托管平台
        private string agentToken = WebConfigurationManager.AppSettings["WeixinAgentToken"]; //Token

        private string wiweihiKey = WebConfigurationManager.AppSettings["WeixinAgentWeiweihiKey"];
            //WeiweihiKey专门用于对接www.Weiweihi.com平台，获取方式见：http://www.weiweihi.com/ApiDocuments/Item/25#51
#endif

        public CustomMessageHandler(Stream inputStream, PostModel postModel, int maxRecordCount = 0)
            : base(inputStream, postModel, maxRecordCount)
        {
            //这里设置仅用于测试，实际开发可以在外部更全局的地方设置，
            //比如MessageHandler<MessageContext>.GlobalWeixinContext.ExpireMinutes = 3。
            WeixinContext.ExpireMinutes = 3;
        }

        public override void OnExecuting()
        {
            //测试MessageContext.StorageData
            if (CurrentMessageContext.StorageData == null)
            {
                CurrentMessageContext.StorageData = 0;
            }

            //var blackUserOpenId = "oHow7xBPWRBwb_-4hPvjMQX8KO-E";
            ////在执行后续的请求之前处理相关逻辑，例如可以阻止黑名单用户的后续请求
            //if (RequestMessage.FromUserName== blackUserOpenId)
            //{
            //    CancelExcute = true;
            //    var responseMessage = CreateResponseMessage<ResponseMessageText>();
            //    responseMessage.Content = "你已经被拉黑啦,hahhahah！";
            //    ResponseMessage = responseMessage;
            //}

            base.OnExecuting();
        }

        public override void OnExecuted()
        {
            base.OnExecuted();
            CurrentMessageContext.StorageData = ((int) CurrentMessageContext.StorageData) + 1;
        }

        /// <summary>
        /// 处理文字请求
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            //TODO:这里的逻辑可以交给Service处理具体信息，参考OnLocationRequest方法或/Service/LocationSercice.cs

            //方法一（v0.1），此方法调用太过繁琐，已过时（但仍是所有方法的核心基础），建议使用方法二到四
            //var responseMessage =
            //    ResponseMessageBase.CreateFromRequestMessage(RequestMessage, ResponseMsgType.Text) as
            //    ResponseMessageText;

            //方法二（v0.4）
            //var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(RequestMessage);

            //方法三（v0.4），扩展方法，需要using Senparc.Weixin.MP.Helpers;
            //var responseMessage = RequestMessage.CreateResponseMessage<ResponseMessageText>();

            //方法四（v0.6+），仅适合在HandlerMessage内部使用，本质上是对方法三的封装
            //注意：下面泛型ResponseMessageText即返回给客户端的类型，可以根据自己的需要填写ResponseMessageNews等不同类型。

            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();

            GetWeatherForeast(requestMessage, responseMessage);
            return responseMessage;
        }

        /// <summary>
        /// 获取天气预报信息
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="responseMessage"></param>
        private static void GetWeatherForeast(RequestMessageText requestMessage, ResponseMessageText responseMessage)
        {
            //1.获取天气预报
            var weatherAddress = System.Configuration.ConfigurationManager.AppSettings["weatherApiAddress"].ToString();
            var dic = new Dictionary<string, string>
            {
                {"cityname", requestMessage.Content.ToString()},
                {"dtype", "json"},
                {"key", "e8a21b15c21dca288459135283295192"}
            };
            var weatherResult = CommonLib.Helper.JsonDeserializeObjectWithNS(Helper.SendHttpGet(weatherAddress, dic));
            var cityName = weatherResult["result"]["data"]["realtime"]["city_name"]; //当前城市
            var humidity = weatherResult["result"]["data"]["realtime"]["weather"]["humidity"]; //湿度
            var info = weatherResult["result"]["data"]["realtime"]["weather"]["info"]; //天气
            var temperature = weatherResult["result"]["data"]["realtime"]["weather"]["temperature"]; //温度
            var sportSuggestion = weatherResult["result"]["data"]["life"]["info"]["yundong"]; //运动
            var result = string.Format("当前城市：{0},今天的天气温度为：{1}度,湿度{2},天气{3},运动建议：{4}", cityName, temperature, humidity,
                info,
                sportSuggestion);
            responseMessage.Content = result;
        }

        /// <summary>
        /// 处理位置请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnLocationRequest(RequestMessageLocation requestMessage)
        {
            var locationService = new LocationService();
            var responseMessage = locationService.GetResponseMessage(requestMessage as RequestMessageLocation);
           // var wifiAddressList = GetFreeWifiAddress(requestMessage, responseMessage);
            responseMessage.Articles.Add(new Article()
            {
                Title = "免费Wifi地点",
                Description = "地图",
                PicUrl = "http://img-cdn.hopetrip.com.hk/2015/0117/20150117092654239.jpg",
                Url = "http://tools.juhe.cn/map/map_lng.html"
            });
            return responseMessage;
        }

        /// <summary>
        /// 获取当前位置范围内3km以内的免费Wifi
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="responseMessage"></param>
        private static string GetFreeWifiAddress(RequestMessageLocation requestMessage, ResponseMessageNews responseMessage)
        {
            var locationX = requestMessage.Location_X.ToString(CultureInfo.InvariantCulture);
            var locationY = requestMessage.Location_Y.ToString(CultureInfo.InvariantCulture);
            var wifiRequestAddress = System.Configuration.ConfigurationManager.AppSettings["wifiApiAddress"].ToString();
            var dic = new Dictionary<string, string>
            {
                {"lon", locationX},
                {"lat", locationY},
                {"gtype", "2"},
                {"r", "3000"},
                {"dtype", "json"},
                {"key", "2ffa1edcc61d3096aef7e5d56a1d1036"}
            };
            var wifiResult = CommonLib.Helper.JsonDeserializeObjectWithNS(Helper.SendHttpGet(wifiRequestAddress, dic));
            var wifiAddressList = string.Empty;
            if (wifiResult["reason"].ToString() != "EMPTY!")
            {
                var addressList = wifiResult["result"]["data"];
                for (int i = 0; i < 5; i++)
                {
                    wifiAddressList += addressList[i]["name"] + " <br/>;";
                }
            }
            else
            {
                wifiAddressList = "当前位置周围没有搜索到免费Wifi";
            }
            return wifiAddressList;
        }

        /// <summary>
        /// 处理图片请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnImageRequest(RequestMessageImage requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageNews>();

            responseMessage.Articles.Add(new Article()
            {
                Description = "你的图片好牛逼！赠送给你一张别的！",
                Title="图片请求",
                Url = "http://www.sj88.com/attachments/201412/21/14/4939o2rti.jpg",
                PicUrl = "http://www.sj88.com/attachments/201412/21/14/4939o2rti.jpg"
            });
            return responseMessage;
        }

        /// <summary>
        /// 处理语音请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnVoiceRequest(RequestMessageVoice requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageMusic>();
            responseMessage.Music.Description = "您的声音太优美了，小编都醉了";
            responseMessage.Music.MusicUrl = "http://music.163.com/#/m/song?id=109196&userid=14220464";
            responseMessage.Music.Title = "送你一首歌曲！";
            return responseMessage;
        }

        /// <summary>
        /// 处理视频请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnVideoRequest(RequestMessageVideo requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "这视频太劲爆了";
            return responseMessage;
        }

        /// <summary>
        /// 处理链接消息请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnLinkRequest(RequestMessageLink requestMessage)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNews>(requestMessage);
               responseMessage.Articles.Add(new Article()
            {
                Description = "莫愁不开怀！",
                Title="链接请求",
                Url = "http://www.cnblogs.com/liupeng61624/p/4354983.html",
                PicUrl = "http://www.cnblogs.com/liupeng61624/p/4354983.html"
            });
            return responseMessage;
        }

        /// <summary>
        /// 处理事件请求（这个方法一般不用重写，这里仅作为示例出现。除非需要在判断具体Event类型以外对Event信息进行统一操作
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEventRequest(IRequestMessageEventBase requestMessage)
        {
            var eventResponseMessage = base.OnEventRequest(requestMessage);
            //对于Event下属分类的重写方法，见：CustomerMessageHandler_Events.cs
            //TODO: 对Event信息进行统一操作
            return eventResponseMessage;
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            /* 所有没有被处理的消息会默认返回这里的结果，
             * 因此，如果想把整个微信请求委托出去（例如需要使用分布式或从其他服务器获取请求），
             * 只需要在这里统一发出委托请求，如：
             * var responseMessage = MessageAgent.RequestResponseMessage(agentUrl, agentToken, RequestDocument.ToString());
             * return responseMessage;
             */

            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "钛合金狗眼";
            return responseMessage;
        }
    }
}