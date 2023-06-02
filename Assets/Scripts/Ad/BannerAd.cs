using System;
using GoogleMobileAds.Api;
using UnityEngine;

namespace Ad
{
    public class BannerAd : MonoBehaviour
    {
        private BannerView _bannerView;
        public AdPosition bannerPos;
        public string unitId;
        public string testId = "ca-app-pub-3940256099942544/6300978111";
        private void Start()
        {
            MobileAds.Initialize(status => { });
            RequestBanner();
        }

        private void RequestBanner()
        {
            if (unitId == String.Empty) return;
            
            if (_bannerView != null)
            {
                _bannerView.Destroy();
            }

            _bannerView = new BannerView(unitId, AdSize.Banner, bannerPos);

            AdRequest request = new AdRequest.Builder().Build();
            _bannerView.LoadAd(request);
        }
    }
}
