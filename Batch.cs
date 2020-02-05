using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace DigialMaketingLauncher
{
    public static class Batch
    {
        public static void Execute(string url)
        {
            var action = url.Split('/').Last();
            Console.WriteLine($"exec {action} started at {DateTime.Now.ToString()}");
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["BaseAddress"]);
                    var response = url.ToLower().Contains("extract") ? client.GetAsync(url).Result :
                                                                        client.PostAsync(url, null).Result;
                    Log(action, GetLogContent(response));
                }
            }
            catch (Exception ex)
            {
                Log(action, $"http call for {url} has failed, detail {ex.Message} ");
            }
            Console.WriteLine($"exec {action} finished at {DateTime.Now.ToString()}");

        }

        private static string GetLogContent(HttpResponseMessage response)
        {
            var logContent = "";
            if (response.IsSuccessStatusCode)
            {
                logContent = "OK";
            }
            else
            {
                logContent += response.StatusCode.ToString() + Environment.NewLine;
                logContent += response.Content.ReadAsStringAsync().Result;
            }
            return logContent;
        }

        private static void Log(string action, string logContent)
        {
            var logFolder = ConfigurationManager.AppSettings["logFolder"];
            var logFilePath = Path.Combine(logFolder, $@"{action}_{DateTime.Now.ToString("yyyyMMddHHmmss")}");

            Directory.CreateDirectory(logFolder);
            File.WriteAllText(logFilePath, logContent);
        }
    }
}
