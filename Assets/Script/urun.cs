using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class urun : MonoBehaviour
{
    Vector3 _DokunmaBaslangicPoz;
    Vector3 _DokunmaBitisPoz;

    bool islemTamamlandi = false;

    [Header("Ürün Adı")]
    [SerializeField] string urunAdi;

    [Header("Fizik")]
    [SerializeField] Rigidbody _Rb;

    bool _AtildiMi;
    bool ZemineDustumu, SepeteGirdimi;

    float _Guc = 0.2f;

    [Header("Zemin Efekti (İstersen boş bırak)")]
    public GameObject ZeminEfektiPrefab;

    private void OnMouseDown()
    {
        if (Time.timeScale == 0) return;
        _DokunmaBaslangicPoz = Input.mousePosition;
    }

    private void OnMouseUp()
    {
        if (Time.timeScale == 0) return;

        _DokunmaBitisPoz = Input.mousePosition;
        Vector3 fark = _DokunmaBitisPoz - _DokunmaBaslangicPoz;

        if (fark.y > 0)
        {
            if (fark.y > 1200) fark.y = 1000;
            Firlat(fark);
        }
    }

    void Firlat(Vector3 Guc)
    {
        if (_AtildiMi) return;

        GameManager.Instance.SesCal(4);

        _Rb.AddForce(new Vector3(Guc.x, Guc.y, Guc.y) * _Guc);
        _AtildiMi = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // ================================
        // 📌 SEPET
        // ================================
        if (other.CompareTag("Sepetici") && !SepeteGirdimi)
        {
            SepeteGirdimi = true;

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;   // 🔥 GameManager artık bunu aktif saymaz
            }

            Collider col = GetComponent<Collider>();
            if (col != null)
                col.enabled = false;

            GameManager.Instance.UrunTopla(gameObject);
            GameManager.Instance.SesCal(0);
            GameManager.Instance.OnYiyecekAlindi(urunAdi);

            Debug.Log($"{urunAdi} sepete girdi!");
            return;
        }

        // ================================
        // 📌 DUVAR
        // ================================
        if (other.CompareTag("Duvar"))
        {
            GameManager.Instance.SesCal(5);
            Debug.Log($"{urunAdi} duvara çarptı!");
            return;
        }

        // ================================
        // 📌 ZEMİN
        // ================================
        if (other.CompareTag("Zemin") && !ZemineDustumu)
        {
            ZemineDustumu = true;

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;  // 🔥 Yere düşen ürün = pasif ürün
            }

            Collider col = GetComponent<Collider>();
            if (col != null)
                col.enabled = false;

            if (ZeminEfektiPrefab != null)
            {
                GameObject efekt = Instantiate(ZeminEfektiPrefab, transform.position, Quaternion.identity);
                Destroy(efekt, 1.2f);
            }

            if (!islemTamamlandi)
            {
                islemTamamlandi = true;

                GameManager.Instance.UrunCikar(gameObject, pasif: true);
                GameManager.Instance.SesCal(5);

                Debug.Log($"{urunAdi} yere düştü → pasif!");
            }

            return;
        }

        // ================================
        // 📌 BANT SONU PASİFLEŞTİRİCİ
        // ================================
        if (other.CompareTag("MalzemePasiflestirici"))
        {
            gameObject.SetActive(false);

            GameManager.Instance.UrunCikar(gameObject, pasif: true);

            Debug.Log($"{urunAdi} bant sonunda pasifleştirildi!");
            return;
        }
    }

    private void OnEnable()
    {
        islemTamamlandi = false;
        _AtildiMi = false;
        ZemineDustumu = false;
        SepeteGirdimi = false;

        if (_Rb != null)
            _Rb.isKinematic = false;

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = true;
    }
}
