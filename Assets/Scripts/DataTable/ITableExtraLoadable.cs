using CsvHelper;

public interface ITableExtraLoadable
{
    public void ExtraLoad(CsvReader reader);
}