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

namespace Cinema.Tests
{
    public class DoAnBUSTests
    {
        private readonly Mock<QuanLyRapPhimContext> _mockContext;
        private readonly DoAnBUS _doAnBus;

        public DoAnBUSTests()
        {
            _mockContext = new Mock<QuanLyRapPhimContext>();
            _doAnBus = new DoAnBUS(_mockContext.Object);
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

            var mockDbSet = data.BuildMockDbSet();
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

            var mockDbSet = data.BuildMockDbSet();
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
            var mockDbSet = data.BuildMockDbSet();
            _mockContext.Setup(c => c.DoAns).Returns(mockDbSet.Object);

            // Act
            var result = _doAnBus.LayTheoId(99);

            // Assert
            Assert.Null(result);
        }
    }
}
