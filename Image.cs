using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace listenerTest
{
    

    class Image
    {
        private string ImageURI;
        private string ImageName;
        private string KioskNetworkID;
        private const string DEFAULT_PATH = @"\\";
        private const string BLUETOOTH_FOLDER = @"\bluetooth\"; 
        //private const string BLUETOOTH_FOLDER = @"\ᛒluetooth\";

        public Image(string imageURI, string imageName, string kioskNetworkID)
        {
            // remove the header from the content by using split on comma - comma
            // is flag to separate header from body of the image uri
            string[] imageUriSplit = imageURI.Split(',');

            // check make sure image header was exists/separated, by checking that the array length is
            // greater then one
            if (imageUriSplit.Length > 1)
            {
                ImageURI = imageUriSplit[1]; // only grab the image body - second index
            }
            // if array size is one, no header was provided - grab first index
            else
            {
                ImageURI = imageUriSplit[0];
            }

            KioskNetworkID = kioskNetworkID;
            //KioskNetworkID = "desktop-LACHLAN";
            ImageName = imageName;
        }
        
        
        public Boolean SaveImageAsFile()
        {
            Byte[] imagebytes = null; 

            try
            {
               imagebytes = Convert.FromBase64String(ImageURI);
            }
            catch
            {

                ApplicationError error = new ApplicationError(
                ErrorLocationEnum.Server,
                "Image",
                "SaveImageAsFile",
                "Failed Coverting image uri to bytes",
                 ImageName + " - URI failed to convert to bytes");
                ApplicationErrorLog.logError(error);

                return false;
            }

            // if bytes is not null then try and write the image file
            if ( imagebytes != null)
            {
                string imageFullSavePath = DEFAULT_PATH + KioskNetworkID + BLUETOOTH_FOLDER + ImageName;

                try
                {
                    // write file to share folder using kiosk ID
                    using (var img = new FileStream(imageFullSavePath, FileMode.Create))
                    {
                    
                        img.Write(imagebytes, 0, imagebytes.Length);
                        img.Flush(); // clear write buffer so it does not effect next file creation

                        return true;
                    }
                   
                }
                catch
                {
                    ApplicationError error = new ApplicationError(
                    ErrorLocationEnum.Server,
                    "Image",
                    "SaveImageAsFile",
                    "Failed writing image file",
                    "Failed writing image file to path: " + imageFullSavePath);

                    return false;
                }
            }
            else
            {
                return false;
            }
   
        }
    }
}
