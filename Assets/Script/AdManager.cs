using UnityEngine;
using GoogleMobileAds.Api;
using System;
using UnityEngine.SceneManagement;


public class AdManager : MonoBehaviour
{
    // Singleton yapısı
    public static AdManager Instance { get; private set; }
private int _targetSceneIndex = -1;

    // Geçiş Reklamı Nesnesi
    private InterstitialAd interstitialAd;

    // Reklam birimi ID'si
    private string _adUnitId = "ca-app-pub-5401991319150609/1057741773";

    private void Awake()
    {
        // Singleton Kontrolü
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeAdMob();
    }

    private void InitializeAdMob()
    {
        // ⚠️ ESKİ SDK UYUMLULUĞU
        // RequestConfiguration.Builder BU SDK'DA YOK
        // Bu yüzden Families Policy ayarı BURADA KALDIRILDI
        // (Reklam çalışmasını bozmaz)

        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("AdMob initialization complete.");
            LoadInterstitialAd();
        });
    }

    public void LoadInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        Debug.Log("AdMob Loading Ad: " + _adUnitId);

        AdRequest request = new AdRequest();

        InterstitialAd.Load(_adUnitId, request,
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null)
                {
                    Debug.LogError("Interstitial ad failed to load: " + error.GetMessage());
                    return;
                }

                interstitialAd = ad;
                Debug.Log("Interstitial ad loaded successfully.");
                RegisterEventHandlers(interstitialAd);
            });
    }

    private void RegisterEventHandlers(InterstitialAd ad)
    {
           ad.OnAdFullScreenContentClosed += () =>
    {
        Debug.Log("Interstitial kapandı");

        LoadInterstitialAd();

        if (_targetSceneIndex != -1)
        {
            SceneManager.LoadScene(_targetSceneIndex);
            _targetSceneIndex = -1;
        }
    };

    ad.OnAdFullScreenContentFailed += (AdError error) =>
    {
        Debug.LogError("Interstitial hata verdi");

        if (_targetSceneIndex != -1)
        {
            SceneManager.LoadScene(_targetSceneIndex);
            _targetSceneIndex = -1;
        }
    };
    }

    public void ShowInterstitialAd()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            Debug.Log("AdMob Showing Interstitial Ad.");
            interstitialAd.Show();
        }
        else
        {
            Debug.LogWarning("Interstitial ad not ready.");
            LoadInterstitialAd();
        }
    }
   public void ShowInterstitialWithCount(int sceneIndex)
{
    _targetSceneIndex = sceneIndex;

    if (interstitialAd != null && interstitialAd.CanShowAd())
    {
        Time.timeScale = 1; // 🔥 çok önemli
        interstitialAd.Show();
    }
    else
    {
        // Reklam yoksa direkt geç
        SceneManager.LoadScene(_targetSceneIndex);
    }
}

public void ShowInterstitialWithScene(int sceneIndex)
{
    _targetSceneIndex = sceneIndex;

    if (interstitialAd != null && interstitialAd.CanShowAd())
    {
        Time.timeScale = 1f;
        interstitialAd.Show();
    }
    else
    {
        SceneManager.LoadScene(_targetSceneIndex);
    }
}
}
