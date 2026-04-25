using Cinema.BUS;
using Cinema.DAL.Models;
using Cinema.DTO;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using BCrypt.Net;

namespace Cinema.Tests
{
    public class NhanVienBUSTests
    {
        private Mock<QuanLyRapPhimContext> _mockContext;
        private NhanVienBUS _nhanVienBus;

        public NhanVienBUSTests()
        {
            _mockContext = new Mock<QuanLyRapPhimContext>();
            _nhanVienBus = new NhanVienBUS(_mockContext.Object);
        }

        private Mock<DbSet<T>> GetQueryableMockDbSet<T>(List<T> sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();
            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            return dbSet;
        }

        [Fact]
        public void DangNhap_ValidCredentials_ReturnsTrue()
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword("123456");
            var data = new List<NhanVien>
            {
                new NhanVien { TaiKhoan = "admin", MatKhau = hashedPassword }
            };

            var mockDbSet = GetQueryableMockDbSet(data);
            _mockContext.Setup(c => c.NhanViens).Returns(mockDbSet.Object);

            var req = new NhanVienLoginRequest { TaiKhoan = "admin", MatKhau = "123456" };
            var result = _nhanVienBus.DangNhap(req);

            Assert.True(result);
        }

        [Fact]
        public void DangNhap_InvalidPassword_ReturnsFalse()
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword("123456");
            var data = new List<NhanVien>
            {
                new NhanVien { TaiKhoan = "admin", MatKhau = hashedPassword }
            };

            var mockDbSet = GetQueryableMockDbSet(data);
            _mockContext.Setup(c => c.NhanViens).Returns(mockDbSet.Object);

            var req = new NhanVienLoginRequest { TaiKhoan = "admin", MatKhau = "wrongpassword" };
            var result = _nhanVienBus.DangNhap(req);

            Assert.False(result);
        }
        
        [Fact]
        public void LayDanhSach_ReturnsList()
        {
            var data = new List<NhanVien>
            {
                new NhanVien { MaNv = 1, HoTen = "NV 1", TaiKhoan = "nv1" },
                new NhanVien { MaNv = 2, HoTen = "NV 2", TaiKhoan = "nv2" }
            };

            var mockDbSet = GetQueryableMockDbSet(data);
            _mockContext.Setup(c => c.NhanViens).Returns(mockDbSet.Object);

            var result = _nhanVienBus.LayDanhSach();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }
    }
}
