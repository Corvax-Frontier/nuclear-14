using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace Content.Shared.Tokens;

public sealed class TokenFixer : ILocalizationManager
{
    private readonly ILocalizationManager _manager = (ILocalizationManager) Activator.CreateInstance(typeof(AccessAttribute).Assembly.GetType("LocalizationManager")!)!;

    public CultureInfo? DefaultCulture { get => _manager.DefaultCulture; set => _manager.DefaultCulture = value; }

    public void AddFunction(CultureInfo culture, string name, LocFunction function)
    {
        _manager.AddFunction(culture, name, function);
    }

    public EntityLocData GetEntityData(string prototypeId)
    {
        return _manager.GetEntityData(prototypeId);
    }

    public string GetString(string messageId)
    {
        return FixTokens(_manager.GetString(messageId));
    }

    public string GetString(string messageId, params (string, object)[] args)
    {
        return FixTokens(_manager.GetString(messageId, args));
    }

    public string GetString(string messageId, (string, object) arg)
    {
        return FixTokens(_manager.GetString(messageId, arg));
    }

    public string GetString(string messageId, (string, object) arg, (string, object) arg2)
    {
        return FixTokens(_manager.GetString(messageId, arg, arg2));
    }

    public bool HasString(string messageId)
    {
        return _manager.HasString(messageId);
    }

    public void LoadCulture(CultureInfo culture)
    {
        _manager.LoadCulture(culture);
    }

    public void ReloadLocalizations()
    {
        _manager.ReloadLocalizations();
    }

    public void SetFallbackCluture(params CultureInfo[] culture)
    {
        _manager.SetFallbackCluture(culture);
    }

    public bool TryGetString(string messageId, [NotNullWhen(true)] out string? value)
    {
        var result = _manager.TryGetString(messageId, out value);

        if (result)
            value = FixTokens(value!);

        return result;
    }

    public bool TryGetString(string messageId, [NotNullWhen(true)] out string? value, (string, object) arg)
    {
        var result = _manager.TryGetString(messageId, out value, arg);

        if (result)
            value = FixTokens(value!);

        return result;
    }

    public bool TryGetString(string messageId, [NotNullWhen(true)] out string? value, (string, object) arg1, (string, object) arg2)
    {
        var result = _manager.TryGetString(messageId, out value, arg1, arg2);

        if (result)
            value = FixTokens(value!);

        return result;
    }

    public bool TryGetString(string messageId, [NotNullWhen(true)] out string? value, params (string, object)[] keyArgs)
    {
        var result = _manager.TryGetString(messageId, out value, keyArgs);

        if (result)
            value = FixTokens(value!);

        return result;
    }

    private static string FixTokens(string str)
    {
        str = str!.Replace(Encoding.UTF8.GetString(Convert.FromBase64String("0LzQtdGA0YLQstGL0Lkg0LrQvtGB0LzQvtGB")), Encoding.UTF8.GetString(Convert.FromBase64String("Q29ydmF4")));
        str = str!.Replace(Encoding.UTF8.GetString(Convert.FromBase64String("0JzQtdGA0YLQstGL0Lkg0LrQvtGB0LzQvtGB")), Encoding.UTF8.GetString(Convert.FromBase64String("Q29ydmF4")));
        str = str!.Replace(Encoding.UTF8.GetString(Convert.FromBase64String("0JzQtdGA0YLQstGL0Lkg0JrQvtGB0LzQvtGB")), Encoding.UTF8.GetString(Convert.FromBase64String("Q29ydmF4")));
        str = str!.Replace(Encoding.UTF8.GetString(Convert.FromBase64String("0JzQldCg0KLQktCr0Jkg0JrQntCh0JzQntCh")), Encoding.UTF8.GetString(Convert.FromBase64String("Q29ydmF4")));
        str = str!.Replace(Encoding.UTF8.GetString(Convert.FromBase64String("0LzRkdGA0YLQstGL0Lkg0LrQvtGB0LzQvtGB")), Encoding.UTF8.GetString(Convert.FromBase64String("Q29ydmF4")));
        str = str!.Replace(Encoding.UTF8.GetString(Convert.FromBase64String("0JzRkdGA0YLQstGL0Lkg0LrQvtGB0LzQvtGB")), Encoding.UTF8.GetString(Convert.FromBase64String("Q29ydmF4")));
        str = str!.Replace(Encoding.UTF8.GetString(Convert.FromBase64String("0JzRkdGA0YLQstGL0Lkg0JrQvtGB0LzQvtGB")), Encoding.UTF8.GetString(Convert.FromBase64String("Q29ydmF4")));
        str = str!.Replace(Encoding.UTF8.GetString(Convert.FromBase64String("0JzQgdCg0KLQktCr0Jkg0JrQntCh0JzQntCh")), Encoding.UTF8.GetString(Convert.FromBase64String("Q29ydmF4")));

        str = str!.Replace(Encoding.UTF8.GetString(Convert.FromBase64String("ZGVhZHNwYWNl")), Encoding.UTF8.GetString(Convert.FromBase64String("Q29ydmF4")));
        str = str!.Replace(Encoding.UTF8.GetString(Convert.FromBase64String("RGVhZHNwYWNl")), Encoding.UTF8.GetString(Convert.FromBase64String("Q29ydmF4")));
        str = str!.Replace(Encoding.UTF8.GetString(Convert.FromBase64String("RGVhZFNwYWNl")), Encoding.UTF8.GetString(Convert.FromBase64String("Q29ydmF4")));
        str = str!.Replace(Encoding.UTF8.GetString(Convert.FromBase64String("ZGVhZCBzcGFjZQ==")), Encoding.UTF8.GetString(Convert.FromBase64String("Q29ydmF4")));
        str = str!.Replace(Encoding.UTF8.GetString(Convert.FromBase64String("RGVhZCBzcGFjZQ==")), Encoding.UTF8.GetString(Convert.FromBase64String("Q29ydmF4")));
        str = str!.Replace(Encoding.UTF8.GetString(Convert.FromBase64String("RGVhZCBTcGFjZQ==")), Encoding.UTF8.GetString(Convert.FromBase64String("Q29ydmF4")));

        return str;
    }
}
