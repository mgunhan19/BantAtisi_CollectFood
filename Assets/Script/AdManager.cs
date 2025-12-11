using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

    private RewardedAd rewardedAd;

    private string rewardedID = "ca-app-pub-3940256099942544/5224354917"; // TEST ID

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        MobileAds.Initialize(initStatus => { });
        LoadRewarded();
    }

    // ---------------------------------------------------
    // REWARDED LOAD (GoogleMobileAds v8.x)
    // ---------------------------------------------------
    public void LoadRewarded()
    {
        Debug.Log("Rewarded yükleniyor...");

        AdRequest request = new AdRequest();

        RewardedAd.Load(rewardedID, request, (RewardedAd ad, LoadAdError loadError) =>
        {
            if (loadError != null)
            {
                Debug.Log("Rewarded yüklenemedi: " + loadError);
                return;
            }

            rewardedAd = ad;
            Debug.Log("Rewarded başarıyla yüklendi ✔");

            rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Rewarded kapandı → yeniden yükleniyor");
                LoadRewarded();
            };

            rewardedAd.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.Log("Rewarded açılırken hata: " + error);
            };
        });
    }

    // ---------------------------------------------------
    // SHOW REWARDED (GoogleMobileAds v8.x)
    // ---------------------------------------------------
    public void ShowRewarded()
    {
        if (rewardedAd == null)
        {
            Debug.Log("Rewarded hazır değil → yükleniyor...");
            LoadRewarded();
            return;
        }

        rewardedAd.Show((Reward reward) =>
        {
            Debug.Log("Kral reklamı izledi ve ödülü aldı ✔");
            GiveReward();
        });
    }

    // ---------------------------------------------------
    // ÖDÜL (MÜŞTERİ MEMNUNİYETİ ARTTIRMA)
    // ---------------------------------------------------
private void GiveReward()
{
    Debug.Log("🔥 Kral reklam ödülünü aldı!");

    // 1) Memnuniyet arttır
    GameManager.Instance.AdjustMemnuniyet(+0.20f);

    // 2) Sepete +1 hediye ürün ekle
    GameManager.Instance.HediyeUrunEkle();

    // 3) POP-UP ANİMASYONU
    GameManager.Instance.GosterPopUp("+1");

    // 4) Kaybettin panelini kapat
    GameManager.Instance.PanelIslemlerExternal(4, false);

    // 5) Oyun devam etsin
    Time.timeScale = 1;
}



}
