using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

public class CsvStringConverter : TypeConverter<string>
{
    private static readonly string oldLinebreak = "<br>";
    private static readonly string newLinebreak = "\n";
    public override string ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        text.Replace(oldLinebreak, newLinebreak);
        return text;
    }

    public override string ConvertToString(string value, IWriterRow row, MemberMapData memberMapData)
    {
        value.Replace(newLinebreak, oldLinebreak);
        return value;
    }
}

public class CsvFloatConverter : TypeConverter<float>
{
    public override float ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        float result = 0f;
        float.TryParse(text, out result);
        return result;
    }

    public override string ConvertToString(float value, IWriterRow row, MemberMapData memberMapData)
    {
        return value.ToString();
    }
}

public class CsvIntConverter : TypeConverter<int>
{
    public override int ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        int result = 0;
        int.TryParse(text, out result);
        return result;
    }

    public override string ConvertToString(int value, IWriterRow row, MemberMapData memberMapData)
    {
        return value.ToString();
    }
}