using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonBusiness
{
    public class ImapSmtpMailServer
    {
        public int id { set; get; }
        public string name { set; get; }
        public string imapServer { set; get; }
        public string smtpServer { set; get; }
        public int? imapPort { set; get; }
        public int? smtpPort { set; get; }

        public ImapSmtpMailServer()
        {

        }
        public ImapSmtpMailServer(int id,string name, string imapServer,string smtpServer, int? imapPort, int? smtpPort)
        {
            this.id = id;
            this.name = name;
            this.imapServer = imapServer;
            this.smtpServer = smtpServer;
            this.imapPort = imapPort;
            this.smtpPort = smtpPort;
        }
    }
}
