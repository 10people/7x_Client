using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using System.Text;
 

public class SettingUpButtonController : MonoBehaviour 
{
    public UILabel m_tiShiLabel;
    public enum ControlTYpe
    {
        E_MUSIC,
        E_AUDIO_EFFERT,
        E_POWER_GET,
        E_POWER_FULL,
        E_PAWNSHOP_FRESH,
        E_BIAOJU,
    };
    public ControlTYpe m_ButtonType;
    public GameObject m_ObjOn;
    public GameObject m_ObjOff;
    public  bool m_IsTurnOn = false;
	
	void Start ()
    {
	
	}

    void OnClick()
    {
     
        switch(m_ButtonType)
        {
            case ControlTYpe.E_MUSIC:
                {
                    //m_tiShiLabel.GetComponent<UILabel>().text = LanguageTemplate.GetText(LanguageTemplate.Text.SET_UP_1);
                    //PopUpLabelTool.Instance().AddPopLabelWatcher(m_tiShiLabel.gameObject, Vector3.zero, Vector2.zero,
                    //                                               iTween.EaseType.easeOutQuart, 0.5f, iTween.EaseType.linear, 1.0f);
                    if (!m_IsTurnOn)
                    {
                        m_IsTurnOn = true;
                       // PlayerPrefs.SetInt("MUSIC", 1);
                        SettingData.Instance().m_jsonSaved["MUSIC"].AsInt = 1;
                        m_ObjOn.SetActive(true);
                        m_ObjOff.SetActive(false);
                        ClientMain.m_sound_manager.setMaxVolume(0);
                    }
                    else
                    {
                      //  PlayerPrefs.SetInt("MUSIC", 2);
                        SettingData.Instance().m_jsonSaved["MUSIC"].AsInt = 2;
                        m_IsTurnOn = false;
                        m_ObjOn.SetActive(false);
                        m_ObjOff.SetActive(true);
                        ClientMain.m_sound_manager.setMaxVolume(1);
                    }
                }
                break;
            case ControlTYpe.E_AUDIO_EFFERT:
                {
                    //m_tiShiLabel.GetComponent<UILabel>().text = LanguageTemplate.GetText(LanguageTemplate.Text.SET_UP_2);
                    //PopUpLabelTool.Instance().AddPopLabelWatcher(m_tiShiLabel.gameObject, Vector3.zero, Vector2.zero,
                    //                                               iTween.EaseType.easeOutQuart, 0.5f, iTween.EaseType.linear, 1.0f);
                    if (!m_IsTurnOn)
                    {
                       // PlayerPrefs.SetInt("AUDIO_EFFECT",1);
                        SettingData.Instance().m_jsonSaved["AUDIO_EFFECT"].AsInt = 1;
                        m_IsTurnOn = true;
                        m_ObjOn.SetActive(true);
                        m_ObjOff.SetActive(false);
                        ClientMain.m_sound_manager.setMaxEffVolume(0);
                    }
                    else
                    {
                       // PlayerPrefs.SetInt("AUDIO_EFFECT", 2);
                        SettingData.Instance().m_jsonSaved["AUDIO_EFFECT"].AsInt = 2;
                        m_IsTurnOn = false;
                        m_ObjOn.SetActive(false);
                        m_ObjOff.SetActive(true);
                        ClientMain.m_sound_manager.setMaxEffVolume(1);
                    }
                   
                }
                break;
            case ControlTYpe.E_POWER_GET:
                {
                   // m_tiShiLabel.GetComponent<UILabel>().text = LanguageTemplate.GetText(LanguageTemplate.Text.SET_UP_1);
                    LabelMovePosManagerment.CreateMove(m_tiShiLabel.gameObject, LanguageTemplate.GetText(LanguageTemplate.Text.SET_UP_1), 40);
                    //PopUpLabelTool.Instance().AddPopLabelWatcher(m_tiShiLabel.gameObject, Vector3.zero, Vector2.zero,
                    //                                               iTween.EaseType.easeOutQuart, 0.5f, iTween.EaseType.linear, 1.0f);
                    if (!m_IsTurnOn)
                    {
                       // PlayerPrefs.SetInt("POWER_GET", 1);
                        SettingData.Instance().m_jsonSaved["POWER_GET"].AsInt = 1;
                        m_IsTurnOn = true;
                        m_ObjOn.SetActive(true);
                        m_ObjOff.SetActive(false);

                    }
                    else
                    {
                       // PlayerPrefs.SetInt("POWER_GET", 2);
                        SettingData.Instance().m_jsonSaved["POWER_GET"].AsInt = 2;
                        m_IsTurnOn = false;
                        m_ObjOn.SetActive(false);
                        m_ObjOff.SetActive(true);
                    }
                }
                break;
            case ControlTYpe.E_POWER_FULL:
                {
                    //m_tiShiLabel.GetComponent<UILabel>().text = LanguageTemplate.GetText(LanguageTemplate.Text.SET_UP_2);
                    //PopUpLabelTool.Instance().AddPopLabelWatcher(m_tiShiLabel.gameObject, Vector3.zero, Vector2.zero,
                    //                                               iTween.EaseType.easeOutQuart, 0.5f, iTween.EaseType.linear, 1.0f);
                    LabelMovePosManagerment.CreateMove(m_tiShiLabel.gameObject, LanguageTemplate.GetText(LanguageTemplate.Text.SET_UP_2), 40);
                    if (!m_IsTurnOn)
                    {
                       // PlayerPrefs.SetInt("POWER_FULL", 1);
                        SettingData.Instance().m_jsonSaved["POWER_FULL"].AsInt = 1;
                        m_IsTurnOn = true;
                        m_ObjOn.SetActive(true);
                        m_ObjOff.SetActive(false);
                    }
                    else
                    {
                       //PlayerPrefs.SetInt("POWER_FULL", 2);
                        SettingData.Instance().m_jsonSaved["POWER_FULL"].AsInt = 2;
                        m_IsTurnOn = false;
                        m_ObjOn.SetActive(false);
                        m_ObjOff.SetActive(true);
                    }
                    
                }
                break;
            case ControlTYpe.E_PAWNSHOP_FRESH:
                {
                    //m_tiShiLabel.GetComponent<UILabel>().text = LanguageTemplate.GetText(LanguageTemplate.Text.SET_UP_4);
                    //PopUpLabelTool.Instance().AddPopLabelWatcher(m_tiShiLabel.gameObject, Vector3.zero, Vector2.zero,
                    //                                               iTween.EaseType.easeOutQuart, 0.5f, iTween.EaseType.linear, 1.0f);
                    LabelMovePosManagerment.CreateMove(m_tiShiLabel.gameObject, LanguageTemplate.GetText(LanguageTemplate.Text.SET_UP_4), 40);
                    if (!m_IsTurnOn)
                    {
                       // PlayerPrefs.SetInt("PAWNSHOP_FRESH", 1);
                        SettingData.Instance().m_jsonSaved["PAWNSHOP_FRESH"].AsInt = 1;
                        m_IsTurnOn = true;
                        m_ObjOn.SetActive(true);
                        m_ObjOff.SetActive(false);
                        MainCityUIRB.LockRedAlert(9, true);
                    }
                    else
                    {
                        //PlayerPrefs.SetInt("PAWNSHOP_FRESH", 2);
                        SettingData.Instance().m_jsonSaved["PAWNSHOP_FRESH"].AsInt = 2;
                        m_IsTurnOn = false;
                        m_ObjOn.SetActive(false);
                        m_ObjOff.SetActive(true);
                        MainCityUIRB.LockRedAlert(9, false);
                    }
                }
                break;
            case ControlTYpe.E_BIAOJU:
                {
                    //m_tiShiLabel.GetComponent<UILabel>().text = LanguageTemplate.GetText(LanguageTemplate.Text.SET_UP_3);
                    //PopUpLabelTool.Instance().AddPopLabelWatcher(m_tiShiLabel.gameObject, Vector3.zero, Vector2.zero,
                    //                                               iTween.EaseType.easeOutQuart, 0.5f, iTween.EaseType.linear, 1.0f);
                    LabelMovePosManagerment.CreateMove(m_tiShiLabel.gameObject, LanguageTemplate.GetText(LanguageTemplate.Text.SET_UP_3), 40);
                    if (!m_IsTurnOn)
                    {
                        // PlayerPrefs.SetInt("PAWNSHOP_FRESH", 1);
                        SettingData.Instance().m_jsonSaved["BIAOJU"].AsInt = 1;
                        m_IsTurnOn = true;
                        m_ObjOn.SetActive(true);
                        m_ObjOff.SetActive(false);
                        MainCityUIRB.LockRedAlert(310, true);
                    }
                    else
                    {
                        //PlayerPrefs.SetInt("PAWNSHOP_FRESH", 2);
                        SettingData.Instance().m_jsonSaved["BIAOJU"].AsInt = 2;
                        m_IsTurnOn = false;
                        m_ObjOn.SetActive(false);
                        m_ObjOff.SetActive(true);
                        MainCityUIRB.LockRedAlert(310, false);
                    }
                }
                break;
            default:
                break;
        }
        //PlayerPrefs.DeleteKey("MUSIC");
        //PlayerPrefs.DeleteKey("AUDIO_EFFECT");
        //PlayerPrefs.DeleteKey("POWER_GET");
        //PlayerPrefs.DeleteKey("POWER_FULL");
        //PlayerPrefs.DeleteKey("PAWNSHOP_FRESH");

        //json["AUDIO_EFFECT"].AsInt = PlayerPrefs.GetInt("AUDIO_EFFECT");
        //json["POWER_GET"].AsInt = PlayerPrefs.GetInt("POWER_GET");
        //json["POWER_FULL"].AsInt = PlayerPrefs.GetInt("POWER_FULL");
        //json["PAWNSHOP_FRESH"].AsInt = PlayerPrefs.GetInt("POWER_FULL");

       // if (string.IsNullOrEmpty(json.ToString()))
        {
 
         //   SettingData.Instance().SendSettingsInfo("{}");
        }
       // else
        {
       //     Debug.Log("jsonjsonjsonjsonjsonjsonjson");
            SettingData.Instance().SendSettingsInfo(SettingData.Instance().m_jsonSaved.ToString());
        }
    }

    private GameObject clone = null;
    public void CreateMove(GameObject move, string content, int move_index)
    {
        if (clone == null)
        {
            clone = NGUITools.AddChild(move.transform.parent.gameObject, move);
            clone.transform.localPosition = move.transform.localPosition;
            clone.transform.localRotation = move.transform.localRotation;
            clone.transform.localScale = move.transform.localScale;
            clone.GetComponent<UILabel>().text = content;//MyColorData.getColorString(4, content);
            clone.AddComponent(typeof(TweenPosition));
            //clone.AddComponent(typeof(TweenAlpha));
            clone.GetComponent<TweenPosition>().from = move.transform.localPosition;
            clone.GetComponent<TweenPosition>().to = move.transform.localPosition + Vector3.up * move_index;
            clone.GetComponent<TweenPosition>().duration = 0.5f;
            //clone.GetComponent<TweenAlpha>().from = 1.0f;
            //clone.GetComponent<TweenAlpha>().to = 0;
            clone.GetComponent<TweenPosition>().duration = 0.8f;
            StartCoroutine(WatiFor(clone));
        }
    }


    IEnumerator WatiFor(GameObject obj)
    {
        yield return new WaitForSeconds(0.8f);
        Destroy(obj);
    }
 
}
