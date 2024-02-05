using System;
using UnityEditor;

public interface IDataModel<out T>
{
    public void SetDefaultData();
}
