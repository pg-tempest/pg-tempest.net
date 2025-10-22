using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Unicode;

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

    public static TemplateHash Parse(string s, IFormatProvider? provider)
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
}
