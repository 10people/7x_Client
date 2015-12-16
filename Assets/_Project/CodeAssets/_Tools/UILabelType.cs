using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UILabel))]
[AddComponentMenu("Tools/UILabelType")]
public class UILabelType : MonoBehaviour 
{
	public int m_iType = 0;//0一级标题 1二级标题 2按钮的黄色文字 3按钮的红色文字 4黄色影子文字 6 2好字体+粗 100灰色未开启时使用 101  2号类型的暗色未选择时使用
	public int m_iSize = 0;//Font Size
	UILabel m_UILabel;
	// Use this for initialization
	void Start () 
	{
		init ();
	}

	public void init ()
	{
		m_UILabel = gameObject.GetComponent<UILabel>();

		if( ClientMain.GetInstance() != null ){
			m_UILabel.trueTypeFont = ClientMain.GetInstance().m_Font;
		}
		else{
			m_UILabel.trueTypeFont = PrepareBundles.Instance().m_font;
		}

	    m_UILabel.bitmapFont = null;
		switch(m_iType)
		{
		case 0:
			if(m_UILabel.text.Length == 2)
			{
				m_UILabel.text = m_UILabel.text.Substring(0,1) + " " + m_UILabel.text.Substring(1,1);
			}
			m_UILabel.fontSize = 37;
			m_UILabel.fontStyle = FontStyle.Normal;
			m_UILabel.applyGradient = true;
			m_UILabel.gradientTop = new Color(1.0f, 245f/255f, 135f/255f);
			m_UILabel.gradientBottom = new Color(235f/255f, 168f/255f, 0f/255f);
			m_UILabel.effectStyle = UILabel.Effect.Shadow;
			m_UILabel.effectColor = Color.black;
			m_UILabel.effectDistance = new Vector2(3, 3);
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
		case 100:
			m_UILabel.fontSize = 25;
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
		}
		if(m_iSize != 0)
		{
			m_UILabel.fontSize = m_iSize;
		}
	}

	public void setType(int type)
	{
		m_iType = type;
		init();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
