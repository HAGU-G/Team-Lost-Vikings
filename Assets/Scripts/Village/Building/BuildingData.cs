public class BuildingData : ITableAvaialable<int>
{
    public string StructureName { get; set; }
    public int StructureId { get; set; }
    public int Width { get; set; }
    public int Length { get; set; }
    public STRUCTURE_TYPE StructureType { get; set; }
    public int UnlockTownLevel { get; set; }
    public bool CanMultiBuild { get; set; }
    public bool CanReverse { get; set; }
    public bool CanReplace { get; set; }
    public bool CanDestroy { get; set; }
    public int UpgradeId { get; set; }
    public string StructureAssetFileName { get; set; }
    public string StructureDesc { get; set; }

    public int TableID => StructureId;

    // 순환참조 되는 클래스를 멤버로 둬야하면 [Ignore]을 붙이거나
    // 다른 방법으로 csvhelper가 무시할 수 있도록 해주세요.
    // MonoBehaviour 상속x
}
