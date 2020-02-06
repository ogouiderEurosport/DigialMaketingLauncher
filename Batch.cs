using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace DigialMaketingLauncher
{
    public static class Batch
    {
        public static void Execute(ActionParam actionParam)
        {
            Console.WriteLine($"exec {actionParam.Name} started at {DateTime.Now.ToString()}");
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["BaseAddress"]);
                    var response = actionParam.Type.ToLower() == "get" ? client.GetAsync(actionParam.Url).Result : client.PostAsync(actionParam.Url, null).Result;
                    Log(actionParam.Name, GetLogContent(response));
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"exec {actionParam.Name} finished at {DateTime.Now.ToString()} with success");
                    }
                    else
                    {
                        Console.WriteLine($"exec {actionParam.Name} finished at {DateTime.Now.ToString()} with error {response.StatusCode}, reason {response.ReasonPhrase}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log(actionParam.Name, $"http call for {actionParam.Url} has failed, detail : {ex.Message}, more details : {ex.InnerException?.Message} ");
                Console.WriteLine($"exec {actionParam.Name} finished at {DateTime.Now.ToString()} with server error");
            }
        }

        private static void Log(string action, string logContent)
        {
            var subFolder = action.ToLower().Contains("extract") ? "Extract" : "Ingest";
            var logFolder = Path.Combine(ConfigurationManager.AppSettings["logFolder"], subFolder);
            var logFilePath = Path.Combine(logFolder, $@"{action}_{DateTime.Now.ToString("yyyyMMddHHmmss")}");

            Directory.CreateDirectory(logFolder);
            File.WriteAllText(logFilePath, logContent);
        }

        private static string GetLogContent(HttpResponseMessage response)
        {
            var logContent = string.Empty;
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
    }
}
