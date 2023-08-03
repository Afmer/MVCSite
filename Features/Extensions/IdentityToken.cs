using MVCSite.Settings;
namespace MVCSite.Features.Extensions;
public static class IdentityToken
{
    ///<summary>
    ///Генерирует строку длинной, которая задана в DBSettings
    ///(Generates a string with the length specified in DBSettings)
    ///</summary>
    public static string Generate()
    {
        return Generate(DBSettings.IdentityTokenLength / 2);
    }
    ///<summary>
    ///Длина исходной строки будет равна length * 2
    ///(The length of the original string will be equal to length * 2)
    ///</summary>
    public static string Generate(int length)
    {
        byte[] byteString = new byte[length];
        Random.Shared.NextBytes(byteString);
        return BitConverter.ToString(byteString).Replace("-", "").ToLower();
    }
}