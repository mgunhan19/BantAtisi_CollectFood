using System.Collections.Generic;
using UnityEngine;

public class TargetManager3D : MonoBehaviour
{
   [Header("Hedef Noktaları (TargetPoint1,2,3...)")]
    public List<Transform> hedefNoktalari = new List<Transform>();

    [Header("Hedef Prefabları (3D ürünler)")]
    public List<GameObject> hedefPrefabList = new List<GameObject>();

    [Header("Hedef İsimleri (Prefab isimleriyle aynı olmalı)")]
    public List<string> hedefIsimleri = new List<string>(); 

    private List<GameObject> aktifHedefler = new List<GameObject>();

    void Start()
    {
        Debug.Log("Hedef sistemi başlatıldı");
        HedefleriOlustur();
    }

    void HedefleriOlustur()
    {
        if (hedefNoktalari.Count == 0)
        {
            Debug.LogError("❌ Hedef noktaları atanmadı!");
            return;
        }

        if (hedefIsimleri.Count == 0)
        {
            Debug.LogError("❌ Hedef isimleri listesi boş!");
            return;
        }

        // Önceki aktif hedefleri temizle
        foreach (var hedef in aktifHedefler)
        {
            if (hedef != null)
                Destroy(hedef);
        }
        aktifHedefler.Clear();

        for (int i = 0; i < hedefIsimleri.Count && i < hedefNoktalari.Count; i++)
        {
            string isim = hedefIsimleri[i].ToUpper();
            GameObject prefab = hedefPrefabList.Find(p => p.name.ToUpper() == isim);

            if (prefab == null)
            {
                Debug.LogError($"❌ Prefab bulunamadı: {isim}");
                continue;
            }

            Transform nokta = hedefNoktalari[i];
            // Yeni hedefi belirtilen noktada oluştur
            GameObject yeni = Instantiate(prefab, nokta.position, Quaternion.identity);
            yeni.SetActive(true);

            // =======================================================
            // 🛑 GÖRSEL MOD: Rigidbody ve Collider'ları Devre Dışı Bırak
            // =======================================================

            // 1. Rigidbody'yi Devre Dışı Bırak (Fizik motorundan çıkarır)
            Rigidbody rb = yeni.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true; 
                rb.detectCollisions = false;
            }

            // 2. Tüm Collider'ları Devre Dışı Bırak (Çarpışma algılamasını kapatır)
            // Kendi üzerindeki ve altındaki tüm Collider'ları bulup deaktif eder.
            Collider[] colliders = yeni.GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = false;
            }

            // =======================================================

            // Kameraya dönmesini sağla
            yeni.transform.LookAt(Camera.main.transform);
            yeni.transform.Rotate(270, 0, 0);

            // Tüm prefablarda aynı maksimum boyutu ayarlama (Ölçeklendirme)
            Vector3 targetScale = yeni.transform.localScale;
            Bounds bounds = new Bounds(yeni.transform.position, Vector3.zero);
            Renderer[] rends = yeni.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in rends)
                bounds.Encapsulate(r.bounds);

            float maxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
            yeni.transform.localScale = targetScale * (0.3f / maxSize);

            aktifHedefler.Add(yeni);

            Debug.Log($"✅ {isim} hedefi oluşturuldu ve ölçeklendirildi (Görsel Modda).");
        }

        if (aktifHedefler.Count == 0)
        {
            Debug.LogError("❌ Hiç hedef oluşturulamadı! Listeleri kontrol et.");
        }
    }
}