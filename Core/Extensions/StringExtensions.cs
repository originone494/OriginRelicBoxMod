using OriginRelicBox.Scripts;

namespace OriginRelicBox.Extensions;

//Mostly utilities to get asset paths.
public static class StringExtensions
{
    public static string ImagePath(this string path)
    {
        return Path.Join(Entry.ModId, "images", path);
    }

    public static string CardImagePath(this string path)
    {
        return Path.Join(Entry.ModId, "images", "card_portraits", path);
    }
    public static string BigCardImagePath(this string path)
    {
        return Path.Join(Entry.ModId, "images", "card_portraits", "big", path);
    }

    public static string PowerImagePath(this string path)
    {
        return Path.Join(Entry.ModId, "images", "powers", path);
    }

    public static string BigPowerImagePath(this string path)
    {
        return Path.Join(Entry.ModId, "images", "powers", "big", path);
    }

    public static string RelicImagePath(this string path)
    {
        return Path.Join(Entry.ModId, "images", "relics", path);
    }

    public static string BigRelicImagePath(this string path)
    {
        return Path.Join(Entry.ModId, "images", "relics", "big", path);
    }

    public static string CharacterUiPath(this string path)
    {
        return Path.Join(Entry.ModId, "images", "charui", path);
    }
}