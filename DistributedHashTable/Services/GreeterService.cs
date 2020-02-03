using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Library;
using Microsoft.Extensions.Logging;

namespace DistributedHashTable
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly NodeIdentifier _identifier;

        public GreeterService(ILogger<GreeterService> logger, INodeIdentifierFactory factory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _identifier = factory.Create();
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Goodbye");
            return Task.FromResult(new HelloReply
            {
                Message = $"Hello {context.Peer} from {_identifier.Name}"
            });
        }
    }
}
