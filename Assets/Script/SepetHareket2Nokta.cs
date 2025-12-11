using UnityEngine;

public class SepetHareket : MonoBehaviour
{
   [Header("Hareket Ayarları")]
    public float speed = 2f;       // Hız
    public float mesafe = 3f;      // Gidip geleceği mesafe
    private Vector3 baslangicPoz;  // İlk pozisyon

    [Header("Fizik Ayarları")]
    public bool objeyiKinematicYap = true; // Obje girince fizik kapanacak mı?

    void Start()
    {
        baslangicPoz = transform.position;
    }

    void Update()
    {
        // Sepetin ileri–geri hareketi
        float x = Mathf.PingPong(Time.time * speed, mesafe);
        transform.position = baslangicPoz + new Vector3(x, 0, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Obje sepete girince parent yapılır
        other.transform.SetParent(transform);

        if (objeyiKinematicYap && other.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true; // Fizik kapatılır, sepetle düzgün gider
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Obje sepetten çıkınca parent kaldırılır
        other.transform.SetParent(null);

        if (objeyiKinematicYap && other.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = false; // Fizik tekrar açılır
        }
    }
}
