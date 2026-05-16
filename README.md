# Collect Food 🍎🍔🍕

**Collect Food**, Unity motoru ile geliştirilmiş, banttan gelen yiyecek ve malzemeleri doğru zamanlama ve fırlatma mekanikleriyle sepetin içine atmaya çalıştığınız eğlenceli ve dinamik bir mobil/PC arcade oyunudur. 

Fizik tabanlı fırlatma kontrolleri, nesne havuzlama (Object Pooling) sistemi ve akıcı oynanışı ile hem reflekslerinizi test eder hem de keyifli bir oyun deneyimi sunar.

---

## 🎮 Nasıl Oynanır?

Oyunun temel amacı, bant üzerinde ilerleyen yiyecekleri ıskalamadan sepetin içerisine fırlatıp en yüksek skoru toplamaktır.

1. **Malzemeyi Seçin:** Bant üzerinde hareket eden bir yiyeceğin (ürünün) üzerine farenin sol tuşuyla (veya mobil ekranda parmağınızla) **basılı tutun**.
2. **Kaydırın ve Fırlatın (Swipe):** Parmağınızı veya farenizi **yukarı doğru hızlıca kaydırıp bırakın**. Kaydırma hızınız ve mesafeniz, ürünün fırlatılma gücünü belirler.
3. **Sepete İsabet Ettirin:** Fırlatılan yiyecek **"Sepetici"** alanına girdiğinde başarıyla toplanır, puan kazandırır ve sepetin içinde fiziksel olarak durarak (kinematik hale gelerek) ekranda kalır.
4. **Zemine Dikkat Edin:** Eğer yiyeceği yanlış bir açıyla fırlatırsanız veya sepeti ıskalarsanız yiyecek **Zemine** düşer. Zemine düşen yiyecekler de sepet içindekiler gibi oldukları yerde durup ekranda kalmaya devam eder ancak size puan kazandırmaz.
5. **Bant Sonunu Kaçırmayın:** Eğer bant üzerindeki bir yiyeceğe hiç dokunmazsanız, bandın sonundaki **"MalzemePasiflestirici"** alanına çarparak otomatik olarak kaybolur ve nesne havuzuna geri döner.

---

## 🚀 Öne Çıkan Özellikler

* **Fizik Tabanlı Kaydırma (Swipe) Mekaniği:** Gerçekçi `AddForce` hesaplamaları ile kaydırma yönünüze ve hızınıza duyarlı fırlatma sistemi.
* **Gelişmiş Nesne Havuzlama (Object Pooling):** `GameManager` entegrasyonu sayesinde dinamik olarak üretilen yiyecekler belleği yormaz, performans optimizasyonu sağlar.
* **Görsel ve Sesli Geri Bildirimler:** Sepete girme, yere düşme, duvara çarpma ve fırlatma anları için özel ses efektleri entegrasyonu.
* **Hassas Çarpışma Yönetimi:** `isKinematic` ve `Collider` kontrolleri sayesinde sepete giren ya da yere düşen objelerin fizik motorunu yormadan ekranda estetik bir şekilde sabit kalması.

---

## 💻 Kod Yapısı Hakkında Bilgi

Oyundaki ana mekaniklerden biri olan `urun.cs` script'i şu görevleri üstlenir:
* **Giriş Algılama:** `OnMouseDown` ve `OnMouseUp` fonksiyonları ile oyuncunun dokunma başlangıç ve bitiş noktaları arasındaki farkı (`Vector3`) hesaplar.
* **Hız Limitörü:** Aşırı hızlı kaydırmalarda nesnenin haritadan çıkmasını engellemek için maksimum fırlatma gücü sınırlandırılmıştır.
* **Durum Yönetimi:** Objelerin tek bir kez tetiklenmesi için `SepeteGirdimi` ve `ZemineDustumu` gibi boolean bayraklar (flags) kullanılır. Çarpışma anında hızlar sıfırlanarak nesne kinematik hale getirilir, böylece ekranda sabit durması sağlanır.

---

## 🛠️ Kurulum ve Geliştirme

Projeyi kendi bilgisayarınızda açmak ve geliştirmek için:

1. Bu depoyu klonlayın:
   ```bash
   git clone https://github.com/kullaniciadi/collect-food.git
Unity Hub uygulamasını açın.

Projects > Add butonuna tıklayarak projenin ana klasörünü seçin.

Projeyi uygun Unity sürümü ile açın (Önerilen: Unity 2021.3 LTS veya üzeri).

Scenes klasöründeki ana sahneyi açarak Play butonuna basın.

Mehmet Günhan
