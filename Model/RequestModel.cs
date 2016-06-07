using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class RequestModel
    {
        public class TemplateQuery
        {
            public string touser { get; set; }
            public TemplateItem first { get; set; }
            public TemplateItem keyword1 { get; set; }
            public TemplateItem keyword2 { get; set; }
            public TemplateItem remark { get; set; }
            public string url { get; set; }
        }

    }
}
