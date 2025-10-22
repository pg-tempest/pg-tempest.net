using PgTempest.Sdk.Models;

namespace PgTempest.Sdk.Tests.Extensions;

internal static class RandomExtensions
{
    internal static byte[] NextBytes(this Random random, int length)
    {
        var result = new byte[length];
        random.NextBytes(result);
        return result;
    }

    internal static TemplateHash NextTemplateHash(this Random random)
    {
        return new TemplateHash(random.NextBytes(16));
    }
}
