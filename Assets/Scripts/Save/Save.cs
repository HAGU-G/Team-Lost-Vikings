public abstract class SaveData
{
    public readonly int version;
    public SaveData(int version)
    {
        this.version = version;
    }

    public abstract SaveData VersionUp();
    public abstract SaveData VersionDown();
}