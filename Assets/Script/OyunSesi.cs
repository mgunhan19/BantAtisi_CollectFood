using UnityEngine;

public class OyunSesi : MonoBehaviour
{
    public static OyunSesi Instance;

    [SerializeField] AudioSource Ses;

    void Awake()
    {
        // BURAYA GELİYOR !!!
        if (Instance != null && Instance != this)
        {
            //Destroy(gameObject); // Yeni gelen fazlayı yok ediyoruz
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        
        int durum = PlayerPrefs.GetInt("OyunSesi", 1);
        Ses.mute = (durum == 0);
    }

    public void SesKontrol(bool kapat)
    {
        Ses.mute = kapat;
    }
}
