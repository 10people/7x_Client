using UnityEngine;
using System.Collections;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
using SimpleJSON;
public class SettingData : MonoBehaviour, SocketProcessor
{
    private static SettingData m_SettingData;
    public JSONClass m_jsonSaved = new JSONClass();
    public List<int> m_listSettingsInfo = new List<int>();
    public static SettingData Instance()
    {
        if (m_SettingData == null)
        {
			GameObject t_GameObject = UtilityTool.GetDontDestroyOnLoadGameObject();

            m_SettingData = t_GameObject.AddComponent<SettingData>();
        }

        return m_SettingData;
    }


    void Awake()
    {
 		SocketTool.RegisterMessageProcessor( this );
    }

	void Start ()
    {
        RequestSettingsInfo();
	}
 

	void OnDestroy()
    {
		SocketTool.UnRegisterMessageProcessor( this );
	}
    public void RequestSettingsInfo( )
    {
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_SETTINGS_GET);
	}
    public void SendSettingsInfo(string ss)
    {
        m_listSettingsInfo.Clear();
        JSONNode tempNode = JSON.Parse(ss);
        m_listSettingsInfo.Add(tempNode["MUSIC"].AsInt);

        m_listSettingsInfo.Add(tempNode["AUDIO_EFFECT"].AsInt);

        m_listSettingsInfo.Add(tempNode["POWER_GET"].AsInt);

        m_listSettingsInfo.Add(tempNode["POWER_FULL"].AsInt);

        m_listSettingsInfo.Add(tempNode["PAWNSHOP_FRESH"].AsInt);

        m_listSettingsInfo.Add(tempNode["BIAOJU"].AsInt);
        
        //for (int i = 0; i < m_listSettingsInfo.Count; i++)
        //{
        //    Debug.Log("SettingData.Instance().m_listSettingsInfoSettingData.Instance().m_listSettingsInfo ::: " + SettingData.Instance().m_listSettingsInfo[i]);
            
        //}
        MemoryStream tempStream = new MemoryStream();
        QiXiongSerializer t_serializer = new QiXiongSerializer();
        ConfSave save = new ConfSave();
        save.json = ss;
        t_serializer.Serialize(tempStream, save);

        byte[] t_protof = tempStream.ToArray();

        t_protof = tempStream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_SETTINGS_SAVE, ref t_protof);
    }
    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message != null)
		{
            switch (p_message.m_protocol_index)
			{
                case ProtoIndexes.S_SETTINGS:
	            {
                    MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                    QiXiongSerializer t_qx = new QiXiongSerializer();

                    ConfGet tempInfo = new ConfGet();


                    t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());
                    if (string.IsNullOrEmpty(tempInfo.json))
                    {

                   
                    }
                    else
                    {
                        if (tempInfo.json.Equals("{}"))
                        {
                            m_listSettingsInfo.Clear();
                            ClientMain.m_sound_manager.setMaxVolume(1);
                            ClientMain.m_sound_manager.setMaxEffVolume(1);
                            int[] tt = { 0, 0, 0, 0, 0,0 };
                            m_listSettingsInfo.AddRange(tt);
                            m_jsonSaved["MUSIC"].AsInt = m_listSettingsInfo[0];
                            m_jsonSaved["AUDIO_EFFECT"].AsInt = m_listSettingsInfo[1];
                            m_jsonSaved["POWER_GET"].AsInt = m_listSettingsInfo[2];
                            m_jsonSaved["POWER_FULL"].AsInt = m_listSettingsInfo[3];
                            m_jsonSaved["PAWNSHOP_FRESH"].AsInt = m_listSettingsInfo[4];
                            m_jsonSaved["BIAOJU"].AsInt = m_listSettingsInfo[5];
                            
                        }
                        else
                        {
                            m_listSettingsInfo.Clear();
                            JSONNode tempNode = JSON.Parse(tempInfo.json);
                            m_listSettingsInfo.Add(tempNode["MUSIC"].AsInt);

                            m_listSettingsInfo.Add(tempNode["AUDIO_EFFECT"].AsInt);

                            m_listSettingsInfo.Add(tempNode["POWER_GET"].AsInt);

                            m_listSettingsInfo.Add(tempNode["POWER_FULL"].AsInt);

                            m_listSettingsInfo.Add(tempNode["PAWNSHOP_FRESH"].AsInt);

                            m_listSettingsInfo.Add(tempNode["BIAOJU"].AsInt);
                            if (m_listSettingsInfo[0] == 1)
                            {
                              ClientMain.m_sound_manager.setMaxVolume(0);
                            }
                            else 
                            {
                               ClientMain.m_sound_manager.setMaxVolume(1);
                            }

                            if (m_listSettingsInfo[1] == 1)
                            {
                                ClientMain.m_sound_manager.setMaxEffVolume(0);
                            }
                            else
                            {
                                ClientMain.m_sound_manager.setMaxEffVolume(1);
                            }


                            if (m_listSettingsInfo[4] == 1)
                            {
                                MainCityUIRB.LockRedAlert(310, true);
                            }
                            else
                            {
                                MainCityUIRB.LockRedAlert(310, false);   
                            }

                            if (m_listSettingsInfo[5] == 1)
                            {
                                MainCityUIRB.LockRedAlert(9, true);
                            }
                            else
                            {
                                MainCityUIRB.LockRedAlert(9, false);
                            }
 

                            m_jsonSaved["MUSIC"].AsInt = m_listSettingsInfo[0];
                            m_jsonSaved["AUDIO_EFFECT"].AsInt = m_listSettingsInfo[1];
                            m_jsonSaved["POWER_GET"].AsInt = m_listSettingsInfo[2];
                            m_jsonSaved["POWER_FULL"].AsInt = m_listSettingsInfo[3];
                            m_jsonSaved["PAWNSHOP_FRESH"].AsInt = m_listSettingsInfo[4];
                            m_jsonSaved["BIAOJU"].AsInt = m_listSettingsInfo[5];
                         
                        }   
                    }
	                return true;
	            }
				 
	        	default: return false;
            }
        }
        return false;
    }
}