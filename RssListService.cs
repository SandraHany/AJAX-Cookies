using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using CookiesOpml.Pages;
using static CookiesOpml.Pages.OpmlModel;
namespace CookiesOpml
{
    public class RssListService
    {
        public List<RssModelClass> RssListGlobal { get; set; } = new List<RssModelClass>();
        public void Update(List<OpmlModel.RssModelClass> newList)
        {
            RssListGlobal = newList;
        }
        public void LoadPage(int page)
        {

        }
        public void PrintListJson()
        {
            var jsonString = JsonSerializer.Serialize(RssListGlobal);
            File.WriteAllText("output.json", jsonString);
        }
        public RssItem FindItemByGuid(string guid)
        {
            foreach (var rss in RssListGlobal)
            {
                var item = rss.Items.FirstOrDefault(i => i.Guid == guid);
                if (item != null)
                {
                    return item;
                }
            }
            return null;
        }

    }
}
