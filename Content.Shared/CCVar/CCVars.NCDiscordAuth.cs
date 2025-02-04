using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

public sealed partial class CCVars
{
    public static readonly CVarDef<bool> ForgeDiscordAuthEnabled =
        CVarDef.Create("forge.discord_auth_enabled", true, CVar.SERVERONLY);

    public static readonly CVarDef<string> ForgeDiscordAuthUrl =
        CVarDef.Create("forge.discord_auth_url", "http://localhost:8000/sponsors", CVar.SERVERONLY | CVar.CONFIDENTIAL);

    public static readonly CVarDef<string> ForgeDiscordAuthToken =
        CVarDef.Create("forge.discord_auth_token", "token", CVar.SERVERONLY | CVar.CONFIDENTIAL);
}
