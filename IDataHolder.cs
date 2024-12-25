public interface IDataHolder
{
    public bool IsDoneLoadData { get; }
    public void SaveData();
    public void LoadData();
}
