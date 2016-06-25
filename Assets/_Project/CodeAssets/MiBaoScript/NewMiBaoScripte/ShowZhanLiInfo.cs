using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ShowZhanLiInfo : MonoBehaviour {

	public UILabel ZhanLi;

	public UILabel GongJi;
	public UILabel FangYU;
	public UILabel Life;
	
	public UILabel WQSHJS;
	public UILabel WQBJL;
	public UILabel WQBJJS;

	public UILabel SkillSHJS;	
	public UILabel SkillBJL;
	public UILabel SkillBJLJS;
	
	public UILabel WQSHDK;
	public UILabel WQBJDK;
	
	public UILabel JNSHDK;
	public UILabel JNBJDK;

	//public MibaoInfo mMiBaoinfo;

	void Start () {
	
		Init ();
	}

	public void Init()
	{
		GongJi.text = MyColorData.getColorString (3,JunZhuData.Instance().m_junzhuInfo.gongJi.ToString())+MyColorData.getColorString (6,"("+JunZhuData.Instance().m_junzhuInfo.gongjiMibao.ToString()+")");

		FangYU.text = MyColorData.getColorString (3,JunZhuData.Instance().m_junzhuInfo.fangYu.ToString())+MyColorData.getColorString (6,"("+JunZhuData.Instance().m_junzhuInfo.fangYuMibao.ToString()+")");

		Life.text = MyColorData.getColorString (3,JunZhuData.Instance().m_junzhuInfo.shengMing.ToString())+MyColorData.getColorString (6,"("+JunZhuData.Instance().m_junzhuInfo.shengMingMibao.ToString()+")");

		ZhanLi.text = MyColorData.getColorString (3,JunZhuData.Instance().m_junzhuInfo.zhanLi.ToString())+MyColorData.getColorString (6,"("+JunZhuData.Instance().m_junzhuInfo.zhanLiMibao.ToString()+")");


		WQSHJS.text  = MyColorData.getColorString(6, JunZhuData.Instance().m_junzhuInfo.wqSH.ToString()) ;

		WQBJL.text  = MyColorData.getColorString(6, JunZhuData.Instance().m_junzhuInfo.wqBJL.ToString() + "%") ;

		WQBJJS.text  = MyColorData.getColorString(6, JunZhuData.Instance().m_junzhuInfo.wqBJ.ToString()) ;

		SkillSHJS.text = MyColorData.getColorString (7, JunZhuData.Instance().m_junzhuInfo.jnSH.ToString ());

		SkillBJL.text  = MyColorData.getColorString (7 , JunZhuData.Instance().m_junzhuInfo.jnBJL.ToString()+"%");

		SkillBJLJS.text  = MyColorData.getColorString (7 , JunZhuData.Instance().m_junzhuInfo.jnBJ.ToString());

		WQSHDK.text  = MyColorData.getColorString (6 , JunZhuData.Instance().m_junzhuInfo.wqJM.ToString()); 

		WQBJDK.text = MyColorData.getColorString (6 , JunZhuData.Instance().m_junzhuInfo.wqRX.ToString()); 

		JNSHDK.text  = MyColorData.getColorString(7, JunZhuData.Instance().m_junzhuInfo.jnJM.ToString()) ;

		JNBJDK.text  = MyColorData.getColorString(7,JunZhuData.Instance().m_junzhuInfo.jnRX.ToString());
	}
	public void CloseBtn()
	{
		NewMiBaoManager.Instance().BackToFirstPage (this.gameObject);
	}
}
