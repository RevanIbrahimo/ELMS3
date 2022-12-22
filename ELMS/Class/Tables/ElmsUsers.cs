using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELMS.Class.Tables
{
    public class ElmsUsers
    {
        public int ID { get; set; }       
        public string SURNAME { get; set; }
        public string NAME { get; set; }
        public string PATRONYMIC { get; set; }
        public string FULL_NAME { get; set; }
        public string LOGIN_NAME { get; set; }
        public string PASSWORD { get; set; }
        public int STATUS_ID { get; set; }
        public string SEX_NAME { get; set; }
        public int SESSION_ID { get; set; }
        public int GROUP_ID { get; set; }        
        public DateTime BIRTHDAY { get; set; }
        public string NOTE { get; set; }
        public int USED_USER_ID { get; set; }
    }
}
