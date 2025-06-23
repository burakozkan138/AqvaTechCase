# Sözcü Haber Arama Uygulaması

Bu proje, **Sözcü** gazetesinden haber makalelerini otomatik olarak toplayan (web scraping) ve **Elasticsearch** ile güçlü arama özelliği sunan bir web uygulamasıdır.

## 🔗 Demo

- **Web UI**: [http://localhost:5001](http://localhost:5001) 
- **Elasticsearch API**: [http://localhost:9200](http://localhost:9200)

## 🏗️ Proje Yapısı

```
AqvaTechCase/
├── src/
│   ├── Crawler/           # Web scraping uygulaması
│   │   ├── Models/        # Article model
│   │   ├── Services/      # Scraper servisi
│   │   └── Program.cs     # Ana crawler uygulaması
│   └── WebUI/             # ASP.NET Razor Pages web uygulaması
│       ├── Models/        # Web modelleri
│       ├── Services/      # Elasticsearch servisi
│       ├── Pages/         # Razor Pages
│       └── Program.cs     # Web server
├── docker-compose.yml     # Elasticsearch
├── .gitignore
└── readme.md
```

## ⚡ Hızlı Başlangıç

### 1. Ön Gereksinimler

- **.NET 9 SDK**: [İndir](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Docker Desktop**: [İndir](https://www.docker.com/products/docker-desktop)

### 2. Elasticsearch & Kibana Başlatma

```bash
# Repository'yi klonla
git clone https://github.com/burakozkan138/AqvaTechCase
cd AqvaTechCase

# Elasticsearch'ü başlat
docker run -d --name elasticsearch -e "discovery.type=single-node" -e "xpack.security.enabled=false" -p 9200:9200 elasticsearch:8.18.2

# Elasticsearch'in hazır olup olmadığını kontrol et
curl http://localhost:9200
```

### 3. Haber Verilerini Toplama

```bash
# Crawler'ı çalıştır (veri toplama)
cd src/Crawler
dotnet restore
dotnet run
```

### 4. Web Uygulamasını Başlatma

```bash
# Web UI'ı çalıştır
cd ../WebUI
dotnet restore
dotnet run
```

### 5. Uygulamayı Kullanma

- **Ana sayfa**: [https://localhost:5001](https://localhost:5001)
- Arama kutusuna istediğiniz kelimeyi yazın
- Haberler arasında anlık arama yapın