using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using qxmobile.protobuf;

public class ProtoItem
{
    public string typeName;
    public string fieldName;
    public string fieldValue;
}

public class ProtoIndex
{
    public int index;
    public int protoIndex;
}

public class ProtoStructureAnalyze
{

    public static List<ProtoItem> GetProtoItemListWithDefaultProto(string className)
    {
        Type classType = Type.GetType("qxmobile.protobuf." + className + ",ProtoClasses");

        if (classType != null)
        {
            object instance = Activator.CreateInstance(classType);
            return GetProtoItemListWithProto(instance);
        }
        else
        {
            ProtoToolManager.Instance.ShowError(true, "获取指定类：" + className + "类型失败");
            return null;
        }
    }

    public static List<ProtoItem> GetProtoItemListWithProto(object classInstanced)
    {
        List<ProtoItem> returnList = new List<ProtoItem>();

        FieldInfo[] fields = classInstanced.GetType().GetFields();
        foreach (var item in fields)
        {
            if (item.FieldType == typeof(byte) || item.FieldType == typeof(sbyte) || item.FieldType == typeof(char) || item.FieldType == typeof(short) || item.FieldType == typeof(ushort) || item.FieldType == typeof(int) || item.FieldType == typeof(uint) || item.FieldType == typeof(long) || item.FieldType == typeof(ulong) || item.FieldType == typeof(float) || item.FieldType == typeof(decimal) || item.FieldType == typeof(double) || item.FieldType == typeof(bool) || item.FieldType == typeof(string))
            {

                if (item.FieldType == typeof (string))
                {
                    returnList.Add(new ProtoItem
                    {
                        typeName = item.FieldType.Name,
                        fieldName = item.Name,
                        //object.ToString() cause Null Exception if it is a string object
                        fieldValue = item.GetValue(classInstanced) as string
                    });
                }
                else
                {
                    returnList.Add(new ProtoItem
                    {
                        typeName = item.FieldType.Name,
                        fieldName = item.Name,
                        fieldValue = item.GetValue(classInstanced).ToString()
                    });
                }
            }

            //if (item.GetType().GetInterface("IEnumerable") != null)
            //{
            //    IEnumerable temp = item as IEnumerable;
            //    string value = "";

            //    foreach (object item2 in temp)
            //    {

            //    }
            //    returnList.Add(new ProtoItem
            //    {
            //        typeName = item.GetType().Name,
            //        fieldName = item.Name,
            //        fieldValue = item.GetValue(classInstanced).ToString()
            //    });
            //}
        }

        return returnList;
    }

    public static bool SetProtoWithProtoItemList(ref object proto, List<ProtoItem> list)
    {
        FieldInfo[] fields = proto.GetType().GetFields();
        foreach (var field in fields)
        {
            if (field.FieldType == typeof(byte) || field.FieldType == typeof(sbyte) || field.FieldType == typeof(char) || field.FieldType == typeof(short) || field.FieldType == typeof(ushort) || field.FieldType == typeof(int) || field.FieldType == typeof(uint) || field.FieldType == typeof(long) || field.FieldType == typeof(ulong) || field.FieldType == typeof(float) || field.FieldType == typeof(decimal) || field.FieldType == typeof(double) || field.FieldType == typeof(bool) || field.FieldType == typeof(string))
            {
                if (!list.Select(listItem => listItem.fieldName).Contains(field.Name))
                {
                    return false;
                }

                List<ProtoItem> sameNameList = list.Where(item2 => item2.fieldName == field.Name).ToList();
                //var sameNameList = list.Select(item2 => item2.fieldName).Where(item2 => item2 == field.Name).ToList();
                if (sameNameList.Count != 1)
                {
                    return false;
                }

                ProtoItem sameName = sameNameList[0];
                if (sameName.typeName != field.FieldType.Name)
                {
                    return false;
                }

                try
                {
                    field.SetValue(proto, Convert.ChangeType(sameName.fieldValue, field.FieldType));
                }
                catch (Exception)
                {
                    ProtoToolManager.Instance.ShowError(true, "转换为指定的类型：" + field.FieldType.Name + "时失败");
                    return false;
                }
            }
        }

        return true;
    }
}
