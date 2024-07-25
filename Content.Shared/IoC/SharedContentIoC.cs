using Content.Shared.Humanoid.Markings;
using Content.Shared.Localizations;
using Content.Shared.Tokens;

namespace Content.Shared.IoC
{
    public static class SharedContentIoC
    {
        public static void Register()
        {
            IoCManager.Register<MarkingManager, MarkingManager>();
            IoCManager.Register<ContentLocalizationManager, ContentLocalizationManager>();
            IoCManager.Register<ILocalizationManager, TokenFixer>(true);
        }
    }
}
