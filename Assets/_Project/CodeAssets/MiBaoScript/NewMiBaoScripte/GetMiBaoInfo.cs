using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class GetMiBaoInfo : MonoBehaviour {
	
	public	int gongji;
	public	int shengming ;
	public int fangyu ;
	public int wqshjs;
	public int wqshdk ;
	public int wqbjjs ;
	public int wqbjdk ;
	public int jnshjs ;
	public int jnshdk ;
	public int jnbjjs ;
	public int jnbjdk ;
	public int MiBaoid;

	public int MiBaoLv;

	public int MiBaoStar;

	public static GetMiBaoInfo mGetMiBaoInfo;

	public static GetMiBaoInfo Initance()
	{
		if(!mGetMiBaoInfo )
		{
			mGetMiBaoInfo = (GetMiBaoInfo)GameObject.FindObjectOfType(typeof(GetMiBaoInfo));
		}
		return mGetMiBaoInfo;
	}

	public void addAttrToX(string x1, int addValue){
		char []x = x1.ToCharArray ();
		//Debug.Log ("x[0] = "+x[0]);
		switch(x[0]){
		case 'A':
			wqshjs += addValue;

			return;
		case 'B':
			wqshdk += addValue;
			return;
		case 'C':
			wqbjjs += addValue;
			return;
		case 'D':
			wqbjdk += addValue;
			return;
		case 'E':
			jnshjs += addValue;
			return;
		case 'F':
			jnshdk += addValue;
			return;
		case 'G':
			jnbjjs += addValue;
			return;
		case 'H':
			jnbjdk += addValue;
			return;
		default:
			break;
		}
	}
	/// <summary>
	/// ReturnType 1返回攻击 2 返回防御 3 返回生命 4 返回战力；	/// </summary>
	/// <returns>The suan zhan li.</returns>
	/// <param name="id">Identifier.</param>
	/// <param name="level">Level.</param>
	/// <param name="Stars">Stars.</param>
	/// <param name="ReturnType">Return type.</param>
	public int JiSuanZhanLi(int id, int level, int Stars,int ReturnType) // 计算秘宝战力  直接调用
	{
		MiBaoid = id;
		MiBaoLv = level;
		MiBaoStar = Stars;
		int chengZhang = (int)MiBaoStarTemp.getMiBaoStarTempBystar (MiBaoStar).chengzhang;
		int gj = getGongji ();
		int gjRate = getGongjiRate ();

		int Fy = getFangyu ();
		int FyRate = getFangyuRate ();

		int SM = getShengming ();
		int SMRate = getShengmingRate ();

		int gongJi = clacMibaoAttr(chengZhang, gj, gjRate, MiBaoLv);
		int fangYu = clacMibaoAttr(chengZhang, Fy, FyRate, MiBaoLv);
		int shengMing = clacMibaoAttr(chengZhang, SM, SMRate, MiBaoLv);

		int zhanLi = calcMibaoZhanLi(gongJi, fangYu, shengMing, 
		                             MiBaoid, MiBaoLv);
		switch(ReturnType) // 1返回攻击 2 返回防御 3 返回生命 4 返回战力；
		{
		case 1:
			return gongJi;
			break;
		case 2:
			return fangYu;
			break;
		case 3:
			return shengMing;
			break;
		case 4:
			return zhanLi;
			break;
		default:
			return 0;
			break;
		}
	}

	public int calcMibaoZhanLi(int gongJi, int fangYu,
	                           int shengMing, int mibaoid, int mibaolevel) {
//		JunZhuInfo jz = new JunZhuInfo();
//		jz.fangyu = fangYu;
//		jz.shengMingMax = shengMing;
//		jz.gongji = gongJi;
		/*
		 * 秘宝等级达到一定程度，会有额外属性加成
		 */
		GetMiBAoExtrattrit ();
		int zhanLi = getZhanli(gongJi,fangYu,shengMing);
		return zhanLi;
	}

	public int clacMibaoAttr(float chengZhang, double attrValue, double attrRate, int level) {
		return (int)(chengZhang * attrRate * level + attrValue );
	}
	public void  GetMiBAoExtrattrit()
	{
		int L_lv = 0;
		if((MiBaoLv/10) >0)
		{
			L_lv = (MiBaoLv/10)*10;
		}
		else
		{
			return;
		}
		MiBaoExtrattributeTemplate mMiBaoExtrattributeTemplate = MiBaoExtrattributeTemplate.GetMiBaoExtrattributeTemplate_By_Id_and_level (MiBaoid,L_lv);
		//Debug.Log ("mMiBaoExtrattributeTemplate.shuxing = "+mMiBaoExtrattributeTemplate.shuxing);
		//Debug.Log ("mMiBaoExtrattributeTemplate.Num = "+mMiBaoExtrattributeTemplate.Num);
		addAttrToX (mMiBaoExtrattributeTemplate.shuxing,mMiBaoExtrattributeTemplate.Num);
	}


	public int getZhanli(int _gj,int _fy,int  _sm) {
		int gongji = _gj;
		int shengming = _sm;
		int fangyu = _fy;
		int wqshjs = getWqSH();
		int wqshdk = getWqJM();
		int wqbjjs = getWqBJ();
		int wqbjdk = getWqRX();
		int jnshjs = getJnSH();
		int jnshdk = getJnJM();
		int jnbjjs = getJnBJ();
		int jnbjdk = getJnRX();
//		Debug.Log("参与战力值计算的各项属性值:[gongji=" + gongji + ", fangyu=" + fangyu
//						+ ", shengming=" + shengming + ", wqshjs=" + wqshjs
//						+ ", wqshdk=" + wqshdk + ", wqbjjs=" + wqbjjs + ", wqbjdk="
//						+ wqbjdk + ", jnshjs=" + jnshjs + ", jnshdk=" + jnshdk
//						+ ", jnbjjs=" + jnbjjs + ", jnbjdk=" + jnbjdk + "]");
		float m = (float)CanshuTemplate.GetValueByKey("ZHANLI_M");
		float c = (float)CanshuTemplate.GetValueByKey("ZHANLI_C");
		float r = (float)CanshuTemplate.GetValueByKey("ZHANLI_R");
		float k1 = (float)CanshuTemplate.GetValueByKey("ZHANLI_k1");
		float k2 = (float)CanshuTemplate.GetValueByKey("ZHANLI_k2");
		float m1 = (float)CanshuTemplate.GetValueByKey("ZHANLI_m1");
		float m2 = (float)CanshuTemplate.GetValueByKey("ZHANLI_m2");
		float puGongQuan = (float)CanshuTemplate.GetValueByKey("JUNZHU_PUGONG_QUANZHONG");//CanShu.JUNZHU_PUGONG_QUANZHONG;
		float puGongBei = (float)CanshuTemplate.GetValueByKey("JUNZHU_PUGONG_BEISHU");//CanShu.JUNZHU_PUGONG_BEISHU;
		float jiNengQuan = (float)CanshuTemplate.GetValueByKey("JUNZHU_JINENG_QUANZHONG");// CanShu.JUNZHU_JINENG_QUANZHONG;
		float jiNengBei = (float)CanshuTemplate.GetValueByKey("JUNZHU_JINENG_BEISHU");// CanShu.JUNZHU_JINENG_BEISHU;
		float zhanliL = (float)CanshuTemplate.GetValueByKey("ZHANLI_L");// CanShu.ZHANLI_L;
		float baoJiLv = 0.2f;
		float w = gongji + fangyu + shengming / c;
		
		float wqk = 0; // 默认值设为0
		
		float jnk = 0; // 默认值设为1
		
		float mb = 1; // 设置默认值设为1
		
		float wq = puGongQuan * puGongBei * (1 + wqk) * mb;
		
		float wz = (1 + baoJiLv + wqshjs / zhanliL + wqbjjs / zhanliL)
			* (1 + baoJiLv + wqshdk / zhanliL + wqbjdk / zhanliL);
		
		float jN = jiNengQuan * jiNengBei *(1 + jnk) * mb;
		
		float jZ = (1 + baoJiLv + jnshjs / zhanliL + jnbjjs / zhanliL)
			* (1 + baoJiLv + jnshdk / zhanliL + jnbjdk / zhanliL);
		float result = m * w * Mathf.Pow(wq * wz + jN * jZ, (1 / r));
		int zhanli = (int) Mathf.RoundToInt(result);
		//Debug.Log("name 是 :{}的君主或者其他npc的综合战力是:{}"+zhanli);
		return zhanli;
	}



	public int getGongji()
	{
		//MiBaoXmlTemp mMiBaoXML = MiBaoXmlTemp.getMiBaoXmlTempById (MiBaoid);
		MiBaoStarTemp mMiBaoStarTemp = MiBaoStarTemp.getMiBaoStarTempBystar (MiBaoStar);
		return mMiBaoStarTemp.GongJi;
	}
	public int getGongjiRate()
	{
		MiBaoXmlTemp mMiBaoXML = MiBaoXmlTemp.getMiBaoXmlTempById (MiBaoid);
		return (int)mMiBaoXML.gongjiRate;
	}

	public int getFangyuRate()
	{
		MiBaoXmlTemp mMiBaoXML = MiBaoXmlTemp.getMiBaoXmlTempById (MiBaoid);
		return (int)mMiBaoXML.fangyuRate;
	}

	public int getShengming()
	{
		MiBaoStarTemp mMiBaoStarTemp = MiBaoStarTemp.getMiBaoStarTempBystar (MiBaoStar);
		return mMiBaoStarTemp.ShengMing;
	}
	public int getShengmingRate()
	{
		MiBaoXmlTemp mMiBaoXML = MiBaoXmlTemp.getMiBaoXmlTempById (MiBaoid);
		return (int)mMiBaoXML.shengmingRate;
	}

	public int getFangyu()
	{
		MiBaoStarTemp mMiBaoStarTemp = MiBaoStarTemp.getMiBaoStarTempBystar (MiBaoStar);
		return mMiBaoStarTemp.FangYu;
	}
	public int getWqSH()
	{
		MiBaoXmlTemp mMiBaoXML = MiBaoXmlTemp.getMiBaoXmlTempById (MiBaoid);
		return mMiBaoXML.wqSH;
	}
	public int getWqJM()
	{
		MiBaoXmlTemp mMiBaoXML = MiBaoXmlTemp.getMiBaoXmlTempById (MiBaoid);
		return mMiBaoXML.wqJM;
	}
	public int  getWqBJ()
	{
		MiBaoXmlTemp mMiBaoXML = MiBaoXmlTemp.getMiBaoXmlTempById (MiBaoid);
		return mMiBaoXML.wqBJ;
	}
	public int getWqRX()
	{
		MiBaoXmlTemp mMiBaoXML = MiBaoXmlTemp.getMiBaoXmlTempById (MiBaoid);
		return mMiBaoXML.wqRX;
	}
	public int getJnSH()
	{
		MiBaoXmlTemp mMiBaoXML = MiBaoXmlTemp.getMiBaoXmlTempById (MiBaoid);
		return mMiBaoXML.jnSH;
	}
	public int getJnJM()
	{
		MiBaoXmlTemp mMiBaoXML = MiBaoXmlTemp.getMiBaoXmlTempById (MiBaoid);
		return mMiBaoXML.jnJM;
	}
	public int getJnBJ()
	{
		MiBaoXmlTemp mMiBaoXML = MiBaoXmlTemp.getMiBaoXmlTempById (MiBaoid);
		return mMiBaoXML.jnBJ;
	}
	public int getJnRX()
	{
		MiBaoXmlTemp mMiBaoXML = MiBaoXmlTemp.getMiBaoXmlTempById (MiBaoid);
		return mMiBaoXML.jnRX;
	}
}
