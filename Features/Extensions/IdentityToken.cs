using System.Text;

namespace MVCSite.Features.Extensions;
public static class IdentityToken
{
    public static string Generate()
    {
        return Generate(50);
    }
    public static string Generate(int length)
    {
        byte[] byteString = new byte[length];
        Random.Shared.NextBytes(byteString);
        return BitConverter.ToString(byteString).Replace("-", "").ToLower();
    }
}