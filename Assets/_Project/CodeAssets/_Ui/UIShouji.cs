using UnityEngine;
using System.Collections;

public class UIShouji : MonoBehaviour 
{
	public static UIShouji m_UIShouji;
	public static bool m_isPlayShouji = true;
	public bool m_isPlay = false;
	public UISprite m_spriteJindu;
	public UILabel m_labelJindu;
	public UISprite m_spriteManzu;
	public UILabel m_labelDes;
	private GameObject m_icon;

	public IconSampleManager m_IconSampleManager0;
	public GameObject m_Taozhuang;
	public GameObject m_MibaoShengxing;
	public UISprite m_SpriteMibaoShengxing;
	public GameObject m_MibaoHecheng;
	public UISprite m_SpriteMibaoHecheng;
	public GameObject m_MibaoSkill;
	public UISprite m_SpriteMibaoSkill;
	public float playTime;

	private int m_iFirstManzuID = -1;
	private int m_iAnimState = -1;
	private int m_iNum = 0;
	// Use this for initialization
	void Awake ()
	{
		m_UIShouji = this;
		DontDestroyOnLoad(gameObject);
		gameObject.SetActive(false);
	}

	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
		switch(m_iAnimState)
		{
		case 0:
			m_spriteManzu.gameObject.transform.localScale = new Vector3(3f - m_iNum * 0.2f, 3f - m_iNum * 0.2f, 3f - m_iNum * 0.2f);
			m_spriteManzu.gameObject.transform.localPosition = new Vector3(38f, -137 - m_iNum, 0);
			if(m_iNum == 10)
			{
				m_iAnimState = 1;
				m_iNum = 0;
			}
			m_iNum ++;
			break;
		case 1:

			break;
		case 2:
			break;
		}
	}

	public void setData(int id, int type, int cur, int max, string des)
	{
		gameObject.SetActive(true);
		m_iNum = 0;
		m_iAnimState = -1;
		if(cur >= max)
		{
			m_spriteManzu.gameObject.SetActive(true);
			m_spriteManzu.spriteName = "Type" + type;
			if(m_iFirstManzuID == -1 || m_iFirstManzuID != id)
			{
				m_iFirstManzuID = id;
				m_spriteManzu.depth = 50;
				m_spriteManzu.gameObject.transform.localScale = new Vector3(3f, 3f, 3f);
				m_spriteManzu.gameObject.transform.localPosition = new Vector3(38f, -137, 0);
				m_iAnimState = 0;
			}
		}
		else
		{
			m_spriteManzu.gameObject.SetActive(false);
		}
		m_labelDes.text = des;
		m_isPlay = true;
		m_labelJindu.text = "[ff0000]" + cur + "[-] / " + max;
		int w = Global.getBili(142, cur, max);
		if(w > 142)
		{
			w = 142;
		}
		if(w < 2)
		{
			m_spriteJindu.gameObject.SetActive(false);
		}
		else
		{
			m_spriteJindu.gameObject.SetActive(true);
			m_spriteJindu.SetDimensions(w, m_spriteJindu.height);
		}

		gameObject.SetActive(true);
		playTime = Time.realtimeSinceStartup;
		switch(type)
		{
		case 0:
			m_icon = m_IconSampleManager0.gameObject;
			m_IconSampleManager0.gameObject.SetActive(true);
			m_IconSampleManager0.SetIconByID(id);
			m_IconSampleManager0.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
			break;
		case 1:
			m_icon = m_Taozhuang;
			m_Taozhuang.SetActive(true);
			break;
		case 2:
			m_icon = m_MibaoShengxing;
			m_MibaoShengxing.SetActive(true);
			m_SpriteMibaoShengxing.spriteName = id.ToString();
			break;
		case 3:
			m_icon = m_MibaoHecheng;
			m_MibaoHecheng.SetActive(true);
			m_SpriteMibaoHecheng.spriteName = id.ToString();
			break;
		case 4:
			m_icon = m_MibaoSkill;
			m_MibaoSkill.SetActive(true);
			m_SpriteMibaoSkill.spriteName = id.ToString();
			break;
		}
	}

	public void close()
	{
		m_isPlay = false;
		if(m_icon != null)
		{
			m_icon.SetActive(false);
		}
	}
}
