using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
