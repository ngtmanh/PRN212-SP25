using Prn212.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prn212.Resource
{
    class Resoure
    {
        public static void saveLog (int userId, String action, Prn212Sp25Context _context)
        {
            Log log = new Log();
            log.UserId = userId;
            log.Action = action;
            log.Timestamp = DateTime.Now;

            _context.Logs.Add(log);
            _context.SaveChanges();
        }
        public static int getUserId()
        {
            return 1;
        }
    }
}
