using System.Diagnostics;
using System.Linq;
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
        private string[] _lines;

        public GrpcDemoService(ILogger<GrpcDemoService> logger)
        {
            _logger = logger;
            _lines = new string[] {
                "Sphinx of black quartz, judge my vow",
                "The early bird catches the worm",
                "The second mouse gets the cheese",
                "The first mouse gets the cheese",
                "The third mouse gets the cheese",
                "The fourth mouse gets the cheese", };
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
        /// <param name="responseStream">Stream that the client uses to receive messages from the server</param>
        /// <param name="context"></param>
        /// <returns>A response stream</returns>
        public override async Task GetTextStream(EmptyParams request,
            IServerStreamWriter<TextStreamResponse> responseStream,
            ServerCallContext context)
        {
            foreach (var line in _lines)
            {
                await responseStream.WriteAsync(new TextStreamResponse { Line = line });

                // Delay so that the streaming response is clearly noticeable from the console client
                await Task.Delay(500);
            }
        }

        /// <summary>
        /// A client-side streaming RPC where the client writes a sequence of messages and sends them to the server
        /// </summary>
        /// <param name="requestStream">Stream that the client uses to write messages</param>
        /// <param name="context"></param>
        /// <returns>Total elapsed time for the call in seconds</returns>
        public override async Task<SendTextStreamResponse> SendTextStream(IAsyncStreamReader<SendTextStreamRequest> requestStream, ServerCallContext context)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            _lines = new string[] { };
            while (await requestStream.MoveNext())
            {
                var currentRequest = requestStream.Current;
                _lines.Append(currentRequest.Line);
            }

            return new SendTextStreamResponse { ElapsedTimeSec = (int)(stopwatch.ElapsedMilliseconds / 1000) };
        }

        /// <summary>
        /// A bidirectional streaming RPC where both sides send a sequence of messages using a read-write stream. 
        /// The two streams operate independently, so clients and servers can read and write in whatever order they like.
        /// </summary>
        /// <param name="requestStream">Stream that the client uses to write messages</param>
        /// <param name="responseStream">Stream that the client uses to receive messages from the server</param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task ReverseText(IAsyncStreamReader<ReverseTextRequest> requestStream, IServerStreamWriter<ReverseTextResponse> responseStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var currentRequest = requestStream.Current;
                var reversedLine = new string(currentRequest.Line.Reverse().ToArray());
                await responseStream.WriteAsync(new ReverseTextResponse { Line = reversedLine });
                
                // Delay so that the streaming response is clearly noticeable from the console client
                await Task.Delay(1200);
            }
        }
    }
}
