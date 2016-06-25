using System;
using UnityEngine;
using System.Collections;
using System.Linq;

public class MessageFieldController : MonoBehaviour
{
    [HideInInspector]
    public bool isInputField;

    public UILabel FieldNameLabel;
    public UILabel FieldValueLabel;

    public UIInput FieldValueInput;

    public string FieldName
    {
        get { return FieldNameLabel.text; }
        set
        {
            FieldNameLabel.text = value;
        }
    }

    public string FieldValue
    {
        get { return FieldValueLabel.text; }
        set
        {
            FieldValueLabel.text = value;
            if (isInputField)
            {
                FieldValueInput.value = value;

                ////Set proto tool
                //ProtoItem protoItem =
                //    ProtoToolManager.Instance.SendMessageDataList.Where(item => item.fieldName == FieldNameLabel.text)
                //        .FirstOrDefault();
                //protoItem.fieldValue = value;
            }

        }
    }

    void Awake()
    {
        if (GetComponentInChildren<UIInput>() != null)
        {
            isInputField = true;

            FieldValueInput = FieldValueLabel.GetComponent<UIInput>();
        }
        else
        {
            isInputField = false;
        }
    }
}
