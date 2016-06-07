using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class TemplateMessage
    {
        public TemplateMessage()
        {
            data = new Dictionary<string, TemplateItem>();
            TemplateId = "rIAZd9ZP7yIT0NVgT8VUUYq6yjJGaP_lYElgzTouEd0";
            //aFcTLJuKtJE2niyK2fnxbSMLWR8GiyaI8aMBSSbv22g
            //rIAZd9ZP7yIT0NVgT8VUUYq6yjJGaP_lYElgzTouEd0
            url = "";
            topcolor = "";
        }
        public string touser { get; set; }
        public string url { get; set; }
        public string topcolor { get; set; }
        public Dictionary<string, TemplateItem> data { get; set; }
        public string TemplateId { get; set; }
    }

    public class TemplateItem
    {
        public TemplateItem()
        {
            color = "#000000";
        }
        public TemplateItem(string _value)
        {
            this.value = _value;
        }
        public string value { get; set; }
        public string color { get; set; }

    }
}
