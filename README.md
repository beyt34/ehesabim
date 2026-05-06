# eHesabim

eHesabim, bireysel finans takibi icin gelistirilmis bir ASP.NET MVC uygulamasidir. Proje; musteri bakiyeleri, banka hesaplari, krediler, kredi kartlari ve harcama kayitlarini tek bir panelde toplar.

## Ne Yapar?

- Musteri bazli borc / alacak takibi yapar.
- Banka hesaplari icin hareket ve bakiye yonetimi sunar.
- Banka kredileri ve taksit planlarini izler.
- Kredi kartlari icin limit, kalan limit, ekstre borcu, donem ici borc ve sonraki donem borcunu hesaplar.
- Harcamalari kredi karti donemleriyle iliskilendirir.
- Dashboard ekraninda tum finansal ozeti tek ekranda gosterir.

## Cozum Yapisi

- `eHesabim.Core`: cekirdek altyapi, engine, route, logging ve ortak yardimci siniflar.
- `eHesabim.Framework`: web katmanina yonelik framework bilesenleri, localization ve work context.
- `eHesabim.Services`: uygulama is kurallari ve servisler.
- `eHesabim.Tasks`: arka plan gorevleri.
- `eHesabim.Web.Portal`: ana kullanici arayuzu ve MVC uygulamasi.
- `eHesabim.Web.Task`: gorev odakli web uygulamasi.

## Baslica Ozellik Alanlari

- `HomeController`: kullanicinin gorebilecegi finansal ozet panellerini olusturur.
- `BankController`: kredi, kredi taksitleri, kredi kartlari ve kredi karti odemeleri icin yonetim ekranlarini sunar.
- `ExpenseController`: gider kayitlarini, kart donem baglantilarini ve filtreleme islemlerini yonetir.
- `CustomerController`: musteri cari takip islemlerini yonetir.
- `BankAccountController`: banka hesaplari ve hareketlerini yonetir.

## Kullanilan Teknolojiler

- .NET Framework 4.8
- ASP.NET MVC 5
- Entity Framework 6
- Autofac
- AutoMapper
- NLog
- Kendo UI for ASP.NET MVC
- IIS Express

## Dashboard Mantigi

Ana sayfa, kullanicinin yetkilerine ve tercih ettigi gorunumlere gore asagidaki panelleri doldurur:

- Musteriler
- Banka hesaplari
- Banka kredileri
- Kredi kartlari

Bu sayede toplam varlik, borc ve net durum tek ekranda izlenebilir.

## Kredi Karti Donem Mantigi

Proje kredi kartlari icin donem bazli hesaplama yapar:

- `LastDebt`: gecmis donemin ekstre borcu
- `CurrentDebt`: mevcut donem ici borc
- `NextDebt`: sonraki doneme ait borc
- `DebtTotal`: toplam kart borcu

Kredi karti harcamalari ilgili kart donemine baglanabilir. Bu mantik ozellikle dashboard ve kart listelerinde ozet borc gostermek icin kullanilir.

## Gereksinimler

- Visual Studio 2022 veya uyumlu MSBuild kurulumu
- .NET Framework 4.8 Developer Pack
- IIS Express
- Cozum icindeki NuGet paketleri

## Build

Repo kokunden PowerShell ile:

```powershell
.\run.ps1
```

Bu script:

- `eHesabim.sln` dosyasini Debug konfigurasyonunda build eder.
- Ardindan `eHesabim.Web.Portal` uygulamasini IIS Express ile `http://localhost:16347` adresinde baslatir.

MSBuild yolu projede su sekilde kullanilmaktadir:

```powershell
C:\Program Files\Microsoft Visual Studio\18\Professional\MSBuild\Current\Bin\MSBuild.exe
```

## Publish

Release publish icin:

```powershell
.\publish.ps1
```

Varsayilan publish hedefi:

```text
C:\Publish\ehesabim.com
```

Istenirse farkli klasor verilerek de kullanilabilir:

```powershell
.\publish.ps1 -PublishDir "C:\Publish\farkli-klasor"
```

## Gelistirme Notlari

- Dependency injection icin Autofac kullanilir.
- Object mapping islemleri AutoMapper ile yapilir.
- Loglama NLog ile saglanir.
- Route kayitlari uygulama baslangicinda `IRoutePublisher` uzerinden yapilir.
- Web arayuzunde grid ve form bilesenleri icin Kendo UI kullanilir.

## Amac

Bu proje, tek bir ekrandan kisisel veya kucuk olcekli finansal durum takibi yapmayi kolaylastirmayi hedefler. Cari hesap, banka, kredi ve kart hareketlerinin ayni sistemde toplanmasi, gunluk finans operasyonlarini ve borc takibini sadeleştirir.
