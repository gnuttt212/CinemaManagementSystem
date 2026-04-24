using Cinema.BUS;
using Cinema.DAL.Models;
using Cinema.DTO;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Cinema.Tests
{
    public class PhimBUSTests
    {
        private Mock<QuanLyRapPhimContext> _mockContext;
        private PhimBUS _phimBus;

        public PhimBUSTests()
        {
            _mockContext = new Mock<QuanLyRapPhimContext>();
            _phimBus = new PhimBUS(_mockContext.Object);
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
        public void LayDanhSachPhim_ReturnsListOfPhimDTO()
        {
            // Arrange
            var data = new List<Phim>
            {
                new Phim { MaPhim = 1, TenPhim = "Phim 1", TheLoai = "Hành động" },
                new Phim { MaPhim = 2, TenPhim = "Phim 2", TheLoai = "Tình cảm" }
            };

            var mockDbSet = GetQueryableMockDbSet(data);
            _mockContext.Setup(c => c.Phims).Returns(mockDbSet.Object);

            // Act
            var result = _phimBus.LayDanhSachPhim();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Phim 1", result[0].TenPhim);
            Assert.Equal("Phim 2", result[1].TenPhim);
        }

        [Fact]
        public void ThemPhim_Success_ReturnsMaPhim()
        {
            // Arrange
            var data = new List<Phim>();
            var mockDbSet = GetQueryableMockDbSet(data);
            
            mockDbSet.Setup(d => d.Add(It.IsAny<Phim>())).Callback<Phim>(p => 
            {
                p.MaPhim = 1; 
                data.Add(p);
            });

            _mockContext.Setup(c => c.Phims).Returns(mockDbSet.Object);
            _mockContext.Setup(m => m.SaveChanges()).Returns(1);

            var newPhim = new PhimDTO { TenPhim = "Phim Moi", ThoiLuong = 120 };

            // Act
            var result = _phimBus.ThemPhim(newPhim);

            // Assert
            Assert.Equal(1, result); 
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
            Assert.Single(data);
            Assert.Equal("Phim Moi", data[0].TenPhim);
        }

        [Fact]
        public void ThemPhim_Exception_ReturnsZero()
        {
            // Arrange
            var data = new List<Phim>();
            var mockDbSet = GetQueryableMockDbSet(data);
            
            _mockContext.Setup(c => c.Phims).Returns(mockDbSet.Object);
            _mockContext.Setup(m => m.SaveChanges()).Throws(new Exception("Database error"));

            var newPhim = new PhimDTO { TenPhim = "Phim Moi", ThoiLuong = 120 };

            // Act
            var result = _phimBus.ThemPhim(newPhim);

            // Assert
            Assert.Equal(0, result);
        }
        
        [Fact]
        public void SuaPhim_Success_ReturnsTrue()
        {
            // Arrange
            var data = new List<Phim>
            {
                new Phim { MaPhim = 1, TenPhim = "Phim Cu", ThoiLuong = 100 }
            };
            
            var mockDbSet = GetQueryableMockDbSet(data);
            mockDbSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids => data.FirstOrDefault(d => d.MaPhim == (int)ids[0]));

            _mockContext.Setup(c => c.Phims).Returns(mockDbSet.Object);
            _mockContext.Setup(m => m.SaveChanges()).Returns(1);

            var updateDto = new PhimDTO { MaPhim = 1, TenPhim = "Phim Moi", ThoiLuong = 120 };

            // Act
            var result = _phimBus.SuaPhim(updateDto);

            // Assert
            Assert.True(result);
            Assert.Equal("Phim Moi", data[0].TenPhim);
            Assert.Equal(120, data[0].ThoiLuong);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Fact]
        public void SuaPhim_NotFound_ReturnsFalse()
        {
            // Arrange
            var data = new List<Phim>();
            
            var mockDbSet = GetQueryableMockDbSet(data);
            mockDbSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids => data.FirstOrDefault(d => d.MaPhim == (int)ids[0]));

            _mockContext.Setup(c => c.Phims).Returns(mockDbSet.Object);

            var updateDto = new PhimDTO { MaPhim = 1, TenPhim = "Phim Moi" };

            // Act
            var result = _phimBus.SuaPhim(updateDto);

            // Assert
            Assert.False(result);
            _mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }

        [Fact]
        public void XoaPhim_Success_ReturnsTrue()
        {
            // Arrange
            var data = new List<Phim>
            {
                new Phim { MaPhim = 1, TenPhim = "Phim 1" }
            };
            
            var mockDbSet = GetQueryableMockDbSet(data);
            mockDbSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids => data.FirstOrDefault(d => d.MaPhim == (int)ids[0]));
            mockDbSet.Setup(m => m.Remove(It.IsAny<Phim>())).Callback<Phim>(p => data.Remove(p));

            _mockContext.Setup(c => c.Phims).Returns(mockDbSet.Object);
            _mockContext.Setup(m => m.SaveChanges()).Returns(1);

            // Act
            var result = _phimBus.XoaPhim(1);

            // Assert
            Assert.True(result);
            Assert.Empty(data);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Fact]
        public void XoaPhim_NotFound_ReturnsFalse()
        {
            // Arrange
            var data = new List<Phim>();
            
            var mockDbSet = GetQueryableMockDbSet(data);
            mockDbSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids => data.FirstOrDefault(d => d.MaPhim == (int)ids[0]));

            _mockContext.Setup(c => c.Phims).Returns(mockDbSet.Object);

            // Act
            var result = _phimBus.XoaPhim(1);

            // Assert
            Assert.False(result);
            _mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }
    }
}
