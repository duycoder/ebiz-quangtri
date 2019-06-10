using Model.Entities;
using System.Collections.Generic;

namespace Web.Models
{
    public class ChatViewModel
    {
        public string listFullName { get; set; }
        public string currentUserName { get; set; }
        public int cosoId { get; set; }
        public string fromUser { get; set; }
        public string toUser { get; set; }
        public string fromFullName { get; set; }
        public string toFullName { get; set; }
        public int soCuaSoChat { get; set; }
        public int reload { get; set; }
        public string chat_id { get; set; }
        public long group_id { get; set; }
        public string groupName { get; set; }
        public string groupChat_id { get; set; }
        public int index { get; set; }
        public List<CHAT_NOIDUNG> listChat { get; set; }
        public string chatPanel_id { get; set; }
    }
}