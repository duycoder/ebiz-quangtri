using Business.BaseBusiness;
using Business.CommonBusiness;
using Business.CommonModel.QUANLYVANBAN;
using Business.CommonModel.QUANLYHOSO;
using CommonHelper;

/**
 * The HiNet License
 *
 * Copyright 2015 Hinet JSC. All rights reserved.
 * HINET PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

using Model.Entities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Business.Business
{
    public class QuanLyHoSoBusiness : BaseBusiness<QUANLY_HOSO>
    {
        public QuanLyHoSoBusiness(UnitOfWork unitOfWork = null)
            : base(unitOfWork)
        {
        }

        public void Save(QUANLY_HOSO item)
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

        public PageListResultBO<QuanLyHoSoBO> GetData(int DonViId, int pageIndex, int pageSize, QUANLY_HOSO HoSo)
        {
            var query = from hoso in this.context.QUANLY_HOSO
                        join phong in this.context.DM_DANHMUC_DATA
                        on hoso.PHONG_ID equals phong.ID
                        into group1
                        from g1 in group1.DefaultIfEmpty()
                        join nguoidung in this.context.DM_NGUOIDUNG
                        on hoso.NGUOITAO equals nguoidung.ID
                        into group2
                        from g2 in group2.DefaultIfEmpty()
                        join kho in this.context.DM_DANHMUC_DATA
                        on hoso.KHO_ID equals kho.ID
                        into group3
                        from g3 in group3.DefaultIfEmpty()
                        orderby hoso.TIEUDE
                        select new QuanLyHoSoBO
                        {
                            #region Gan gia tri

                            CHUGIAI = hoso.CHUGIAI,
                            DONVI_ID = hoso.DONVI_ID,
                            THOIHAN_BAOQUAN_ID = hoso.THOIHAN_BAOQUAN_ID,
                            HOPSO = hoso.HOPSO,
                            HOSO_NAM = hoso.HOSO_NAM,
                            HOSO_SO = hoso.HOSO_SO,
                            ID = hoso.ID,
                            THOIGIAN_TAILIEU = hoso.THOIGIAN_TAILIEU,
                            MUCDO_TRUYCAP = hoso.MUCDO_TRUYCAP,
                            MUCLUC_SO = hoso.MUCLUC_SO,
                            NAM_CHINHLY = hoso.NAM_CHINHLY,
                            NGAYTAO = hoso.NGAYTAO,
                            NGUOITAO = hoso.NGUOITAO,
                            PHONG_ID = hoso.PHONG_ID,
                            PHONGSO = hoso.PHONGSO,
                            SOLUONG_TO = hoso.SOLUONG_TO,
                            TEN_NGUOITAO = g2.HOTEN,
                            TEN_PHONG = g1.TEXT,
                            TIEUDE = hoso.TIEUDE,
                            TRANGTHAI = hoso.TRANGTHAI,
                            KHO_ID = hoso.KHO_ID,
                            TEN_KHO = g3.TEXT

                            #endregion Gan gia tri
                        };
            if (HoSo.KHO_ID.HasValue)
            {
                query = query.Where(x => x.KHO_ID.HasValue && x.KHO_ID == HoSo.KHO_ID);
            }
            if (!string.IsNullOrEmpty(HoSo.TIEUDE))
            {
                query = query.Where(x => !string.IsNullOrEmpty(x.TIEUDE) && x.TIEUDE.Contains(HoSo.TIEUDE));
            }
            if (HoSo.PHONG_ID.HasValue)
            {
                query = query.Where(x => x.PHONG_ID.HasValue && x.PHONG_ID == HoSo.PHONG_ID);
            }
            if (HoSo.MUCDO_TRUYCAP.HasValue)
            {
                query = query.Where(x => x.MUCDO_TRUYCAP.HasValue && x.MUCDO_TRUYCAP == HoSo.MUCDO_TRUYCAP);
            }
            query = query.OrderByDescending(x => x.ID);
            var resultModel = new PageListResultBO<QuanLyHoSoBO>();
            var dataPageList = query.ToPagedList(pageIndex, pageSize);
            resultModel.Count = dataPageList.TotalItemCount;
            resultModel.ListItem = dataPageList.ToList();
            resultModel.TotalPage = dataPageList.PageCount;

            return resultModel;
        }

        public PageListResultBO<QuanLyHoSoBO> GetPage(QuanLyHoSoSearchModel searchModel, int pageNumber, int pageSize)
        {
            IQueryable<QuanLyHoSoBO> query = (
                from hs in this.context.QUANLY_HOSO
                join phong in this.context.DM_DANHMUC_DATA
                on hs.PHONG_ID equals (int)phong.ID
                into group1
                from gPhong in group1.DefaultIfEmpty()
                join nguoidung in this.context.DM_NGUOIDUNG
                on hs.NGUOITAO equals nguoidung.ID
                into group2
                from gNguoiDung in group2.DefaultIfEmpty()
                join kho in this.context.DM_DANHMUC_DATA
                on hs.KHO_ID equals (int)kho.ID
                into group3
                from gKho in group3.DefaultIfEmpty()
                orderby hs.TIEUDE
                select new QuanLyHoSoBO
                {
                    DONVI_ID = hs.DONVI_ID,
                    ID = hs.ID,
                    TIEUDE = hs.TIEUDE,
                    HOSO_NAM = hs.HOSO_NAM,
                    NAM_CHINHLY = hs.NAM_CHINHLY,
                    KHO_ID = hs.KHO_ID,
                    PHONG_ID = hs.PHONG_ID,
                    FTS = hs.FTS,
                    THOIGIAN_TAILIEU = hs.THOIGIAN_TAILIEU,
                    CHUGIAI = hs.CHUGIAI,
                    THOIHAN_BAOQUAN_ID = hs.THOIHAN_BAOQUAN_ID,
                    HOPSO = hs.HOPSO,
                    HOSO_SO = hs.HOSO_SO,
                    MUCDO_TRUYCAP = hs.MUCDO_TRUYCAP,
                    MUCLUC_SO = hs.MUCLUC_SO,
                    NGAYTAO = hs.NGAYTAO,
                    NGUOITAO = hs.NGUOITAO,
                    PHONGSO = hs.PHONGSO,
                    SOLUONG_TO = hs.SOLUONG_TO,
                    TEN_KHO = gKho.TEXT,
                    TEN_NGUOITAO = gNguoiDung.HOTEN,
                    TEN_PHONG = gPhong.TEXT,
                    TRANGTHAI = hs.TRANGTHAI
                });
            if (searchModel != null)
            {
                if (searchModel.HoSoNam > 0)
                {
                    query = query.Where(x => x.HOSO_NAM == searchModel.HoSoNam);
                }
                if (searchModel.NamChinhLy > 0)
                {
                    query = query.Where(x => x.NAM_CHINHLY == searchModel.NamChinhLy);
                }
                if (searchModel.KhoId > 0)
                {
                    query = query.Where(x => x.KHO_ID == searchModel.KhoId);
                }
                if (searchModel.PhongId > 0)
                {
                    query = query.Where(x => x.PHONG_ID == searchModel.PhongId);
                }
                if (!string.IsNullOrEmpty(searchModel.FTS))
                {
                    searchModel.FTS = searchModel.FTS.ConvertToVN();
                    query = query.Where(x => x.FTS.Contains(searchModel.FTS));
                }
                if (!string.IsNullOrEmpty(searchModel.sortQuery))
                {
                    
                    query = query.OrderBy(searchModel.sortQuery);
                }
                else
                {
                    query = query.OrderByDescending(x => x.ID);
                }
            }
            var resultModel = new PageListResultBO<QuanLyHoSoBO>();
            var dataPageList = query.ToPagedList(pageNumber, pageSize);
            var listHoSo = dataPageList.ToList();
            #region lấy count văn bản
            var vanBanBusiness = new QuanLyVanBanBusiness(new UnitOfWork());
            foreach (var item in listHoSo)
            {
                item.CountVanBan = vanBanBusiness.repository.All().Where(x => x.HOSO_ID == item.ID).Count();
            }
            #endregion
            resultModel.Count = dataPageList.TotalItemCount;
            resultModel.ListItem = listHoSo;
            resultModel.TotalPage = dataPageList.PageCount;
            return resultModel;
        }

        public PageListResultBO<VanBanPageListBO> GetPageForVanBan(QuanLyHoSoSearchModel searchModel, int pageNumber, int pageSize)
        {
            IQueryable<VanBanPageListBO> query = (
                from hs in this.context.QUANLY_HOSO
                orderby hs.TIEUDE
                select new VanBanPageListBO
                {
                    DONVI_ID = hs.DONVI_ID,
                    HOSO_ID = hs.ID,
                    HOSO_NAME = hs.TIEUDE,
                    HOSO_NAM = hs.HOSO_NAM,
                    NAM_CHINH_LY = hs.NAM_CHINHLY,
                    KHO_ID = hs.KHO_ID,
                    PHONG_ID = hs.PHONG_ID,
                    FTS = hs.FTS
                });
            if (searchModel != null)
            {
                if (searchModel.HoSoNam > 0)
                {
                    query = query.Where(x => x.HOSO_NAM == searchModel.HoSoNam);
                }
                if (searchModel.NamChinhLy > 0)
                {
                    query = query.Where(x => x.NAM_CHINH_LY == searchModel.NamChinhLy);
                }
                if (searchModel.KhoId > 0)
                {
                    query = query.Where(x => x.KHO_ID == searchModel.KhoId);
                }
                if (searchModel.PhongId > 0)
                {
                    query = query.Where(x => x.PHONG_ID == searchModel.PhongId);
                }
                if (!string.IsNullOrEmpty(searchModel.FTS))
                {
                    searchModel.FTS = searchModel.FTS.ConvertToVN();
                    query = query.Where(x => x.FTS.Contains(searchModel.FTS));
                }
                if (!string.IsNullOrEmpty(searchModel.sortQuery))
                {
                    query = query.OrderBy(searchModel.sortQuery);
                }
                else
                {
                    query = query.OrderByDescending(x => x.HOSO_ID);
                }
            }

            var resultModel = new PageListResultBO<VanBanPageListBO>();
            var dataPageList = query.ToPagedList(pageNumber,pageSize);
            var listHoSo = dataPageList.ToList();
            resultModel.Count = dataPageList.TotalItemCount;
            resultModel.TotalPage = dataPageList.PageCount;

            var listHoSoID = listHoSo.Select(x => x.HOSO_ID).ToList();
            var total = query.Count();
            var listVanBan = new QuanLyVanBanBusiness(new UnitOfWork()).GetByListHoSo(listHoSoID);
            //Lấy list van ban
            foreach (var item in listHoSo)
            {
                var sourceVB = listVanBan.Where(x => x.HOSO_ID == item.HOSO_ID).ToList();
                //item.ListVanBan = sourceVB;
                item.TotalVanBan = sourceVB.Count;
            }
            resultModel.ListItem = listHoSo;
            return resultModel;
        }

        public List<SelectListItem> GetDropDow(long? selected = 0)
        {
            return this.repository.All().Select(x => new SelectListItem { Text = x.TIEUDE, Value = x.ID.ToString(), Selected = (x.ID == selected) }).ToList();
        }

        public QuanLyHoSoDetailBO GetDetailBO(long? hoSoId = 0)
        {
            var hoSo = this.Find(hoSoId);
            if (hoSo != null)
            {
                var dataDanhMuc = new DM_DANHMUC_DATABusiness(new UnitOfWork());
                var result = new QuanLyHoSoDetailBO();
                result.CHUGIAI = hoSo.CHUGIAI;
                result.DONVI_ID = hoSo.DONVI_ID;
                result.FTS = hoSo.FTS;
                result.THOIHAN_BAOQUAN_ID = hoSo.THOIHAN_BAOQUAN_ID;
                result.HOPSO = hoSo.HOPSO;
                result.HOSO_NAM = hoSo.HOSO_NAM;
                result.HOSO_SO = hoSo.HOSO_SO;
                result.ID = hoSo.ID;
                result.KHO_ID = hoSo.KHO_ID;
                result.MUCDO_TRUYCAP = hoSo.MUCDO_TRUYCAP;
                result.THOIGIAN_TAILIEU = hoSo.THOIGIAN_TAILIEU;
                result.MUCLUC_SO = hoSo.MUCLUC_SO;
                result.NAM_CHINHLY = hoSo.NAM_CHINHLY;
                result.NGAYTAO = hoSo.NGAYTAO;
                result.NGUOITAO = hoSo.NGUOITAO;
                result.PHONGSO = hoSo.PHONGSO;
                result.PHONG_ID = hoSo.PHONG_ID;
                result.SOLUONG_TO = hoSo.SOLUONG_TO;
                result.TEN_KHO = dataDanhMuc.GetName(hoSo.KHO_ID);
                result.TEN_MUCDO_TRUYCAP = dataDanhMuc.GetName(hoSo.MUCDO_TRUYCAP);
                result.TEN_PHONG = dataDanhMuc.GetName( hoSo.PHONG_ID);
                result.THOIHAN = dataDanhMuc.GetName( hoSo.THOIHAN_BAOQUAN_ID);
                result.TIEUDE = hoSo.TIEUDE;
                result.TRANGTHAI = hoSo.TRANGTHAI;
                return result;
            }
            return new QuanLyHoSoDetailBO();
        }

        public string GetName(long? id = 0)
        {
            var find = this.Find(id);
            if (find != null)
            {
                return find.TIEUDE;
            }
            return string.Empty;
        }

        public List<QuanLyHoSoShortBO> ListShort()
        {
            return this.repository.All().Where(x => !string.IsNullOrEmpty(x.HOSO_SO) && !string.IsNullOrEmpty(x.TIEUDE)).Select(x => new QuanLyHoSoShortBO { HOSO_SO = x.HOSO_SO.Trim().ToLower(), TIEUDE = x.TIEUDE.Trim().ToLower() }).ToList();
        }

        public QUANLY_HOSO GetByID(int? id = 0)
        {
            if (id > 0)
            {
                var data = this.Find(id);
                if (data != null)
                {
                    return data;
                }
            }
            return new QUANLY_HOSO();
        }

        public ThongKeVanBanBO GetThongKeByYear()
        {
            var result = new ThongKeVanBanBO();
            var sourceHoSo = this.repository.All().Select(x => new { x.HOSO_NAM, x.ID }).ToList();
            var sourceVanBan = new QuanLyVanBanBusiness(new UnitOfWork()).repository.All().Select(x => x.HOSO_ID).ToList();
            var listHoSo = new List<string>();
            var listVanBan = new List<string>();
            var sourceYear = sourceHoSo.Where(x => x.HOSO_NAM.HasValue).Select(x => x.HOSO_NAM).Distinct().OrderBy(x => x.Value).ToList();
            var listYear = new List<string>();
            foreach (var item in sourceYear)
            {
                var listID = sourceHoSo.Where(x => x.HOSO_NAM == item).Select(x => x.ID);
                listHoSo.Add(listID.Count().ToString());
                listVanBan.Add(sourceVanBan.Where(x => listID.Contains(x.Value)).Count().ToString());
                listYear.Add("Năm " + item);
            }
            JavaScriptSerializer javascript = new JavaScriptSerializer();
            result.HoSo = javascript.Serialize(listHoSo);
            result.VanBan = javascript.Serialize(listVanBan);
            result.Nam = javascript.Serialize(listYear);
            return result;
        }

        public bool TieuDeExisted(string tieuDe = "", int id = 0)
        {
            if (!string.IsNullOrEmpty(tieuDe))
            {
                tieuDe = tieuDe.Trim().ToLower();
                if (id > 0)
                {
                    return this.repository.All().Where(x => x.TIEUDE.Trim().ToLower().Equals(tieuDe) && x.ID != id).Any();
                }
                return this.repository.All().Where(x => x.TIEUDE.Trim().ToLower().Equals(tieuDe)).Any();
            }
            return false;
        }

        public bool HoSoSoExisted(string hoSoSo = "", int id = 0)
        {
            if (!string.IsNullOrEmpty(hoSoSo))
            {
                hoSoSo = hoSoSo.Trim().ToLower();
                if (id > 0)
                {
                    return this.repository.All().Where(x => x.HOSO_SO.ToLower().Equals(hoSoSo) && x.ID != id).Any();
                }
                return this.repository.All().Where(x => x.HOSO_SO.ToLower().Equals(hoSoSo)).Any();
            }
            return false;
        }

        public void Delete(object id)
        {
            this.repository.Delete(id);
            this.repository.Save();
        }
    }
}