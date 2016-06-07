using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonService
{
    public static class ReturnModel
    {
        #region NoBind 账号未绑定
        /// <summary>
        /// 账号未绑定
        /// </summary>
        /// <returns></returns>
        public static WeixinResponse NoBind()
        {
            var model = new WeixinResponse
            {
                Status = -1,
                ErrMsg = "请先绑定你的账号!"
            };
            return model;
        }
        #endregion

        #region Success 请求成功
        /// <summary>
        /// 请求成功
        /// </summary>
        /// <returns></returns>
        public static WeixinResponse Success(string dataObj)
        {
            var model = new WeixinResponse
            {
                Status = 0,
                Data = dataObj
            };
            return model;
        }
        #endregion
    }
}
