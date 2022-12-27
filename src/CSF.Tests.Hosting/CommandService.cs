using CSF.Hosting;
using Microsoft.Extensions.Logging;

namespace CSF.Tests.Hosting
{
    internal class CommandService : HostedCommandService<CommandConveyor>
    {
        public CommandService(CommandConveyor conveyor, CommandConfiguration configuration, IServiceProvider provider, ILogger<HostedCommandService<CommandConveyor>> logger) 
            : base(conveyor, configuration, provider, logger)
        {
        }
    }
}
