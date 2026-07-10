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
    public class PhimBUSTests
    {
        private readonly Mock<QuanLyRapPhimContext> _mockContext;
        private readonly PhimBUS _phimBus;

        public PhimBUSTests()
        {
            _mockContext = new Mock<QuanLyRapPhimContext>();
            _phimBus = new PhimBUS(_mockContext.Object);
        }

        [Fact]
        public void LayDanhSachPhim_ReturnsListOfPhimDTO()
        {
            var data = new List<Phim>
            {
                new Phim { MaPhim = 1, TenPhim = "Phim 1", TheLoai = "Hành động" },
                new Phim { MaPhim = 2, TenPhim = "Phim 2", TheLoai = "Tình cảm" }
            };

            var mockDbSet = data.BuildMockDbSet();
            _mockContext.Setup(c => c.Phims).Returns(mockDbSet.Object);
            
            var result = _phimBus.LayDanhSachPhim();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Phim 1", result[0].TenPhim);
            Assert.Equal("Phim 2", result[1].TenPhim);
        }

        [Fact]
        public void ThemPhim_Success_ReturnsMaPhim()
        {
            var data = new List<Phim>();
            var mockDbSet = data.BuildMockDbSet();
            
            mockDbSet.Setup(d => d.Add(It.IsAny<Phim>())).Callback<Phim>(p => 
            {
                p.MaPhim = 1; 
                data.Add(p);
            });

            _mockContext.Setup(c => c.Phims).Returns(mockDbSet.Object);
            _mockContext.Setup(m => m.SaveChanges()).Returns(1);

            var newPhim = new PhimDTO { TenPhim = "Phim Moi", ThoiLuong = 120 };

            var result = _phimBus.ThemPhim(newPhim);

            Assert.Equal(1, result); 
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
            Assert.Single(data);
            Assert.Equal("Phim Moi", data[0].TenPhim);
        }

        [Fact]
        public void ThemPhim_Exception_ReturnsZero()
        {
            var data = new List<Phim>();
            var mockDbSet = data.BuildMockDbSet();
            
            _mockContext.Setup(c => c.Phims).Returns(mockDbSet.Object);
            _mockContext.Setup(m => m.SaveChanges()).Throws(new Exception("Database error"));

            var newPhim = new PhimDTO { TenPhim = "Phim Moi", ThoiLuong = 120 };

            var result = _phimBus.ThemPhim(newPhim);

            Assert.Equal(0, result);
        }
        
        [Fact]
        public void SuaPhim_Success_ReturnsTrue()
        {
            var data = new List<Phim>
            {
                new Phim { MaPhim = 1, TenPhim = "Phim Cu", ThoiLuong = 100 }
            };
            
            var mockDbSet = data.BuildMockDbSet();
            mockDbSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids => data.FirstOrDefault(d => d.MaPhim == (int)ids[0]));

            _mockContext.Setup(c => c.Phims).Returns(mockDbSet.Object);
            _mockContext.Setup(m => m.SaveChanges()).Returns(1);

            var updateDto = new PhimDTO { MaPhim = 1, TenPhim = "Phim Moi", ThoiLuong = 120 };

            var result = _phimBus.SuaPhim(updateDto);

            Assert.True(result);
            Assert.Equal("Phim Moi", data[0].TenPhim);
            Assert.Equal(120, data[0].ThoiLuong);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Fact]
        public void SuaPhim_NotFound_ReturnsFalse()
        {
            var data = new List<Phim>();
            var mockDbSet = data.BuildMockDbSet();
            mockDbSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids => data.FirstOrDefault(d => d.MaPhim == (int)ids[0]));

            _mockContext.Setup(c => c.Phims).Returns(mockDbSet.Object);

            var updateDto = new PhimDTO { MaPhim = 1, TenPhim = "Phim Moi" };

            var result = _phimBus.SuaPhim(updateDto);

            Assert.False(result);
            _mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }

        [Fact]
        public void XoaPhim_Success_ReturnsTrue()
        {
            var data = new List<Phim>
            {
                new Phim { MaPhim = 1, TenPhim = "Phim 1" }
            };
            
            var mockDbSet = data.BuildMockDbSet();
            mockDbSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids => data.FirstOrDefault(d => d.MaPhim == (int)ids[0]));
            mockDbSet.Setup(m => m.Remove(It.IsAny<Phim>())).Callback<Phim>(p => data.Remove(p));

            _mockContext.Setup(c => c.Phims).Returns(mockDbSet.Object);
            _mockContext.Setup(m => m.SaveChanges()).Returns(1);

            var result = _phimBus.XoaPhim(1);

            Assert.True(result);
            Assert.Empty(data);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Fact]
        public void XoaPhim_NotFound_ReturnsFalse()
        {
            var data = new List<Phim>();
            var mockDbSet = data.BuildMockDbSet();
            mockDbSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids => data.FirstOrDefault(d => d.MaPhim == (int)ids[0]));

            _mockContext.Setup(c => c.Phims).Returns(mockDbSet.Object);

            var result = _phimBus.XoaPhim(1);

            Assert.False(result);
            _mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }

        [Fact]
        public void LayChiTietPhim_Found_ReturnsPhimDTO()
        {
            var data = new List<Phim>
            {
                new Phim { MaPhim = 1, TenPhim = "Phim 1" }
            };
            
            var mockDbSet = data.BuildMockDbSet();
            mockDbSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids => data.FirstOrDefault(d => d.MaPhim == (int)ids[0]));
            _mockContext.Setup(c => c.Phims).Returns(mockDbSet.Object);

            var lichChieuData = new List<LichChieu>();
            var mockLichChieuDbSet = lichChieuData.BuildMockDbSet();
            _mockContext.Setup(c => c.LichChieus).Returns(mockLichChieuDbSet.Object);

            var result = _phimBus.LayChiTietPhim(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.MaPhim);
            Assert.Equal("Phim 1", result.TenPhim);
        }

        [Fact]
        public void LayChiTietPhim_NotFound_ReturnsNull()
        {
            var data = new List<Phim>();
            var mockDbSet = data.BuildMockDbSet();
            mockDbSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids => data.FirstOrDefault(d => d.MaPhim == (int)ids[0]));

            _mockContext.Setup(c => c.Phims).Returns(mockDbSet.Object);

            var result = _phimBus.LayChiTietPhim(1);

            Assert.Null(result);
        }

        [Fact]
        public void TimKiemPhim_ReturnsFilteredList()
        {
            var data = new List<Phim>
            {
                new Phim { MaPhim = 1, TenPhim = "Lật Mặt 7" },
                new Phim { MaPhim = 2, TenPhim = "Mai" },
                new Phim { MaPhim = 3, TenPhim = "Lật Mặt 6" }
            };

            var mockDbSet = data.BuildMockDbSet();
            _mockContext.Setup(c => c.Phims).Returns(mockDbSet.Object);

            var result = _phimBus.TimKiemPhim("Lật Mặt");

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, p => p.TenPhim == "Lật Mặt 7");
            Assert.Contains(result, p => p.TenPhim == "Lật Mặt 6");
        }

        [Fact]
        public void TimKiemPhim_SqlInjection_DoesNotCrashAndReturnsEmpty()
        {
            var data = new List<Phim>
            {
                new Phim { MaPhim = 1, TenPhim = "Lật Mặt 7" }
            };

            var mockDbSet = data.BuildMockDbSet();
            _mockContext.Setup(c => c.Phims).Returns(mockDbSet.Object);

            // XSS / SQLi attempt string
            var result = _phimBus.TimKiemPhim("'; DROP TABLE Phims;--");

            // EF Core string parameters are safe, so it will just search for that literal string.
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
