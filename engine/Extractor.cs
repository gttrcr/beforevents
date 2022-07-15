namespace Beforevents
{
    using HtmlAgilityPack;

    public class Extractor
    {
        public static void Extract(Finder finder, ref List<Event> events)
        {
            bool it = false;
            int from = 0;
            int to = int.MaxValue;
            string target = "";
            if (finder.Url.Contains("(((("))
            {
                int startIndex = finder.Url.IndexOf("((((");
                int stopIndex = finder.Url.LastIndexOf("))))");
                target = finder.Url.Substring(0, startIndex) + finder.Url.Substring(stopIndex + 4);
                Finder newFinder = finder;
                newFinder.Url = target;
                Extract(newFinder, ref events);
                finder.Url = finder.Url.Replace("((((", "").Replace("))))", "");
            }
            if (finder.Url.Contains("####"))
            {
                it = true;
                int[] startIndexes = finder.Url.AllIndexesOf("####").ToArray();
                string sFrom = finder.Url.Substring(startIndexes[0] + 4, startIndexes[1] - startIndexes[0] - 4);
                string sTo = finder.Url.Substring(startIndexes[2] + 4, startIndexes[3] - startIndexes[2] - 4);
                target = finder.Url.Substring(startIndexes[0], startIndexes[3] + 4 - startIndexes[0]);
                if (sFrom != "")
                    from = int.Parse(sFrom);
                else
                    from = 0;
                if (sTo != "")
                    to = int.Parse(sTo);
                else
                    to = int.MaxValue;
            }

            if (it)
                for (int i = from; i <= to; i++)
                {
                    Finder newFinder = finder;
                    newFinder.Url = finder.Url.Replace(target, i.ToString());
                    int eventsCount = events.Count;
                    Extract(newFinder, ref events);
                    if (events.Count == eventsCount)
                        break;
                }
            else
            {
                Console.WriteLine(finder.Url);
                HtmlWeb h = new HtmlWeb();
                HtmlAgilityPack.HtmlDocument doc = h.Load(finder.Url);
                HtmlNodeCollection classes = doc.DocumentNode.SelectNodes(finder.Events);
                if (classes != null)
                    foreach (HtmlNode node in classes)
                    {
                        Event e = new Event();
                        if (finder.Title != null)
                            e.Title = node.SelectSingleNode(finder.Title)?.InnerText.Normal();
                        if (finder.Description != null)
                            e.Description = node.SelectSingleNode(finder.Description)?.InnerText.Normal();
                        if (finder.Where != null)
                        {
                            e.Where = node.SelectSingleNode(finder.Where)?.InnerText.Normal();
                            e.Maps = new Uri("https://www.google.com/maps?q=" + e.Where?.Replace(" ", "+"));
                        }
                        if (finder.From != null)
                            e.From = node.SelectSingleNode(finder.From)?.InnerText.Normal().ToDateTime().ToString("dd/MM/yyyy");
                        if (finder.To != null)
                            e.To = node.SelectSingleNode(finder.To)?.InnerText.Normal().ToDateTime().ToString("dd/MM/yyyy");
                        if (e.To == "" || e.To == null)
                            e.To = e.From;

                        events.Add(e);
                    }
            }
        }
    }
}