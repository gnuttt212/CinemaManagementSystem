using Cinema.BUS;
using Cinema.DAL.Models;
using Cinema.DTO;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Cinema.Tests
{
    public class DoAnBUSTests
    {
        private Mock<QuanLyRapPhimContext> _mockContext;
        private DoAnBUS _doAnBus;

        public DoAnBUSTests()
        {
            _mockContext = new Mock<QuanLyRapPhimContext>();
            _doAnBus = new DoAnBUS(_mockContext.Object);
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
        public void LayDanhSachDoAn_ReturnsList()
        {
            // Arrange
            var data = new List<DoAn>
            {
                new DoAn { MaDoAn = 1, TenDoAn = "Bắp rang bơ", Gia = 50000, Loai = "Đồ ăn" },
                new DoAn { MaDoAn = 2, TenDoAn = "Pepsi", Gia = 30000, Loai = "Nước uống" }
            };

            var mockDbSet = GetQueryableMockDbSet(data);
            _mockContext.Setup(c => c.DoAns).Returns(mockDbSet.Object);

            // Act
            var result = _doAnBus.LayDanhSachDoAn();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Bắp rang bơ", result[0].TenDoAn);
        }

        [Fact]
        public void LayTheoId_Found_ReturnsDoAnDTO()
        {
            // Arrange
            var data = new List<DoAn>
            {
                new DoAn { MaDoAn = 1, TenDoAn = "Bắp rang bơ", Gia = 50000, Loai = "Đồ ăn" }
            };

            var mockDbSet = GetQueryableMockDbSet(data);
            _mockContext.Setup(c => c.DoAns).Returns(mockDbSet.Object);

            // Act
            var result = _doAnBus.LayTheoId(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.MaDoAn);
            Assert.Equal("Bắp rang bơ", result.TenDoAn);
        }

        [Fact]
        public void LayTheoId_NotFound_ReturnsNull()
        {
            // Arrange
            var data = new List<DoAn>();
            var mockDbSet = GetQueryableMockDbSet(data);
            _mockContext.Setup(c => c.DoAns).Returns(mockDbSet.Object);

            // Act
            var result = _doAnBus.LayTheoId(99);

            // Assert
            Assert.Null(result);
        }
    }
}
