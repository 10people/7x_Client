using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Read ItemTemp.xml
/// </summary>
public class ItemTempIOHandler
{
    /// <summary>
    /// main element name
    /// </summary>
    public const string ItemTempName = "ItemTemp";

    /// <summary>
    /// data structure
    /// </summary>
    public class ItemTempItem
    {
        public int id;
        public int name;
        public int funcDesc;
        public int icon;
        public int quality;
        public int repeatNum;
        public int itemType;
        public int effectID;
        public int sellNum;
    }
    
    /// <summary>
    /// read ItemTemp data
    /// </summary>
    /// <param name="itemTempImportPath">file path</param>
    /// <returns>ItemTemp data</returns>
    public static List<ItemTempItem> ReadItemTemp(string itemTempImportPath)
    {
        //load xml file
        var returnList = new List<ItemTempItem>();
        var loadedFile = XMLFileIO.ReadXML(itemTempImportPath, ItemTempName);

        foreach (var dicItem in loadedFile)
        {
            //Try transfer loaded xml file to ItemTempItem class.
            try
            {
                returnList.Add(new ItemTempItem
                {
                    id = String.IsNullOrEmpty(dicItem["id"]) ? 0 : int.Parse(dicItem["id"]),
                    name = String.IsNullOrEmpty(dicItem["name"]) ? 0 : int.Parse(dicItem["name"]),
                    funcDesc = String.IsNullOrEmpty(dicItem["funcDesc"]) ? 0 : int.Parse(dicItem["funcDesc"]),
                    icon = String.IsNullOrEmpty(dicItem["icon"]) ? 0 : int.Parse(dicItem["icon"]),
                    quality = String.IsNullOrEmpty(dicItem["quality"]) ? 0 : int.Parse(dicItem["quality"]),
                    repeatNum = String.IsNullOrEmpty(dicItem["repeatNum"]) ? 0 : int.Parse(dicItem["repeatNum"]),
                    itemType = String.IsNullOrEmpty(dicItem["itemType"]) ? 0 : int.Parse(dicItem["itemType"]),
                    effectID = String.IsNullOrEmpty(dicItem["effectID"]) ? 0 : int.Parse(dicItem["effectID"]),
                    sellNum = String.IsNullOrEmpty(dicItem["sellNum"]) ? 0 : int.Parse(dicItem["sellNum"]),
                });
            }

            //Catch KeyNotFound or other exception.
            catch (Exception ex)
            {
                Debug.LogError("Catch exception in ReadItemTemp, exception message:" + ex.Message
                    + " \n, exception stack trace:" + ex.StackTrace
                    + " \n, file read is not correct, please check.");
                return null;
            }
        }

        return returnList;
    }
}
