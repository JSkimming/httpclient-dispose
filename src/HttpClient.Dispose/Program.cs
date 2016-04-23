using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

namespace HttpClientDispose
{
    internal class Program
    {
        private static HttpClient _singletonClient;

        private static void Main(string[] args)
        {
            _singletonClient = new HttpClient();

            var httpUri = new Uri("http://gimmeip.azurewebsites.net/");
            var httpsUri = new Uri("https://gimmeip.azurewebsites.net/");

            // Warm-up 1
            UseDisposingClientAsync(httpUri).Wait();
            UseDisposingClientAsync(httpsUri).Wait();
            UseSingletonClientAsync(httpUri).Wait();
            UseSingletonClientAsync(httpsUri).Wait();

            // Warm-up 2
            UseDisposingClientAsync(httpUri).Wait();
            UseDisposingClientAsync(httpsUri).Wait();
            UseSingletonClientAsync(httpUri).Wait();
            UseSingletonClientAsync(httpsUri).Wait();

            Console.WriteLine("Using Disposing Client.");
            TimeAsync(httpUri, UseDisposingClientAsync, 100).Wait();

            TimeAsync(httpsUri, UseDisposingClientAsync, 100).Wait();

            Console.WriteLine("Using Singleton Client.");
            TimeAsync(httpUri, UseSingletonClientAsync, 100).Wait();

            TimeAsync(httpsUri, UseSingletonClientAsync, 100).Wait();
        }

        private static async Task UseSingletonClientAsync(Uri requestUri)
        {
            await _singletonClient.GetStringAsync(requestUri);
        }

        private static async Task UseDisposingClientAsync(Uri requestUri)
        {
            using (var client = new HttpClient())
            {
                await client.GetStringAsync(requestUri);
            }
        }

        private static async Task TimeAsync(Uri requestUri, Func<Uri, Task> funcAsync, int count)
        {
            var sw = new Stopwatch();
            sw.Start();

            for(int i = 0; i < count; ++i)
            {
                await funcAsync(requestUri);
            }

            sw.Stop();

            Console.WriteLine($"Request '{requestUri}' {count} times taking '{sw.Elapsed}'. " +
                              $"Average {sw.ElapsedMilliseconds/count}ms/request.");
        }
    }
}
