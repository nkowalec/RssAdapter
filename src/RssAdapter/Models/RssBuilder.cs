using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;

namespace RssAdapter.Models
{
    public abstract class RssResult : IActionResult
    {
        public virtual Task ExecuteResultAsync(ActionContext context)
        {
            return Task.Factory.StartNew(() => 
            {
                context.HttpContext.Response.ContentType = "text/xml";

                using (StreamWriter stream = new StreamWriter(context.HttpContext.Response.Body))
                {
                    using (XmlWriter xml = new XmlTextWriter(stream))
                    {
                        Build().WriteTo(xml);
                    }
                }
            });
        }

        public abstract Rss20FeedFormatter Build();
    }
}
