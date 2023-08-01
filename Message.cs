using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace message_client
{
    public class Message
    {
        public string senderNumber {  get; set; }
        public string username {  get; set; }
        public string targetNumber { get; set; }
        public string data { get; set; }
        public int type { get; set; }
        public int offset { get; set; }
        public int part { get; set; }
        public string fileName { get; set; }
        public string fileExtension { get; set; }

        public Message()
        {
            senderNumber = string.Empty;
            username = string.Empty;
            targetNumber = string.Empty;
            data = string.Empty;
            type = 0;
            offset = 0;
            part = -1;
            fileName = string.Empty;
            fileExtension = string.Empty;
        }
    }
}
