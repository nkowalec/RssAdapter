using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Text;

namespace RssAdapter.Models
{
    public class EnovaRss : RssResult
    {
        public override Rss20FeedFormatter Build()
        {
            var task = Task.Run<HttpResponseMessage>(
                async () =>
                {
                    HttpClient c = new HttpClient();
                    return await c.GetAsync(new Uri(@"http://www.enova.pl/news-and-events/news/"));
                });

            task.Wait(new TimeSpan(0, 0, 30));

            var msg = task.Result;
            var taskRead = msg.Content.ReadAsStringAsync();
            taskRead.Wait();

            string html = taskRead.Result;

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            SyndicationFeed feed = new SyndicationFeed("enova365 - Aktualności", "Aktualności ze strony enova.pl", new Uri(@"http://enova.pl"));

            var nodes = doc.DocumentNode.Descendants("div").Where(x => x.Attributes.Contains("class") && x.Attributes["class"].Value == "news-list-content");
            //.ChildNodes.Where(x => x.Name == "div" && x.Attributes.Contains("name") && x.Attributes["name"].Value == "news-list-content");

            List<SyndicationItem> items = new List<SyndicationItem>();

            foreach (var item in nodes)
            {
                string title = item.ChildNodes["h3"].InnerText;
                string href = item.ChildNodes["h3"].ChildNodes["a"].Attributes["href"].Value;
                string date = item.ChildNodes.Where(x => x.Name == "div" && x.Attributes.Contains("class") && x.Attributes["class"].Value == "news-list-date").First().InnerText;

                var span = item.ChildNodes.Where(x => x.Name == "span" && x.Attributes.Contains("class") && x.Attributes["class"].Value == "news-latest-content").First();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine(span.InnerText);
                //foreach(var spanItem in span.ChildNodes)
                //{
                //    if (!string.IsNullOrEmpty(spanItem.InnerText))
                //        sb.AppendLine(spanItem.InnerText);
                //    if(spanItem.ChildNodes["p"] != null)
                //        sb.AppendLine(spanItem.ChildNodes["p"].InnerText);
                //}

                SyndicationItem _i = new SyndicationItem(title, sb.ToString(), new Uri(@"http://enova.pl/" + href), "", new DateTimeOffset(DateTime.ParseExact(date, "dd.MM.yyyy", null)));
                _i.PublishDate = new DateTimeOffset(DateTime.ParseExact(date, "dd.MM.yyyy", null));
                

                items.Add(_i);
            }

            feed.Items = items;

            return new Rss20FeedFormatter(feed);
        }
    }
}
