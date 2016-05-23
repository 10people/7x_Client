using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
public class BaoShiOpenTemplate : XmlLoadManager
{
    public int buWei;
 
    public string con1Desc;

    public string con2Desc;

    public string con3Desc;

    public string con4Desc;

    
    public string con5Desc;
 

    private static List<BaoShiOpenTemplate> tempTasks = new List<BaoShiOpenTemplate>();



    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "BaoshiOpen.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
    }

    private static TextAsset m_templates_text = null;

    public static void CurLoad(ref WWW www, string path, Object obj)
    {
        if (obj == null)
        {
            Debug.LogError("Asset Not Exist: " + path);

            return;
        }

        m_templates_text = obj as TextAsset;
    }

    private static void ProcessAsset()
    {
        if (tempTasks.Count > 0)
        {
            return;
        }

        if (m_templates_text == null)
        {
            Debug.LogError("Error, Asset Not Exist.");

            return;
        }

        XmlReader t_reader = XmlReader.Create(new StringReader(m_templates_text.text));

        bool t_has_items = true;

        do
        {
            t_has_items = t_reader.ReadToFollowing("BaoshiOpen");

            if (!t_has_items)
            {
                break;
            }

            BaoShiOpenTemplate t_template = new BaoShiOpenTemplate();

            {
                t_reader.MoveToNextAttribute();
                t_template.buWei = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.con1Desc = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.con2Desc = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.con3Desc = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.con4Desc = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.con5Desc = t_reader.Value;

                tempTasks.Add(t_template);
       
            }
        }
        while (t_has_items);

        {
            m_templates_text = null;
        }
    }

    public static string GetTemplatesBuweiAndNum(int buwei,int num)
    {
        {
            ProcessAsset();
        }
        for (int i = 0; i < tempTasks.Count; i++)
        {
            if (tempTasks[i].buWei == buwei)
            {
                switch (num)
                {
                    case 0:
                        {
                            return tempTasks[i].con1Desc;
                        }
                        break;
                    case 1:
                        {
                            return tempTasks[i].con2Desc;
                          
                        }
                        break;
                    case 2:
                        {
                            return tempTasks[i].con3Desc;
                        }
                        break;
                    case 3:
                        {
                            return tempTasks[i].con4Desc;
                        }
                        break;
                    case 4:
                        {
                            return tempTasks[i].con5Desc;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        return null;
    }
}
