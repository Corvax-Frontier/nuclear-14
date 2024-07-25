using Content.Shared.Tokens;
using Robust.Server;

namespace Content.Server
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            IoCManager.Register<ILocalizationManager, TokenFixer>();
            ContentStart.Start(args);
        }
    }
}
