using Business.Business;
using Business.CommonBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Areas.ChatArea.Models;
using Web.Custom;
using Web.FwCore;
using Web.Models;

namespace Web.Areas.ChatArea.Controllers
{
    public class ChatController : BaseController
    {
        // GET: ChatArea/Chat
        private CHAT_NOIDUNGBusiness CHAT_NOIDUNGBusiness;

        public ActionResult Index()
        {
            UserInfoBO user = (UserInfoBO)SessionManager.GetUserInfo();
            ChatIndexViewModel model = new ChatIndexViewModel();
            CHAT_NOIDUNGBusiness = Get<CHAT_NOIDUNGBusiness>();
            model.ListChat = CHAT_NOIDUNGBusiness.GetListChatHistory(user.DeptParentID.Value, user.ID, user.TENDANGNHAP, user.HOTEN);
            return View(model);
        }

        public PartialViewResult ChatContent(string fromUser, string toUser, string fromFullName, string toFullName, string chatId)
        {
            CHAT_NOIDUNGBusiness = Get<CHAT_NOIDUNGBusiness>();
            UserInfoBO user = (UserInfoBO)SessionManager.GetUserInfo();
            ChatViewModel model = new ChatViewModel();
            model.cosoId = user.DeptParentID.Value;
            model.fromUser = fromUser;
            model.toUser = toUser;
            model.fromFullName = fromFullName;
            model.toFullName = toFullName;
            model.currentUserName = user.TENDANGNHAP;
            model.listChat = CHAT_NOIDUNGBusiness.GetListChat(fromUser, toUser, DateTime.Now, 0, 15, 0);
            model.chatPanel_id = chatId;
            return PartialView("_ChatContent", model);
        }

        public PartialViewResult GetMoreChatContent(string fromUser, string toUser, string fromFullName, string toFullName, string chatId, int maxItem)
        {
            CHAT_NOIDUNGBusiness = Get<CHAT_NOIDUNGBusiness>();
            UserInfoBO user = (UserInfoBO)SessionManager.GetUserInfo();
            ChatViewModel model = new ChatViewModel();
            model.cosoId = user.DeptParentID.Value;
            model.fromUser = fromUser;
            model.toUser = toUser;
            model.fromFullName = fromFullName;
            model.toFullName = toFullName;
            model.currentUserName = user.TENDANGNHAP;
            model.listChat = CHAT_NOIDUNGBusiness.GetListChat(fromUser, toUser, DateTime.Now, 0, 15, maxItem);
            model.chatPanel_id = chatId;
            return PartialView("_GetMoreChatContent", model);
        }
    }
}