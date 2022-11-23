using CSF;
using CSF.Hosting;
using Microsoft.Extensions.Logging;

namespace CSF.Samples.Hosting
{
    public class CommandResolver : HostedCommandResolver<CommandFramework, CommandContext>
    {
        public CommandResolver(CommandFramework framework, IServiceProvider collection, ILogger<CommandFramework> logger)
            : base(framework, collection, logger)
        {
        }

        protected override Task<CommandContext> GenerateContextAsync(string input, CancellationToken cancellationToken = default)
        {
            var ctx = new CommandContext(input);
            return Task.FromResult(ctx);
        }

        protected override Task<string> GetInputStreamAsync(CancellationToken cancellationToken = default)
        {
            while (true)
                return Task.FromResult(Console.ReadLine()!);
        }
    }
}
