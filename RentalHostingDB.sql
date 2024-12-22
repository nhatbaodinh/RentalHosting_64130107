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
    DonGia DECIMAL(18) NOT NULL,             -- Đơn giá
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
    DonGia DECIMAL(18) NOT NULL,             -- Đơn giá dịch vụ
    PRIMARY KEY (HopDongID, HostingID),       -- Khóa chính
    FOREIGN KEY (HopDongID) REFERENCES HopDong(HopDongID), -- Liên kết với hợp đồng
    FOREIGN KEY (HostingID) REFERENCES Hosting(HostingID)    -- Liên kết với hosting
);
GO

-- Bảng LichSuThanhToan
CREATE TABLE LichSuThanhToan (
     ThanhToanID INT PRIMARY KEY IDENTITY(1,1),  -- ID thanh toán
     HopDongID INT NOT NULL,                      -- ID hợp đồng
     NgayThanhToan DATE NOT NULL,                 -- Ngày thanh toán
     SoTienThanhToan DECIMAL(18,2) NOT NULL,      -- Số tiền thanh toán
     HinhThucThanhToan NVARCHAR(50),              -- Hình thức thanh toán (ví dụ: chuyển khoản, tiền mặt)
     FOREIGN KEY (HopDongID) REFERENCES HopDong(HopDongID) -- Liên kết với hợp đồng
);
GO

-- Thêm 30 khách hàng (Role = 0)
INSERT INTO NguoiDung (HoTen, Email, MatKhau, SoDienThoai, DiaChi, Role)
VALUES
(N'Khach Hang 1', 'user@mail.com', '123', '0901000001', N'So 1, Quan X', 0),
(N'Khach Hang 2', 'khachhang2@example.com', 'password123', '0901000002', N'So 2, Quan X', 0),
(N'Khach Hang 3', 'khachhang3@example.com', 'password123', '0901000003', N'So 3, Quan X', 0),
(N'Khach Hang 4', 'khachhang4@example.com', 'password123', '0901000004', N'So 4, Quan X', 0),
(N'Khach Hang 5', 'khachhang5@example.com', 'password123', '0901000005', N'So 5, Quan X', 0),
(N'Khach Hang 6', 'khachhang6@example.com', 'password123', '0901000006', N'So 6, Quan X', 0),
(N'Khach Hang 7', 'khachhang7@example.com', 'password123', '0901000007', N'So 7, Quan X', 0),
(N'Khach Hang 8', 'khachhang8@example.com', 'password123', '0901000008', N'So 8, Quan X', 0),
(N'Khach Hang 9', 'khachhang9@example.com', 'password123', '0901000009', N'So 9, Quan X', 0),
(N'Khach Hang 10', 'khachhang10@example.com', 'password123', '0901000010', N'So 10, Quan X', 0),
(N'Khach Hang 11', 'khachhang11@example.com', 'password123', '0901000011', N'So 11, Quan X', 0),
(N'Khach Hang 12', 'khachhang12@example.com', 'password123', '0901000012', N'So 12, Quan X', 0),
(N'Khach Hang 13', 'khachhang13@example.com', 'password123', '0901000013', N'So 13, Quan X', 0),
(N'Khach Hang 14', 'khachhang14@example.com', 'password123', '0901000014', N'So 14, Quan X', 0),
(N'Khach Hang 15', 'khachhang15@example.com', 'password123', '0901000015', N'So 15, Quan X', 0),
(N'Khach Hang 16', 'khachhang16@example.com', 'password123', '0901000016', N'So 16, Quan X', 0),
(N'Khach Hang 17', 'khachhang17@example.com', 'password123', '0901000017', N'So 17, Quan X', 0),
(N'Khach Hang 18', 'khachhang18@example.com', 'password123', '0901000018', N'So 18, Quan X', 0),
(N'Khach Hang 19', 'khachhang19@example.com', 'password123', '0901000019', N'So 19, Quan X', 0),
(N'Khach Hang 20', 'khachhang20@example.com', 'password123', '0901000020', N'So 20, Quan X', 0),
(N'Khach Hang 21', 'khachhang21@example.com', 'password123', '0901000021', N'So 21, Quan X', 0),
(N'Khach Hang 22', 'khachhang22@example.com', 'password123', '0901000022', N'So 22, Quan X', 0),
(N'Khach Hang 23', 'khachhang23@example.com', 'password123', '0901000023', N'So 23, Quan X', 0),
(N'Khach Hang 24', 'khachhang24@example.com', 'password123', '0901000024', N'So 24, Quan X', 0),
(N'Khach Hang 25', 'khachhang25@example.com', 'password123', '0901000025', N'So 25, Quan X', 0),
(N'Khach Hang 26', 'khachhang26@example.com', 'password123', '0901000026', N'So 26, Quan X', 0),
(N'Khach Hang 27', 'khachhang27@example.com', 'password123', '0901000027', N'So 27, Quan X', 0),
(N'Khach Hang 28', 'khachhang28@example.com', 'password123', '0901000028', N'So 28, Quan X', 0),
(N'Khach Hang 29', 'khachhang29@example.com', 'password123', '0901000029', N'So 29, Quan X', 0),
(N'Khach Hang 30', 'khachhang30@example.com', 'password123', '0901000030', N'So 30, Quan X', 0);

-- Thêm 3 admin (Role = 2)
INSERT INTO NguoiDung (HoTen, Email, MatKhau, SoDienThoai, DiaChi, Role)
VALUES
(N'Admin 1', 'admin@mail.com', '123', '0909990001', N'Quan 1', 2),
(N'Admin 2', 'admin2@example.com', 'password123', '0909990002', N'Quan 2', 2),
(N'Admin 3', 'admin3@example.com', 'password123', '0909990003', N'Quan 3', 2);

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
(N'Basic Shared Hosting', N'Dịch vụ hosting cơ bản, phù hợp với cá nhân', 50000, 1),
(N'Pro Shared Hosting', N'Dịch vụ hosting nâng cao cho doanh nghiệp nhỏ', 150000, 1),
(N'VPS Starter', N'Máy chủ ảo hiệu năng tốt, phù hợp phát triển ứng dụng', 300000, 2),
(N'VPS Advanced', N'Dịch vụ VPS với tài nguyên lớn hơn', 500000, 2),
(N'Dedicated Server', N'Máy chủ riêng biệt, hiệu năng cao nhất', 2000000, 3);

-- Đổ dữ liệu mẫu cho bảng HopDong
INSERT INTO HopDong (NgayBatDau, NgayKetThuc, TrangThai, NguoiDungID, NhanVienID)
VALUES
('2024-01-01', '2024-12-31', 1, 1, 31), -- Hợp đồng hoàn thành
('2024-02-01', '2024-08-01', 0, 1, 32), -- Hợp đồng đang xử lý
('2024-03-01', '2024-09-01', 2, 1, 33), -- Hợp đồng bị hủy
('2024-02-15', '2024-10-15', 1, 2, 32),
('2024-03-10', '2024-09-10', 0, 2, 33),
('2024-04-01', '2024-11-01', 2, 2, 31),
('2024-05-01', '2024-12-31', 1, 3, 31),
('2024-06-01', '2024-09-30', 0, 3, 32),
('2024-07-01', '2024-10-31', 2, 3, 33),
('2024-08-01', '2025-07-31', 1, 4, 32),
('2024-09-15', '2025-01-15', 0, 4, 33),
('2024-10-01', '2025-04-01', 2, 4, 31),
('2024-02-10', '2024-12-10', 1, 5, 33),
('2024-03-01', '2024-10-01', 0, 5, 31),
('2024-04-01', '2024-11-01', 2, 5, 32),
('2024-01-15', '2024-07-15', 1, 6, 31),
('2024-02-20', '2024-08-20', 0, 6, 32),
('2024-03-25', '2024-09-25', 2, 6, 33),
('2024-03-25', '2024-09-25', 2, 6, 33),
('2024-01-15', '2024-07-15', 1, 7, 31),
('2024-02-20', '2024-08-20', 0, 7, 32),
('2024-03-25', '2024-09-25', 2, 7, 33),
('2024-01-15', '2024-07-15', 1, 8, 31),
('2024-02-20', '2024-08-20', 0, 8, 32),
('2024-03-25', '2024-09-25', 2, 8, 33),
('2024-01-15', '2024-07-15', 1, 9, 31),
('2024-02-20', '2024-08-20', 0, 9, 32),
('2024-03-25', '2024-09-25', 2, 9, 33),
('2024-01-15', '2024-07-15', 1, 10, 31),
('2024-02-20', '2024-08-20', 0, 10, 32),
('2024-03-25', '2024-09-25', 2, 10, 33),
('2024-01-15', '2024-07-15', 1, 11, 31),
('2024-02-20', '2024-08-20', 0, 11, 32),
('2024-03-25', '2024-09-25', 2, 11, 33),
('2024-01-15', '2024-07-15', 1, 12, 31),
('2024-02-20', '2024-08-20', 0, 12, 32),
('2024-03-25', '2024-09-25', 2, 12, 33),
('2024-01-15', '2024-07-15', 1, 13, 31),
('2024-02-20', '2024-08-20', 0, 13, 32),
('2024-03-25', '2024-09-25', 2, 13, 33),
('2024-01-15', '2024-07-15', 1, 14, 31),
('2024-02-20', '2024-08-20', 0, 14, 32),
('2024-03-25', '2024-09-25', 2, 14, 33),
('2024-01-15', '2024-07-15', 1, 15, 31),
('2024-02-20', '2024-08-20', 0, 15, 32),
('2024-03-25', '2024-09-25', 2, 15, 33),
('2024-01-15', '2024-07-15', 1, 16, 31),
('2024-02-20', '2024-08-20', 0, 16, 32),
('2024-03-25', '2024-09-25', 2, 16, 33),
('2024-01-15', '2024-07-15', 1, 17, 31),    
('2024-02-20', '2024-08-20', 0, 17, 32),
('2024-03-25', '2024-09-25', 2, 17, 33),
('2024-01-15', '2024-07-15', 1, 18, 31),
('2024-02-20', '2024-08-20', 0, 18, 32),
('2024-03-25', '2024-09-25', 2, 18, 33),
('2024-01-15', '2024-07-15', 1, 19, 31),
('2024-02-20', '2024-08-20', 0, 19, 32),
('2024-03-25', '2024-09-25', 2, 19, 33),
('2024-01-15', '2024-07-15', 1, 20, 31),
('2024-02-20', '2024-08-20', 0, 20, 32),
('2024-03-25', '2024-09-25', 2, 20, 33);

-- Đổ dữ liệu mẫu cho bảng ChiTietHopDong
INSERT INTO ChiTietHopDong (HopDongID, HostingID, SoLuong, DonGia)
VALUES
(1, 1, 1, 50000),
(1, 2, 1, 150000),
(2, 3, 1, 300000),
(2, 4, 1, 500000),
(3, 5, 1, 2000000),
(4, 1, 1, 50000),
(4, 2, 1, 150000),
(5, 3, 1, 300000),
(5, 4, 1, 500000),
(6, 5, 1, 2000000),
(7, 1, 1, 50000),
(7, 2, 1, 150000),
(8, 3, 1, 300000),
(8, 4, 1, 500000),
(9, 5, 1, 2000000),
(10, 1, 1, 50000),
(10, 2, 1, 150000),
(11, 3, 1, 300000),
(11, 4, 1, 500000),
(12, 5, 1, 2000000),
(13, 1, 1, 50000),
(13, 2, 1, 150000),
(14, 3, 1, 300000),
(14, 4, 1, 500000),
(15, 5, 1, 2000000),
(16, 1, 1, 50000),
(16, 2, 1, 150000),
(17, 3, 1, 300000),
(17, 4, 1, 500000),
(18, 5, 1, 2000000),
(19, 1, 1, 50000),
(19, 2, 1, 150000),
(20, 3, 1, 300000),
(20, 4, 1, 500000),
(21, 5, 1, 2000000),
(22, 1, 1, 50000),
(22, 2, 1, 150000),
(23, 3, 1, 300),
(23, 4, 1, 500000),
(24, 5, 1, 2000000),
(25, 1, 1, 50000),
(25, 2, 1, 150000),
(26, 3, 1, 300000),
(26, 4, 1, 500000),
(27, 5, 1, 2000000),
(28, 1, 1, 50000),
(28, 2, 1, 150000),
(29, 3, 1, 300000),
(29, 4, 1, 500000),
(30, 5, 1, 2000000),
(31, 1, 1, 50000),
(31, 2, 1, 150000),
(32, 3, 1, 300000),
(32, 4, 1, 500000),
(33, 5, 1, 2000000),
(34, 1, 1, 50000),
(34, 2, 1, 150000),
(35, 3, 1, 300000),
(35, 4, 1, 500000),
(36, 5, 1, 2000000),
(37, 1, 1, 50000),
(37, 2, 1, 150000),
(38, 3, 1, 300000),
(38, 4, 1, 500000),
(39, 5, 1, 2000000),
(40, 1, 1, 50000),
(40, 2, 1, 150000),
(41, 3, 1, 300000),
(41, 4, 1, 500000),
(42, 5, 1, 2000000),
(43, 1, 1, 50000),
(43, 2, 1, 150000),
(44, 3, 1, 300000),
(44, 4, 1, 500000),
(45, 5, 1, 2000000),
(46, 1, 1, 50000),
(46, 2, 1, 150000),
(47, 3, 1, 300000),
(47, 4, 1, 500000),
(48, 5, 1, 2000000),
(49, 1, 1, 50000),
(49, 2, 1, 150000),
(50, 3, 1, 300000),
(50, 4, 1, 500000),
(51, 5, 1, 2000000),
(52, 1, 1, 50000),
(52, 2, 1, 150000),
(53, 3, 1, 300),
(53, 4, 1, 500000),
(54, 5, 1, 2000000),
(55, 1, 1, 50000),
(55, 2, 1, 150000),
(56, 3, 1, 300000),
(56, 4, 1, 500000),
(57, 5, 1, 2000000),
(58, 1, 1, 50000),
(58, 2, 1, 150000),
(59, 3, 1, 300000),
(59, 4, 1, 500000),
(60, 5, 1, 2000000);

-- Thêm dữ liệu mẫu cho bảng LichSuThanhToan
INSERT INTO LichSuThanhToan (HopDongID, NgayThanhToan, SoTienThanhToan, HinhThucThanhToan)
VALUES
(1, '2024-01-10', 50000, N'Chuyển khoản'),
(1, '2024-02-10', 50000, N'Tiền mặt'),
(2, '2024-02-05', 300000, N'Chuyển khoản'),
(2, '2024-03-05', 500000, N'Chuyển khoản'),
(3, '2024-03-10', 2000000, N'Chuyển khoản');

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