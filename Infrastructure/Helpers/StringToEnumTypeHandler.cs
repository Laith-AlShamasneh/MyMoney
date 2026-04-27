using Dapper;
using System.Data;

namespace Infrastructure.Helpers;

// This generic handler works for ANY Enum you want to store as a String
public class StringToEnumTypeHandler<T> : SqlMapper.TypeHandler<T> where T : struct, Enum
{
    // Write: Enum -> DB String
    public override void SetValue(IDbDataParameter parameter, T value)
    {
        parameter.Value = value.ToString();
    }

    // Read: DB String -> Enum
    public override T Parse(object value)
    {
        if (value == null || value is DBNull) return default;

        // Convert the DB string (e.g., "InProgress") back to the Enum
        return Enum.Parse<T>(value.ToString()!, true);
    }
}