using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcDemo.Client;
using static GrpcDemo.Client.GrpcDemo;

namespace Grpc.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // The port number must match the port of the gRPC server.
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new GrpcDemoClient(channel);

            var selectedOption = 0;
            do
            {
                switch (selectedOption)
                {
                    case 1:
                        await SayHello(client);
                        break;

                    case 2:
                        await GetTextStream(client);
                        break;

                    case 3:
                        await SendTextStream(client);
                        break;

                    case 4:
                        await ReverseText(client);
                        break;

                    default: break;
                };
                selectedOption = DisplayOptions();
            } while (selectedOption < 5);
        }

        private static int DisplayOptions()
        {
            Console.WriteLine("\r\nSelect an option:");
            Console.WriteLine("1. SayHello");
            Console.WriteLine("2. GetTextStream");
            Console.WriteLine("3. SendTextStream");
            Console.WriteLine("4. ReverseText");
            Console.WriteLine("5. Exit");

            var selectedOption = Console.ReadKey().KeyChar;
            Console.WriteLine();
            return int.Parse(selectedOption.ToString());
        }

        private static async Task ReverseText(GrpcDemoClient client)
        {
            using var call = client.ReverseText();
            var responseReaderTask = Task.Run(async () =>
            {
                while (await call.ResponseStream.MoveNext(CancellationToken.None))
                {
                    Console.WriteLine($"Server response: {call.ResponseStream.Current.ToString()}");
                }
            });

            var lines = new string[] { 
                "The first mouse gets the cheese",
                "The second mouse gets the cheese",
                "The third mouse gets the cheese",
                "The fourth mouse gets the cheese" };

            foreach (var line in lines)
            {
                await call.RequestStream.WriteAsync(new ReverseTextRequest { Line = line });
                Console.WriteLine($"Sent: {line}");
                
                // Delay so that the streaming response is clearly noticeable
                await Task.Delay(500);
            }
            await call.RequestStream.CompleteAsync();
            await responseReaderTask;
        }

        private static async Task SendTextStream(GrpcDemoClient client)
        {
            using var call = client.SendTextStream();
            var lines = new string[] {
                "Hi there",
                "What's up?",
                "How are you?",
                "The quick brown fox jumps over the lazy dog 1",
                "The quick brown fox jumps over the lazy dog 2",
                "The quick brown fox jumps over the lazy dog 3",
                "The quick brown fox jumps over the lazy dog 4",
            };
            foreach (var line in lines)
            {
                await call.RequestStream.WriteAsync(new SendTextStreamRequest { Line = line });
                Console.WriteLine($"Sent: {line}");
                await Task.Delay(500);
            }
            await call.RequestStream.CompleteAsync();
            var updateResponse = await call.ResponseAsync;
            Console.WriteLine($"Server received in {updateResponse.ElapsedTimeSec} seconds");
        }

        private static async Task GetTextStream(GrpcDemoClient client)
        {
            using var call = client.GetTextStream(new EmptyParams());
            var responseStream = call.ResponseStream;

            Console.WriteLine("Response: ");
            while(await responseStream.MoveNext(new CancellationToken()))
            {
                var line = responseStream.Current;
                Console.WriteLine(line.ToString());
            }
        }

        private static async Task SayHello(GrpcDemoClient client)
        {
            var reply = await client.SayHelloAsync(
                          new HelloRequest { Name = "GrpcClient" });
            Console.WriteLine("Greeting: " + reply.Message);
        }
    }
}