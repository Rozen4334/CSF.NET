using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF.Samples.Hosting.Modules
{
    public sealed class Module : ModuleBase<CommandContext>
    {
        private readonly IServiceProvider _provider;

        [InjectionConstructor]
        public Module(IServiceProvider provider)
        {
            _provider = provider;
        }

        [Command("help", "h")]
        public IResult Help()
            => Success("helped");
    }
}
