using Cinema.DAL.Models;
using Cinema.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Cinema.BUS
{
    public class PhongChieuBUS : IPhongChieuBUS
    {
        private readonly QuanLyRapPhimContext _db;

        public PhongChieuBUS(QuanLyRapPhimContext db)
        {
            _db = db;
        }

        public List<PhongChieuDTO> LayDanhSachPhong()
        {
            return _db.PhongChieus
                .Include(p => p.Ghes)
                .Select(p => new PhongChieuDTO
                {
                    MaPhong = p.MaPhong,
                    TenPhong = p.TenPhong,
                    LoaiPhong = p.LoaiPhong,
                    SucChua = p.SucChua,
                    SoGheDaTao = p.Ghes.Count
                })
                .ToList();
        }

        public PhongChieuDTO LayChiTietPhong(int maPhong)
        {
            var p = _db.PhongChieus.Include(ph => ph.Ghes).FirstOrDefault(ph => ph.MaPhong == maPhong);
            if (p == null) return null;

            return new PhongChieuDTO
            {
                MaPhong = p.MaPhong,
                TenPhong = p.TenPhong,
                LoaiPhong = p.LoaiPhong,
                SucChua = p.SucChua,
                SoGheDaTao = p.Ghes.Count
            };
        }

        public bool ThemPhong(PhongChieuDTO dto)
        {
            try
            {
                var phong = new PhongChieu
                {
                    TenPhong = dto.TenPhong,
                    LoaiPhong = dto.LoaiPhong,
                    SucChua = dto.SucChua
                };
                
                _db.PhongChieus.Add(phong);
                _db.SaveChanges();

                if (phong.SucChua.HasValue && phong.SucChua.Value > 0)
                {
                    SinhGheTuDong(phong.MaPhong, phong.SucChua.Value);
                    _db.SaveChanges();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public string SuaPhong(PhongChieuDTO dto)
        {
            try
            {
                var phong = _db.PhongChieus.Find(dto.MaPhong);
                if (phong == null) return "Không tìm thấy phòng chiếu.";

                bool sucChuaChanged = phong.SucChua != dto.SucChua;

                if (sucChuaChanged)
                {
                    bool hasBookedSeats = _db.ChiTietHoaDons.Any(ct => ct.MaGheNavigation.MaPhong == dto.MaPhong);
                    if (hasBookedSeats)
                    {
                        return "Không thể thay đổi sức chứa vì đã có ghế được đặt cho phòng này.";
                    }
                }

                phong.TenPhong = dto.TenPhong;
                phong.LoaiPhong = dto.LoaiPhong;
                phong.SucChua = dto.SucChua;

                if (sucChuaChanged)
                {
                    var oldSeats = _db.Ghes.Where(g => g.MaPhong == dto.MaPhong).ToList();
                    _db.Ghes.RemoveRange(oldSeats);

                    if (phong.SucChua.HasValue && phong.SucChua.Value > 0)
                    {
                        SinhGheTuDong(phong.MaPhong, phong.SucChua.Value);
                    }
                }

                _db.SaveChanges();
                return string.Empty; 
            }
            catch (Exception ex)
            {
                return "Lỗi khi cập nhật phòng chiếu: " + ex.Message;
            }
        }

        public string XoaPhong(int maPhong)
        {
            try
            {
                var phong = _db.PhongChieus.Find(maPhong);
                if (phong == null) return "Không tìm thấy phòng chiếu.";

                bool hasLichChieu = _db.LichChieus.Any(lc => lc.MaPhong == maPhong);
                if (hasLichChieu) return "Không thể xóa phòng chiếu vì đã có lịch chiếu.";

                var seats = _db.Ghes.Where(g => g.MaPhong == maPhong).ToList();
                _db.Ghes.RemoveRange(seats);

                _db.PhongChieus.Remove(phong);
                _db.SaveChanges();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Lỗi khi xóa phòng chiếu: " + ex.Message;
            }
        }

        private void SinhGheTuDong(int maPhong, int sucChua)
        {
            int gheTheoHang = 10;
            int soHang = (int)Math.Ceiling((double)sucChua / gheTheoHang);
            int gheDaTao = 0;

            for (int r = 0; r < soHang; r++)
            {
                string hang = ((char)('A' + r)).ToString();
                for (int s = 1; s <= gheTheoHang; s++)
                {
                    if (gheDaTao >= sucChua) break;

                    var ghe = new Ghe
                    {
                        MaPhong = maPhong,
                        Hang = hang,
                        SoGhe = s,
                        LoaiGhe = "Thường"
                    };
                    _db.Ghes.Add(ghe);
                    gheDaTao++;
                }
            }
        }
    }
}
