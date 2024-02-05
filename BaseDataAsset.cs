using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public abstract class BaseDataAsset : ScriptableObject
{
    public abstract bool IsDoneLoadData { get; }
    public abstract void SaveData();
    public abstract void LoadData();
}


public abstract class BaseDataAsset<DataModel> : BaseDataAsset where DataModel : struct, IDataModel<DataModel>
{
    [SerializeField] protected DataModel dataModel;

    [Space(12)]
    [SerializeField] private string fileName = string.Empty;

#if !SECURITY_DATA
    [SerializeField] private bool binaryFormat = false;
#else
    [SerializeField] private bool binaryFormat = true;
#endif
    protected bool isDoneLoadData = false;

    public override bool IsDoneLoadData => isDoneLoadData;
    public override void SaveData()
    {
        string filePath = Application.persistentDataPath + "/" +fileName;

        if(!File.Exists(filePath))
        {
            dataModel = new DataModel();
            dataModel.SetDefaultData();
        }

        try
        {
            if (binaryFormat)
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    IFormatter formatter = new BinaryFormatter();

                    formatter.Serialize(fileStream, dataModel);
                }
            }
            else
            {
                string json = JsonConvert.SerializeObject(dataModel, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
        }catch (Exception e)
        {
#if DATA_LOG
            Debug.LogError($"Save Game Service: Error: {e}");
#endif
        }
    }

    public override void LoadData()
    {
        string filePath = Application.persistentDataPath + "/" + fileName;
#if DATA_LOG
        Debug.Log($"Save Game Service: Save to {filePath}");
#endif
        try
        {
            if (!File.Exists(filePath))
            {
                SaveData();
            }
            else
            {
                if (binaryFormat)
                {
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                    {
                        IFormatter formatter = new BinaryFormatter();
                        dataModel = (DataModel)formatter.Deserialize(fileStream);
                    }
                }
                else
                {
                    string json = File.ReadAllText(filePath);
                    dataModel = JsonConvert.DeserializeObject<DataModel>(json);
                }
            }
        }
        catch (Exception e)
        {
#if DATA_LOG
            Debug.LogError($"Save Game Service: Error: {e}");
#endif
        }
        finally
        {
            isDoneLoadData = true;
        }
    }
}
