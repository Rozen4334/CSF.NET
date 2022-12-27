namespace CSF
{
    /// <summary>
    ///     Represents a command prefix with provided generic type.
    /// </summary>
    /// <typeparam name="T">The type this prefix represents.</typeparam>
    public interface IPrefix<T> : IPrefix
    {
        /// <summary>
        ///     Gets the prefix as the provided <typeparamref name="T"/>.
        /// </summary>
        /// <returns></returns>
        T GetValueAs();
    }

    /// <summary>
    ///     Represents a command prefix.
    /// </summary>
    public interface IPrefix
    {
        /// <summary>
        ///     The prefix of the command.
        /// </summary>
        string Value { get; }

        /// <summary>
        ///     Compares 2 values of <see cref="IPrefix"/> with eachother.
        /// </summary>
        /// <param name="prefix">The prefix to compare with.</param>
        /// <returns></returns>
        bool Equals(IPrefix prefix);
    }
}
