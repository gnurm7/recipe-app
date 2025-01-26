# Tarif Rehberi Uygulaması  

## Kocaeli Üniversitesi  
### Bilgisayar Mühendisliği Bölümü  
### Yazılım Lab. I - I. Proje  
 

---  

## Proje Hakkında  

Kullanıcının yemek tariflerini saklayabileceği ve mevcut malzemelerle hangi yemeklerin yapılabileceğini gösteren bir masaüstü uygulaması geliştirilecektir. Uygulama, veritabanı yönetimi, dinamik arama ve filtreleme gibi işlevleri desteklemektedir.  

## Amaçlar  
1. Dinamik arama ve filtreleme özelliklerine sahip bir masaüstü uygulaması geliştirmek.  
2. Uygulama içerisinde istenilen özellikteki ürünlerin filtrelenmesi ve sıralanması özelliklerini sağlamak.  
3. Veritabanı yönetimi ve algoritma geliştirme konularındaki becerileri geliştirmek.  
4. Kullanıcı arayüzü tasarımı ve kullanıcı dostu yazılım geliştirme hakkında deneyim kazandırmak.  

## Programlama Dili  
- C# ya da Java  

## Veritabanı Tasarımı  

### Veritabanı Tabloları  

- **Tarifler Tablosu:**  
  - `TarifID (Primary Key, int)`: Benzersiz tarif ID'si.  
  - `TarifAdi (varchar)`: Tarifin adı.  
  - `Kategori (varchar)`: Tarifin ait olduğu kategori (Ana Yemek, Tatlı, vb.).  
  - `HazirlamaSuresi (int)`: Tarifin hazırlanma süresi (dakika).  
  - `Talimatlar (text)`: Tarifin hazırlanış adımları.  

- **Malzemeler Tablosu:**  
  - `MalzemeID (Primary Key, int)`: Benzersiz malzeme ID'si.  
  - `MalzemeAdi (varchar)`: Malzemenin adı.  
  - `ToplamMiktar (varchar)`: Depodaki toplam miktar.  
  - `MalzemeBirim (varchar)`: Malzemenin birimi (kilo, litre, gram).  
  - `BirimFiyat (float)`: Malzemenin birim maliyeti.  

- **Tarif-Malzeme İlişkisi Tablosu:**  
  - `TarifID (Foreign Key)`: İlgili tarifin ID'si.  
  - `MalzemeID (Foreign Key)`: İlgili malzemenin ID'si.  
  - `MalzemeMiktar (float)`: Tarif için gerekli malzeme miktarı.  

### Normalizasyon  
Tablolar, veritabanı normalizasyon kurallarına göre tasarlanacaktır. Tarif-Malzeme İlişkisi tablosu, birçok-taneye çoklu ilişkiyi temsil ediyor.  

## Kullanıcı Arayüzü (GUI) Tasarımı  

- Ana ekranda tüm tariflerin listelendiği bir alan olmalıdır.  
- Menü: Tarif ekleme, güncelleme ve silme işlemleri için seçenekler.  
- Tarif Listesi: Tarif adı, hazırlama süresi ve maliyet bilgileriyle görüntülenecektir.  
- Arama ve Filtreleme Alanı: Tarif arama ve filtreleme işlemleri yapılacaktır.  
- Sonuç Ekranı: Farklı kriterdeki aramalara göre yapılabilecek yemekler listelenecektir.  

## Fonksiyonel Özellikler  

### Tarif Ekleme  
- Kullanıcı, tarifin adı, kategorisi, hazırlama süresi ve talimatları ekler.  
- Malzemeler, mevcutsa seçilip eklenir, aksi takdirde yeni malzeme eklenebilir.  
- "Tarif Ekle" butonuyla tarif ve malzeme bilgileri veritabanına kaydedilir.  

### Tarif Önerisi  
- Yetersiz malzemeli tarifler kırmızı, yeterli malzemeye sahip olanlar yeşil renkte gösterilecektir.  
- Kırmızı tariflerde eksik malzemelerin toplam maliyeti gösterilecektir.  

### Dinamik Arama  
- Tarif adı veya malzemeye göre arama yapılabilmektedir.  
- Eşleşme yüzdesine göre tarifler listelenecektir.  

### Filtreleme ve Sıralama  
- Kullanıcı, tarifleri hazırlama süresine veya maliyetine göre filtreleyebilir
![entry](https://github.com/user-attachments/assets/51f46f2b-664d-4344-a59f-97067ad4f2c5)
