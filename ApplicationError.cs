using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace listenerTest
{
    public enum ErrorLocationEnum { Mobile, Server } // location of the error - app or serve application

    class ApplicationError
    {
        public ErrorLocationEnum ErrorLocation { get; } // ...
        public string ClassError { get; } // name of class where the error occured
        public string MethodError { get; } // name of the methods where error occured
        public string TypeOfError { get; } // error type 
        public string Description { get; } // longer description of error

        public ApplicationError(  ErrorLocationEnum errorLocation, string classError, string methodError, 
            string typeOfError, string description )
        {
            this.ErrorLocation = errorLocation;
            this.ClassError = classError;
            this.MethodError = methodError;
            this.TypeOfError = typeOfError;
            this.Description = description;

            writeErrorConsole();



        }

        public void writeErrorConsole()
        {
            Console.WriteLine("ERROR -" + "ORIGIN: " + ErrorLocation + " - " + "CLASS: " + ClassError + " - " +
                 "METHOD: " + MethodError);
            Console.WriteLine("ERROR TYPE: " + TypeOfError);
            Console.WriteLine("DESCRIPTION: " + Description);
        }
    }
}
