/**
 * The HiNet License
 *
 * Copyright 2015 Hinet JSC. All rights reserved.
 * HINET PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

using Business.BaseBusiness;
using Business.CommonModel;
using Business.CommonModel.QUANLYVANBAN;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Business.Business
{
    public class QuanLyVanBanBusiness : BaseBusiness<QUANLY_VANBAN>
    {
        public QuanLyVanBanBusiness(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public void Save(QUANLY_VANBAN item)
        {
            try
            {
                if (item.ID == 0)
                {
                    item.NGAYTAO = DateTime.Now;
                    this.repository.Insert(item);
                }
                else
                {
                    this.repository.Update(item);
                }
                this.repository.Save();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<QUANLY_VANBAN> GetData(long id)
        {
            var result = from vanban in this.context.QUANLY_VANBAN
                         where vanban.HOSO_ID.HasValue && vanban.HOSO_ID == id
                         select vanban;
            return result.ToList();
        }

        public List<VanBanChildBO> GetByListHoSo(List<long> listHoSo)
        {
            var result = (from vb in this.context.QUANLY_VANBAN
                          join mst in this.context.DM_DANHMUC_DATA
                          on vb.COQUAN_BANHANH_ID equals (int)mst.ID
                          into group1
                          from gCoQuanBanHanh in group1.DefaultIfEmpty()
                          where vb.HOSO_ID.HasValue && listHoSo.Contains(vb.HOSO_ID.Value)
                          select new VanBanChildBO
                          {
                              HOSO_ID = vb.HOSO_ID.Value,
                              COQUAN_BANHANH_ID = vb.COQUAN_BANHANH_ID,
                              VANBAN_ID = vb.ID,
                              COQUAN_BANHANH_NAME = gCoQuanBanHanh.TEXT,
                              NGAYBANHANH = vb.NGAYBANHANH,
                              SO_KYHIEU = vb.SO_KYHIEU,
                              TRICHYEU_VANBAN = vb.TRICHYEU_VANBAN,
                          }).OrderByDescending(x => x.VANBAN_ID).ToList();
            if (result.Any())
            {
                foreach (var item in result)
                {
                    //  public const int VANBAN = 1999;
                    var listTailieu = new TAILIEUDINHKEMBusiness(new UnitOfWork()).GetDataByItemID(item.VANBAN_ID, 1999);
                    if (listTailieu.Any())
                    {
                        var taiLieu = listTailieu.FirstOrDefault();
                        if (taiLieu != null)
                        {
                            item.TAILIEU_ID = taiLieu.TAILIEU_ID;
                            item.TAILIEU_NAME = taiLieu.TENTAILIEU;
                        }
                    }
                    item.NGAYBANHANH_FORMAT = string.Format("{0:dd/MM/yyyy}", item.NGAYBANHANH);
                }
            }
            return result;
        }

        public VanBanChildBO GetBO(long id)
        {
            var result = (from vb in this.context.QUANLY_VANBAN
                          join mst in this.context.DM_DANHMUC_DATA
                          on vb.COQUAN_BANHANH_ID equals (int)mst.ID
                          into group1
                          from gCoQuanBanHanh in group1.DefaultIfEmpty()
                          join tl in this.context.TAILIEUDINHKEM
                          on vb.HOSO_ID equals tl.ITEM_ID
                          into group2
                          from gTaiLieu in group2.DefaultIfEmpty()
                          where vb.ID == id
                          select new VanBanChildBO
                          {
                              HOSO_ID = vb.HOSO_ID.Value,
                              COQUAN_BANHANH_ID = vb.COQUAN_BANHANH_ID,
                              VANBAN_ID = vb.ID,
                              COQUAN_BANHANH_NAME = gCoQuanBanHanh.TEXT,
                              NGAYBANHANH = vb.NGAYBANHANH,
                              SO_KYHIEU = vb.SO_KYHIEU,
                              TRICHYEU_VANBAN = vb.TRICHYEU_VANBAN,
                              TAILIEU_ID = gTaiLieu.TAILIEU_ID,
                              TAILIEU_NAME = gTaiLieu.TENTAILIEU
                          }).FirstOrDefault();

            if (result != null)
            {
                //  public const int VANBAN = 1999;
                var listTailieu = new TAILIEUDINHKEMBusiness(new UnitOfWork()).GetDataByItemID(result.VANBAN_ID, 1999);
                if (listTailieu.Any())
                {
                    var taiLieu = listTailieu.FirstOrDefault();
                    if (taiLieu != null)
                    {
                        result.TAILIEU_ID = taiLieu.TAILIEU_ID;
                        result.TAILIEU_NAME = taiLieu.TENTAILIEU;
                    }
                }
                result.NGAYBANHANH_FORMAT = string.Format("{0:dd/MM/yyyy}", result.NGAYBANHANH);
                return result;
            }
            return new VanBanChildBO();
        }

        public List<VanBanChildBO> GetByHoSo(long? hoSoId = 0)
        {
            var result = (from vb in this.context.QUANLY_VANBAN
                          join mst in this.context.DM_DANHMUC_DATA
                          on vb.COQUAN_BANHANH_ID equals (int)mst.ID
                          into group1
                          from gCoQuanBanHanh in group1.DefaultIfEmpty()
                          where vb.HOSO_ID == hoSoId
                          select new VanBanChildBO
                          {
                              HOSO_ID = vb.HOSO_ID.Value,
                              COQUAN_BANHANH_ID = vb.COQUAN_BANHANH_ID,
                              VANBAN_ID = vb.ID,
                              COQUAN_BANHANH_NAME = gCoQuanBanHanh.TEXT,
                              NGAYBANHANH = vb.NGAYBANHANH,
                              SO_KYHIEU = vb.SO_KYHIEU,
                              TRICHYEU_VANBAN = vb.TRICHYEU_VANBAN
                          }).ToList();

            if (result.Any())
            {
                foreach (var item in result)
                {
                    //  public const int VANBAN = 1999;
                    var listTailieu = new TAILIEUDINHKEMBusiness(new UnitOfWork()).GetDataByItemID(item.VANBAN_ID, 1999);
                    if (listTailieu.Any())
                    {
                        var taiLieu = listTailieu.FirstOrDefault();
                        if (taiLieu != null)
                        {
                            item.TAILIEU_ID = taiLieu.TAILIEU_ID;
                            item.TAILIEU_NAME = taiLieu.TENTAILIEU;
                        }
                    }
                    item.NGAYBANHANH_FORMAT = string.Format("{0:dd/MM/yyyy}", item.NGAYBANHANH);
                }
                return result;
            }
            return new List<VanBanChildBO>();
        }

        public DetailVanBanBO GetDetail(long? id = 0)
        {
            var result = new DetailVanBanBO();
            var findVanBan = this.Find(id);
            if (findVanBan != null)
            {
                var DM_DANHMUC_DATABusiness = new DM_DANHMUC_DATABusiness(new UnitOfWork());
                result.COQUAN_BANHANH_ID = findVanBan.COQUAN_BANHANH_ID;
                result.COQUAN_BANHANH_NAME = DM_DANHMUC_DATABusiness.GetName(findVanBan.COQUAN_BANHANH_ID);
                result.DOMAT_ID = findVanBan.DOMAT_ID;
                result.DOMAT_NAME = DM_DANHMUC_DATABusiness.GetName(findVanBan.DOMAT_ID);
                result.GHICHU = findVanBan.GHICHU;
                result.HOSO_ID = findVanBan.HOSO_ID;
                result.HOSO_NAME = new QuanLyHoSoBusiness(new UnitOfWork()).GetName(findVanBan.HOSO_ID);
                result.ID = findVanBan.ID;
                result.LINHVUC_ID = findVanBan.LINHVUC_ID;
                result.LINHVUC_NAME = DM_DANHMUC_DATABusiness.GetName(findVanBan.LINHVUC_ID);
                result.LOAI_VANBAN_ID = findVanBan.LOAI_VANBAN_ID;
                result.LOAI_VANBAN_NAME = DM_DANHMUC_DATABusiness.GetName(findVanBan.LOAI_VANBAN_ID);
                result.MUCDO_TRUYCAP = findVanBan.MUCDO_TRUYCAP;
                result.MUCDO_TRUYCAP_NAME = DM_DANHMUC_DATABusiness.GetName(findVanBan.MUCDO_TRUYCAP);
                result.NGAYBANHANH = findVanBan.NGAYBANHANH;
                result.NGAYTAO = findVanBan.NGAYTAO;
                result.NGONNGU_ID = findVanBan.NGONNGU_ID;
                result.NGONNGU_NAME = DM_DANHMUC_DATABusiness.GetName(findVanBan.NGONNGU_ID);
                result.NGUOITAO = findVanBan.NGUOITAO;
                result.SO_KYHIEU = findVanBan.SO_KYHIEU;
                result.TIEUDE = findVanBan.TIEUDE;
                result.TINHTRANG_VATLY = findVanBan.TINHTRANG_VATLY;
                result.TINHTRANG_VATLY_NAME = DM_DANHMUC_DATABusiness.GetName(findVanBan.TINHTRANG_VATLY);
                result.TRICHYEU_VANBAN = findVanBan.TRICHYEU_VANBAN;
            }
            return result;
        }

        public ThongKeVanBanBO ThongKeByMonth(int? year = 0)
        {
            if (year <= 0)
            {
                year = DateTime.Now.Year;
            }
            var result = new ThongKeVanBanBO();
            var sourceMonth = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var sourceVanBan = this.repository.All().Where(x => x.NGAYBANHANH.HasValue && x.NGAYBANHANH.Value.Year == year).Select(x => x.NGAYBANHANH);
            var listVanBan = new List<string>();
            foreach (var item in sourceMonth)
            {
                listVanBan.Add(sourceVanBan.Where(x => x.Value.Month == item).Count().ToString());
            }
            JavaScriptSerializer javascript = new JavaScriptSerializer();
            result.VanBan = javascript.Serialize(listVanBan);
            result.YearText = "Năm " + year;
            return result;
        }

        public List<int> GetThongKe(int? year = 0)
        {
            if (year <= 0)
            {
                year = DateTime.Now.Year;
            }
            var sourceMonth = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var sourceVanBan = this.repository.All().Where(x => x.NGAYBANHANH.HasValue && x.NGAYBANHANH.Value.Year == year).Select(x => x.NGAYBANHANH);
            var listVanBan = new List<int>();
            foreach (var item in sourceMonth)
            {
                listVanBan.Add(sourceVanBan.Where(x => x.Value.Month == item).Count());
            }
            return listVanBan;
        }

        public List<SelectListItem> GetDropDowYear(int selected)
        {
            return this.repository.All().Where(x => x.NGAYBANHANH.HasValue).Select(x => x.NGAYBANHANH.Value.Year).Distinct().OrderBy(x => x).Select(x => new SelectListItem { Value = x.ToString(), Text = "Năm " + x, Selected = (x == selected) }).ToList();
        }

        public void Delete(object id)
        {
            this.repository.Delete(id);
            this.repository.Save();
        }
    }
}