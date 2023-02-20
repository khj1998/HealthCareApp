using System;
using UnityEngine;
using UnityEngine.Purchasing;
using Unity.Services.Core;
using Unity.Services.Analytics;
using System.Collections;
using System.Collections.Generic;
using SOMA.UI;
using SOMA.firebaseDB;
using SOMA.Managers;

namespace SOMA.IAP
{
    public class IAPManager : MonoBehaviour
{

    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.
    private static UnityEngine.Purchasing.Product test_product = null;

    IGooglePlayStoreExtensions m_GooglePlayStoreExtensions;

    [SerializeField]
    private SOMA.Ads.BannerAds BannderScript;
    [SerializeField]
    private SetNoAdsUI setNoAdsUI;
    [SerializeField]
    private IAPButton _iapButton;

    private const string _Android_BackgroundColor="com.purchasingtest.inapppurchasetest.changecolor";
    private const string _Android_RemoveAds="com.hahaha.lvupbody.removeadstest";
    private const string _IOS_BackgroundColor = "";
    private const string _IOS_RemoveAds="";

    private Boolean return_complete = true;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => (ConnectFirebaseDB.Instance.inAppProductsBackupList.Count>=2));
        if (DataManager.Instance.InAppProductsTable.RemoveBanner)
        {
            Debug.Log("이미 구매한 상품입니다.");
            _iapButton.enabled = false;
        }
    }

    public void OnPurchaseFailed(UnityEngine.Purchasing.Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogWarning($"구매 실패 - {product.definition.id}, {failureReason}");
    }

    public void OnRemoveAdsPurchaseComplete(UnityEngine.Purchasing.Product product)
    {
        Debug.Log("Remove Ads product: "+product.definition.id);
        updateRemoveAdsPurchase();
    }

    private void updateRemoveAdsPurchase()
    {
        DataManager.Instance.InAppProductsTable.RemoveBanner = true;
        DataManager.Instance.WriteUserProductFile();

        #if !UNITY_EDITOR
        SOMA.firebaseDB.ConnectFirebaseDB.Instance.UpdateRemoveAdsInAppPurchase(SOMA.Managers.FirebaseManager.Instance.IdToken,true);
        #endif
        
        BannderScript.DestroyBanner();
        setNoAdsUI.setTopMargin();
        Debug.Log("구매 업데이트 완료");
    }

    public void RestorePurchases()
    {
        m_StoreExtensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions(result =>
        {
            if (result)
            {
                Debug.Log("Restore purchases succeeded.");
                Dictionary<string, object> parameters = new Dictionary<string, object>()
                {
                    { "restore_success", true },
                };
                AnalyticsService.Instance.CustomData("myRestore", parameters);
            }
            else
            {
                Debug.Log("Restore purchases failed.");
                Dictionary<string, object> parameters = new Dictionary<string, object>()
                {
                    { "restore_success", false },
                };
                AnalyticsService.Instance.CustomData("myRestore", parameters);
            }

            AnalyticsService.Instance.Flush();
        });

    }
}
}
