using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeRun.Models
{
    public class GetAdIds
    {
        public static string GetInterstitialIds()
        {
            var random = new Random();
            var idList = new List<string> { "ca-app-pub-9850737619470713/7247601640", "ca-app-pub-9850737619470713/1504320373", "ca-app-pub-9850737619470713/3938912029" };
            // Test Ad
            //var idList = new List<string> { "ca-app-pub-3940256099942544/1033173712" };
            int index = random.Next(idList.Count);
            return idList[index];
        }
    }
}
