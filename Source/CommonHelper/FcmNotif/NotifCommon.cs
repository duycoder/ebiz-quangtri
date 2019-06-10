using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace CommonHelper.FcmNotif
{
    public class NotifCommon
    {
        private string ServerKey { get; set; }
        private string SenderId { get; set; }
        public NotifCommon(string serverKey, string senderId)
        {
            ServerKey = serverKey;
            SenderId = senderId;
        }
        /// <summary>
        /// Send token
        /// </summary>
        /// <param name="to">token thiết bị</param>
        /// <param name="title">tiêu đề</param>
        /// <param name="body">Nội dung</param>
        /// <returns></returns>
        public bool NotifyAsync(string to, string title, string body)
        {
            try
            {
                var serverKey = string.Format("key={0}", ServerKey);
                var senderId = string.Format("id={0}", SenderId);
                dynamic notif = new
                {
                    to = to,
                    data = new
                    {
                        custom_notification = new
                        {
                            title = title,
                            body = body,
                            sound = "default",
                            priority = "high",
                            show_in_foreground = true,
                            targetScreen = "",
                            isTaskNotification = true,
                            targetDocId = "",
                            tagetDocType = "",
                            targetTaskId = 1,
                            targetTaskType = 1
                        }
                    },
                    priority = 10
                };
                var data = new
                {
                    to,
                    notification = new { title, body },
                    sound = "default"
                };
                var jsonBody = JsonConvert.SerializeObject(notif);
                using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://fcm.googleapis.com/fcm/send"))
                {
                    httpRequest.Headers.TryAddWithoutValidation("Authorization", serverKey);
                    httpRequest.Headers.TryAddWithoutValidation("Sender", senderId);
                    httpRequest.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                    using (var httpClient = new HttpClient())
                    {
                        var result = httpClient.SendAsync(httpRequest);
                        if (result != null)
                        {
                            var successCode = result.Result;
                            if (successCode.IsSuccessStatusCode)
                            {
                                return true;
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
            }

            return false;
        }

        public bool NotifyAsync(string to, string title, string body, string targetScreen
            , bool isTaskNotification, long objId, int docType)
        {
            try
            {
                var serverKey = string.Format("key={0}", ServerKey);
                var senderId = string.Format("id={0}", SenderId);
                dynamic notif = new
                {
                    to = to,
                    data = new
                    {
                        custom_notification = new
                        {
                            title = title.ToUpper(),
                            body = body,
                            sound = "default",
                            priority = "high",
                            show_in_foreground = true,
                            targetScreen = targetScreen,
                            isTaskNotification = isTaskNotification,
                            targetDocId = isTaskNotification ? 0 : objId,
                            tagetDocType = isTaskNotification ? 0 : docType,
                            targetTaskId = isTaskNotification ? objId : 0,
                            targetTaskType = isTaskNotification ? docType : 0
                        }
                    },
                    priority = 10
                };
                var data = new
                {
                    to,
                    notification = new { title, body },
                    sound = "default"
                };
                var jsonBody = JsonConvert.SerializeObject(notif);
                using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://fcm.googleapis.com/fcm/send"))
                {
                    httpRequest.Headers.TryAddWithoutValidation("Authorization", serverKey);
                    httpRequest.Headers.TryAddWithoutValidation("Sender", senderId);
                    httpRequest.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                    using (var httpClient = new HttpClient())
                    {
                        var result = httpClient.SendAsync(httpRequest);
                        if (result != null)
                        {
                            var successCode = result.Result;
                            if (successCode.IsSuccessStatusCode)
                            {
                                return true;
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
            }

            return false;
        }
    }
}
