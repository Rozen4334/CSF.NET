using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents a precondition that checks if the provided context is valid.
    /// </summary>
    public sealed class RequireContextAttribute : PreconditionAttribute
    {
        /// <summary>
        ///     The context type to compare against.
        /// </summary>
        public Type ContextType { get; }

        /// <summary>
        ///     Compiles a new <see cref="RequireContextAttribute"/> from provided <paramref name="contextType"/>.
        /// </summary>
        /// <param name="contextType"></param>
        public RequireContextAttribute(Type contextType)
        {
            ContextType = contextType;
        }

        public override ValueTask<PreconditionResult> CheckAsync(IContext context, CommandInfo command, IServiceProvider provider, CancellationToken cancellationToken)
        {
            var providedType = context.GetType();

            if (providedType != ContextType)
                return PreconditionResult.FromError(
                        errorMessage: $"Invalid context was passed into the command. Expected: '{ContextType.FullName}', got '{providedType.FullName}'");

            return PreconditionResult.FromSuccess();
        }
    }
}
