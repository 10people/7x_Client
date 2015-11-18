using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using qxmobile.protobuf;

public class ProtoToolManager : Singleton<ProtoToolManager>, SocketListener
{
    public static bool isExecuteAwake;

    public UILabel ProtoIndexLabel;
    public EventHandler GetProtoHandler;
    public EventHandler SendMessageHandler;
    public EventHandler ClearProtoHandler;

    private string sendMessageClass;
    private int sendMessageProtoIndex;

    public GameObject ReceiveIndexObject;
    public GameObject SendIndexObject;
    public EventHandler ReceiveIndexHandler;
    public EventHandler SendIndexHandler;

    #region Scroll View Controller

    public UIGrid SendMessageFieldGrid;
    public UIGrid ViewMessageFieldGrid;
    public UIGrid StoredReceiveProtoGrid;
    public UIGrid StoredSendProtoGrid;

    private GameObject SendMessageFieldPrefab;
    private GameObject ViewMessageFieldPrefab;
    private GameObject StoredProtoPrefab;

    public List<ProtoItem> SendMessageDataList = new List<ProtoItem>();
    private List<MessageFieldController> SendMessageControllerList = new List<MessageFieldController>();

    /// <summary>
    /// data from a value in StoredViewProtoIndexDataDic
    /// </summary>
    private List<ProtoItem> ViewMessageDataList = new List<ProtoItem>();
    private List<MessageFieldController> ViewMessageControllerList = new List<MessageFieldController>();

    private Dictionary<ProtoIndex, List<ProtoItem>> StoredReceiveProtoIndexDataDic = new Dictionary<ProtoIndex, List<ProtoItem>>();
    private List<StoredProtoController> StoredReceiveProtoIndexControllerList = new List<StoredProtoController>();

    public Dictionary<ProtoIndex, List<ProtoItem>> StoredSendProtoIndexDataDic = new Dictionary<ProtoIndex, List<ProtoItem>>();
    private List<StoredProtoController> StoredSendProtoIndexControllerList = new List<StoredProtoController>();

    private void RefreshSendMessageGrid()
    {
        TransformHelper.AddOrDelItemUsingPool(SendMessageFieldGrid.transform, SendMessageDataList.Count, PoolManagerListController.Instance, "ProtoSend");

        SendMessageControllerList.Clear();
        foreach (Transform child in SendMessageFieldGrid.transform)
        {
            SendMessageControllerList.Add(child.GetComponent<MessageFieldController>());
        }

        for (int i = 0; i < SendMessageDataList.Count; i++)
        {
            SendMessageControllerList[i].isInputField = true;
            SendMessageControllerList[i].FieldNameLabel.text = SendMessageDataList[i].fieldName;
            SendMessageControllerList[i].FieldValueLabel.text = SendMessageDataList[i].fieldValue;
            SendMessageControllerList[i].FieldValueInput.value = SendMessageDataList[i].fieldValue;
        }

        SendMessageFieldGrid.Reposition();
    }

    /// <summary>
    /// Set view message data with protoindex dic
    /// </summary>
    /// <param name="index">ProtoIndex</param>
    public void SetViewMessage(ProtoIndex index)
    {
        if (ReceiveIndexObject.activeSelf)
        {
            ViewMessageDataList = StoredReceiveProtoIndexDataDic[index];
        }
        else if(SendIndexObject.activeSelf)
        {
            ViewMessageDataList = StoredSendProtoIndexDataDic[index];
        }
    }

    public void RefreshViewMessageGrid()
    {
        TransformHelper.AddOrDelItemUsingPool(ViewMessageFieldGrid.transform, ViewMessageDataList.Count, PoolManagerListController.Instance, "ProtoView");

        ViewMessageControllerList.Clear();
        foreach (Transform child in ViewMessageFieldGrid.transform)
        {
            ViewMessageControllerList.Add(child.GetComponent<MessageFieldController>());
        }

        for (int i = 0; i < ViewMessageDataList.Count; i++)
        {
            ViewMessageControllerList[i].isInputField = false;
            ViewMessageControllerList[i].FieldNameLabel.text = ViewMessageDataList[i].fieldName;
            ViewMessageControllerList[i].FieldValueLabel.text = ViewMessageDataList[i].fieldValue;
        }

        ViewMessageFieldGrid.Reposition();
    }

    private void RefreshReceiveStoredProtoIndexGrid()
    {
        TransformHelper.AddOrDelItemUsingPool(StoredReceiveProtoGrid.transform, StoredReceiveProtoIndexDataDic.Count, PoolManagerListController.Instance, "ProtoIndex");

        StoredReceiveProtoIndexControllerList.Clear();
        foreach (Transform child in StoredReceiveProtoGrid.transform)
        {
            StoredReceiveProtoIndexControllerList.Add(child.GetComponent<StoredProtoController>());
        }

        StoredReceiveProtoIndexDataDic = StoredReceiveProtoIndexDataDic.OrderByDescending(item => item.Key.index).ToDictionary(item => item.Key, item => item.Value);
        for (int i = 0; i < StoredReceiveProtoIndexDataDic.Count; i++)
        {
            StoredReceiveProtoIndexControllerList[i].m_ProtoIndex = StoredReceiveProtoIndexDataDic.ToList()[i].Key;
            StoredReceiveProtoIndexControllerList[i].m_Label.text = StoredReceiveProtoIndexDataDic.ToList()[i].Key.protoIndex.ToString();
        }

        StoredReceiveProtoGrid.Reposition();
    }

    public void RefreshSendStoredProtoIndexGrid()
    {
        TransformHelper.AddOrDelItemUsingPool(StoredSendProtoGrid.transform, StoredSendProtoIndexDataDic.Count, PoolManagerListController.Instance, "ProtoIndex");

        StoredSendProtoIndexControllerList.Clear();
        foreach (Transform child in StoredSendProtoGrid.transform)
        {
            StoredSendProtoIndexControllerList.Add(child.GetComponent<StoredProtoController>());
        }

        StoredSendProtoIndexDataDic = StoredSendProtoIndexDataDic.OrderByDescending(item => item.Key.index).ToDictionary(item => item.Key, item => item.Value);
        for (int i = 0; i < StoredSendProtoIndexDataDic.Count; i++)
        {
            StoredSendProtoIndexControllerList[i].m_ProtoIndex = StoredSendProtoIndexDataDic.ToList()[i].Key;
            StoredSendProtoIndexControllerList[i].m_Label.text = StoredSendProtoIndexDataDic.ToList()[i].Key.protoIndex.ToString();
        }

        StoredSendProtoGrid.Reposition();
    }

    #endregion

    #region Warning

    public UILabel WarningLabel;
    public GameObject WarningPanel;

    public void ShowError(bool isShow, string text)
    {
        if (isShow)
        {
            StopCoroutine("DoShowWarning");

            WarningLabel.text = text;

            if (gameObject.activeInHierarchy)
            {
                StartCoroutine("DoShowWarning");
            }

            Debug.LogError(text);
        }
        else
        {
            StopCoroutine("DoShowWarning");
            WarningLabel.gameObject.SetActive(false);
        }
    }

    private IEnumerator DoShowWarning()
    {
        WarningLabel.gameObject.SetActive(true);
        yield return new WaitForSeconds(3.0f);

        WarningLabel.gameObject.SetActive(false);
    }

    #endregion

    private void OnGetProtoClick(GameObject go)
    {
        try
        {
            sendMessageProtoIndex = int.Parse(ProtoIndexLabel.text);
        }
        catch (Exception)
        {
            ShowError(true, "catch获取协议号失败");
            return;
        }

        sendMessageClass = ProtoIndexStructureTransfer.GetClassNameByIndex(sendMessageProtoIndex);
        if (string.IsNullOrEmpty(sendMessageClass))
        {
            ShowError(true, "协议号无效");
            return;
        }

        SendMessageDataList = ProtoStructureAnalyze.GetProtoItemListWithDefaultProto(sendMessageClass);

        RefreshSendMessageGrid();
    }

    private void OnSendMessageClick(GameObject go)
    {
        UpdateSendMessageData();

        Type classType = Type.GetType("qxmobile.protobuf." + sendMessageClass + ",ProtoClasses");

        object instance = Activator.CreateInstance(classType);
        if (!ProtoStructureAnalyze.SetProtoWithProtoItemList(ref instance, SendMessageDataList))
        {
            ShowError(true, "给生成的发送消息赋值失败");
            return;
        }

        SocketHelper.SendQXMessage(instance, sendMessageProtoIndex);
    }

    public void OnOpenCloseClick()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        //WarningPanel.SetActive(!WarningPanel.activeSelf);
    }

    private void OnClearProtoAreaClick(GameObject go)
    {
        if (ReceiveIndexObject.activeSelf)
        {
            StoredReceiveProtoIndexDataDic.Clear();
            RefreshReceiveStoredProtoIndexGrid();
        }
        else if (SendIndexObject.activeSelf)
        {
            StoredSendProtoIndexDataDic.Clear();
            RefreshSendStoredProtoIndexGrid();
        }
    }

    private void OnReceiveIndexClick(GameObject go)
    {
        ReceiveIndexObject.SetActive(false);
        SendIndexObject.SetActive(true);
    }

    private void OnSendIndexClick(GameObject go)
    {
        SendIndexObject.SetActive(false);
        ReceiveIndexObject.SetActive(true);
    }

    private void UpdateSendMessageData()
    {
        foreach (var protoItem in SendMessageDataList)
        {
            List<MessageFieldController> controller = SendMessageControllerList.Where(item => item.FieldNameLabel.text == protoItem.fieldName).ToList();
            if (controller != null && controller.Count() != 0)
            {
                protoItem.fieldValue = controller[0].FieldValue;
            }
            else
            {
                ShowError(true, "给生成的发送消息赋值失败");
                return;
            }
        }
    }

    void OnEnable()
    {
        RefreshReceiveStoredProtoIndexGrid();
        StopCoroutine("DoShowWarning");
        WarningLabel.gameObject.SetActive(false);
    }

    void OnDisable()
    {
        StopCoroutine("DoShowWarning");
        WarningLabel.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        ShowError(false, "");

        GetProtoHandler.m_handler -= OnGetProtoClick;
        SendMessageHandler.m_handler -= OnSendMessageClick;
        ClearProtoHandler.m_handler -= OnClearProtoAreaClick;
        ReceiveIndexHandler.m_handler -= OnReceiveIndexClick;
        SendIndexHandler.m_handler -= OnSendIndexClick;

        SocketTool.UnRegisterSocketListener(this);
    }

    void Start()
    {
        GetProtoHandler.m_handler += OnGetProtoClick;
        SendMessageHandler.m_handler += OnSendMessageClick;
        ClearProtoHandler.m_handler += OnClearProtoAreaClick;
        ReceiveIndexHandler.m_handler += OnReceiveIndexClick;
        SendIndexHandler.m_handler += OnSendIndexClick;
    }

    // Use this for initialization
    void Awake()
    {
        SendMessageFieldPrefab = Resources.Load("_Remove_When_Build/SendMessageFieldPrefab") as GameObject;
        ViewMessageFieldPrefab = Resources.Load("_Remove_When_Build/ViewMessageFieldPrefab") as GameObject;
        StoredProtoPrefab = Resources.Load("_Remove_When_Build/StoredProtoPrefab") as GameObject;

        //Initialize pool manager.
        PoolManagerListController.Instance.ItemDic.Add("ProtoSend", SendMessageFieldPrefab);

        PoolManagerListController.Instance.ItemDic.Add("ProtoView", ViewMessageFieldPrefab);

        PoolManagerListController.Instance.ItemDic.Add("ProtoIndex", StoredProtoPrefab);

        PoolManagerListController.Instance.Initialize();

        SocketTool.RegisterSocketListener(this);

        isExecuteAwake = true;
    }

    public static T DeepClone<T>(T obj)
    {
        using (var ms = new MemoryStream())
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(ms, obj);
            ms.Position = 0;

            return (T)formatter.Deserialize(ms);
        }
    }

    public bool OnSocketEvent(QXBuffer p_message)
    {
        if (p_message != null)
        {
            Debug.LogWarning("Listen message:" + p_message.m_protocol_index + " in proto tool");

            string className = ProtoIndexStructureTransfer.GetClassNameByIndex(p_message.m_protocol_index);
            if (string.IsNullOrEmpty(className))
            {
                ShowError(true, "接收到未能识别消息号的消息：" + p_message.m_protocol_index);
                return false;
            }

            MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
            QiXiongSerializer t_qx = new QiXiongSerializer();

            try
            {
                Type classType = Type.GetType("qxmobile.protobuf." + className + ",ProtoClasses");
                object instance = Activator.CreateInstance(classType);

                if (instance == null)
                {
                    ShowError(true, "生成的消息体为空，消息类型：" + className + "错误或未收入hash表中");
                    return false;
                }

                t_qx.Deserialize(t_tream, instance, instance.GetType());

                StoredReceiveProtoIndexDataDic.Add(new ProtoIndex()
                {
                    index = StoredReceiveProtoIndexDataDic.Count,
                    protoIndex = p_message.m_protocol_index
                },
                ProtoStructureAnalyze.GetProtoItemListWithProto(instance));

                RefreshReceiveStoredProtoIndexGrid();
                return true;
            }
            catch (Exception)
            {
                ShowError(true, "catch尝试生成消息失败，消息类型：" + className);
                return false;
            }
        }
        return false;
    }

    public void AddToSendProtoIndexWithRefresh(QXBuffer buffer, int startOffSet)
    {
        if (buffer.m_protocol_index == 0)
        {
            return;
        }

        string className = ProtoIndexStructureTransfer.GetClassNameByIndex(buffer.m_protocol_index);
        if (string.IsNullOrEmpty(className))
        {
            ShowError(true, "接收到未能识别消息号的消息：" + buffer.m_protocol_index);
            return;
        }

        MemoryStream t_tream = new MemoryStream(buffer.m_protocol_message, startOffSet, buffer.position - startOffSet );
        QiXiongSerializer t_qx = new QiXiongSerializer();

        try
        {
        Type classType = Type.GetType("qxmobile.protobuf." + className + ",ProtoClasses");
        object instance = Activator.CreateInstance(classType);

        if (instance == null)
        {
            ShowError(true, "生成的消息体为空，消息类型：" + className + "错误或未收入hash表中");
            return;
        }

        t_qx.Deserialize(t_tream, instance, instance.GetType());

        StoredSendProtoIndexDataDic.Add(new ProtoIndex()
        {
            index = StoredSendProtoIndexDataDic.Count,
            protoIndex = buffer.m_protocol_index
        },
        ProtoStructureAnalyze.GetProtoItemListWithProto(buffer));

        }
        catch (Exception)
        {
            ShowError(true, "catch尝试生成消息失败，消息类型：" + className);
            return;
        }
        RefreshSendStoredProtoIndexGrid();
    }
}
