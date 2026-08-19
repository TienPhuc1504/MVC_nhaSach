# MVC Nhà Sách

Ứng dụng nhà sách trực tuyến xây dựng bằng ASP.NET Core MVC, Entity Framework Core, SQL Server và ASP.NET Core Identity. Project hỗ trợ duyệt/tìm kiếm sách, giỏ hàng, đặt hàng, theo dõi đơn và khu vực quản trị.

## Chức năng chính

- Danh mục và tìm kiếm sách, trang chi tiết, sách nổi bật.
- Giỏ hàng lưu theo session và quy trình đặt hàng.
- Đăng ký, đăng nhập và phân quyền `Admin` / `Customer`.
- Khách hàng xem lịch sử và chi tiết đơn hàng.
- Admin quản lý sách, danh mục, đơn hàng, người dùng, ảnh nền thành viên nhóm LXM và dashboard thống kê.
- Dữ liệu mẫu được tạo tự động theo cơ chế idempotent khi ứng dụng khởi động.

## Công nghệ

- .NET 10 / ASP.NET Core MVC
- Entity Framework Core 10 (Code First + Migrations)
- SQL Server / SQL Server LocalDB
- ASP.NET Core Identity
- Bootstrap 5, CSS và JavaScript thuần

## Chạy project sau khi clone

### 1. Yêu cầu

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Windows: SQL Server Express LocalDB (có thể cài cùng Visual Studio)
- Hoặc một SQL Server khác và connection string tương ứng

### 2. Clone và khôi phục package

```powershell
git clone https://github.com/TienPhuc1504/MVC_nhaSach.git
cd MVC_nhaSach
dotnet restore
```

### 3. Tạo tài khoản demo bằng User Secrets

Các lệnh dưới đây chỉ lưu mật khẩu trên máy đang chạy, không ghi vào Git:

```powershell
dotnet user-secrets set "AdminSeed:Password" "NhaSach@2026"
dotnet user-secrets set "CustomerSeed:Password" "Customer@2026"
```

Đây là mật khẩu demo cho môi trường local. Hãy thay bằng mật khẩu riêng nếu triển khai project lên server.

### 4. Chạy ứng dụng

```powershell
dotnet run
```

Mở `http://localhost:5241`. Lần chạy đầu, ứng dụng sẽ tự động:

1. Tạo database `MVCNhaSach` nếu chưa có.
2. Áp dụng toàn bộ EF Core migrations.
3. Tạo hai role và các tài khoản demo đã cấu hình.
4. Thêm danh mục, sách, thành viên nhóm LXM và đơn hàng mẫu.

| Vai trò | Tài khoản | Mật khẩu demo |
|---|---|---|
| Admin | `admin@nhasach.local` | `NhaSach@2026` |
| Customer | `customer@nhasach.local` | `Customer@2026` |

## Cấu hình database

Mặc định project dùng SQL Server LocalDB:

```text
Server=(localdb)\mssqllocaldb;Database=MVCNhaSach;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True
```

Nếu dùng SQL Server/SQL Server Express khác, nên ghi connection string vào User Secrets:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=TEN_SERVER;Database=MVCNhaSach;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

Không cần chạy lệnh migration thủ công khi khởi động ứng dụng. Khi thay đổi model trong quá trình phát triển, tạo migration mới bằng:

```powershell
dotnet ef migrations add TenMigration
```

## Cấu trúc project

```text
Areas/Admin/       Khu vực quản trị
Controllers/       Controller phía khách hàng
Data/              DbContext, migrations và dữ liệu mẫu
Models/            Entity và enum
Services/          Nghiệp vụ giỏ hàng, đơn hàng, hình ảnh
ViewModels/         Model dành cho giao diện
Views/              Razor views
wwwroot/            CSS, JavaScript và hình ảnh tĩnh
```

## Lưu ý bảo mật

- Không commit mật khẩu, User Secrets, file database hoặc cấu hình production.
- Các file sinh bởi Visual Studio và output `bin`/`obj` đã được loại khỏi Git.
- Ảnh do Admin tải lên được lưu ngoài source tại `%LOCALAPPDATA%\MVC_nhaSach\uploads` để không làm gián đoạn Hot Reload.
- Khi triển khai thật, dùng secret manager của nền tảng triển khai và connection string riêng.
