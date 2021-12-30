using System;
using Grpc.Net.Client;
using GrpcDemo.Client;
using static GrpcDemo.Client.GrpcDemo;

// The port number must match the port of the gRPC server.
using var channel = GrpcChannel.ForAddress("https://localhost:5001");

var client = new GrpcDemoClient(channel);
var reply = await client.SayHelloAsync(
                  new HelloRequest { Name = "GrpcClient" });
Console.WriteLine("Greeting: " + reply.Message);
Console.WriteLine("Press any key to exit...");
Console.ReadKey();