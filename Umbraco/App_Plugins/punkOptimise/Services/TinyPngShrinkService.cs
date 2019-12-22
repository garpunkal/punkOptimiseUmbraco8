using punkOptimise.Extensions;
using punkOptimise.Interfaces;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;

namespace punkOptimise.Services
{
    public class TinyPngShrinkService : IImageShrinkService
    {
        private readonly string _apiKey; 
        private readonly string _apiUrl;
        private readonly string _auth;

        public TinyPngShrinkService()
        {
            _apiKey = ConfigurationManager.AppSettings["punkOptimise:TinyPng:ApiKey"]?.ToString();
            _apiUrl = ConfigurationManager.AppSettings["punkOptimise:TinyPng:ApiUrl"]?.ToString();

            _auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"api:{_apiKey}"));
        }

        public byte[] ShrinkImage(byte[] data)
        {
            using (var client = new WebClient())
            {
                client.Proxy = null;
                client.Headers.Add(HttpRequestHeader.Authorization, "Basic " + _auth);

                client.UploadData(_apiUrl, data);
                return client.DownloadData(client.ResponseHeaders["Location"]);
            }
        }

        public byte[] ShrinkImage(Stream inStream) => ShrinkImage(inStream.ReadAsBytes());
        
        public bool IsEnabled() => !string.IsNullOrEmpty(_apiUrl) && !string.IsNullOrEmpty(_apiKey);      
    }
}