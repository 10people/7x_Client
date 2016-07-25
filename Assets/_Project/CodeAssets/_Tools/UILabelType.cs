using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UILabel))]
[AddComponentMenu("Tools/UILabelType")]
public class UILabelType : MonoBehaviour 
{
	//0		一级标题 
	//1		二级标题 
	//2		按钮的黄色文字 
	//3		按钮的红色文字 
	//4		黄色影子文字 
	//6		2好字体+粗 
	//100	灰色未开启时使用 
	//101	2号类型的暗色未选择时使用
	public int m_iType = 0;
	public int m_iSize = 0;//Font Size
	public FontStyle m_iFontStype = FontStyle.Normal;//Font Style

	public bool m_redBtn = false;

	UILabel m_UILabel;
	// Use this for initialization

	public bool IsBold;
	void Awake () 
	{
		init ();
	}

	public void init ()
	{
		m_UILabel = gameObject.GetComponent<UILabel>();

		if( ClientMain.GetInstance() != null ){
			m_UILabel.trueTypeFont = ClientMain.GetInstance().m_Font;
		}
		else if( PrepareBundles.Instance() != null ){
			m_UILabel.trueTypeFont = PrepareBundles.Instance().m_font;
		}
		else{
			Debug.LogError( "Error, no font found." );
		}

	    m_UILabel.bitmapFont = null;
		switch(m_iType)
		{
		case 0:
			if(m_UILabel.text.Length == 2)
			{
				m_UILabel.spacingX = 13;

//				m_UILabel.text = "[b]" + m_UILabel.text.Substring(0,1) + " " + m_UILabel.text.Substring(1,1) + "[-]";
			}
			else
			{
				m_UILabel.spacingX = 2;
			}
			m_UILabel.text = "[b]" + m_UILabel.text + "[-]";
			m_UILabel.fontSize = 32;
			m_UILabel.fontStyle = FontStyle.Normal;
			m_UILabel.applyGradient = true;
			m_UILabel.gradientTop = new Color(1.0f, 1.0f, 225f/255f);
			m_UILabel.gradientBottom = new Color(1.0f, 181f/255f, 38f/255f);
			m_UILabel.effectStyle = UILabel.Effect.Shadow;
			m_UILabel.effectColor = Color.black;
			m_UILabel.effectDistance = new Vector2(2, 2);
			break;
		case 1:
			m_UILabel.fontSize = 31;
			m_UILabel.fontStyle = FontStyle.Normal;
			m_UILabel.applyGradient = true;
			m_UILabel.gradientTop = new Color(1.0f, 1.0f, 162f/255f);
			m_UILabel.gradientBottom = new Color(255f/255f, 148f/255f, 43f/255f);
			m_UILabel.effectStyle = UILabel.Effect.Shadow;
			m_UILabel.effectColor = Color.black;
			m_UILabel.effectDistance = new Vector2(2, 2);
			break;
		case 2:
			m_UILabel.fontSize = 25;
			m_UILabel.fontStyle = FontStyle.Normal;
			m_UILabel.applyGradient = true;
			m_UILabel.gradientTop = new Color(1.0f, 239f/255f, 163f/255f);
			m_UILabel.gradientBottom = new Color(240f/255f, 168f/255f, 64f/255f);
			m_UILabel.effectStyle = UILabel.Effect.Outline;
			m_UILabel.effectColor = new Color(37f/255f, 17f/255f, 2f/255f);
			m_UILabel.effectDistance = new Vector2(1, 1);
			break;
		case 3:
			m_UILabel.fontSize = 25;
			m_UILabel.fontStyle = FontStyle.Normal;
			m_UILabel.applyGradient = true;
			m_UILabel.gradientTop = new Color(254f/255f, 253f/255f, 160f/255f);
			m_UILabel.gradientBottom = new Color(255f/255f, 152f/255f, 75f/255f);
			m_UILabel.effectStyle = UILabel.Effect.Outline;
			m_UILabel.effectColor = new Color(97f/255f, 2f/255f, 2f/255f);
			m_UILabel.effectDistance = new Vector2(1, 1);
			break;
		case 4:
			m_UILabel.fontSize = 25;
			m_UILabel.fontStyle = FontStyle.Normal;
			m_UILabel.applyGradient = true;
			m_UILabel.gradientTop = new Color(255f/255f, 255f/255f, 162f/255f);
			m_UILabel.gradientBottom = new Color(255f/255f, 148f/255f, 43f/255f);
			m_UILabel.effectStyle = UILabel.Effect.Shadow;
			m_UILabel.effectColor = new Color(0f, 0f, 0f);
			m_UILabel.effectDistance = new Vector2(1, 1);
			break;
		case 5:
			m_UILabel.fontSize = 25;
			m_UILabel.fontStyle = FontStyle.Normal;
			m_UILabel.applyGradient = true;
			m_UILabel.gradientTop = new Color(255f/255f, 221f/255f, 88f/255f);
			m_UILabel.gradientBottom = new Color(245f/255f, 150f/255f, 1f/255f);
			m_UILabel.effectStyle = UILabel.Effect.Shadow;
			m_UILabel.effectColor = new Color(56f/255f, 0f/255f, 0f/255f);
			m_UILabel.effectDistance = new Vector2(0, 4);
			break;
		case 6:
			m_UILabel.fontSize = 25;
			m_UILabel.fontStyle = FontStyle.Normal;
			m_UILabel.applyGradient = true;
			m_UILabel.gradientTop = new Color(1.0f, 239f/255f, 163f/255f);
			m_UILabel.gradientBottom = new Color(240f/255f, 168f/255f, 64f/255f);
			m_UILabel.effectStyle = UILabel.Effect.Outline;
			m_UILabel.effectColor = new Color(37f/255f, 17f/255f, 2f/255f);
			m_UILabel.effectDistance = new Vector2(1, 1);
			break;
		case 7:
			m_UILabel.fontSize = 18;
			m_UILabel.fontStyle = FontStyle.Normal;
			m_UILabel.applyGradient = true;
			m_UILabel.gradientTop = new Color(247f/255f, 101f/255f, 56f/255f);
			m_UILabel.gradientBottom = new Color(253/255f, 220/255f, 201/255f);
			m_UILabel.effectStyle = UILabel.Effect.Outline;
			m_UILabel.effectColor = new Color(45/255f, 21/255f, 4/255f);
			m_UILabel.effectDistance = new Vector2(1, 1);
			break;
		case 8:
			if(m_UILabel.text.Length == 2)
			{
				m_UILabel.spacingX = 13;
				
				//				m_UILabel.text = "[b]" + m_UILabel.text.Substring(0,1) + " " + m_UILabel.text.Substring(1,1) + "[-]";
			}
			else
			{
				m_UILabel.spacingX = 2;
			}
			m_UILabel.text = "[b]" + m_UILabel.text + "[-]";
			m_UILabel.fontSize = 32;
			m_UILabel.fontStyle = FontStyle.Normal;
			m_UILabel.applyGradient = true;
			m_UILabel.gradientTop = new Color(248f/255f, 242f/255f, 223f/255f);
			m_UILabel.gradientBottom = new Color(252f/255f, 225f/255f, 106f/255f);
			m_UILabel.effectStyle = UILabel.Effect.Outline;
			m_UILabel.effectColor = Color.black;
			m_UILabel.effectDistance = new Vector2(2, 2);
			break;
		case 9:
			m_UILabel.fontSize = 22;
			m_UILabel.fontStyle = FontStyle.Normal;
//			m_UILabel.applyGradient = true;
//			m_UILabel.gradientTop = Global.getStringColor("ffeed1");
//			m_UILabel.gradientBottom = Global.getStringColor("efc67f");
			m_UILabel.color = Global.getStringColor("d1faff");
			m_UILabel.effectStyle = UILabel.Effect.Outline;
//			m_UILabel.effectColor = Global.getStringColor("251102");
			m_UILabel.effectColor = Global.getStringColor("185d84");
			m_UILabel.effectDistance = new Vector2(1, 1);
//			m_UILabel.spacingX = 3;
		
			break;
		case 10:
			m_UILabel.fontSize = 22;
			m_UILabel.fontStyle = FontStyle.Normal;
//			m_UILabel.color = Global.getStringColor("ffeed1");
			m_UILabel.color = Global.getStringColor("fefbed");
			m_UILabel.effectStyle = UILabel.Effect.Outline;
//			m_UILabel.effectColor = Global.getStringColor("251102");
			m_UILabel.effectColor = Global.getStringColor(m_redBtn ? "841818" : "185d84");
			m_UILabel.effectDistance = new Vector2(1, 1);
			break;
		case 11:
			m_UILabel.fontSize = 22;
			m_UILabel.fontStyle = FontStyle.Normal;
			m_UILabel.color = Global.getStringColor("a69986");
			m_UILabel.effectStyle = UILabel.Effect.Outline;
			m_UILabel.effectColor = Global.getStringColor("33120b");
			m_UILabel.effectDistance = new Vector2(1, 1);
			break;
		case 12:
			m_UILabel.fontSize = 20;
			m_UILabel.fontStyle = FontStyle.Normal;
			m_UILabel.applyGradient = true;
			m_UILabel.gradientTop = Global.getStringColor("fefea1");
			m_UILabel.gradientBottom = Global.getStringColor("ff984b");
			m_UILabel.effectStyle = UILabel.Effect.Outline;
			m_UILabel.effectColor = Global.getStringColor("251102");
			m_UILabel.effectDistance = new Vector2(1, 1);
			break;
		case 13:
			m_UILabel.fontSize = 16;
			m_UILabel.fontStyle = FontStyle.Normal;
			m_UILabel.applyGradient = true;
			m_UILabel.gradientTop = Global.getStringColor("f7c28c");
			m_UILabel.gradientBottom = Global.getStringColor("ac7a47");
			m_UILabel.effectStyle = UILabel.Effect.Outline;
			m_UILabel.effectColor = Color.black;
			m_UILabel.effectDistance = new Vector2(1, 1);
			break;
		case 100:
			m_UILabel.fontSize = 22;
			m_UILabel.fontStyle = FontStyle.Normal;
			m_UILabel.applyGradient = false;
			m_UILabel.effectStyle = UILabel.Effect.Outline;
			m_UILabel.effectColor = new Color(70f/255f, 70f/255f, 70f/255f);
			m_UILabel.effectDistance = new Vector2(1, 1);
			m_UILabel.color = Color.grey;
			break;
		case 101:
			m_UILabel.fontSize = 25;
			m_UILabel.fontStyle = FontStyle.Normal;
			m_UILabel.applyGradient = true;
			m_UILabel.gradientTop = new Color(199f/255f, 184f/255f, 115f/255f);
			m_UILabel.gradientBottom = new Color(144f/255f, 101f/255f, 39f/255f);
			m_UILabel.effectStyle = UILabel.Effect.Outline;
			m_UILabel.effectColor = new Color(13f/255f, 6f/255f, 1f/255f);
			m_UILabel.effectDistance = new Vector2(1, 1);
			break;
		case 102://level text
			m_UILabel.fontSize = 14;
			m_UILabel.fontStyle = FontStyle.Normal;
			m_UILabel.applyGradient = true;
			m_UILabel.gradientTop = new Color(254f/255f, 254f/255f, 161f/255f);
			m_UILabel.gradientBottom = new Color(1.0f, 152f/255f, 75f/255f);
			m_UILabel.effectStyle = UILabel.Effect.None;
			m_UILabel.effectColor = new Color(255f/255f, 248f/255f, 217f/255f);
			m_UILabel.effectDistance = new Vector2(1, 1);
			break;
		case 1000:
			m_UILabel.fontStyle = FontStyle.Normal;
			m_UILabel.effectStyle = UILabel.Effect.Outline;
			m_UILabel.effectColor = Color.black;
			m_UILabel.effectDistance = new Vector2(1, 1);
			break;
		}
		if(m_iSize != 0)
		{
			m_UILabel.fontSize = m_iSize;
		}
		if (m_iFontStype != FontStyle.Normal)
		{
			m_UILabel.fontStyle = m_iFontStype;
		}
	}
	void Update()
	{
		if(IsBold )
		{
			if(m_UILabel.text != "")
			{
				m_UILabel.text = "[b]"+m_UILabel.text+"[-]";
//				Debug.Log(m_UILabel.text);
				IsBold = false;
				m_UILabel.spacingX = 3;
			}
		}
	}
	public void setType(int type)
	{
		m_iType = type;
		init();
	}
}
