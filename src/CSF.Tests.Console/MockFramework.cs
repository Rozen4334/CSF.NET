namespace CSF.Tests
{
    public sealed class MockFramework : CommandFramework
    {
        private readonly LogResolver _resolver;

        public MockFramework(LogResolver resolver, CommandConfiguration config)
            : base(config)
        {
            _resolver = resolver;
        }

        protected override ILogger ConfigureLogger()
        {
            return new DefaultLogger(Configuration.DefaultLogLevel, _resolver);
        }
    }
}
