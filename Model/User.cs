using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
   public class User
    {
       /// <summary>
       /// 编号
       /// </summary>
       public int Id { get; set; }

       /// <summary>
       /// 用户的OpenId
       /// </summary>
       public string UserOpenId { get; set; }

       /// <summary>
       /// 是否开启
       /// </summary>
       public int IsOpen { get; set; }

       /// <summary>
       /// /用户类型
       /// </summary>
       public int UserType { get; set; }

       /// <summary>
       /// 备注
       /// </summary>
       public string Remark { get; set; }
    }
}
