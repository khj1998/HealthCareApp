using System.Collections;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using SOMA;
using SOMA.UI;
using SOMA.firebaseDB;
using SOMA.Managers;

namespace SOMA.Ads
{
    public class BannerAds : MonoBehaviour
    {
        [SerializeField]
        private IAPButton _removeAdsButton;
        private BannerView bannerView;
        private AdSize adaptiveAdSize;
        private string adsKey;
        private bool destroyAds;
        [SerializeField]
        private SetNoAdsUI setNoAdsUI;

        private IEnumerator Start() 
        {
            destroyAds = false;

            MobileAds.Initialize(InitializationStatus => {}); 
            Debug.Log("배너 광고를 생성합니다.");
            CreateBannerAds();
            
            yield return new WaitUntil(() => (ConnectFirebaseDB.Instance.inAppProductsBackupList.Count>=2));

            Debug.Log("배너 Start 시작");

            if (DataManager.Instance.InAppProductsTable.RemoveBanner)
            {
                _removeAdsButton.enabled = false;
                Debug.Log(DataManager.Instance.InAppProductsTable.RemoveBanner);
                Debug.Log("배너 광고를 제거합니다.");
                destroyAds = true;
                DestroyBanner();
                setNoAdsUI.setTopMargin();
            }
        }

        void Update() 
        {
            if (!destroyAds && DataManager.Instance.InAppProductsTable.RemoveBanner)
            {
                DestroyBanner();
                destroyAds = true;
            } 
        }

        void CreateBannerAds()
        {
            if (this.bannerView!=null)
            {
                this.bannerView.Destroy();
            }

            if (Application.platform == RuntimePlatform.Android)
            {
                adsKey = "ca-app-pub-6042302827066076/7376047351";
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                adsKey = "ca-app-pub-3940256099942544/2934735716";
            }
            else
            {
                adsKey = "unexpected_platform";
            }

            adaptiveAdSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
            //adaptiveAdSize = new AdSize(1080, 168);
            this.bannerView = new BannerView(adsKey,adaptiveAdSize,AdPosition.Top);

            AdRequest request = new AdRequest.Builder().Build();
            this.bannerView.LoadAd(request);
        }

        public void DestroyBanner()
        {
            if (this.bannerView!=null)
            {
                this.bannerView.Destroy();
            }
        }
    }
}
