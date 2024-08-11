using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using System.Data;
using System.Data.Common;
using NHibernate.Engine;
using Newtonsoft.Json;
public class TimeZoneType : IUserType
{
    public bool IsMutable => false;
    public Type ReturnedType => typeof(TimeZoneInfo);
    public SqlType[] SqlTypes => new SqlType[] { new SqlType(DbType.String) };

    public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
    {
        var identifier = NHibernateUtil.String.NullSafeGet(rs, names[0], session) as string;
        return identifier == null ? null : TimeZoneInfo.FindSystemTimeZoneById(identifier);
    }


    public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
    {
        if (value == null)
        {
            NHibernateUtil.String.NullSafeSet(cmd, null, index, session);
        }
        else
        {
            var timeZone = (TimeZoneInfo)value;
            NHibernateUtil.String.NullSafeSet(cmd, timeZone.Id, index, session);
        }
    }

    
    public object DeepCopy(object value)
    {
        return value;
    }

    public object Replace(object original, object target, object owner)
    {
        return original;
    }

    public object Assemble(object cached, object owner)
    {
        return cached;
    }

    public object Disassemble(object value)
    {
        return value;
    }

    public new bool Equals(object x, object y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x == null || y == null)
        {
            return false;
        }

        return x.Equals(y);
    }

    public int GetHashCode(object x)
    {
        return x?.GetHashCode() ?? 0;
    }
}

public class TimeZoneInfoConverter : JsonConverter<TimeZoneInfo>
{
    public override TimeZoneInfo? ReadJson(JsonReader reader, Type objectType, TimeZoneInfo existingValue,bool hasExistingValue, JsonSerializer serializer)
    {
        // Odczytaj ID strefy czasowej
        string zoneId = (string)reader.Value;
        return string.IsNullOrEmpty(zoneId) ? null : TimeZoneInfo.FindSystemTimeZoneById(zoneId);
    }

    public override void WriteJson(JsonWriter writer, TimeZoneInfo value, JsonSerializer serializer)
    {
        // Zapisz ID strefy czasowej
        writer.WriteValue(value?.Id);
    }
    
}
