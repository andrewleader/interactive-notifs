using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace InteractiveNotifs.Hub.Helpers
{
    public static class PushNotificationsWNS
    {
        /// <summary>
        /// The notification was accepted by WNS.
        /// </summary>
        public static readonly HttpStatusCode RESPONSE_OK = HttpStatusCode.OK;

        /// <summary>
        /// One or more headers were specified incorrectly or conflict with another header. Log the details of your request. Inspect your request and compare against this documentation.
        /// </summary>
        public static readonly HttpStatusCode RESPONSE_BAD_REQUEST = HttpStatusCode.BadRequest;

        /// <summary>
        /// The cloud service did not present a valid authentication ticket. The OAuth ticket may be invalid. Request a valid access token by authenticating your cloud service using the access token request.
        /// </summary>
        public static readonly HttpStatusCode RESPONSE_UNAUTHORIZED = HttpStatusCode.Unauthorized;

        /// <summary>
        /// The cloud service is not authorized to send a notification to this URI even though they are authenticated. The access token provided in the request does not match the credentials of the app that requested the channel URI. Ensure that your package name in your app's manifest matches the cloud service credentials given to your app in the Dashboard.
        /// </summary>
        public static readonly HttpStatusCode RESPONSE_FORBIDDEN = HttpStatusCode.Forbidden;

        /// <summary>
        /// The channel URI is not valid or is not recognized by WNS. Log the details of your request. Do not send further notifications to this channel; notifications to this address will fail.
        /// </summary>
        public static readonly HttpStatusCode RESPONSE_NOT_FOUND = HttpStatusCode.NotFound;

        /// <summary>
        /// Invalid method (GET, CREATE); only POST (Windows or Windows Phone) or DELETE (Windows Phone only) is allowed. Log the details of your request. Switch to using HTTP POST.
        /// </summary>
        public static readonly HttpStatusCode RESPONSE_METHOD_NOT_ALLOWED = HttpStatusCode.MethodNotAllowed;

        /// <summary>
        /// The cloud service exceeded its throttle limit. Log the details of your request. Reduce the rate at which you are sending notifications.
        /// </summary>
        public static readonly HttpStatusCode RESPONSE_NOT_ACCEPTABLE = HttpStatusCode.NotAcceptable;

        /// <summary>
        /// The channel expired. Log the details of your request. Do not send further notifications to this channel. Have your app request a new channel URI.
        /// </summary>
        public static readonly HttpStatusCode RESPONSE_GONE = HttpStatusCode.Gone;

        /// <summary>
        /// The notification payload exceeds the 5000 byte size limit. Log the details of your request. Inspect the payload to ensure it is within the size limitations.
        /// </summary>
        public static readonly HttpStatusCode RESPONSE_REQUEST_ENTITY_TOO_LARGE = HttpStatusCode.RequestEntityTooLarge;

        /// <summary>
        /// An internal failure caused notification delivery to fail. Log the details of your request. Report this issue through the developer forums.
        /// </summary>
        public static readonly HttpStatusCode RESPONSE_INTERNAL_SERVER_ERROR = HttpStatusCode.InternalServerError;

        /// <summary>
        /// The server is currently unavailable. Log the details of your request. Report this issue through the developer forums.
        /// </summary>
        public static readonly HttpStatusCode RESPONSE_SERVICE_UNAVAILABLE = HttpStatusCode.ServiceUnavailable;

        public enum NotificationType
        {
            Toast,
            Tile,
            Raw,
            Badge
        }

        public static HttpStatusCode Push(string body, string pushChannel, string secret, string sid, NotificationType notificationType)
        {
            return push(body, pushChannel, secret, sid, notificationType, 0);
        }

        private static HttpStatusCode push(string xml, string pushChannel, string secret, string sid, NotificationType notificationType, int attempt = 0)
        {
            if (attempt > 1)
                return HttpStatusCode.ServiceUnavailable;

            string accessToken = getCachedAccessToken(secret, sid);

            if (accessToken != null)
            {
                try
                {
                    byte[] contentInBytes = Encoding.UTF8.GetBytes(xml);

                    HttpWebRequest req = HttpWebRequest.Create(pushChannel) as HttpWebRequest;
                    req.Method = "POST";
                    req.Headers.Add("X-WNS-Type", getNotificationTypeString(notificationType));
                    req.ContentType = getContentType(notificationType);
                    req.Headers.Add("Authorization", "Bearer " + accessToken);

                    using (Stream requestStream = req.GetRequestStream())
                    {
                        requestStream.Write(contentInBytes, 0, contentInBytes.Length);
                    }

                    using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
                    {
                        return resp.StatusCode;
                    }
                }

                catch (WebException ex)
                {
                    try
                    {
                        HttpStatusCode code = ((HttpWebResponse)ex.Response).StatusCode;

                        //access token has expired
                        if (code == RESPONSE_UNAUTHORIZED)
                        {
                            //fall through, letting it refresh access token
                        }

                        else
                            return code;
                    }

                    catch { return HttpStatusCode.SeeOther; }
                }
            }

            //if access token didn't exist, or it was expired, this happens
            refreshAccessToken(secret, sid);
            return push(xml, pushChannel, secret, sid, notificationType, attempt + 1);
        }

        private static string getContentType(NotificationType type)
        {
            switch (type)
            {
                case NotificationType.Tile:
                case NotificationType.Toast:
                case NotificationType.Badge:
                    return "text/xml";

                case NotificationType.Raw:
                    return "application/octet-stream";

                default:
                    throw new NotImplementedException();
            }
        }

        private static string getNotificationTypeString(NotificationType type)
        {
            switch (type)
            {
                case NotificationType.Tile:
                    return "wns/tile";

                case NotificationType.Toast:
                    return "wns/toast";

                case NotificationType.Badge:
                    return "wns/badge";

                case NotificationType.Raw:
                    return "wns/raw";

                default:
                    throw new NotImplementedException();
            }
        }

        public static string getCachedAccessToken(string secret, string sid)
        {
            lock (_lock)
            {
                string accessToken;

                cachedAccessTokens.TryGetValue(secret + sid, out accessToken);

                return accessToken;
            }
        }

        private static Dictionary<string, string> cachedAccessTokens = new Dictionary<string, string>();

        [DataContract]
        public class OAuthToken
        {
            [DataMember(Name = "access_token")]
            public string AccessToken { get; set; }
            [DataMember(Name = "token_type")]
            public string TokenType { get; set; }
        }

        private static object _lock = new object();
        public static void refreshAccessToken(string secret, string sid)
        {
            lock (_lock)
            {
                string urlEncodedSecret = HttpUtility.UrlEncode(secret);
                string urlEncodedSid = HttpUtility.UrlEncode(sid);

                string body = String.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=notify.windows.com", urlEncodedSid, urlEncodedSecret);

                string accessToken;
                using (WebClient wc = new WebClient())
                {
                    wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    string response = wc.UploadString("https://login.live.com/accesstoken.srf", body);

                    using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(response)))
                    {
                        var ser = new DataContractJsonSerializer(typeof(OAuthToken));
                        var oAuthToken = (OAuthToken)ser.ReadObject(ms);
                        accessToken = oAuthToken.AccessToken;
                    }
                }

                cachedAccessTokens[secret + sid] = accessToken;
            }
        }
    }
}
