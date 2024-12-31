using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using UnityEngine;

public abstract class BaseDataHolder : MonoBehaviour, IDataHolder
{
    public abstract bool IsDoneLoadData { get; }
    public abstract bool IsExistData { get; }
    public abstract void SaveData();
    public abstract void LoadData();
}


public abstract class BaseDataHolder<TDataModel> : BaseDataHolder where TDataModel : struct, IDataModel<TDataModel>
{
    [SerializeField] protected TDataModel dataModel;

    [Space(12)] [SerializeField] private string _fileName = string.Empty;

    protected bool isDoneLoadData;

    private const string DEFAULT_KEY = "1234567890123456";
    private const string DEFAULT_IV = "6543210987654321";

    protected virtual string GetKey() => DEFAULT_KEY;
    protected virtual string GetIV() => DEFAULT_IV;

    public override bool IsDoneLoadData => isDoneLoadData;
    public override bool IsExistData => File.Exists(GetFilePath());

    private string GetFilePath()
    {
        if (string.IsNullOrEmpty(_fileName))
            _fileName = this.GetType().Name;
        return Application.persistentDataPath + "/" + _fileName;
    }

    public override void SaveData()
    {
        var filePath = GetFilePath();

        try
        {
            if (!File.Exists(filePath))
            {
#if DATA_LOG
            Debug.Log($"Data Game Service: Create new file {filePath}");
#endif
                dataModel = new TDataModel();
                dataModel.SetDefaultData();
            }

            var json = JsonConvert.SerializeObject(dataModel, Formatting.Indented);

#if SECURITY_DATA
        var key = Encoding.UTF8.GetBytes(GetKey()); // Key 16 bytes
        var iv = Encoding.UTF8.GetBytes(GetIV());  // IV 16 bytes
        var encryptedData = DataEncryptor.Encrypt(json, key, iv);
        File.WriteAllBytes(filePath, encryptedData);
#else
            File.WriteAllText(filePath, json);
#endif
        }
        catch (Exception e)
        {
#if DATA_LOG
        Debug.LogError($"Data Game Service: Save Data Error: {e}");
#endif
        }
    }

    public override void LoadData()
    {
        var filePath = GetFilePath();
#if DATA_LOG
    Debug.Log($"Data Game Service: Load data from {filePath}");
#endif
        try
        {
            if (!File.Exists(filePath))
            {
                SaveData();
            }
            else
            {
#if SECURITY_DATA
                var key = Encoding.UTF8.GetBytes(GetKey()); // Key 16 bytes
                var iv = Encoding.UTF8.GetBytes(GetIV()); // IV 16 bytes
                var encryptedData = File.ReadAllBytes(filePath);
                var json = DataEncryptor.Decrypt(encryptedData, key, iv);
#else
                var json = File.ReadAllText(filePath);
#endif
                dataModel = JsonConvert.DeserializeObject<TDataModel>(json);
            }
        }
        catch (Exception e)
        {
            isDoneLoadData = true;
#if DATA_LOG
        Debug.LogError($"Data Game Service: Load Data Error: {e}");
#endif
        }
        finally
        {
            isDoneLoadData = true;
        }
    }

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(_fileName))
            _fileName = this.GetType().Name;
    }
}