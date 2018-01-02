using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.DirectoryServices;
using System.Xml;

namespace listenerTest
{
    class Kiosks
    {
        //private  DirectoryEntry Root;
        private  const string WIN_NT = "WinNT://";
        private  const string KIOSK_IDENTIFIER = "ik";
        private  List<string> KiosksHostNames;
        private const string KIOSK_SETTING_FILENAME = "Settings/kioskSettings.xml";
        private const string AVAILABLE_KIOSKS_NODE_LOCATION = "/availableKiosks/kioskHostName";
        private XmlDocument kioskSettingsXml;


        public Kiosks()
        {
            //Root = new DirectoryEntry(WIN_NT);
            KiosksHostNames = new List<string>();

            // load kiosk setting file
            loadKiosksSettingFile();
        }


        public void addKiosk(string kioskHostName)
        {
            KiosksHostNames.Add(kioskHostName);
        }

        public void removeKiosk(string kioskHostName)
        {
            KiosksHostNames.Remove(kioskHostName);
        }


        public List<string> getKiosksHostNames()
        {
            return KiosksHostNames;
        }

        private void loadKiosksSettingFile()
        {
            kioskSettingsXml = new XmlDocument();
            kioskSettingsXml.Load(KIOSK_SETTING_FILENAME);
            
            foreach (XmlNode node in kioskSettingsXml.SelectNodes(AVAILABLE_KIOSKS_NODE_LOCATION))
            {
                KiosksHostNames.Add(node.InnerText);
            }
        }


        private void updateKioskDataFile()
        {
           


        }

        /// <summary>
        /// Searchs the local private network for photo kiosk with identifiers that contain
        /// 'ik' and stores them in KiosksIdentifiers list.
        /// </summary>
        public List<string> findAllKioskOnNetwork()
        {

            using (DirectoryEntry Root = new DirectoryEntry(WIN_NT))
            {
                foreach (DirectoryEntry computer in Root.Children)
                {
                    Console.WriteLine(computer.Name);
                    //foreach (DirectoryEntry computer in domain.Children)
                    //{
                    //    Console.WriteLine(computer.Name);
                    //    if (computer.Name.Contains(KIOSK_IDENTIFIER.ToLower()))
                    //    {
                    //        KiosksIdentifiers.Add(computer.Name);
                    //        Console.WriteLine(computer.Name);
                    //    }
                    //}

                }

                //return KiosksIdentifiers;
                return null;
            }

        }
    }
}
