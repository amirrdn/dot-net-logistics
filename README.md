# Web App

## Persyaratan Sistem
- .NET 8.0 SDK atau lebih baru
- Node.js (untuk menjalankan Tailwind CSS)
- Visual Studio 2022 atau Visual Studio Code dengan C# extension

## Langkah-langkah Menjalankan Proyek

1. Clone repository ini ke komputer lokal Anda

2. Buka terminal di direktori proyek dan jalankan perintah berikut untuk menginstall dependencies:
   ```bash
   dotnet restore
   npm install
   ```

3. Jalankan Tailwind CSS build process:
   ```bash
   npm run build
   ```

4. Jalankan proyek:
   ```bash
   dotnet run
   ```

5. Buka browser dan akses aplikasi di:
   ```
   https://localhost:5001
   ```

## Cara Melakukan Migrasi Database

1. Pastikan Anda berada di direktori proyek `Logistics` di terminal
   ```bash
   cd /path/to/Logistics
   ```

2. Untuk membuat migrasi baru:
   ```bash
   dotnet ef migrations add NamaMigrasi
   ```
   File migrasi akan dibuat di folder `Logistics/Migrations/`

3. Untuk menerapkan migrasi ke database:
   ```bash
   dotnet ef database update
   ```
   Migrasi akan diterapkan ke database yang dikonfigurasi di `appsettings.json`

4. Untuk menghapus migrasi terakhir:
   ```bash
   dotnet ef migrations remove
   ```
   File migrasi terakhir akan dihapus dari folder `Logistics/Migrations/`

5. Untuk melihat daftar migrasi yang ada:
   ```bash
   dotnet ef migrations list
   ```
   Menampilkan semua file migrasi yang ada di folder `Logistics/Migrations/`

## API Endpoints

### Autentikasi (api/Auth)
- `POST /api/Auth/login` - Login untuk mendapatkan token JWT
- `GET /api/Auth/check-token` - Memeriksa validitas token

### Pengiriman (api/Shipment)
- `GET /api/Shipment` - Mendapatkan daftar semua pengiriman (memerlukan autentikasi)
- `GET /api/Shipment/{id}` - Mendapatkan detail pengiriman berdasarkan ID (memerlukan autentikasi)
- `GET /api/Shipment/AWB/{awb}` - Mendapatkan detail pengiriman berdasarkan AWB (memerlukan autentikasi)
- `POST /api/Shipment` - Membuat pengiriman baru (memerlukan autentikasi)
- `PUT /api/Shipment/{id}` - Mengupdate data pengiriman (memerlukan autentikasi)
- `DELETE /api/Shipment/{id}` - Menghapus pengiriman (memerlukan autentikasi)

### Tracking (api/Tracking)
- `GET /api/Tracking` - Mendapatkan riwayat tracking semua pengiriman (memerlukan autentikasi)
- `GET /api/Tracking/{id}` - Mendapatkan detail status tracking berdasarkan ID (memerlukan autentikasi)
- `GET /api/Tracking/AWB/{awb}` - Mendapatkan riwayat tracking berdasarkan AWB (memerlukan autentikasi)
- `POST /api/Tracking` - Membuat status tracking baru (memerlukan autentikasi)
- `PUT /api/Tracking/{id}` - Mengupdate status tracking (memerlukan autentikasi)
- `DELETE /api/Tracking/{id}` - Menghapus status tracking (memerlukan autentikasi)

## Kredensial Login
Untuk mengakses sistem, gunakan kredensial berikut:

- **User ID**: admin
- **Password**: admin123

## Catatan Penting
- Pastikan port 5001 tidak sedang digunakan oleh aplikasi lain
- Jika mengalami masalah dengan Tailwind CSS, coba jalankan `npm run watch` untuk mode development
- Database akan otomatis dibuat saat pertama kali menjalankan aplikasi 