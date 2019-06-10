using Business.BaseBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.CommonModel;
using Business.CommonBusiness;
using Model.Entities;
using Business.CommonModel.CHIASETAILIEU;
using System.Linq.Dynamic;
using PagedList;
using Business.CommonModel.CONSTANT;

namespace Business.Business
{
    public class ChiaSeTaiLieuBusiness:BaseBusiness<CHIASE_TAILIEU>
    {
        public ChiaSeTaiLieuBusiness(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public PageListResultBO<ChiaSeTaiLieuBO> GetPage(ChiaSeTaiLieuSearchModel searchModel,int pageIndex,int pageSize)
        {
            var query = (from share in this.context.CHIASE_TAILIEU
                         join userRQ in this.context.DM_NGUOIDUNG
                           on share.USER_YEU_CAU equals userRQ.ID
                         join userShare in this.context.DM_NGUOIDUNG on share.USER_CHIA_SE equals userShare.ID
                         into group1
                         from gUserShare in group1.DefaultIfEmpty()
                         join userApprove in this.context.DM_NGUOIDUNG on share.USER_PHE_DUYET equals userApprove.ID
                         into group2
                         from gUserApprove in group2.DefaultIfEmpty()
                         orderby share.ID descending
                         select new ChiaSeTaiLieuBO
                          {
                            DATE_CHIA_SE=share.DATE_CHIA_SE,
                            DATE_YEU_CAU=share.DATE_YEU_CAU,
                            ID=share.ID,
                            DATE_PHE_DUYET=share.DATE_PHE_DUYET,
                            STATUS=share.STATUS,
                            TIEUDE=share.TIEUDE,
                            USER_CHIA_SE=share.USER_CHIA_SE,
                            USER_NAME_CHIA_SE=gUserShare.HOTEN,
                            USER_NAME_YEU_CAU=userRQ.HOTEN,
                            USER_YEU_CAU=share.USER_YEU_CAU,
                            USER_PHE_DUYET=share.USER_PHE_DUYET,
                            USER_NAME_PHE_DUYET=gUserApprove.HOTEN
                          });

            if(searchModel != null)
            {
                if (searchModel.STATUS.HasValue)
                {
                    query = query.Where(x => x.STATUS == searchModel.STATUS);
                }
                if (searchModel.USER_CHIA_SE.HasValue)
                {
                    query = query.Where(x => x.USER_CHIA_SE == searchModel.USER_CHIA_SE);
                }
                if (searchModel.USER_YEU_CAU.HasValue)
                {
                    query = query.Where(x => x.USER_YEU_CAU == searchModel.USER_YEU_CAU);
                }
                if (searchModel.USER_PHE_DUYET.HasValue)
                {
                    query = query.Where(x => x.USER_PHE_DUYET == searchModel.USER_PHE_DUYET);
                }
                if (!string.IsNullOrEmpty(searchModel.KEYWORD))
                {
                    query = query.Where(x => x.TIEUDE.IndexOf(searchModel.KEYWORD) >= 0);
                }
                if(!string.IsNullOrEmpty(searchModel.sortQuery))
                {
                    query = query.OrderBy(searchModel.sortQuery);
                }
                else
                {
                    query = query.OrderByDescending(x => x.ID);
                }
            }
            var model = new PageListResultBO<ChiaSeTaiLieuBO>();
            var dataPageList = query.ToPagedList(pageIndex, pageSize);
            var listData = dataPageList.ToList();
            foreach(var item in listData)
            {
                if (item.STATUS.HasValue)
                {
                    if(item.STATUS == SHARE_STATUS_CONSTANT.YEU_CAU_CHIA_SE)
                    {
                        item.STR_STATUS = "Chờ phê duyệt chia sẻ";
                    }else if (item.STATUS == SHARE_STATUS_CONSTANT.PHE_DUYET_CHIA_SE)
                    {
                        item.STR_STATUS = "Chờ chia sẻ";
                    }else if (item.STATUS == SHARE_STATUS_CONSTANT.DA_CHIA_SE)
                    {
                        item.STR_STATUS = "Đã chia sẻ";
                    }
                    else
                    {
                        item.STR_STATUS = "Không chia sẻ";
                    }
                }
            }
            model.Count = dataPageList.Count;
            model.ListItem = listData;
            model.TotalPage = dataPageList.PageCount;
            return model;
        }
        public ChiaSeTaiLieuBO GetBO(int? id)
        {
            var find = this.Find(id);
            if (find == null)
            {
                return new ChiaSeTaiLieuBO();
            }
            var result = new ChiaSeTaiLieuBO();
            result.DATE_CHIA_SE = find.DATE_CHIA_SE;
            result.DATE_PHE_DUYET = find.DATE_PHE_DUYET;
            result.DATE_YEU_CAU = find.DATE_YEU_CAU;
            result.ID = find.ID;
            result.NOIDUNG_CHIASE = find.NOIDUNG_CHIASE;
            result.NOIDUNG_PHEDUYET = find.NOIDUNG_PHEDUYET;
            result.NOIDUNG_YEUCAU = find.NOIDUNG_YEUCAU;
            result.STATUS = find.STATUS;
            result.TIEUDE = find.TIEUDE;
            result.USER_CHIA_SE = find.USER_CHIA_SE;
            result.USER_PHE_DUYET = find.USER_PHE_DUYET;
            result.USER_YEU_CAU = find.USER_YEU_CAU;
            var NguoiDungBusiness = new DM_NGUOIDUNGBusiness(new UnitOfWork());
            result.USER_NAME_CHIA_SE = NguoiDungBusiness.GetName(find.USER_CHIA_SE);
            result.USER_NAME_PHE_DUYET = NguoiDungBusiness.GetName(find.USER_PHE_DUYET);
            result.USER_NAME_YEU_CAU = NguoiDungBusiness.GetName(find.USER_YEU_CAU);
            return result;


        }
    }
}
