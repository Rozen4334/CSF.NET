using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSF
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public sealed class RequirePrefixAttribute : PreconditionAttribute
    {
        /// <summary>
        ///     The allowed prefixes.
        /// </summary>
        public IPrefix[] AllowedPrefixes;

        /// <summary>
        ///     Creates a new <see cref="RequirePrefixAttribute"/> with provided required prefixes.
        /// </summary>
        /// <remarks>
        ///     This constructor is not CLS compliant.
        /// </remarks>
        /// <param name="prefixes">The prefix or prefixes this command requires.</param>
        [CLSCompliant(false)]
        public RequirePrefixAttribute(params IPrefix[] prefixes)
        {
            AllowedPrefixes = prefixes;
        }

        /// <summary>
        ///     Creates a new <see cref="RequirePrefixAttribute"/> with provided required prefix.
        /// </summary>
        /// <remarks>
        ///     If you want to allow multiple prefixes to enter this command and CLS compliancy is no concern, consider using <see cref="RequirePrefixAttribute(IPrefix[])"/> instead.
        /// </remarks>
        /// <param name="prefix">The prefix this command requires.</param>
        [CLSCompliant(true)]
        public RequirePrefixAttribute(string prefix)
        {
            AllowedPrefixes = new IPrefix[] { new StringPrefix(prefix) };
        }

        /// <summary>
        ///     Creates a new <see cref="RequirePrefixAttribute"/> with provided required prefix.
        /// </summary>
        /// <remarks>
        ///     If you want to allow multiple prefixes to enter this command and CLS compliancy is no concern, consider using <see cref="RequirePrefixAttribute(IPrefix[])"/> instead.
        /// </remarks>
        /// <param name="prefix">The prefix this command requires.</param>
        [CLSCompliant(true)]
        public RequirePrefixAttribute(char prefix)
        {
            AllowedPrefixes = new IPrefix[] { new CharPrefix(prefix) };
        }

        public override ValueTask<PreconditionResult> CheckAsync(IContext context, CommandInfo command, IServiceProvider provider, CancellationToken cancellationToken)
        {
            foreach (var prefix in AllowedPrefixes)
            {
                if (context.Parameters[0].ToString().StartsWith(prefix.Value))
                    return PreconditionResult.FromSuccess();
            }
            return PreconditionResult.FromError("Failed to find any allowed prefix with matching type.");
        }
    }
}
