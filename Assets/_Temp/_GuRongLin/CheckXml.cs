using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

public class CheckXml
{
    private static List<string> MainList = new List<string>();

    private static List<string> LesList = new List<string>();

    public static void StartCheckXML()
    {
        string filepath = "Assets/Resources/_Data/Design/" + "CheckIndxTemp.xml";

        XmlDocument mXml = new XmlDocument();

        mXml.Load(filepath);

        LoadXml(mXml);
    }

    private static void LoadXml(XmlDocument m_Xml)
    {
        XmlNodeList m_XmlNodeList = m_Xml.SelectSingleNode("dataset").ChildNodes;

        foreach (XmlNode mlist in m_XmlNodeList)
        {
            string filepath1 = "Assets/Resources/_Data/Design/" + mlist.Attributes[1].Value + ".xml";

            string filepath2 = "Assets/Resources/_Data/Design/" + mlist.Attributes[0].Value + ".xml";

            XmlDocument mXml1 = new XmlDocument();

            XmlDocument mXml2 = new XmlDocument();

            mXml1.Load(filepath1);

            mXml2.Load(filepath2);

            int index1 = int.Parse(mlist.Attributes[3].Value);

            int index2 = int.Parse(mlist.Attributes[2].Value);

            DownloadXml(mXml1, mXml2, index1, index2);
        }

        //Ends log.
        Debug.LogWarning("Check XML period2 ends.");
    }

    public static void DownloadXml(XmlDocument MainXml, XmlDocument LessXml, int Main_index, int LessIndex)
    {
        MainList.Clear();

        LesList.Clear();

        XmlNodeList m_XmlNodeList1 = MainXml.SelectSingleNode("dataset").ChildNodes;

        XmlNodeList m_XmlNodeList2 = LessXml.SelectSingleNode("dataset").ChildNodes;

        foreach (XmlNode mlist1 in m_XmlNodeList1)
        {

            if (!MainList.Contains(mlist1.Attributes[Main_index].Value))
            {
                if (mlist1.Attributes[Main_index].Value != "0" && mlist1.Attributes[Main_index].Value != "")
                {
                    MainList.Add(mlist1.Attributes[Main_index].Value);
                }
            }
        }

        foreach (XmlNode mlist2 in m_XmlNodeList2)
        {
            if (mlist2.Attributes[LessIndex].Value != "0" && mlist2.Attributes[LessIndex].Value != "")
            {
                if (!MainList.Contains(mlist2.Attributes[LessIndex].Value))
                {
                    Debug.LogError(mlist2.Attributes[LessIndex].Value + " of " + m_XmlNodeList2[0].Name + "  cant Be found  from  " + m_XmlNodeList1[0].Name);

                    break;
                }
            }
        }
    }
}
