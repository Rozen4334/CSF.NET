using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace CSF
{
    /// <summary>
    ///     Represents a component with preconditions available.
    /// </summary>
    public interface IConditionalComponent : IComponent
    {
        /// <summary>
        ///     The aliases of this component.
        /// </summary>
        string[] Aliases { get; }

        /// <summary>
        ///     The preconditions of this component.
        /// </summary>
        IReadOnlyCollection<PreconditionAttribute> Preconditions { get; }

        /// <summary>
        ///     Calls the pipeline to handle the exposed result.
        /// </summary>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        internal async Task RequestToHandleAsync(ICommandConveyor service, CancellationToken cancellationToken)
        {
            await service.OnRegisteredAsync(this, cancellationToken);
        }
    }
}
