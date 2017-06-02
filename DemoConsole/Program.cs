using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tyrrrz.Extensions;
using YoutubeExplode.Models;

namespace YoutubeExplode.DemoConsole
{
    public static class Program
    {
        
        private static string NormalizeId(string input)
        {
            if (!YoutubeClient.TryParseVideoId(input, out string id))
                id = input;
            return id;
        }

        private static async Task MainAsync()
        {
            var client = new YoutubeClient();
            string id = Console.ReadLine();
            id = NormalizeId(id);

            var videoInfo = await client.GetVideoInfoAsync(id);
            Console.WriteLine('-'.Repeat(100));

            Console.WriteLine($"Id: {videoInfo.Id} | Title: {videoInfo.Title} | Author: {videoInfo.Author.Title}");

            var streamInfo = videoInfo.MixedStreams
                .OrderBy(s => s.VideoQuality)
                .Last();
            string normalizedFileSize = NormalizeFileSize(streamInfo.ContentLength);
            Console.WriteLine($"Quality: {streamInfo.VideoQualityLabel} | Container: {streamInfo.Container} | Size: {normalizedFileSize}");

            string fileExtension = streamInfo.Container.GetFileExtension();
            string fileName = $"{videoInfo.Title}.{fileExtension}";

            fileName = fileName.Except(Path.GetInvalidFileNameChars());

            Console.WriteLine($"Downloading [{fileName}]...");
            Console.WriteLine('-'.Repeat(100));

            var progress = new Progress<double>(p => Console.Title = $"YoutubeExplode Demo [{p:P0}]");
            await client.DownloadMediaStreamAsync(streamInfo, fileName, progress);

            Console.WriteLine("Download complete!");
            Console.ReadKey();
        }

        public static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }
    }
}
