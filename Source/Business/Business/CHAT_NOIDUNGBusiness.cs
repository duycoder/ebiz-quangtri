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
using Business.CommonModel.CCTCTHANHPHAN;
using System.Collections;
using System.Web.Mvc;



namespace Business.Business
{
    public class CHAT_NOIDUNGBusiness : BaseBusiness<CHAT_NOIDUNG>
    {
        public CHAT_NOIDUNGBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        public List<CHAT_NOIDUNG> GetListChat(string from, string to, DateTime date, long groupId, int count, int maxItem)
        {
            if (groupId > 0)
            {
                var result = this.repository.All().Where(o => o.GROUPCHAT_ID == groupId
                                        && (maxItem > 0 ? o.ID < maxItem : true)
                                        ).OrderByDescending(o => o.NGAYGUI).ToList().Take(count);
                return result.OrderBy(o => o.NGAYGUI).ToList();
            }
            else
            {
                var result = this.repository.All().Where(o => (o.FROMUSER == from || o.FROMUSER == to)
                                        && (o.TOUSER == from || o.TOUSER == to)
                                        && (maxItem > 0 ? o.ID < maxItem : true)
                                        ).OrderByDescending(o => o.NGAYGUI).ToList().Take(count);
                return result.OrderBy(o => o.NGAYGUI).ToList();
            }
        }

        public List<ChatBO> GetListChatHistory(int coso_id, decimal user_id, string username, string fullname)
        {
            var list_Nguoi_HoiThoai = new List<ChatBO>();
            if (coso_id > 0 && user_id > 0)
            {
                //Lấy thông tin mà user_id là NGUOIGUI_ID
                var list_NguoiNhan = this.repository.All().Where(o => o.NGUOIGUI_ID == user_id && o.GROUPCHAT_ID.HasValue == false)
                                .OrderByDescending(o => o.NGAYGUI)
                                .GroupBy(test => new { test.NGUOINHAN_ID, /*test.NOIDUNG,*/ test.TOFULLNAME, test.TOUSER/*, test.NGAYGUI*/ })
                                .ToList();
                //Lấy thông tin mà user_id là NGUOINHAN_ID
                var list_NguoiGui = this.repository.All().Where(o => o.NGUOINHAN_ID == user_id && o.GROUPCHAT_ID.HasValue == false)
                                .OrderByDescending(o => o.NGAYGUI)
                                .GroupBy(test => new { test.NGUOIGUI_ID/*, test.NOIDUNG*/, test.FROMFULLNAME, test.FROMUSER/*, test.NGAYGUI*/ })
                                .ToList();

                if (list_NguoiNhan != null && list_NguoiNhan.Count > 0)
                {
                    var lst_nguoiNhan = list_NguoiNhan.Select(chat => new ChatBO
                    {
                        //NOIDUNG = chat.Key.NOIDUNG,
                        FROMFULLNAME = fullname,
                        TOFULLNAME = chat.Key.TOFULLNAME,
                        FROMUSER = username,
                        TOUSER = chat.Key.TOUSER,
                        NGUOIGUI_ID = (long)user_id,
                        NGUOINHAN_ID = chat.Key.NGUOINHAN_ID,
                        //NGAYGUI = chat.Key.NGAYGUI
                    }).ToList();
                    list_Nguoi_HoiThoai.AddRange(lst_nguoiNhan);
                    if (list_NguoiGui != null && list_NguoiGui.Count > 0)
                    {
                        var lst_nguoiGui = list_NguoiGui.Select(chat => new ChatBO
                        {
                            //NOIDUNG = chat.Key.NOIDUNG,
                            FROMFULLNAME = chat.Key.FROMFULLNAME,
                            TOFULLNAME = fullname,
                            FROMUSER = chat.Key.FROMUSER,
                            TOUSER = username,
                            NGUOIGUI_ID = chat.Key.NGUOIGUI_ID,
                            NGUOINHAN_ID = (long)user_id,
                            //NGAYGUI = chat.Key.NGAYGUI
                        }).Where(o => lst_nguoiNhan.Any(x => x.NGUOINHAN_ID == o.NGUOIGUI_ID) == false).ToList();
                        if (lst_nguoiGui != null && lst_nguoiGui.Count > 0)
                        {
                            list_Nguoi_HoiThoai.AddRange(lst_nguoiGui);
                        }
                    }
                }

            }
            return list_Nguoi_HoiThoai;
        }

    }
}

