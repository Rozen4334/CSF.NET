namespace CSF.Tests.Modules
{
    public class Module : ModuleBase<CommandContext>
    {
        [Command("test")]
        public void Test(bool test, bool test1)
        {
            Success("Success");
        }
    }
}
