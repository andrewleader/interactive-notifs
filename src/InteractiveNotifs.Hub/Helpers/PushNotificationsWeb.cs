using JWT.Algorithms;
using JWT.Builder;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebPush;

namespace InteractiveNotifs.Hub.Helpers
{
    public static class PushNotificationsWeb
    {
        private const string PublicKey = "BGg3UxXo3J_rH6VrJB2er_F8o7m2ZTGb2jiNm3tmlK4ORxsskX1HIVys5TA8lGkCYC-ur8GwrZMy-v0LZOwazvk";
        private const string PrivateKey = "_RwmE-l--jTgxtb8IQcL3cUiKRcjc5-a7SFdDgFL5nU";

        private static HttpClient _client = new HttpClient();
        private static WebPushClient _webPushClient = new WebPushClient();

        public class Subscription
        {
            public string Endpoint { get; set; }
            public SubscriptionKeys Keys { get; set; }
        }

        public class SubscriptionKeys
        {
            public string P256DH { get; set; }
            public string Auth { get; set; }
        }

        public static async Task SendAsync(string identifier, string payload)
        {
            var subscription = JsonConvert.DeserializeObject<Subscription>(identifier);
            await _webPushClient.SendNotificationAsync(
                subscription: new PushSubscription(
                    endpoint: subscription.Endpoint,
                    p256dh: subscription.Keys.P256DH,
                    auth: subscription.Keys.Auth),
                payload: payload,
                vapidDetails: new VapidDetails(
                    subject: "mailto:nothanks@microsoft.com",
                    publicKey: PublicKey,
                    privateKey: PrivateKey));
            return;


            //var jwt = new JwtBuilder()
            //    .WithAlgorithm(new HMACSHA256Algorithm())
            //    .WithSecret(PrivateKey)
            //    .Audience("https://interactivenotifs.azurewebsites.net")
            //    .ExpirationTime(DateTime.UtcNow.AddMinutes(5))
            //    .Subject("mailto:nothanks@microsoft.com")
            //    .Build();

            //JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            //try
            //{
            //    var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(PrivateKey)), SecurityAlgorithms.EcdsaSha256);
            //    var token = handler.CreateJwtSecurityToken(new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor()
            //    {
            //        Audience = "https://interactivenotifs.azurewebsites.net",
            //        Expires = DateTime.UtcNow.AddMinutes(5),
            //        Claims = new Dictionary<string, object>()
            //        {
            //            { "sub", "mailto:nothanks@microsoft.com" }
            //        },
            //        SigningCredentials = signingCredentials
            //    });

            //    string jwt = handler.WriteToken(token);

            //    using (var req = new HttpRequestMessage(HttpMethod.Post, endpoint))
            //    {
            //        req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("WebPush", jwt);
            //        req.Headers.Add("Crypto-Key", $"p256ecdsa=${PublicKey}");

            //        req.Content = new StringContent(payload);

            //        using (var resp = await _client.SendAsync(req))
            //        {
            //            resp.EnsureSuccessStatusCode();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }
    }
}
