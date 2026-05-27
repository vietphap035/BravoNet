# BravoNet (DACS_1)

BravoNet (tên project: **DACS_1**) là một ứng dụng desktop quản lý hệ thống máy trạm (Cyber Game / Tiệm Net) được phát triển bằng nền tảng **WinUI 3** mới nhất của Microsoft kết hợp với **.NET 8**.  

Ứng dụng cung cấp giao diện hiện đại, mượt mà và hiệu năng cao giúp quản lý:

- Máy trạm
- Người dùng
- Nhân viên
- Hóa đơn
- Dịch vụ ăn uống
- Thống kê doanh thu

---

#  Tính năng chính

## Hệ thống Đăng nhập (Authentication)

- Xác thực tài khoản nhân viên/quản lý trực tiếp với Database
- Tự động cập nhật:
  - `is_online`
  - `last_login`
- Quản lý phiên đăng nhập toàn cục

---

## Quản lý Máy trạm

Trang: `DanhSachMayPage`

- Theo dõi trạng thái máy:
  - Đang hoạt động
  - Đang tắt
  - Đang sử dụng
- Quản lý thời gian sử dụng máy
- Hiển thị trực quan trạng thái từng máy

---

## Quản lý Người dùng

Trang: `DanhSachNguoiDungPage`

- Quản lý tài khoản hội viên
- Nạp tiền tài khoản
- Nạp thời gian sử dụng
- Theo dõi thông tin khách hàng

---

## Quản lý Nhân viên

Trang: `DanhSachNhanVien`

- Quản lý thông tin nhân viên
- Phân quyền hệ thống
- Theo dõi hoạt động nhân sự

---

## Quản lý Dịch vụ & Thực phẩm

Trang: `DanhSachThucPham`

- Quản lý menu đồ ăn/thức uống
- Gọi món trực tiếp
- Quản lý kho dịch vụ

---

## Quản lý Hóa đơn

Trang: `DanhSachHoaDon`

- Lưu trữ lịch sử giao dịch
- Theo dõi hóa đơn thanh toán
- Quản lý lịch sử nạp tiền

---

## Thống kê & Báo cáo

Trang: `Thongkepage`

- Biểu đồ doanh thu
- Thống kê hiệu suất máy trạm
- Báo cáo trực quan bằng biểu đồ

---

# Công nghệ sử dụng

| Thành phần | Công nghệ |
|---|---|
| Framework | WinUI 3 |
| Windows App SDK | 1.7.250606001 |
| Target Framework | .NET 8 |
| Database | MySQL |
| UI Components | CommunityToolkit.WinUI |
| Chart Library | LiveChartsCore + SkiaSharp |
| Real-time | SignalR Client |

---

# NuGet Packages

```xml
<PackageReference Include="CommunityToolkit.WinUI.Controls.Primitives" Version="8.2.250402" />
<PackageReference Include="CommunityToolkit.WinUI.UI.Controls.DataGrid" Version="7.1.2" />
<PackageReference Include="LiveChartsCore.SkiaSharpView.WinUI" Version="2.0.0-rc5.3" />
<PackageReference Include="Microsoft.AspNetCore.SignalR.Client" Version="10.0.7" />
<PackageReference Include="Microsoft.Data.SqlClient" Version="7.0.1" />
<PackageReference Include="MySql.Data" Version="9.3.0" />
```

---

# Cấu trúc thư mục chính

```plaintext
BravoNet-0d90e04f735a8788795cae8bc37b04802ea8e877/
│
├── Assets/
│   ├── ACTIVE.png
│   └── NOT_ACTIVE.png
│
├── Database/
│   └── DatabaseConnection.cs
│
├── Model/
│   ├── Machine.cs
│   ├── UserModel.cs
│   └── OrderModel.cs
│
├── App.xaml
├── App.xaml.cs
│
├── Login.xaml
├── Login.xaml.cs
│
├── DanhSachMayPage.xaml
├── DanhSachNguoiDungPage.xaml
├── DanhSachNhanVien.xaml
├── DanhSachThucPham.xaml
├── DanhSachHoaDon.xaml
└── Thongkepage.xaml
```

---

# Hướng dẫn cài đặt & chạy ứng dụng

## 1️. Yêu cầu hệ thống

### Hệ điều hành

- Windows 10 (Build 19041 trở lên)
- Windows 11

### Công cụ phát triển

- Visual Studio 2022 v17.8+
- Đã cài workload:
  - **Windows Application Development**

### Cơ sở dữ liệu

- MySQL Server
- SQL Server

---

## 2️. Cấu hình Database

Mở file:

```plaintext
Database/DatabaseConnection.cs
```

Thay đổi chuỗi kết nối:

```csharp
private static readonly string connectionString =
    "Server=localhost;Database=dacs1;Uid=your_username;Pwd=your_password";
```

---

## Yêu cầu bảng Database

Database cần có bảng `accounts` với các trường tối thiểu:

| Field | Type |
|---|---|
| username | VARCHAR |
| pwd | VARCHAR |
| UId | INT |
| is_online | BOOLEAN |
| last_login | DATETIME |

---

## 3️. Build và chạy dự án

### Bước 1

Mở file:

```plaintext
DACS_1.sln
```

bằng Visual Studio 2022.

---

### Bước 2

Chờ Visual Studio tự động restore NuGet Packages.

---

### Bước 3

Chọn cấu hình chạy:

```plaintext
Debug | x64
```

---

### Bước 4

Nhấn:

```plaintext
F5
```

hoặc chọn:

```plaintext
Start
```

để build và chạy ứng dụng.

---

# Giao diện ứng dụng

## Login

- Đăng nhập nhân viên/quản lý
- Kết nối trực tiếp MySQL

## Dashboard

- Theo dõi trạng thái hệ thống
- Điều hướng nhanh các chức năng

## Machine Management

- Danh sách máy
- Trạng thái online/offline

## Revenue Statistics

- Thống kê doanh thu bằng biểu đồ

---

# Định hướng phát triển

- Đồng bộ trạng thái máy trạm real-time bằng SignalR
- Tích hợp SQL Server
- Hệ thống đặt máy online
- Quản lý từ xa qua mobile app

---

# Tác giả

Project được phát triển nhằm phục vụ mục đích:

- Đồ án chuyên ngành
- Học tập WinUI 3
- Quản lý Cyber Game thực tế

---

# License

Dự án phục vụ mục đích học tập và nghiên cứu.
```
