using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;
using Business.BaseBusiness;
using Business.CommonBusiness;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using PagedList;
using Business.CommonModel.DMDANHMUCDATA;
using System.Web.Mvc;
using CommonHelper;


namespace Business.Business
{
    public class DM_DANHMUC_DATABusiness : BaseBusiness<DM_DANHMUC_DATA>
    {
        public DM_DANHMUC_DATABusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        public JsonResultBO saveImport(List<DM_DANHMUC_DATA> lstObj)
        {
            var result = new JsonResultBO(true);
            using (var transaction = repository.Context.Database.BeginTransaction())
            {
                try
                {

                    repository.Context.DM_DANHMUC_DATA.AddRange(lstObj);
                    repository.Context.SaveChanges();
                    transaction.Commit();
                }
                catch
                {
                    result.Status = false;
                    result.Message = "Không import được dữ liệu";
                    transaction.Rollback();
                }
            }
            return result;
        }
        /// <summary>
        /// Chec
        /// </summary>
        /// <param name="idDanhMuc"></param>
        /// <param name="valueData"></param>
        /// <param name="IdData"></param>
        /// <returns></returns>
        public JsonResultBO ExistValue(int idDanhMuc, int valueData, int IdData = 0)
        {
            var result = new JsonResultBO(false);
            var obj = repository.All().Where(x => x.DM_NHOM_ID == idDanhMuc && x.DATA == valueData).FirstOrDefault();
            if (obj != null)
            {
                if (IdData > 0)
                {
                    if (obj.ID != IdData)
                    {
                        result.Status = true;
                        result.Message = "Giá trị đã tồn tại trong nhóm";
                    }
                }
                else
                {
                    result.Status = true;
                    result.Message = "Giá trị đã tồn tại trong nhóm";
                }
            }
            return result;

        }

        public List<SelectListItem> DsByMaNhom(string maNhom, long currentuser_id, int selectedItem = 0, bool init = false, bool is_search = false)
        {
            var lstDataIds = (from dtvt in this.context.DM_DANHMUC_DATA_VAITRO select dtvt.DATA_ID).Distinct().ToList();
            if (currentuser_id > 0)
            {
                // lay danh sach vai tro cua nguoi dung hien tai
                var lstVaiTro = (from vaitro in this.context.NGUOIDUNG_VAITRO where vaitro.NGUOIDUNG_ID == currentuser_id select vaitro.VAITRO_ID).ToList();
                // end
                // lay toan bo data id duoc gan voi vai tro
                var lstCurrentUserDataIds = (from dtvt in this.context.DM_DANHMUC_DATA_VAITRO
                                             where lstVaiTro.Contains(dtvt.VAITRO_ID)
                                             select dtvt.DATA_ID.Value).Distinct().ToList();
                lstDataIds = lstDataIds.Where(x => !lstCurrentUserDataIds.Contains(x.Value)).ToList();
                // end  
            }

            var query = from nhom in this.context.DM_NHOMDANHMUC
                        where nhom.GROUP_CODE == maNhom
                        join tbl_data in this.context.DM_DANHMUC_DATA on nhom.ID
                        equals tbl_data.DM_NHOM_ID into jdata
                        select jdata.DefaultIfEmpty();
            var lstitem = (from data in query.FirstOrDefault()
                           where !lstDataIds.Contains(data.ID)
                           select new SelectListItem
                           {
                               Text = data.TEXT,
                               Value = data.ID.ToString(),
                               Selected = selectedItem > 0 && selectedItem == data.ID
                           }).ToList();


            if (init == true)
            {
                lstitem.Add(new SelectListItem() { Text = "Tất cả", Value = "", Selected = is_search });
            }

            return lstitem;
        }

        public DM_DANHMUC_DATA GetSoVanBan(string maNhom, int YEAR, int dept)
        {
            var result = from data in this.context.DM_DANHMUC_DATA
                         join nhomdata in this.context.DM_NHOMDANHMUC on data.DM_NHOM_ID equals nhomdata.ID
                         where data.DEPTID == dept && nhomdata.GROUP_CODE == maNhom && YEAR == data.DATA
                         select data;
            return result.FirstOrDefault();
        }
        public List<SelectListItem> DsByMaNhomByDept(string maNhom, long currentuser_id, int dept, int selectedItem = 0, bool init = false, bool is_search = false)
        {
            var lstDataIds = (from dtvt in this.context.DM_DANHMUC_DATA_VAITRO select dtvt.DATA_ID).Distinct().ToList();
            if (currentuser_id > 0)
            {
                // lay danh sach vai tro cua nguoi dung hien tai
                var lstVaiTro = (from vaitro in this.context.NGUOIDUNG_VAITRO where vaitro.NGUOIDUNG_ID == currentuser_id select vaitro.VAITRO_ID).ToList();
                // end
                // lay toan bo data id duoc gan voi vai tro
                var lstCurrentUserDataIds = (from dtvt in this.context.DM_DANHMUC_DATA_VAITRO
                                             where lstVaiTro.Contains(dtvt.VAITRO_ID)
                                             select dtvt.DATA_ID.Value).Distinct().ToList();
                lstDataIds = lstDataIds.Where(x => !lstCurrentUserDataIds.Contains(x.Value)).ToList();
                // end  
            }

            var query = from nhom in this.context.DM_NHOMDANHMUC
                        where nhom.GROUP_CODE == maNhom
                        join tbl_data in this.context.DM_DANHMUC_DATA on nhom.ID
                        equals tbl_data.DM_NHOM_ID into jdata
                        select jdata.DefaultIfEmpty();
            var lstitem = (from data in query.FirstOrDefault()
                           where !lstDataIds.Contains(data.ID) && data.DEPTID == dept
                           select new SelectListItem
                           {
                               Text = data.TEXT,
                               Value = data.ID.ToString(),
                               Selected = selectedItem > 0 && selectedItem == data.ID
                           }).ToList();


            if (init == true)
            {
                lstitem.Add(new SelectListItem() { Text = "Tất cả", Value = "", Selected = is_search });
            }

            return lstitem;
        }
        public List<SelectListItem> DsByMaNhomNull(string maNhom, int selectedItem = 0, bool init = false)
        {
            var query = from nhom in this.context.DM_NHOMDANHMUC
                        where nhom.GROUP_CODE == maNhom
                        join tbl_data in this.context.DM_DANHMUC_DATA on nhom.ID equals tbl_data.DM_NHOM_ID into jdata
                        select jdata.DefaultIfEmpty();
            var lstitem = (from data in query.FirstOrDefault()
                           select new SelectListItem
                           {
                               Text = data.TEXT,
                               Value = data.ID.ToString(),
                               Selected = selectedItem > 0 && selectedItem == data.ID
                           }).ToList();
            if (init == true)
            {
                lstitem.Add(new SelectListItem() { Text = "Không", Value = "", Selected = selectedItem == 0 });
            }

            return lstitem;
        }
        public List<object> ListIDByMaNhom(string maNhom)
        {
            var query = from nhom in this.context.DM_NHOMDANHMUC
                        where nhom.GROUP_CODE == maNhom
                        join tbl_data in this.context.DM_DANHMUC_DATA on nhom.ID equals tbl_data.DM_NHOM_ID into jdata
                        select jdata.DefaultIfEmpty();
            var lstitem = (from data in query.FirstOrDefault()
                           select (object)data.ID).ToList();
            return lstitem;
        }

        /// <summary>
        /// id là id nhóm danh mục
        /// </summary>
        /// <param name="id"></param>
        /// <param name="searchModel"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PageListResultBO<DM_DANHMUC_DATA_BO> GetDaTaByPage(int idDanhMuc, DM_DANHMUC_DATA_SEARCHBO searchModel, int pageIndex = 1, int pageSize = 20)
        {

            var query = from tbl in this.context.DM_DANHMUC_DATA
                        join dept in this.context.CCTC_THANHPHAN on tbl.DEPTID equals dept.ID into g1
                        from group1 in g1.DefaultIfEmpty()
                        where tbl.DM_NHOM_ID == idDanhMuc
                        select new DM_DANHMUC_DATA_BO
                        {
                            ID = tbl.ID,
                            DM_NHOM_ID = tbl.DM_NHOM_ID,
                            TEXT = tbl.TEXT,
                            CODE = tbl.CODE,
                            GHICHU = tbl.GHICHU,
                            DATA = tbl.DATA,
                            TENCAP = group1.NAME
                        };

            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.sortQuery))
                {
                    query = query.OrderBy(searchModel.sortQuery);
                }
                else
                {
                    query = query.OrderByDescending(x => x.ID);
                }
            }
            else
            {
                query = query.OrderByDescending(x => x.ID);
            }

            var resultmodel = new PageListResultBO<DM_DANHMUC_DATA_BO>();
            if (pageSize == -1)
            {
                var dataPageList = query.ToList();
                resultmodel.Count = dataPageList.Count;
                resultmodel.TotalPage = 1;
                resultmodel.ListItem = dataPageList;
            }
            else
            {
                var dataPageList = query.ToPagedList(pageIndex, pageSize);
                resultmodel.Count = dataPageList.TotalItemCount;
                resultmodel.TotalPage = dataPageList.PageCount;
                resultmodel.ListItem = dataPageList.ToList();
            }
            return resultmodel;
        }
        public DM_DANHMUC_DATA_BO GetDaTaByID(int ID)
        {
            var query = from tbl in this.context.DM_DANHMUC_DATA
                        where tbl.ID == ID
                        select new DM_DANHMUC_DATA_BO
                        {
                            ID = tbl.ID,
                            DM_NHOM_ID = tbl.DM_NHOM_ID,
                            TEXT = tbl.TEXT,
                            CODE = tbl.CODE,
                            GHICHU = tbl.GHICHU,
                            DATA = tbl.DATA,
                        };
            var resultmodel = query.FirstOrDefault();
            return resultmodel;
        }
        public List<DM_DANHMUC_DATA> GetData(int id)
        {
            var result = from danhmuc in this.context.DM_DANHMUC_DATA
                         where danhmuc.DM_NHOM_ID.HasValue && danhmuc.DM_NHOM_ID == id
                         select danhmuc;
            return result.ToList();
        }
        public List<DM_DANHMUC_DATA> GetDataByCode(string maNhom)
        {
            var query = from detail in this.context.DM_DANHMUC_DATA
                        join danhmuc in this.context.DM_NHOMDANHMUC
                        on detail.DM_NHOM_ID equals danhmuc.ID
                        where danhmuc.GROUP_CODE.ToLower().Equals(maNhom.ToLower())
                        select detail;
            return query.ToList();
        }

        public string GetName(int? id)
        {
            var result = string.Empty;
            if (id.HasValue)
            {
                var search = this.Find(id);
                if (search != null)
                {
                    return search.TEXT;
                }
            }
            return result;
        }

        /// <summary>
        ///@author: duynn
        ///@description: kiểm tra đã tồn tại
        /// </summary>
        /// <param name="code"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public DM_DANHMUC_DATA GetItemByCodeAndText(string code, string text)
        {
            text = string.IsNullOrEmpty(text) ? string.Empty : text.ToLower();
            DM_DANHMUC_DATA item = (from data in this.context.DM_DANHMUC_DATA.Where(x => string.IsNullOrEmpty(x.TEXT) == false && x.TEXT.ToLower().Equals(text))
                                    join category in this.context.DM_NHOMDANHMUC.Where(x => string.IsNullOrEmpty(x.GROUP_CODE) == false)
                                    on data.DM_NHOM_ID equals category.ID
                                    where category.GROUP_CODE.ToLower().Equals(code.ToLower())
                                    select data).FirstOrDefault();
            return item;
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy chuỗi giá trị bằng mã nhóm
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public List<string> GetGroupTextByCode(string code)
        {
            IQueryable<DM_DANHMUC_DATA> queryResult = (from data in this.context.DM_DANHMUC_DATA
                                                       join category in this.context.DM_NHOMDANHMUC.Where(x => string.IsNullOrEmpty(x.GROUP_CODE) == false)
                                                       on data.DM_NHOM_ID equals category.ID
                                                       where category.GROUP_CODE.ToLower().Equals(code.ToLower())
                                                       select data);
            List<string> result = queryResult.Select(x => x.TEXT).ToList();
            return result;
        }

        ///// <summary>
        ///// @description: cập nhật số đi theo sổ
        ///// @author: duynn
        ///// @since: 13/08/2018
        ///// </summary>
        ///// <param name="idSoVanBanDen"></param>
        ///// <param name="currentSoDiTheoSo"></param>
        public void UpdateSoVanBan(int idSoVanBan, int currentSoDiTheoSo)
        {
            DM_DANHMUC_DATA data = this.repository.Find(idSoVanBan);
            if (data != null)
            {
                data.GHICHU = currentSoDiTheoSo.ToString();
                this.Save(data);
            }
        }

        /// <summary>
        /// @description: lấy số đi theo sổ tiếp theo cho văn bản
        /// @author: duynn
        /// @since: 13/08/2018
        /// </summary>
        /// <param name="idSoVanBan"></param>
        /// <returns></returns>
        public string GetSoDiTheoSoVanBan(int idSoVanBan)
        {
            string result = string.Empty;
            DM_DANHMUC_DATA data = this.repository.Find(idSoVanBan);
            if (data != null)
            {
                int numbSoDiTheoSo = data.GHICHU.GetPrefixNumber();
                result = (numbSoDiTheoSo + 1).ToString();
            }
            return result;
        }

        /// <summary>
        /// @description: lấy các số văn bản đi còn thiếu
        /// @author: duynn
        /// @since: 13/08/2018
        /// </summary>
        /// <param name="idSoVanBanDi"></param>
        /// <returns></returns>
        public List<int> GetSuggestSoVanBanDi(int idSoVanBanDi)
        {
            List<int> result = new List<int>();
            DM_DANHMUC_DATA data = this.repository.Find(idSoVanBanDi);
            if (data != null)
            {
                //lấy số hiện tại
                int numbSoDiTheoSo = data.GHICHU.GetPrefixNumber();
                //tất cả các số đi theo sổ có thể
                List<int> allNumbers = Enumerable.Range(1, numbSoDiTheoSo).ToList();

                //tất cả các số đã lưu
                List<string> listSavedNumbersStr = (from doc in this.context.HSCV_VANBANDI
                                                    where doc.SOVANBAN_ID == idSoVanBanDi && !string.IsNullOrEmpty(doc.SOTHEOSO)
                                                    select doc.SOTHEOSO).ToList();
                List<int> listSavedNumbers = new List<int>();

                listSavedNumbersStr.ForEach(x =>
                {
                    int number = x.GetPrefixNumber();
                    if (number > 0)
                    {
                        listSavedNumbers.Add(number);
                    }
                });

                result = allNumbers.Where(x => listSavedNumbers.Contains(x) == false).ToList();
            }
            return result;
        }

        public List<SelectListItem> GetDropDow(string magroup, int? selected = 0)
        {
            var result = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(magroup))
            {
                var group = new DM_NHOMDANHMUCBusiness(new UnitOfWork()).GetByCode(magroup);
                if (group != null)
                {
                    return this.repository.All().Where(x => x.DM_NHOM_ID == group.ID).Select(x => new SelectListItem { Text = x.TEXT, Value = x.ID.ToString(), Selected = (x.ID == selected) }).ToList();
                }
            }
            return result;
        }

        /// <summary>
        /// @author:duynn
        /// @description: kiểm tra code tồn tại
        /// </summary>
        /// <param name="code"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public bool CheckExistCode(string code, int groupId)
        {
            if (string.IsNullOrEmpty(code))
            {
                return false;
            }

            DM_DANHMUC_DATA entity = this.context.DM_DANHMUC_DATA
                .Where(x => x.DM_NHOM_ID == groupId && x.CODE != null
                && x.CODE.Trim().ToLower() == code.Trim().ToLower())
                .FirstOrDefault();
            if (entity != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// @author:duynn
        /// @description: kiểm tra text tồn tại
        /// </summary>
        /// <param name="text"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public bool CheckExistText(string text, int groupId)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            DM_DANHMUC_DATA entity = this.context.DM_DANHMUC_DATA
                .Where(x => x.DM_NHOM_ID == groupId && x.TEXT != null
                && x.TEXT.Trim().ToLower() == text.Trim().ToLower())
                .FirstOrDefault();
            if (entity != null)
            {
                return true;
            }
            return false;
        }
    }
}



