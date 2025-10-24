using System.Buffers;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace PgTempest.Sdk.Models;

public sealed class TemplateHash
{
    public const int TemplateHashLength = 16;

    private byte[] _hash;
    public IReadOnlyList<byte> Hash => _hash;

    public TemplateHash(byte[] hash)
    {
        if (hash.Length != TemplateHashLength)
        {
            throw new ArgumentException(
                $"Hash length must be equal to {TemplateHashLength}",
                nameof(hash)
            );
        }

        _hash = hash;
    }

    public static TemplateHash Parse(string s)
    {
        var hash = new byte[TemplateHashLength];

        return Convert.FromHexString(s, hash.AsSpan(), out _, out _) == OperationStatus.Done
            ? new TemplateHash(hash)
            : throw new FormatException("Invalid template hash");
    }

    public override string ToString()
    {
        return Convert.ToHexString(_hash);
    }

    public static TemplateHash Calculate(params IEnumerable<string> hashSources)
    {
        // TODO: Optimize hash calculation.
        var combinedString = string.Join("", hashSources);
        var hash = new byte[TemplateHashLength];

        MD5.HashData(
            source: MemoryMarshal.AsBytes(combinedString.AsSpan()),
            destination: hash.AsSpan()
        );

        return new TemplateHash(hash);
    }
}
