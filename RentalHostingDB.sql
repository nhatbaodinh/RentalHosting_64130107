-- Xóa cơ sở dữ liệu cũ nếu tồn tại
USE master;
GO

DROP DATABASE IF EXISTS RentalHostingDB;
GO

-- Tạo cơ sở dữ liệu mới
CREATE DATABASE RentalHostingDB;
GO

USE RentalHostingDB;
GO

-- Bảng NguoiDung
CREATE TABLE NguoiDung (
    NguoiDungID INT PRIMARY KEY IDENTITY(1,1),       -- ID duy nhất cho mỗi người dùng
    HoTen NVARCHAR(100) NOT NULL,              -- Họ và tên
    Email NVARCHAR(100) NOT NULL UNIQUE,       -- Email duy nhất
    MatKhau NVARCHAR(200) NOT NULL,            -- Mật khẩu
    SoDienThoai NVARCHAR(15),                  -- Số điện thoại
    DiaChi NVARCHAR(200),                      -- Địa chỉ
    Role INT CHECK (Role IN (0, 1, 2)) NOT NULL -- 0: Khách hàng, 1: Nhân viên, 2: Admin
);
GO

-- Bảng LoaiHosting
CREATE TABLE LoaiHosting (
    LoaiHostingID INT PRIMARY KEY IDENTITY(1,1),      -- ID loại hosting
    TenLoai NVARCHAR(100) NOT NULL             -- Tên loại hosting
);
GO

-- Bảng Hosting (dịch vụ hosting/server)
CREATE TABLE Hosting (
    HostingID INT PRIMARY KEY IDENTITY(1,1),   -- ID hosting
    TenHosting NVARCHAR(100) NOT NULL,         -- Tên dịch vụ hosting
    MoTa NVARCHAR(MAX),                        -- Mô tả dịch vụ
    DonGia DECIMAL(18,2) NOT NULL,             -- Đơn giá
    LoaiHostingID INT NOT NULL,                       -- Loại hosting
    FOREIGN KEY (LoaiHostingID) REFERENCES LoaiHosting(LoaiHostingID)
);
GO

-- Bảng HopDong
CREATE TABLE HopDong (
    HopDongID INT PRIMARY KEY IDENTITY(1,1),  -- ID hợp đồng
    NgayBatDau DATE NOT NULL,                  -- Ngày bắt đầu
    NgayKetThuc DATE NOT NULL,                 -- Ngày kết thúc
    TrangThai INT CHECK (TrangThai IN (0, 1, 2)) NOT NULL, -- 0: Đang xử lý, 1: Hoàn thành, 2: Hủy
    NguoiDungID INT NOT NULL,                   -- ID khách hàng
    NhanVienID INT NOT NULL,                   -- ID nhân viên
    FOREIGN KEY (NguoiDungID) REFERENCES NguoiDung(NguoiDungID), -- Liên kết với khách hàng
    FOREIGN KEY (NhanVienID) REFERENCES NguoiDung(NguoiDungID)  -- Liên kết với nhân viên
);
GO

-- Bảng ChiTietHopDong
CREATE TABLE ChiTietHopDong (
    HopDongID INT NOT NULL,                   -- ID hợp đồng
    HostingID INT NOT NULL,                    -- ID hosting
    SoLuong INT NOT NULL CHECK (SoLuong > 0),  -- Số lượng dịch vụ
    DonGia DECIMAL(18,2) NOT NULL,             -- Đơn giá dịch vụ
    PRIMARY KEY (HopDongID, HostingID),       -- Khóa chính
    FOREIGN KEY (HopDongID) REFERENCES HopDong(HopDongID), -- Liên kết với hợp đồng
    FOREIGN KEY (HostingID) REFERENCES Hosting(HostingID)    -- Liên kết với hosting
);
GO

-- Đổ dữ liệu mẫu cho bảng NguoiDung
INSERT INTO NguoiDung (HoTen, Email, MatKhau, SoDienThoai, DiaChi, Role)
VALUES
(N'Nguyen Van A', 'a.nguyen@example.com', 'password123', '0901234567', N'123 Đường A, Quận B', 0),
(N'Tran Thi B', 'b.tran@example.com', 'securepassword', '0912345678', N'45 Đường B, Quận C', 1),
(N'Le Van C', 'c.le@example.com', 'password@123', '0923456789', N'67 Đường C, Quận D', 0),
(N'Pham Minh D', 'd.pham@example.com', 'mypassword', '0934567890', N'89 Đường D, Quận E', 0),
(N'Hoang Thi E', 'e.hoang@example.com', '123password', '0945678901', N'101 Đường E, Quận F', 1),
(N'user', 'user@mail.com', '123', '', '', 0),
(N'manager', 'manager@mail.com', '123', '', '', 1),
(N'admin', 'admin@mail.com', '123', '', '', 2);

-- Đổ dữ liệu mẫu cho bảng LoaiHosting
INSERT INTO LoaiHosting (TenLoai)
VALUES
(N'Shared Hosting'),
(N'VPS Hosting'),
(N'Dedicated Hosting'),
(N'Cloud Hosting'),
(N'Reseller Hosting');

-- Đổ dữ liệu mẫu cho bảng Hosting
INSERT INTO Hosting (TenHosting, MoTa, DonGia, LoaiHostingID)
VALUES
(N'Basic Shared Hosting', N'Dịch vụ hosting cơ bản, phù hợp với cá nhân', 50000.00, 1),
(N'Pro Shared Hosting', N'Dịch vụ hosting nâng cao cho doanh nghiệp nhỏ', 150000.00, 1),
(N'VPS Starter', N'Máy chủ ảo hiệu năng tốt, phù hợp phát triển ứng dụng', 300000.00, 2),
(N'VPS Advanced', N'Dịch vụ VPS với tài nguyên lớn hơn', 500000.00, 2),
(N'Dedicated Server', N'Máy chủ riêng biệt, hiệu năng cao nhất', 2000000.00, 3);

-- Đổ dữ liệu mẫu cho bảng HopDong
INSERT INTO HopDong (NgayBatDau, NgayKetThuc, TrangThai, NguoiDungID, NhanVienID)
VALUES
('2024-01-01', '2024-12-31', 1, 1, 2),
('2024-02-01', '2024-08-01', 0, 3, 2),
('2024-03-01', '2024-09-01', 2, 4, 2),
('2024-04-01', '2024-10-01', 1, 1, 5),
('2024-05-01', '2024-11-01', 0, 3, 5);

-- Đổ dữ liệu mẫu cho bảng ChiTietHopDong
INSERT INTO ChiTietHopDong (HopDongID, HostingID, SoLuong, DonGia)
VALUES
(1, 1, 1, 50000.00),
(1, 3, 1, 300000.00),
(2, 2, 2, 150000.00),
(3, 4, 1, 500000.00),
(4, 5, 1, 2000000.00);

-- Kiểm tra dữ liệu trong bảng NguoiDung
SELECT * FROM NguoiDung;

-- Kiểm tra dữ liệu trong bảng LoaiHosting
SELECT * FROM LoaiHosting;

-- Kiểm tra dữ liệu trong bảng Hosting
SELECT * FROM Hosting;

-- Kiểm tra dữ liệu trong bảng HopDong
SELECT * FROM HopDong;

-- Kiểm tra dữ liệu trong bảng ChiTietHopDong
SELECT * FROM ChiTietHopDong;