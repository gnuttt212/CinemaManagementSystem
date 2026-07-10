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
    public class PhongChieuBUSTests
    {
        private readonly Mock<QuanLyRapPhimContext> _mockContext;
        private readonly PhongChieuBUS _phongChieuBus;

        public PhongChieuBUSTests()
        {
            _mockContext = new Mock<QuanLyRapPhimContext>();
            _phongChieuBus = new PhongChieuBUS(_mockContext.Object);
        }

        [Fact]
        public void LayDanhSachPhong_ReturnsList()
        {
            var data = new List<PhongChieu>
            {
                new PhongChieu { MaPhong = 1, TenPhong = "P1", Ghes = new List<Ghe>() },
                new PhongChieu { MaPhong = 2, TenPhong = "P2", Ghes = new List<Ghe>() }
            };

            var mockDbSet = data.BuildMockDbSet();
            _mockContext.Setup(c => c.PhongChieus).Returns(mockDbSet.Object);

            var result = _phongChieuBus.LayDanhSachPhong();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void ThemPhong_Success_ReturnsTrue()
        {
            var data = new List<PhongChieu>();
            var mockDbSet = data.BuildMockDbSet();
            
            mockDbSet.Setup(m => m.Add(It.IsAny<PhongChieu>())).Callback<PhongChieu>(p => data.Add(p));
            _mockContext.Setup(c => c.PhongChieus).Returns(mockDbSet.Object);
            
            var gheData = new List<Ghe>();
            var gheMockDbSet = gheData.BuildMockDbSet();
            gheMockDbSet.Setup(m => m.Add(It.IsAny<Ghe>())).Callback<Ghe>(g => gheData.Add(g));
            _mockContext.Setup(c => c.Ghes).Returns(gheMockDbSet.Object);
            
            _mockContext.Setup(m => m.SaveChanges()).Returns(1);

            var newPhong = new PhongChieuDTO { TenPhong = "P3", SucChua = 20 };

            var result = _phongChieuBus.ThemPhong(newPhong);

            Assert.True(result);
            Assert.Single(data);
            Assert.Equal("P3", data[0].TenPhong);
            Assert.Equal(20, gheData.Count); // Tự động sinh ghế
        }

        [Fact]
        public void XoaPhong_HasLichChieu_ReturnsErrorString()
        {
            var phongData = new List<PhongChieu> { new PhongChieu { MaPhong = 1, TenPhong = "P1" } };
            var mockPhongDbSet = phongData.BuildMockDbSet();
            mockPhongDbSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids => phongData.FirstOrDefault(d => d.MaPhong == (int)ids[0]));
            _mockContext.Setup(c => c.PhongChieus).Returns(mockPhongDbSet.Object);

            var lichChieuData = new List<LichChieu> { new LichChieu { MaLich = 1, MaPhong = 1 } };
            var mockLichChieuDbSet = lichChieuData.BuildMockDbSet();
            _mockContext.Setup(c => c.LichChieus).Returns(mockLichChieuDbSet.Object);

            var result = _phongChieuBus.XoaPhong(1);

            Assert.Equal("Không thể xóa phòng chiếu vì đã có lịch chiếu.", result);
        }
    }
}
