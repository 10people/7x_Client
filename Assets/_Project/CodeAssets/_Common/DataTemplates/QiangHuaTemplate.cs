using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class QiangHuaTemplate : XmlLoadManager {

	public int id;

	public int qianghuaId;

	public int nameId;

	public int level;

	public int gongji;

	public int fangyu;

	public int shengming;

    public struct AttributeAppend
    {
        public int gongAdd;
        public int fangAdd;
        public int xueAdd;
    };
   
	public static AttributeAppend m_AttributeAddInfo = new  AttributeAppend();

	private static List<QiangHuaTemplate> templates = new List<QiangHuaTemplate>();

	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "QiangHua.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}

	private static TextAsset m_templates_text = null;
	
	public static void CurLoad( ref WWW www, string path, Object obj ){
		if( obj == null ) {
			Debug.LogError ( "Asset Not Exist: " + path );
			
			return;
		}
		
		m_templates_text = obj as TextAsset;
	}

	private static void ProcessAsset(){
		if( templates.Count > 0 ) {
			return;
		}
		
		if( m_templates_text == null ) {
			Debug.LogError( "Error, Asset Not Exist." );
			
			return;
		}
		
		XmlReader t_reader = XmlReader.Create( new StringReader( m_templates_text.text ) );

		bool t_has_items = true;
		
		do{
			t_has_items = t_reader.ReadToFollowing( "QiangHua" );
			
			if( !t_has_items ){
				break;
			}
			
			QiangHuaTemplate t_template = new QiangHuaTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.qianghuaId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.nameId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.level = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.gongji = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.fangyu = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.shengming = int.Parse( t_reader.Value );
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );

		{
			m_templates_text = null;
		}
	}

	public static  QiangHuaTemplate GetTemplateByItemId(int tempQianghuaId,int tempLevel){
		{
			ProcessAsset();
		}

		foreach(QiangHuaTemplate template in templates)
		{
			if((template.qianghuaId == tempQianghuaId) && (template.level == tempLevel))
			{
				return template;
			}
		}
		return null;
	}

    public static AttributeAppend GetAppendAttributeAddInfo(int QianghuaId, int levelUpgrade,int fewardLevel){
		{
			ProcessAsset();
		}

        List<QiangHuaTemplate> list = new List<QiangHuaTemplate>();
        int gongji = 0;
        int fangyu = 0;
        int shengming = 0;
        foreach (QiangHuaTemplate template in templates)
        {
            if (template.qianghuaId == QianghuaId && template.level == fewardLevel)
            {
                gongji = template.gongji;
                fangyu = template.fangyu;
                shengming = template.shengming;
            }


            if (template.qianghuaId == QianghuaId && template.level == levelUpgrade)
            {
                list.Add(template);
            }
        }

        int sumGongJi_add = 0;
        int sumFangYu_add = 0;
        int sumShengMing_add = 0;
        //int size = templates.Count;

        //if (levelUpgrade - 1 >= 0)
        //{

        if (list.Count > 0)
        {
          //  Debug.Log("gongjigongjigongjigongjigongji ::" + gongji);

            sumGongJi_add = list[0].gongji - gongji;
            sumFangYu_add = list[0].fangyu - fangyu;
            sumShengMing_add = list[0].shengming - shengming;
        }

        //}
        //else 
        //{
        //    sumGongJi_add = gongji;
        //    sumFangYu_add = fangyu;
        //    sumShengMing_add = shengming;
        //}

        //Debug.Log("sumFangYu_addsumFangYu_addsumFangYu_add :::" + sumFangYu_add);
        AttributeAppend AttributeAddInfo = new AttributeAppend();

        //if (EquipGrowthMaterialUseManagerment.Levelsaved == levelUpgrade)
        //{
        //    //  Debug.Log("00000000000000000000000000000000");
        //    AttributeAddInfo.gongAdd = 0;
        //    AttributeAddInfo.fangAdd = 0;
        //    AttributeAddInfo.xueAdd = 0;
        //}
        //else
        {
            // Debug.Log("11111111111111111111111111111111111111");

          //  Debug.Log("sumGongJi_addsumGongJi_addsumGongJi_add" + sumGongJi_add);
            AttributeAddInfo.gongAdd = sumGongJi_add;
            AttributeAddInfo.fangAdd = sumFangYu_add;
            AttributeAddInfo.xueAdd = sumShengMing_add;

        }
        // m_AttributeAddInfo = AttributeAddInfo;
        return AttributeAddInfo;

    }
}
