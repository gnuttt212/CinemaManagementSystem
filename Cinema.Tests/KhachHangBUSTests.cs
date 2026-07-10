using Cinema.BUS;
using Cinema.DAL.Models;
using Cinema.DTO;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using MockQueryable.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using BCrypt.Net;

namespace Cinema.Tests
{
    public class KhachHangBUSTests
    {
        private readonly Mock<QuanLyRapPhimContext> _mockContext;
        private readonly KhachHangBUS _khachHangBus;

        public KhachHangBUSTests()
        {
            _mockContext = new Mock<QuanLyRapPhimContext>();
            _khachHangBus = new KhachHangBUS(_mockContext.Object);
        }

        [Fact]
        public void DangNhap_ValidCredentials_ReturnsTrue()
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword("123456");
            var data = new List<KhachHang>
            {
                new KhachHang { TaiKhoan = "khachhang1", MatKhau = hashedPassword }
            };

            var mockDbSet = data.BuildMockDbSet();
            _mockContext.Setup(c => c.KhachHangs).Returns(mockDbSet.Object);

            var req = new KhachHangLoginRequest { TaiKhoan = "khachhang1", MatKhau = "123456" };
            var result = _khachHangBus.DangNhap(req);

            Assert.True(result);
        }

        [Fact]
        public void DangNhap_SqlInjection_ReturnsFalse()
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword("123456");
            var data = new List<KhachHang>
            {
                new KhachHang { TaiKhoan = "khachhang1", MatKhau = hashedPassword }
            };

            var mockDbSet = data.BuildMockDbSet();
            _mockContext.Setup(c => c.KhachHangs).Returns(mockDbSet.Object);

            var req = new KhachHangLoginRequest { TaiKhoan = "khachhang1' OR '1'='1", MatKhau = "123456" };
            var result = _khachHangBus.DangNhap(req);

            Assert.False(result);
        }
        
        [Fact]
        public void DangKy_TaiKhoanDaTonTai_ReturnsError()
        {
            var data = new List<KhachHang>
            {
                new KhachHang { TaiKhoan = "khachhang1" }
            };

            var mockDbSet = data.BuildMockDbSet();
            _mockContext.Setup(c => c.KhachHangs).Returns(mockDbSet.Object);

            var req = new KhachHangRegisterRequest { TaiKhoan = "khachhang1", MatKhau = "newpass" };
            var result = _khachHangBus.DangKy(req);

            Assert.False(result);
        }

        [Fact]
        public void DangKy_MatKhauHasBeenHashed_ReturnsSuccess()
        {
            var data = new List<KhachHang>();
            var mockDbSet = data.BuildMockDbSet();
            
            KhachHang savedKhachHang = null;
            mockDbSet.Setup(m => m.Add(It.IsAny<KhachHang>())).Callback<KhachHang>(k => savedKhachHang = k);

            _mockContext.Setup(c => c.KhachHangs).Returns(mockDbSet.Object);
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            var req = new KhachHangRegisterRequest { TaiKhoan = "newkhachhang", MatKhau = "PlainTextPass", HoTen = "Test", Email = "test@test.com", SDT = "123456789" };
            var result = _khachHangBus.DangKy(req);

            Assert.True(result); // true means success
            Assert.NotNull(savedKhachHang);
            Assert.NotEqual("PlainTextPass", savedKhachHang.MatKhau); // Must be hashed
            Assert.True(BCrypt.Net.BCrypt.Verify("PlainTextPass", savedKhachHang.MatKhau)); // Verify hash
        }
    }
}
