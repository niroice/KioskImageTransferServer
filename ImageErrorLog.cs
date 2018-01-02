using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace listenerTest
{
    public static class ImageErrorLog
    {
        internal static List<ImageError> errorLog;


        //public static void AddImageError(ImageError error)
        //{
        //    errorLog.Add(error);
        //}

        public static void ClearErrorList()
        {
            errorLog.Clear();
        }

    }
}
