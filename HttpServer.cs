using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace listenerTest
{
    class HttpSever
    {
        private HttpListener listener;
        private string[] ImagesToCreate; //
        private const int NUMBER_REQUESTS_ALLOWED = 1000;
        private const string IMAGE_SUCCESS_MSG = "image-sent";
        private const string IMAGE_FAILED_MSG = "image-failed";
        private const string ACTION_SENDING_IMAGE = "image-upload";
        private const string ACTION_REQUESTING_KIOSKS = "get-kiosks-available";
        private const string ACTION_REQUEST_FOUND_SERVER = "found-transpix-server";

        private const string KIOSK_KEY = "kioskNumber";
        private const string IMAGE_NAME_KEY = "imageName";
        private const string ACTION_KEY = "action";
        private const string IMAGE_URI_KEY = "imageUri";
        private const string SEVER_ADDRESS = "http://+:80/imagetransfer/";

        private const string ERROR_NO_IMAGE_KEY = "no-image-key-provided";
        private const string ERROR_IMAGE_URI = "no-image-uri-key-provided";
        private const string ERROR_KIOSK_NUMBER = "no-kiosk-identification-provided";
        private const string ERROR_ACTION_KEY = "no-action-key-provided";
        //private IAsyncResult request;

        private Kiosks Kiosks;

        public HttpSever(Kiosks kiosks)
        {
            Kiosks = kiosks;
            StartListeningForImages();

            ImagesToCreate = new string[NUMBER_REQUESTS_ALLOWED]; // number of requested loaded at once
        }


        private Task StartListeningForImages()
        {
            
            listener = new HttpListener();
            listener.Prefixes.Add(SEVER_ADDRESS);
            listener.Start();

            while (true)
            {
                IAsyncResult request;
                Console.WriteLine("Waiting for request..");
                request = listener.BeginGetContext(new AsyncCallback(ProccessRequest), listener);
                request.AsyncWaitHandle.WaitOne();
            }
        }



        // processes the clients request
        private void ProccessRequest(IAsyncResult result)
        {
            Dictionary<string, string> clientRequest;
            List<string> responseData = new List<string>();
            HttpListenerContext context;

            HttpListener httpListener = (HttpListener)result.AsyncState;
            context = httpListener.EndGetContext(result);

            // convert body request (JSON object) to dictionary
            clientRequest = ConvertJsonRequestToDictionary(context.Request);

            // if client request is not null, continue to process the request
            if (clientRequest != null)
            {
                string actionKeyValue;

                // try and get value from action key and check action required
                if (clientRequest.TryGetValue(ACTION_KEY, out actionKeyValue))
                {

                    if (actionKeyValue == ACTION_REQUESTING_KIOSKS)
                    {

                        ProccessAvailableKiosksRequest(context.Response);
                    }
                    else if (actionKeyValue == ACTION_SENDING_IMAGE)
                    {
                        // proccess image request type and send response
                        ProcessImageUploadRequest(clientRequest, context.Response);
                    }
                    else if (actionKeyValue == ACTION_REQUEST_FOUND_SERVER)
                    {
                        SendResponse(context.Response, true, RequestType.FoundServer, null);
                    }
                    else
                    {
                        SendResponse(context.Response, true, RequestType.FoundServer, null);
                    }
                }
                // if no action key provided by mobile app log error
                else
                {
                    ApplicationError error = new ApplicationError(
                    ErrorLocationEnum.Mobile,
                    "HttpServer",
                    "DetermainRequestAction",
                    "No action Key in json",
                    "Client failed to provided 'action' key");
                    ApplicationErrorLog.logError(error);

                    // send response - invaild request
                    // send response error
                    responseData.Add(ERROR_ACTION_KEY);
                    SendResponse(context.Response, false, RequestType.ImageUpload, responseData);

                }
            }
            else
            {
                Console.WriteLine();
            }
        }

        private Dictionary<string, string> ConvertJsonRequestToDictionary(HttpListenerRequest request)
        {
            string json;
            Dictionary<string, string> clientRequest;

            // try to retrieve JSON object from body of the request
            // if error occurs return null
            try
            {
                // access body (POST) of the message using stream
                using (Stream body = request.InputStream)
                {
                    using (StreamReader reader = new StreamReader(body, request.ContentEncoding))
                    {
                        json = reader.ReadToEnd();
                        clientRequest = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                        Console.WriteLine("Json successfully loaded from body");
                        return clientRequest;
                    }
                }
            }
            catch
            {
                ApplicationError error = new ApplicationError(
                    ErrorLocationEnum.Mobile,
                    "HttpServer",
                    "ConvertJsonRequestToDictionary",
                    "Error - Converting POST to json object, check json is correctly formated.",
                    "");
                ApplicationErrorLog.logError(error);
                return null;
            }
           
        }


        private void ProcessImageUploadRequest( Dictionary<string, string> clientRequest, HttpListenerResponse responseObj)
        {
            string imageUriKeyValue;
            string kioskKeyValue;
            string imageFilenameKeyValue;
            Boolean imageUriFound = false;
            Boolean imageNameFound = false;
            Boolean kioskNumberFound = false;
            Boolean imageCreated;
            List<string> responseData = new List<string>();

            // check request has image uri
            if (clientRequest.TryGetValue(IMAGE_URI_KEY, out imageUriKeyValue))
            {
                imageUriFound = true;
            }
            else
            {
                ApplicationError error = new ApplicationError(
               ErrorLocationEnum.Mobile,
               "HttpServer",
               "DetermainRequestAction",
               "No '" + IMAGE_URI_KEY + "' key in json",
               "Client failed to provid '" + IMAGE_URI_KEY + "' key");
                ApplicationErrorLog.logError(error);

                // add error to response data
                responseData.Add(ERROR_IMAGE_URI);
            }

            // check request has kiosk number key
            if (clientRequest.TryGetValue(KIOSK_KEY, out kioskKeyValue))
            {
                kioskNumberFound = true;
            }
            else
            {
                ApplicationError error = new ApplicationError(
                ErrorLocationEnum.Mobile,
                "HttpServer",
                "DetermainRequestAction",
                "No '" + KIOSK_KEY + "' key in json",
                "Client failed to provid '" + KIOSK_KEY + "' key");
                ApplicationErrorLog.logError(error);

                // add error to response data
                responseData.Add(ERROR_KIOSK_NUMBER);
            }

            // check request has image name
            if (clientRequest.TryGetValue(IMAGE_NAME_KEY, out imageFilenameKeyValue))
            {
                imageNameFound = true;
            }
            else
            {
                ApplicationError error = new ApplicationError(
                ErrorLocationEnum.Mobile,
                "HttpServer",
                "DetermainRequestAction",
                "No '" + IMAGE_NAME_KEY + "' key in json",
                "Client failed to provid '" + IMAGE_NAME_KEY + "' key");
                ApplicationErrorLog.logError(error);

                // add error to response data
                responseData.Add(ERROR_NO_IMAGE_KEY);
            }
            
            // create image file if all keys where found - create image
            if ((imageUriFound == true) && (kioskNumberFound == true) && ( imageNameFound == true)){

                Image img = new Image(imageUriKeyValue, imageFilenameKeyValue, kioskKeyValue);
                imageCreated = img.SaveImageAsFile();

                // put failed image filename into firt index of the data array
                responseData.Add(imageFilenameKeyValue);

                if  (imageCreated == false)
                {
                    // send response to the client with fail to create image
                    SendResponse(responseObj, false, RequestType.ImageUpload, responseData);
                }
                // if created succcesfully, send success response
                else
                {
                    // send response to the client with success response
                    SendResponse(responseObj, true, RequestType.ImageUpload, responseData);
                }
            }
            // if an error occurred on any required keys - send error message/s back to client
            else
            {
                SendResponse(responseObj, false, RequestType.ImageUpload, responseData);
            }
        }


        private void ProccessAvailableKiosksRequest(HttpListenerResponse responseObj)
        {
            List<string> kiosksAvailable = Kiosks.getKiosksHostNames();

            // if kiosk reference has kiosk available send response back containing them
            if ((Kiosks.getKiosksHostNames() != null) && (Kiosks.getKiosksHostNames().Count > 0))
            {
                SendResponse(responseObj, true, RequestType.KiosksAvailable, kiosksAvailable);
            }
            // if there are no kiosk available send a response error no kiosk found
            else
            {
                SendResponse(responseObj, false, RequestType.KiosksAvailable, kiosksAvailable);
            }
        }


        // send response to the user to confirm file recieved
        private void SendResponse(HttpListenerResponse response, Boolean success, RequestType requestType, List<string> data)
        {
            string json;
            HttpResponse responseMessage = new HttpResponse(success, requestType, data);

            // convert response message object into json string to send in the response
            json = JsonConvert.SerializeObject(responseMessage, Formatting.Indented);

            response.ContentLength64 = Encoding.UTF8.GetByteCount(json); // response message
            response.StatusCode = (int)HttpStatusCode.OK; // setting response status code to ok

            using ( Stream stream = response.OutputStream){

                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(json);
                    Console.WriteLine(json);
                }
            }
        }
    }
}
