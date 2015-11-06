using UnityEngine;
using System.Collections;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Security.Permissions;

public class CartTemplate : XmlLoadManager
{
    public int Quality;

    public string Name;

    public float CartSpeed;

    public float ProfitPara;

    public float FailProfit;

    public float RobProfit;

    public float ProtectTime;

    public float RecoverRate;

	public int CartTime;

	public int ShengjiCost;

	public int CartProbability;

    public static List<CartTemplate> Templates = new List<CartTemplate>();

    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "CartTemp.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
    }

    public static void CurLoad(ref WWW www, string path, Object obj)
    {
        {
            Templates.Clear();
        }

        XmlReader t_reader = null;

        if (obj != null)
        {
            TextAsset t_text_asset = obj as TextAsset;

            t_reader = XmlReader.Create(new StringReader(t_text_asset.text));
        }
        else
        {
            t_reader = XmlReader.Create(new StringReader(www.text));
        }

        bool t_has_items = true;

        do
        {
            t_has_items = t_reader.ReadToFollowing("CartTemp");

            if (!t_has_items)
            {
                break;
            }

            CartTemplate t_template = new CartTemplate();

            {
                t_reader.MoveToNextAttribute();
                t_template.Quality = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.Name = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.CartSpeed = float.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.ProfitPara = float.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.FailProfit = float.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.RobProfit = float.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.ProtectTime = float.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.RecoverRate = float.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.CartTime = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.ShengjiCost = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.CartProbability = int.Parse(t_reader.Value);

            }

            Templates.Add(t_template);
        }
        while (t_has_items);
    }

	public static CartTemplate GetCartTemplateByType (int type)
	{
		for (int i = 0;i < Templates.Count;i ++)
		{
			if (Templates[i].Quality == type)
			{
				return Templates[i];
			}
		}

		return null;
	}
}
