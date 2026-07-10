using Cinema.BUS;
using Cinema.DAL.Models;
using Cinema.DTO;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using MockQueryable.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Cinema.Tests
{
    public class NhanVienBUSTests
    {
        private readonly Mock<QuanLyRapPhimContext> _mockContext;
        private readonly NhanVienBUS _nhanVienBus;

        public NhanVienBUSTests()
        {
            _mockContext = new Mock<QuanLyRapPhimContext>();
            _nhanVienBus = new NhanVienBUS(_mockContext.Object);
        }

        [Fact]
        public void DangNhap_ValidCredentials_ReturnsTrue()
        {
            // Arrange
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword("123456");
            var data = new List<NhanVien>
            {
                new NhanVien { TaiKhoan = "admin", MatKhau = hashedPassword }
            };

            var mockDbSet = data.BuildMockDbSet();
            _mockContext.Setup(c => c.NhanViens).Returns(mockDbSet.Object);

            var req = new NhanVienLoginRequest { TaiKhoan = "admin", MatKhau = "123456" };

            // Act
            var result = _nhanVienBus.DangNhap(req);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void DangNhap_InvalidPassword_ReturnsFalse()
        {
            // Arrange
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword("123456");
            var data = new List<NhanVien>
            {
                new NhanVien { TaiKhoan = "admin", MatKhau = hashedPassword }
            };

            var mockDbSet = data.BuildMockDbSet();
            _mockContext.Setup(c => c.NhanViens).Returns(mockDbSet.Object);

            var req = new NhanVienLoginRequest { TaiKhoan = "admin", MatKhau = "wrongpassword" };

            // Act
            var result = _nhanVienBus.DangNhap(req);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void DangNhap_SqlInjectionAttempt_SanitizesAndReturnsFalse()
        {
            // Arrange: SQLi strings like ' OR '1'='1
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword("123456");
            var data = new List<NhanVien>
            {
                new NhanVien { TaiKhoan = "admin", MatKhau = hashedPassword }
            };

            var mockDbSet = data.BuildMockDbSet();
            _mockContext.Setup(c => c.NhanViens).Returns(mockDbSet.Object);

            var req = new NhanVienLoginRequest { TaiKhoan = "admin' OR '1'='1", MatKhau = "123456" };

            // Act
            var result = _nhanVienBus.DangNhap(req);

            // Assert: System should treat SQLi string as literal username, not find it, and fail login.
            Assert.False(result);
        }

        [Fact]
        public void LayDanhSach_ReturnsList()
        {
            // Arrange
            var data = new List<NhanVien>
            {
                new NhanVien { MaNv = 1, HoTen = "NV 1", TaiKhoan = "nv1" },
                new NhanVien { MaNv = 2, HoTen = "NV 2", TaiKhoan = "nv2" }
            };

            var mockDbSet = data.BuildMockDbSet();
            _mockContext.Setup(c => c.NhanViens).Returns(mockDbSet.Object);

            // Act
            var result = _nhanVienBus.LayDanhSach();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }
    }
}
