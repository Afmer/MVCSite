using System.Text;

namespace MVCSite.Features.Extensions;
public static class IdentityToken
{
    ///<summary>
    ///Генерирует строку длинной 100
    ///(Generates a string of length 100)
    ///</summary>
    public static string Generate()
    {
        return Generate(50);
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