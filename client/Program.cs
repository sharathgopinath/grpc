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

                    default: break;
                };
                selectedOption = DisplayOptions();
            } while (selectedOption < 3);
        }

        private static int DisplayOptions()
        {
            Console.WriteLine("\r\nSelect an option:");
            Console.WriteLine("1. SayHello");
            Console.WriteLine("2. GetTextStream");
            Console.WriteLine("3. Exit");

            var selectedOption = Console.ReadKey().KeyChar;
            Console.WriteLine();
            return int.Parse(selectedOption.ToString());
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