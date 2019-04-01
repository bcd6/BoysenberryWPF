using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Boysenberry.Services
{
    public class WeiboService
    {

        private static string USER_AGENT = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36";

        /// <summary>
        /// NicknameToContainerId
        /// </summary>
        /// <param name="nickname"></param>
        /// <returns></returns>
        public string NicknameToUserId(string nickname)
        {
            string url = "http://m.weibo.com/n/";
            var client = new RestClient(url);
            var request = new RestRequest(nickname, Method.POST);
            request.AddHeader("User-Agent", USER_AGENT);
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK && response.ResponseUri.LocalPath.StartsWith("/p"))
            {
                string userId = response.ResponseUri.ToString().Substring(27);
                return userId;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// UserId To ContainerId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string UserIdToContainerId(string userId)
        {
            return "107603" + userId;
        }

        public List<string> GetAllImgURL(string userId)
        {
            List<string> urls = new List<string>();
            int i = 1;
            while (GetImgURL(userId, i, urls) > 0)
            {
                i++;
                Thread.Sleep(500);
            }
            return urls;
        }

        private int GetImgURL(string userId, int page, List<string> urls)
        {
            Debug.WriteLine("pages: " + page);
            string url = "https://m.weibo.cn/api/container/getIndex?count=25&page=" + page + "&containerid=" + UserIdToContainerId(userId);
            var client = new RestClient(url);
            var request = new RestRequest("", Method.GET);
            request.AddHeader("User-Agent", USER_AGENT);
            IRestResponse response = client.Execute(request);
            return ParseImgURL(response.Content, urls);
        }

        private int ParseImgURL(string response, List<string> urls)
        {
            JObject root = JObject.Parse(response);
            IList<JToken> cardsList = root["data"]["cards"].Children().ToList();
            for (int i = 0; i < cardsList.Count; i++)
            {
                JToken mblog = cardsList[i]["mblog"];
                if (mblog != null)
                {
                    JToken pics = mblog["pics"];
                    if (pics != null)
                    {
                        IList<JToken> picsList = pics.Children().ToList();
                        for (int j = 0; j < picsList.Count; j++)
                        {
                            JToken large = pics[j]["large"];
                            if (large != null)
                            {
                                urls.Add(large["url"].ToString());
                                Debug.WriteLine("urls: " + urls.Count);
                            }

                        }
                    }

                }
            }
            return cardsList.Count;
        }

        public async Task DownlaodImg(string imgUrl, string dstFolder)
        {
            
            Uri source = new Uri(imgUrl);
            string fileName = source.LocalPath.Split("/").Last();
            string filePath = $"{dstFolder}\\{fileName}";
            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists && fileInfo.Length > 0)
            {
                return;
            }
            FileStream file = File.Create(filePath);
            var client = new RestClient(source);
            var request = new RestRequest("", Method.GET);
            var img = client.DownloadData(request);
            await file.WriteAsync(img);
        }
    }
}
