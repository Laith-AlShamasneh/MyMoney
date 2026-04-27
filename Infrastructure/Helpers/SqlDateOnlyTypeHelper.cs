using Dapper;
using System.Data;

namespace Infrastructure.Helpers;

public class SqlDateOnlyTypeHelper : SqlMapper.TypeHandler<DateOnly>
{
    // Convert from Database (DateTime) to C# (DateOnly)
    public override DateOnly Parse(object value)
    {
        return DateOnly.FromDateTime((DateTime)value);
    }

    // Convert from C# (DateOnly) to Database (DateTime/Date)
    public override void SetValue(IDbDataParameter parameter, DateOnly value)
    {
        parameter.Value = value.ToDateTime(TimeOnly.MinValue);
        parameter.DbType = DbType.Date;
    }
}
