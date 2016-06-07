using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{

    #region ProxyRequestModel 请求参数
    /// <summary>
    /// 请求参数
    /// </summary>
    public class ProxyRequestModel
    {
        /// <summary>
        /// 加密签名
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public string Timestamp { get; set; }

        /// <summary>
        /// 随机数
        /// </summary>
        public string Nonce { get; set; }

        /// <summary>
        /// 操作人员Id
        /// </summary>
        public int OperatorId { get; set; }

        /// <summary>
        /// 用户权限
        /// </summary>
        public int UserPower { get; set; }

        /// <summary>
        /// 微信Openid
        /// </summary>
        public string WeixinOpenId { get; set; }

        /// <summary>
        /// 请求名称
        /// </summary>
        public string RequestName { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        public string RequestJson { get; set; }
    }
    #endregion

    #region ProxyResponseModel 响应返回格式
    /// <summary>
    /// 响应返回格式
    /// </summary>
    public class ProxyResponseModel
    {
        /// <summary>
        /// 执行状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 错误描述
        /// </summary>
        public string ErrDesc { get; set; }

        /// <summary>
        /// 操作人员Id
        /// </summary>
        public int OperatorId { get; set; }

        /// <summary>
        /// 对象数据Json
        /// </summary>
        public string StrObj { get; set; }
    }
    #endregion

    #region OpenRequestModel Open请求附加信息
    /// <summary>
    /// Open请求附加信息
    /// </summary>
    public class OpenRequestModel
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public int OperatorId { get; set; }

        /// <summary>
        /// 用户权限
        /// </summary>
        public int UserPower { get; set; }

        /// <summary>
        /// 微信Openid
        /// </summary>
        public String WeixinOpenId { get; set; }
    }
    #endregion

    #region ManageUserLite 缓存用户信息Model
    /// <summary>
    /// 缓存用户信息Model
    /// </summary>
    public class ManageUserLite
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public int OperatorId { get; set; }

        /// <summary>
        /// 用户权限
        /// </summary>
        public int UserPower { get; set; }

        /// <summary>
        /// 微信openid
        /// </summary>
        public string WeixinOpenid { get; set; }

        /// <summary>
        /// 缓存时间
        /// </summary>
        public DateTime CacheTime { get; set; }
    }
    #endregion

    #region WeixinResponse 微信响应Model
    /// <summary>
    /// 微信响应Model
    /// </summary>
    public class WeixinResponse
    {
        /// <summary>
        /// 处理状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrMsg { get; set; }

        /// <summary>
        /// 数据Obj
        /// </summary>
        public string Data { get; set; }
    }
    #endregion

    #region ResponseLogModel 响应日志model
    /// <summary>
    /// 响应日志model
    /// </summary>
    public class ResponseLogModel
    {
        /// <summary>
        /// id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 微信Openid
        /// </summary>
        public string WxOpenid { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int OperatorId { get; set; }

        /// <summary>
        /// 执行命令
        /// </summary>
        public string OrderName { get; set; }

        /// <summary>
        /// 命令执行状态
        /// </summary>
        public int OrderStatus { get; set; }

        /// <summary>
        /// 成功返回结果
        /// </summary>
        public string ObjectStr { get; set; }

        /// <summary>
        /// 错误返回结果
        /// </summary>
        public string ErrDesc { get; set; }
    }
    #endregion


    #region 微信查询
    public class WeiXinOpenIdQueryRequest
    {
        public int OpenIdType { get; set; }
    }

    #endregion
}
