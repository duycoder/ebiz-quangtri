using Business.BaseBusiness;
using Business.CommonBusiness;
using Business.CommonModel.CONSTANT;
using Business.CommonModel.QLTOKEN;
using Business.CommonModel.SYSTINNHAN;
//using CommonHelper.FcmNotif;
using Model.Entities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Net.Sockets;
using System.Web.Compilation;
using SocketIOClient;

namespace Business.Business
{
    public class SYS_TINNHANBusiness : BaseBusiness<SYS_TINNHAN>
    {
        private string SOCKET_SERVER = "http://127.0.0.1:3000";
        public SYS_TINNHANBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        //public void Save(SYS_TINNHAN item)
        //{
        //    try
        //    {
        //        if (item.ID == 0)
        //        {
        //            this.repository.Insert(item);
        //            var FCM_NOTIFICATIONBusiness = new FCM_NOTIFICATIONBusiness(new UnitOfWork());
        //            QL_TOKEN Token = (from token in this.context.QL_TOKEN.Where(x => x.DM_NGUOIDUNG_ID == item.TO_USER_ID).OrderByDescending(x => x.NGAYTAO)
        //                              select token).FirstOrDefault();
        //            if (Token != null)
        //            {
        //                NotifCommon notifCommon = new NotifCommon(FcmConstant.ServerKey, FcmConstant.SenderId);
        //                if (!notifCommon.NotifyAsync(Token.TOKEN, item.TIEUDE, item.NOIDUNG))
        //                {
        //                    FCM_NOTIFICATION Notif = new FCM_NOTIFICATION();
        //                    Notif.NGAYGUI = DateTime.Now;
        //                    Notif.NOIDUNG = item.NOIDUNG;
        //                    Notif.TIEUDE = item.TIEUDE;
        //                    Notif.TRANGTHAI = false;
        //                    FCM_NOTIFICATIONBusiness.Save(Notif);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            this.repository.Update(item);

        //        }
        //        this.repository.Save();
        //    }
        //    catch (Exception ex)
        //    {
        //        //LogHelper.Error(string.Format("UserService.Save: {0}", ex.Message));
        //        throw new Exception(ex.Message);
        //    }
        //}
        public void Save(SYS_TINNHAN item, string targetScreen
            , bool isTaskNotification, long objId, int docType)
        {
            try
            {
                if (item.ID == 0)
                {
                    this.repository.Insert(item);
                    //#region send noti to mobile device
                    //var FCM_NOTIFICATIONBusiness = new FCM_NOTIFICATIONBusiness(new UnitOfWork());
                    //var Tokens = (from token in this.context.QL_TOKEN.Where(x => x.DM_NGUOIDUNG_ID == item.TO_USER_ID && true == x.IS_ACTIVE).OrderByDescending(x => x.NGAYTAO)
                    //              select token);
                    //if (Tokens != null && Tokens.Any())
                    //{
                    //    #region gửi notif đến thiết bị đang active
                    //    foreach (var Token in Tokens)
                    //    {
                    //        NotifCommon notifCommon = new NotifCommon(FcmConstant.ServerKey, FcmConstant.SenderId);
                    //        if (!notifCommon.NotifyAsync(Token.TOKEN, item.TIEUDE, item.NOIDUNG, targetScreen, isTaskNotification, objId, docType))
                    //        {
                    //            FCM_NOTIFICATION Notif = new FCM_NOTIFICATION();
                    //            Notif.NGAYGUI = DateTime.Now;
                    //            Notif.NOIDUNG = item.NOIDUNG;
                    //            Notif.TIEUDE = item.TIEUDE;
                    //            Notif.TRANGTHAI = false;
                    //            FCM_NOTIFICATIONBusiness.Save(Notif);
                    //        }
                    //    }
                    //    #endregion
                    //}
                    //#endregion
                    #region send noti to web user
                    Client socket = new Client(SOCKET_SERVER);
                    socket.Opened += SocketOpened;
                    socket.Message += SocketMessage;
                    socket.SocketConnectionClosed += SocketConnectionClosed;
                    socket.Error += SocketError;
                    // register for 'connect' event with io server
                    socket.Connect();
                    List<long> user_id = new List<long>();
                    user_id.Add(item.TO_USER_ID.Value);
                    socket.Emit("send_notifications", new { message = item.NOIDUNG, LstUId = user_id, id = item.ID, url = item.URL });
                    #endregion
                }
                else
                {
                    this.repository.Update(item);

                }
                this.repository.Save();
            }
            catch (Exception ex)
            {
                //LogHelper.Error(string.Format("UserService.Save: {0}", ex.Message));
                //throw new Exception(ex.Message);
            }
        }
        //public void SaveListMsg(List<SYS_TINNHAN> list_item)
        //{
        //    if (list_item != null && list_item.Count > 0)
        //    {
        //        foreach (var item in list_item)
        //        {
        //            this.Save(item);
        //        }
        //    }
        //}

        /// <summary>
        /// Lấy danh sách tin nhắn, thông báo của người dùng
        /// NAMDV
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="number"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public List<SYS_TINNHAN_BO> GetListTinNhan(long UserID, int number, int page)
        {
            var skip = 0;
            if (page > 1)
            {
                skip = (page - 1) * number;
            }
            var result = GetListTinNhan(UserID).Where(x => true != x.IS_READ).ToList();
            return result.Skip(skip).Take(number).ToList();
        }
        public int TinNhanCount(long UserID)
        {
            var result = GetListTinNhan(UserID).Where(x => x.IS_READ != true).Count();
            return result;
        }
        public List<SYS_TINNHAN_BO> GetListTinNhanChuaDoc(long UserID)
        {
            var result = GetListTinNhan(UserID).Where(x => x.IS_READ != true).ToList();
            return result;
        }
        public List<SYS_TINNHAN_BO> GetListTinNhan(long UserID)
        {
            var result = (from tin_nhan in this.context.SYS_TINNHAN.Where(x => x.TO_USER_ID == UserID).AsEnumerable()
                          join nguoi_gui in this.context.DM_NGUOIDUNG
                          on tin_nhan.FROM_USER_ID equals nguoi_gui.ID
                          into group1
                          from g1 in group1.DefaultIfEmpty()
                          join nguoinhan in this.context.DM_NGUOIDUNG
                          on tin_nhan.TO_USER_ID equals nguoinhan.ID
                          into grouyp2
                          from g2 in grouyp2.DefaultIfEmpty()
                          orderby tin_nhan.ID descending
                          select new SYS_TINNHAN_BO()
                          {
                              ID = tin_nhan.ID,
                              TIEUDE = tin_nhan.TIEUDE,
                              URL = tin_nhan.URL,
                              NGAYTAO = tin_nhan.NGAYTAO,
                              TEN_NGUOINHAN = g2.HOTEN,
                              IS_READ = true == tin_nhan.IS_READ,
                              TEN_NGUOIGUI = g1.HOTEN,
                              NOIDUNG = tin_nhan.NOIDUNG
                          }).ToList();
            return result;
        }

        /// <summary>
        /// @description: lưu tin nhắn vào database
        /// @author: duynn
        /// @since: 21/05/2018
        /// </summary>
        /// <param name="sender">người gửi</param>
        /// <param name="recipientIds">mã người nhận</param>
        /// <param name="title">tiêu đề</param>
        /// <param name="content">nội dung</param>
        /// <param name="path">đường link thông báo</param>
        public void SaveMessageToDb(UserInfoBO sender, List<long> recipientIds,long objectId,int objectType ,string title, string content, string path)
        {
            try
            {
                if (recipientIds != null && recipientIds.Count > 0)
                {
                    foreach (var recipientId in recipientIds)
                    {
                        DM_NGUOIDUNG recipient = this.context.DM_NGUOIDUNG.Find(recipientId);

                        if (recipient != null)
                        {
                            SYS_TINNHAN message = new SYS_TINNHAN();
                            message.TIEUDE = title;
                            message.NOIDUNG = content;
                            message.URL = path;
                            message.TO_USER_ID = recipientId;
                            message.TO_USERNAME = recipient.HOTEN;
                            message.FROM_USER_ID = (long)sender.ID;
                            message.FROM_USERNAME = sender.TENDANGNHAP;
                            message.IS_READ = false;
                            message.NGAYTAO = DateTime.Now;
                            message.NGUOITAO = sender.ID;
                            message.NOTIFY_ITEM_ID = objectId;
                            message.NOTIFY_ITEM_TYPE = objectType;
                            this.repository.Insert(message);
                            this.repository.Save();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }




        public void sendMessageMultipleUsers(List<long> listUserIds, UserInfoBO currentUser,
            string TIEUDETHONGBAO, string NOIDUNGTHONGBAO, string URLTHONGBAO, string targetScreen
            , bool isTaskNotification, long objId, int docType)
        {
            var ListToken = (from token in this.context.QL_TOKEN
                             join user in this.context.DM_NGUOIDUNG
                             on token.DM_NGUOIDUNG_ID equals user.ID
                             into group1
                             from g1 in group1.DefaultIfEmpty()
                             where token.DM_NGUOIDUNG_ID.HasValue &&
                             listUserIds.Contains(token.DM_NGUOIDUNG_ID.Value) && true == token.IS_ACTIVE
                             select new QL_TOKEN_BO
                             {
                                 DM_NGUOIDUNG_ID = token.DM_NGUOIDUNG_ID,
                                 HOTEN = g1.HOTEN,
                                 ID = token.ID,
                                 IS_ACTIVE = token.IS_ACTIVE,
                                 NGAYTAO = token.NGAYTAO,
                                 TOKEN = token.TOKEN,
                                 TEN_DANGNHAP = g1.TENDANGNHAP
                             }).ToList();
            QL_TOKEN_BO Token = null;
            var FCM_NOTIFICATIONBusiness = new FCM_NOTIFICATIONBusiness(new UnitOfWork());
            //NotifCommon notifCommon = new NotifCommon(FcmConstant.ServerKey, FcmConstant.SenderId);
            foreach (long userId in listUserIds)
            {
                Token = ListToken.Where(x => userId == x.DM_NGUOIDUNG_ID).FirstOrDefault();
                SYS_TINNHAN TinNhan = new SYS_TINNHAN();
                TinNhan.TIEUDE = TIEUDETHONGBAO;
                TinNhan.NOIDUNG = NOIDUNGTHONGBAO;
                TinNhan.URL = URLTHONGBAO;
                TinNhan.TO_USER_ID = userId;
                TinNhan.TO_USERNAME = Token != null ? Token.TEN_DANGNHAP : "";
                TinNhan.FROM_USER_ID = (long)currentUser.ID;
                TinNhan.FROM_USERNAME = currentUser.TENDANGNHAP;
                TinNhan.IS_READ = false;
                TinNhan.NGAYTAO = DateTime.Now;
                TinNhan.NGUOITAO = currentUser.ID;
                TinNhan.NOTIFY_ITEM_ID = objId;
                TinNhan.NOTIFY_ITEM_TYPE = isTaskNotification ? THONGBAO_CONSTANT.CONGVIEC : THONGBAO_CONSTANT.VANBAN;
                this.Save(TinNhan, targetScreen, isTaskNotification, objId, docType);
            }
        }

        public void sendMessageUsers(long toUser, long currentUserID, string TIEUDETHONGBAO, string NOIDUNGTHONGBAO, string URLTHONGBAO)
        {
            var userfrom = this.context.DM_NGUOIDUNG.Find(currentUserID);
            var userto = this.context.DM_NGUOIDUNG.Find(toUser);
            SYS_TINNHAN TinNhan = new SYS_TINNHAN();
            TinNhan.TIEUDE = TIEUDETHONGBAO;
            TinNhan.NOIDUNG = NOIDUNGTHONGBAO;
            TinNhan.URL = URLTHONGBAO;
            TinNhan.TO_USER_ID = userto.ID;
            TinNhan.TO_USERNAME = userto.HOTEN;
            TinNhan.FROM_USER_ID = userfrom.ID;
            TinNhan.FROM_USERNAME = userfrom.HOTEN;
            TinNhan.IS_READ = false;
            TinNhan.NGAYTAO = DateTime.Now;
            TinNhan.NGUOITAO = userfrom.ID;
            string fts = TIEUDETHONGBAO + " " + userfrom;
            this.Save(TinNhan);
        }

        /// <summary>
        /// @description: cập nhật tin nhắn chưa đọc (dành cho phần công việc và văn bản)
        /// @author: duynn
        /// @since: 16/07/2018
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool UpdateUnReadMessagesOfUser(long userId)
        {
            try
            {
                List<SYS_TINNHAN> messages = this.repository.All().Where(x => x.IS_READ != true && x.TO_USER_ID == userId
                && (x.NOTIFY_ITEM_TYPE == THONGBAO_CONSTANT.CONGVIEC || x.NOTIFY_ITEM_TYPE == THONGBAO_CONSTANT.VANBAN)).ToList();
                foreach (var item in messages)
                {
                    item.IS_READ = true;
                    this.repository.Update(item);
                }
                this.repository.Save();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// @description: số lượng tin nhắn chưa đọc của người dùng (dành cho phần công việc và văn bản)
        /// @author: duynn
        /// @since: 16/07/2018
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int GetUnReadMessageNumberOfUser(long userId)
        {
            int result = this.repository.All().Where(x => x.TO_USER_ID == userId && x.IS_READ != true
            && (x.NOTIFY_ITEM_TYPE == THONGBAO_CONSTANT.CONGVIEC || x.NOTIFY_ITEM_TYPE == THONGBAO_CONSTANT.VANBAN)).Count();
            return result;
        }

        /// <summary>
        /// @description: cập nhật trạng thái đã đọc tin nhắn (khi người dùng trên mobile nhấn vào thông báo)
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public bool UpdateReadStateOfMessage(int itemType, long itemId)
        {
            SYS_TINNHAN message = this.repository.All().Where(x => x.NOTIFY_ITEM_TYPE == itemType && x.NOTIFY_ITEM_ID == itemId).FirstOrDefault();
            if(message != null)
            {
                if(message.IS_READ != true)
                {
                    message.IS_READ = true;
                    this.repository.Update(message);
                    this.Save();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// @description: danh sách thông báo cho người dùng (dành cho phần công việc và văn bản)
        /// @author: duynn
        /// @since: 16/07/2018
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<SYS_TINNHAN> GetMessagesOfUser(long userId, int pageSize, int pageIndex, string query)
        {
            IQueryable<SYS_TINNHAN> queryResult = this.context.SYS_TINNHAN.Where(x => x.TO_USER_ID == userId
            && (x.NOTIFY_ITEM_TYPE == THONGBAO_CONSTANT.CONGVIEC || x.NOTIFY_ITEM_TYPE == THONGBAO_CONSTANT.VANBAN));
            if (!string.IsNullOrEmpty(query))
            {
                query = query.Trim().ToLower();
                queryResult = queryResult.Where(x => !string.IsNullOrEmpty(x.FROM_USERNAME) && x.FROM_USERNAME.Trim().ToLower().Contains(query));
            }
            List<SYS_TINNHAN> result = queryResult.OrderByDescending(x => x.ID).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return result;
        }

        public PageListResultBO<SYS_TINNHAN_BO> GetDaTaByPage(SYS_TINNHAN_SEARCH searchModel, int pageSize = 20, int pageIndex = 1)
        {
            var query = from mes in this.context.SYS_TINNHAN
                        join nguoidung in this.context.DM_NGUOIDUNG
                        on mes.NGUOITAO equals nguoidung.ID
                        into group1
                        from g1 in group1.DefaultIfEmpty()
                        join nguoinhan in this.context.DM_NGUOIDUNG
                        on mes.TO_USER_ID equals nguoinhan.ID
                        into group2
                        from g2 in group1.DefaultIfEmpty()
                        where searchModel.USER_ID == mes.TO_USER_ID
                        select new SYS_TINNHAN_BO
                        {
                            FROM_USERNAME = mes.FROM_USERNAME,
                            FROM_USER_ID = mes.FROM_USER_ID,
                            ID = mes.ID,
                            IS_READ = mes.IS_READ,
                            NGAYTAO = mes.NGAYTAO,
                            NGUOITAO = mes.NGUOITAO,
                            NOIDUNG = mes.NOIDUNG,
                            TEN_NGUOIGUI = g1.HOTEN,
                            TEN_NGUOINHAN = g2.HOTEN,
                            TIEUDE = mes.TIEUDE,
                            TO_USERNAME = mes.TO_USERNAME,
                            TO_USER_ID = mes.TO_USER_ID,
                            URL = mes.URL
                        };
            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.sortQuery))
                {
                    query = query.OrderBy(searchModel.sortQuery);
                }
                else
                {
                    query = query.OrderByDescending(x => x.NGAYTAO);
                }
                if (!string.IsNullOrEmpty(searchModel.TIEUDE))
                {
                    query = query.Where(x => x.TIEUDE.ToLower().Trim().Contains(searchModel.TIEUDE.ToLower()));
                }
                if (searchModel.TUNGAY.HasValue)
                {
                    query = query.Where(x => x.NGAYTAO >= searchModel.TUNGAY);
                }
                if (searchModel.DENNGAY.HasValue)
                {
                    query = query.Where(x => x.NGAYTAO <= searchModel.DENNGAY);
                }
                if (searchModel.TRANGTHAI.HasValue)
                {
                    query = query.Where(x => searchModel.TRANGTHAI == x.IS_READ);
                }
            }
            else
            {
                query = query.OrderByDescending(x => x.NGAYTAO);
            }
            var resultmodel = new PageListResultBO<SYS_TINNHAN_BO>();
            var dataPageList = query.ToPagedList(pageIndex, pageSize);
            resultmodel.Count = dataPageList.TotalItemCount;
            resultmodel.TotalPage = dataPageList.PageCount;
            resultmodel.ListItem = dataPageList.ToList();
            return resultmodel;
        }
        public SYS_TINNHAN_BO GetInfoBO(long id)
        {
            var query = from mes in this.context.SYS_TINNHAN
                        join nguoidung in this.context.DM_NGUOIDUNG
                        on mes.NGUOITAO equals nguoidung.ID
                        into group1
                        from g1 in group1.DefaultIfEmpty()
                        join nguoinhan in this.context.DM_NGUOIDUNG
                        on mes.TO_USER_ID equals nguoinhan.ID
                        into group2
                        from g2 in group1.DefaultIfEmpty()
                        where mes.ID == id
                        select new SYS_TINNHAN_BO
                        {
                            FROM_USERNAME = mes.FROM_USERNAME,
                            FROM_USER_ID = mes.FROM_USER_ID,
                            ID = mes.ID,
                            IS_READ = mes.IS_READ,
                            NGAYTAO = mes.NGAYTAO,
                            NGUOITAO = mes.NGUOITAO,
                            NOIDUNG = mes.NOIDUNG,
                            TEN_NGUOIGUI = g1.HOTEN,
                            TEN_NGUOINHAN = g2.HOTEN,
                            TIEUDE = mes.TIEUDE,
                            TO_USERNAME = mes.TO_USERNAME,
                            TO_USER_ID = mes.TO_USER_ID,
                            URL = mes.URL
                        };
            return query.FirstOrDefault();
        }

        public PageListResultBO<SYS_TINNHAN> GetDaTaByPage(long userId, bool? ISREAD, int pageSize = 20, int pageIndex = 1)
        {
            var query = from mes in this.context.SYS_TINNHAN
                        where userId == mes.TO_USER_ID
                        && (ISREAD.HasValue ? ISREAD.Value != mes.IS_READ : true)
                        select mes;
            query = query.OrderByDescending(x => x.NGAYTAO);
            var resultmodel = new PageListResultBO<SYS_TINNHAN>();
            var dataPageList = query.ToPagedList(pageIndex, pageSize);
            resultmodel.Count = dataPageList.TotalItemCount;
            resultmodel.TotalPage = dataPageList.PageCount;
            resultmodel.ListItem = dataPageList.ToList();
            return resultmodel;
        }
        void SocketMessage(object sender, MessageEventArgs e)
        {
        }

        void SocketOpened(object sender, EventArgs e)
        {

        }
        void SocketError(object sender, SocketIOClient.ErrorEventArgs e)
        {
            Console.WriteLine("socket client error:");
            Console.WriteLine(e.Message);
        }

        void SocketConnectionClosed(object sender, EventArgs e)
        {
            Console.WriteLine("WebSocketConnection was terminated!");
        }
    }
}
