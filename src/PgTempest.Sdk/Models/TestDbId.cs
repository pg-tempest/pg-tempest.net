using System.Globalization;

namespace PgTempest.Sdk.Models;

public sealed record TestDbId(ushort Value)
{
    public override string ToString() => $"{Value:X4}";

    public static TestDbId Parse(string s)
    {
        return ushort.TryParse(s, NumberStyles.HexNumber, null, out var value)
            ? new TestDbId(value)
            : throw new FormatException("Invalid test db id");
    }
}
