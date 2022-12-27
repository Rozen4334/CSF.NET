using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Defines a precondition attribute.
    /// </summary>
    public abstract class PreconditionAttribute : Attribute
    {
        /// <summary>
        ///     Checks for a certain inbound value to match a desired outcome, and fails if not.
        /// </summary>
        /// <param name="context">The context used to invoke the command.</param>
        /// <param name="command">The command info that is targetted to be used.</param>
        /// <param name="provider">The service provider used to enter this handler.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> holding the <see cref="PreconditionResult"/> of this call.</returns>
        public abstract ValueTask<PreconditionResult> CheckAsync(IContext context, CommandInfo command, IServiceProvider provider, CancellationToken cancellationToken);
    }
}
