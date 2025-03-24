# My_Cv_Web_App

## Proje Açıklaması
Bu proje, kişisel CV/portfolyo bilgilerimi sergilemek üzere tasarlanmış modern ve yönetilebilir bir web uygulamasıdır. Admin paneli aracılığıyla CV içeriğinin dinamik olarak güncellenebilmesini sağlayan bu uygulama, ASP.NET Core MVC mimarisi kullanılarak geliştirilmiştir.

## 🌟 Özellikler

- **Admin Paneli**: CV içeriğini dinamik olarak yönetebilme imkanı
- **Kullanıcı Dostu Arayüz**: Modern ve responsive tasarım
- **Güvenli Yetkilendirme**: Güvenli giriş ve yetkilendirme sistemi
- **Dinamik İçerik Yönetimi**: CV bölümlerinin kolayca düzenlenebilmesi
- **SEO Uyumlu Yapı**: Arama motorlarında daha iyi sıralama için optimize edilmiş

## 🛠️ Kullanılan Teknolojiler

- **Backend**: ASP.NET Core MVC
- **Frontend**: HTML, CSS, JavaScript, Bootstrap
- **Veritabanı**: Entity Framework Core
- **Güvenlik**: ASP.NET Core Identity
- **Deployment**: Plesk Panel

## 📋 Kurulum

1. Projeyi klonlayın:
```bash
git clone https://github.com/erennali/My_Cv_Web_App.git
```

2. Gerekli paketleri yükleyin:
```bash
cd My_Cv_Web_App
dotnet restore
```

3. Veritabanını migration işlemlerini gerçekleştirin:
```bash
dotnet ef database update
```

4. Uygulamayı çalıştırın:
```bash
dotnet run
```

5. Tarayıcınızda şu adresi açın: `http://localhost:5000`

## 🔧 Kullanım

### Admin Paneli Erişimi
- `/Admin/Account/Login` adresinden admin paneline erişim sağlayabilirsiniz
- Varsayılan admin bilgileriyle giriş yapın (Güvenlik nedeniyle ilk kurulumdan sonra şifrenizi değiştirmeniz önerilir)

### CV İçeriğini Düzenleme
1. Admin paneline giriş yapın
2. Dashboard üzerinden düzenlemek istediğiniz bölümü seçin
3. İçeriği güncelleyin ve kaydedin
4. Değişiklikler anında web sitesine yansıyacaktır

## 🚀 Deployment

Bu web uygulaması Plesk panel üzerinde host edilmektedir. Deployment işlemi için:

1. Projeyi build edin:
```bash
dotnet publish -c Release
```

2. Oluşturulan dosyaları sunucunuzdaki `httpdocs` klasörüne yükleyin
3. Sunucu üzerinde gerekli yapılandırmaları tamamlayın

Not: SVG formatında favicon kullanılmıştır, ancak tüm tarayıcı uyumluluğu için ICO ve PNG formatlarında da alternatifler eklenmiştir.

## 🔒 Güvenlik

Bu uygulamada ASP.NET Core Identity kullanılarak güvenli bir yetkilendirme sistemi sağlanmıştır. Admin giriş bilgilerinizi kimseyle paylaşmayın ve düzenli olarak şifrenizi değiştirin.

## 🤝 Katkıda Bulunma

1. Bu repository'yi fork edin
2. Feature branch'i oluşturun (`git checkout -b feature/AmazingFeature`)
3. Değişikliklerinizi commit edin (`git commit -m 'Add some AmazingFeature'`)
4. Branch'inize push edin (`git push origin feature/AmazingFeature`)
5. Pull Request oluşturun


## 👨‍💻 Geliştirici Bilgileri

**Eren Ali Koca**
- GitHub: [erennali](https://github.com/erennali)
- Website: [erenalikoca.com](https://erenalikoca.com.tr)

---

© 2025 Eren Ali Koca. Tüm hakları saklıdır.
