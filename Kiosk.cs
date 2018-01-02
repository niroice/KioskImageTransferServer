using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace listenerTest
{
    class Kiosk
    {
        public string KioskName { get; }
        public string IPaddress { get; }

        public Kiosk(string kioskName, string IPaddress)
        {

        }
        
        private Boolean checkVaildIPaddress(string ipAddress)
        {
            return false;
        }
    }
}
