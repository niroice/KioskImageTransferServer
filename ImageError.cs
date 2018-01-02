using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace listenerTest
{
    class ImageError
    {
        public string Description { get; set;}
        public string ImageName { get; set; }
        public string KioskID { get; set; }
        private DateTime timeRecorded;

        public ImageError( string description, string imageName, string kioskID)
        {
            timeRecorded = DateTime.Now;
            Description = description;
            ImageName = imageName;
            KioskID = kioskID;
        }

    }
}
