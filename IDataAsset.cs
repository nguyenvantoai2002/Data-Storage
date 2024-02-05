public interface IDataAsset
{
    public bool IsDoneLoadData { get; }
    public void SaveData();
    public void LoadData();
}
