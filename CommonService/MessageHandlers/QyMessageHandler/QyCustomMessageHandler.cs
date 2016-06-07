using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Senparc.Weixin.QY.Entities;
using Senparc.Weixin.QY.MessageHandlers;
namespace CommonService.QyMessageHandler
{
    public class QyCustomMessageHandler : QyMessageHandler<QyCustomMessageContext>
    {
        public QyCustomMessageHandler(Stream inputStream, PostModel postModel, int maxRecordCount = 0)
            : base(inputStream, postModel, maxRecordCount)
        {
        }

        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "您发送了消息：" + requestMessage.Content;
            return responseMessage;
        }

        public override Senparc.Weixin.QY.Entities.IResponseMessageBase DefaultResponseMessage(Senparc.Weixin.QY.Entities.IRequestMessageBase requestMessage)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "这是一条没有找到合适回复信息的默认消息。";
            return responseMessage;
        }
    }
}
