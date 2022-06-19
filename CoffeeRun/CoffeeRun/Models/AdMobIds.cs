using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeRun.Models
{
    public class AdMobIds
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

        public static string GetBannerAdIds()
        {
            var random = new Random();
            //var idList = new List<string> { "ca-app-pub-9850737619470713/7725102036", "ca-app-pub-9850737619470713/3773430494", "ca-app-pub-9850737619470713/6241933317", "ca-app-pub-9850737619470713/5706261594", "ca-app-pub-9850737619470713/9453934918" };
            // Test Ad
            var idList = new List<string> { "ca-app-pub-3940256099942544/1033173712" };
            int index = random.Next(idList.Count);
            return idList[index];
        }
    }
}
