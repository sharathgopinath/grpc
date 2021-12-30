using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GrpcDemo.Server
{
    /// <summary>
    /// Server implementation of grpc-demo.proto
    /// </summary>
    public class GrpcDemoService : GrpcDemo.GrpcDemoBase
    {
        private readonly ILogger<GrpcDemoService> _logger;
        public GrpcDemoService(ILogger<GrpcDemoService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// A simple Unary RPC that responds with a greeting message
        /// </summary>
        /// <param name="request">Name of the client</param>
        /// <param name="context"></param>
        /// <returns>A greeting message</returns>
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        /// <summary>
        /// A server-side streaming RPC where the client sends a request to the server and gets a stream to read a sequence of messages back
        /// </summary>
        /// <param name="request">Empty params</param>
        /// <param name="responseStream">Stream of messages</param>
        /// <param name="context"></param>
        /// <returns>A response stream</returns>
        public override async Task GetTextStream(EmptyParams request,
            IServerStreamWriter<TextStreamResponse> responseStream,
            ServerCallContext context)
        {
            var lines = new string[] { 
                "Sphinx of black quartz, judge my vow",
                "The early bird catches the worm",
                "The second mouse gets the cheese",
                "The first mouse gets the cheese",
                "The third mouse gets the cheese",
                "The fourth mouse gets the cheese", };

            foreach (var line in lines)
            {
                await responseStream.WriteAsync(new TextStreamResponse { Line = line });

                // Sleeps for 1 second so that the streaming response is clearly noticeable from the console client
                Thread.Sleep(1000);
            }
        }
    }
}
