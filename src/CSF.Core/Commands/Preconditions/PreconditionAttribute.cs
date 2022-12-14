using System;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Defines a precondition 
    /// </summary>
    public abstract class PreconditionAttribute : Attribute
    {
        public abstract Task<PreconditionResult> CheckAsync(IContext context, Command command, IServiceProvider provider);
    }
}
