using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GrpcDemo.Server
{
    public class GrpcDemoService : GrpcDemo.GrpcDemoBase
    {
        private readonly ILogger<GrpcDemoService> _logger;
        public GrpcDemoService(ILogger<GrpcDemoService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }
    }
}
