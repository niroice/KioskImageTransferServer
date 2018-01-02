using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace listenerTest
{
    class Program
    {
        private static Kiosks kiosks;
        private static HttpSever HttpServer;

        static void Main(string[] args)
        {

            kiosks = new Kiosks();
            //kiosks.findAllKioskOnNetwork();

            // create http server object to listen for request from
            // the transpix clients
            HttpSever HttpServer = new HttpSever(kiosks);

            Console.ReadLine();
        }
    }
}
