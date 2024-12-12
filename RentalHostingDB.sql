USE master;
GO

DROP DATABASE IF EXISTS RentalHostingDB;
GO

CREATE DATABASE RentalHostingDB;
GO

USE RentalHostingDB;
GO

-- Loại Hosting
CREATE TABLE LoaiHosting (
    LoaiID INT PRIMARY KEY IDENTITY(1,1),
    TenLoai NVARCHAR(100) NOT NULL
);

-- Hosting/Server
CREATE TABLE Hosting (
    HostingID INT PRIMARY KEY IDENTITY(1,1),
    TenHosting NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(MAX),
    DonGia DECIMAL(18,2) NOT NULL,
    LoaiID INT NOT NULL,
    FOREIGN KEY (LoaiID) REFERENCES LoaiHosting(LoaiID)
);

-- Khách hàng
CREATE TABLE KhachHang (
    CustomerID INT PRIMARY KEY IDENTITY(1,1),
    HoTen NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    SoDienThoai NVARCHAR(15),
    DiaChi NVARCHAR(200),
    MatKhau NVARCHAR(200) NOT NULL
);

-- Nhân viên
CREATE TABLE NhanVien (
    EmployeeID INT PRIMARY KEY IDENTITY(1,1),
    HoTen NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    SoDienThoai NVARCHAR(15),
    MatKhau NVARCHAR(200) NOT NULL,
    QuyenSuDung INT CHECK (QuyenSuDung IN (0, 1, 2)) NOT NULL
);

-- Hợp đồng
CREATE TABLE HopDong (
    ContractID INT PRIMARY KEY IDENTITY(1,1),
    NgayBatDau DATE NOT NULL,
    NgayKetThuc DATE NOT NULL,
    TrangThai INT CHECK (TrangThai IN (0, 1, 2)) NOT NULL,
    CustomerID INT NOT NULL,
    EmployeeID INT NOT NULL,
    FOREIGN KEY (CustomerID) REFERENCES KhachHang(CustomerID),
    FOREIGN KEY (EmployeeID) REFERENCES NhanVien(EmployeeID)
);

-- Chi tiết hợp đồng
CREATE TABLE ChiTietHopDong (
    ContractID INT NOT NULL,
    HostingID INT NOT NULL,
    SoLuong INT NOT NULL CHECK (SoLuong > 0),
    DonGia DECIMAL(18,2) NOT NULL,
    PRIMARY KEY (ContractID, HostingID),
    FOREIGN KEY (ContractID) REFERENCES HopDong(ContractID),
    FOREIGN KEY (HostingID) REFERENCES Hosting(HostingID)
);

SELECT * from KhachHang;