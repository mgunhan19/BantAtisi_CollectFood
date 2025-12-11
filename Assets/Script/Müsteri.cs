using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Müsteri : MonoBehaviour
{
    public static Müsteri Instance;

    [Header("Yüz Sprite'ı")]
    [SerializeField] RawImage yuzImage;      // Canvas üzerindeki Image
    [SerializeField] Texture mutluYuz;     // 2D mutlu yüz sprite'ı
    [SerializeField] Texture uzgunYuz;     // 2D üzgün yüz sprite'ı

    [Header("Müşterinin sevdiği ürünler")]
    public List<string> dogruUrunler;     // Bu ürünler sepete girerse mutlu olur

    private void Awake()
    {
        Instance = this;
        yuzImage.enabled = false;
    }

    public void UrunDegerlendir(string urunAdi)
    {
        bool dogruMu = dogruUrunler.Contains(urunAdi);

        yuzImage.enabled = true;
        yuzImage.texture = dogruMu ? mutluYuz : uzgunYuz;

        StopAllCoroutines();
        StartCoroutine(YuzKaybol());
    }

    IEnumerator YuzKaybol()
    {
        yield return new WaitForSeconds(1.5f);
        yuzImage.enabled = false;
    }
}
