using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] int HedefUrunSayisi;
    [SerializeField] int MevcutUrunSayisi;
    [SerializeField] int BaslangicUrunSayisi;

    public static GameManager Instance;
    bool OyunBittimi;

    [Header("---- UI POPUP ----")]
    public GameObject popupPrefab;
    public Transform popupParent;
    public Transform sepetKonumu;

    [Header("----URUN HAVUZU----")]
    [SerializeField] Transform[] UrunCikisNoktalari;
    public GameObject[] UrunHavuzu;
    WaitForSeconds UrunCikisZamani = new(1f);
    int KalanUrunAdeti;
    int HavuzIndex;
    int _SahneIndex;

    [Header("----SES YÖNETİMİ----")]
    [SerializeField] AudioSource SesKaynak;       // Efektler için kullanılan kaynak
    [SerializeField] AudioClip[] SesKlipleri;

    public Image muzikIcon;
    public Sprite muzikAcikSprite;
    public Sprite muzikKapaliSprite;

    public Image efektIcon;
    public Sprite efektAcikSprite;
    public Sprite efektKapaliSprite;

    [Header("--- UI YÖNETİMİ ---")]
    [SerializeField] GameObject[] _Paneller;
    [SerializeField] TextMeshProUGUI[] _GenelTextler;

    [Header("---- MEMNUNİYET SİSTEMİ ----")]
    [SerializeField] Slider memnuniyetSlider;
    [SerializeField] float memnuniyetDegisim;
    [SerializeField] string[] DogruYiyecekler;
    [SerializeField] string[] YanlisYiyecekler;
    [SerializeField] Image sliderFill;

    float memnuniyetPuan = 0.5f;
    Color hedefRenk;

    void Awake()
    {
        Instance = this;
        
        SahneIlkIslemleri();
    }

    void Start()
    {
        // ---------------------
        // SES BAŞLANGIÇ AYARLARI
        // ---------------------

        // Efekt sesi (GameManager kontrol eder)
        SesKaynak.mute = PlayerPrefs.GetInt("EfektSesi", 1) == 0;

        // MÜZİK OyunSesi.cs üzerinden yönetiliyor → ikon başlangıç
        muzikIcon.sprite = PlayerPrefs.GetInt("OyunSesi", 1) == 1 ? muzikAcikSprite : muzikKapaliSprite;

        efektIcon.sprite = PlayerPrefs.GetInt("EfektSesi", 1) == 1 ? efektAcikSprite : efektKapaliSprite;

        // ---- Ürün çıkış noktası kontrol
        if (UrunCikisNoktalari == null || UrunCikisNoktalari.Length == 0)
        {
            GameObject tek = GameObject.Find("UrunCikisNoktasi");
            if (tek != null)
            {
                UrunCikisNoktalari = new Transform[1];
                UrunCikisNoktalari[0] = tek.transform;
            }
        }

        if (UrunHavuzu == null || UrunHavuzu.Length == 0)
            UrunHavuzu = GameObject.FindGameObjectsWithTag("Urun");

        PanelIslemler(0, true);
        for (int i = 1; i < _Paneller.Length; i++)
            PanelIslemler(i, false);
    }

    void SahneIlkIslemleri()
    {
        _SahneIndex = SceneManager.GetActiveScene().buildIndex;

        if (UrunHavuzu != null && UrunHavuzu.Length > 0)
        {
            KalanUrunAdeti = UrunHavuzu.Length;
            BaslangicUrunSayisi = UrunHavuzu.Length;
        }

        if (_GenelTextler.Length >= 3)
        {
            _GenelTextler[0].text = HedefUrunSayisi.ToString();
            _GenelTextler[1].text = MevcutUrunSayisi.ToString();
            _GenelTextler[2].text = KalanUrunAdeti.ToString();
        }

        if (memnuniyetSlider != null)
        {
            memnuniyetSlider.minValue = 0f;
            memnuniyetSlider.maxValue = 1f;
            memnuniyetSlider.value = memnuniyetPuan;

            if (sliderFill == null && memnuniyetSlider.fillRect != null)
                sliderFill = memnuniyetSlider.fillRect.GetComponent<Image>();

            hedefRenk = Color.Lerp(Color.red, Color.green, memnuniyetPuan);
            if (sliderFill != null)
                sliderFill.color = hedefRenk;
        }
    }

    IEnumerator UrunUret()
    {
        HavuzIndex = 0;
        OyunBittimi = false;

        int noktaIndex = 0;

        while (!OyunBittimi)
        {
            yield return UrunCikisZamani;

            if (HavuzIndex >= UrunHavuzu.Length)
                break;

            Transform cikis = UrunCikisNoktalari[noktaIndex];
            noktaIndex = (noktaIndex + 1) % UrunCikisNoktalari.Length;

            GameObject urun = UrunHavuzu[HavuzIndex];
            urun.transform.position = cikis.position;
            urun.SetActive(true);

            HavuzIndex++;
            KalanUrunAdeti--;

            if (_GenelTextler.Length >= 3)
                _GenelTextler[2].text = KalanUrunAdeti.ToString();
        }
    }

    // ÜRÜN SEPETE GİRDİ
    public void UrunTopla(GameObject urun)
    {
        MevcutUrunSayisi++;

        if (_GenelTextler.Length >= 2)
            _GenelTextler[1].text = MevcutUrunSayisi.ToString();

        KaybetmeKontrolu();
    }

    public void UrunCikar(GameObject urun, bool pasif = false)
    {
        if (pasif)
        {
            KalanUrunAdeti = Mathf.Max(KalanUrunAdeti - 1, 0);

            if (_GenelTextler.Length >= 3)
                _GenelTextler[2].text = KalanUrunAdeti.ToString();
        }

        KaybetmeKontrolu();
    }

    void KaybetmeKontrolu()
    {
        if (OyunBittimi) return;

        int activeCount = 0;

        foreach (var u in UrunHavuzu)
        {
            if (u != null && u.activeInHierarchy)
            {
                Rigidbody rb = u.GetComponent<Rigidbody>();

                if (rb != null && rb.isKinematic == false)
                    activeCount++;
            }
        }

        if (MevcutUrunSayisi + activeCount < HedefUrunSayisi)
        {
            Kaybettin();
            return;
        }

        if (MevcutUrunSayisi >= HedefUrunSayisi)
        {
            Kazandin();
            return;
        }
    }

    void Kazandin()
    {
        if (OyunBittimi) return;
        OyunBittimi = true;
        PanelIslemler(3, true);
    }

    void Kaybettin()
    {
        if (OyunBittimi) return;
        OyunBittimi = true;
        PanelIslemler(4, true);
    }

    // -------------------
    // 🔊 EFEKT ÇALMA
    // -------------------
    public void SesCal(int index)
    {
        if (SesKaynak != null && SesKlipleri != null && index < SesKlipleri.Length)
            SesKaynak.PlayOneShot(SesKlipleri[index]);
    }

    // -------------------
    // 🎵 MÜZİK AÇ/KAPA
    // -------------------
    public void MuzikToggle()
{
    int mevcut = PlayerPrefs.GetInt("OyunSesi", 1);
    mevcut = 1 - mevcut;
    PlayerPrefs.SetInt("OyunSesi", mevcut);

    bool sesKapali = (mevcut == 0);

    if (OyunSesi.Instance != null)
        OyunSesi.Instance.SesKontrol(sesKapali);

    muzikIcon.sprite = (mevcut == 1) ? muzikAcikSprite : muzikKapaliSprite;
}

    // -------------------
    // 🔊 EFEKT AÇ/KAPA
    // -------------------
    public void EfektToggle()
    {
        int mevcut = PlayerPrefs.GetInt("EfektSesi", 1);
        mevcut = 1 - mevcut;
        PlayerPrefs.SetInt("EfektSesi", mevcut);

        SesKaynak.mute = (mevcut == 0);

        efektIcon.sprite = (mevcut == 1) ? efektAcikSprite : efektKapaliSprite;
    }

    public void ButonIslemleri(string Deger)
    {
        switch (Deger)
        {
            case "Basla":
                PanelIslemler(0, false);
                PanelIslemler(1, true);
                StartCoroutine(UrunUret());
                break;

            case "Durdur":
                PanelIslemler(2, true);
                Time.timeScale = 0;
                break;

            case "DevamEt":
                PanelIslemler(2, false);
                Time.timeScale = 1;
                break;

            case "Tekrar":
                Time.timeScale = 1;
                SceneManager.LoadScene(_SahneIndex);
                break;

            case "SonrakiLevel":
                Time.timeScale = 1;
                SceneManager.LoadScene(_SahneIndex + 1);
                break;

            case "Cikis":
                PanelIslemler(5, true);
                break;

            case "Evet":
                Application.Quit();
                break;

            case "Hayir":
                PanelIslemler(5, false);
                break;
        }
    }

    void PanelIslemler(int index, bool durum)
    {
        if (_Paneller != null && index < _Paneller.Length)
            _Paneller[index].SetActive(durum);
    }

    public void AdjustMemnuniyet(float delta)
    {
        memnuniyetPuan = Mathf.Clamp01(memnuniyetPuan + delta);

        memnuniyetSlider.value = memnuniyetPuan;
        sliderFill.color = Color.Lerp(Color.red, Color.green, memnuniyetPuan);
    }

    public void OnYiyecekAlindi(string yiyecekAdi)
    {
        bool dogru = System.Array.Exists(DogruYiyecekler, x => x == yiyecekAdi);
        float delta = dogru ? memnuniyetDegisim : -memnuniyetDegisim;

        AdjustMemnuniyet(delta);
    }

    public void PanelIslemlerExternal(int index, bool durum)
    {
        PanelIslemler(index, durum);
    }

    public void HediyeUrunEkle()
    {
        MevcutUrunSayisi++;

        if (_GenelTextler != null && _GenelTextler.Length >= 2)
            _GenelTextler[1].text = MevcutUrunSayisi.ToString();

        KaybetmeKontrolu();
    }

    public void GosterPopUp(string yazi)
    {
        GameObject popup = Instantiate(popupPrefab, popupParent);
        popup.transform.position = sepetKonumu.position;

        TMPro.TextMeshProUGUI txt = popup.GetComponent<TMPro.TextMeshProUGUI>();
        txt.text = yazi;

        StartCoroutine(PopAnimasyon(popup));
    }

    IEnumerator PopAnimasyon(GameObject popup)
    {
        Vector3 baslangic = Vector3.zero;
        Vector3 buyuk = Vector3.one * 1.3f;

        float sure = 0.2f;
        float t = 0;

        while (t < sure)
        {
            t += Time.deltaTime;
            popup.transform.localScale = Vector3.Lerp(baslangic, buyuk, t / sure);
            yield return null;
        }

        t = 0;
        while (t < sure)
        {
            t += Time.deltaTime;
            popup.transform.localScale = Vector3.Lerp(buyuk, Vector3.one, t / sure);
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        Destroy(popup);
    }
}
