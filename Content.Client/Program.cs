using Content.Shared.Tokens;
using Robust.Client;

namespace Content.Client
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
