using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Linq;

public class PurchaseTemplate : XmlLoadManager {

	// <Purchase id="21001" name="体力1" funDesc="购买体力1次" time="1" yuanbao="50" itemId="900003" number="50" />
		
	public int id;
	
	public string itemName;
	
	public string desc;
	
	public int times;

	public int price;

	public int itemId;

	public float number;

	public int type;
	
	public static List<PurchaseTemplate> templates = new List<PurchaseTemplate>();
	

	public const int BUY_STRENGTH_ITEM_ID	= 900003;

	
	public void Log(){
		Debug.Log( "PurchaseTemplate-  id: " + id +
		          " itemName: " + itemName + 
		          " desc: " + desc + 
		          " times: " + times +
		          " price: " + price +
		          " itemId: " + itemId +
		          " number: " + number );
	}
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "Purchase.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}
	
	public static void CurLoad(ref WWW www, string path, Object obj){
		{
			templates.Clear();
		}

		XmlReader t_reader = null;
		
		if( obj != null ){
			TextAsset t_text_asset = obj as TextAsset;
			
			t_reader = XmlReader.Create( new StringReader( t_text_asset.text ) );
			
			//			Debug.Log( "Text: " + t_text_asset.text );
		}
		else{
			t_reader = XmlReader.Create( new StringReader( www.text ) );
		}
		
		bool t_has_items = true;
		
		do{
			t_has_items = t_reader.ReadToFollowing( "Purchase" );
			
			if( !t_has_items ){
				break;
			}
			
			PurchaseTemplate t_template = new PurchaseTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.itemName = t_reader.Value;
				
				t_reader.MoveToNextAttribute();
				t_template.desc = t_reader.Value;

				t_reader.MoveToNextAttribute();
				t_template.type = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.times = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.price = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.itemId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.number = float.Parse( t_reader.Value );
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}

	public static int GetBuyStrength_Max_Chance(){
		int t_chance = 0;

		for( int i = 0 ; i < templates.Count; i++ ){
			PurchaseTemplate t_item = templates[ i ];
			
			if( t_item.itemId == BUY_STRENGTH_ITEM_ID ){
				if( t_item.times > t_chance ){
					t_chance = t_item.times;
				}
			}
		}
		
		return t_chance;
	}

	public static int GetBuyStrength_Available_Times(){
		return JunZhuData.Instance().m_junzhuInfo.tiLipurchaseTime;
	}

	public static int GeBuyStrength_Price( int p_cur_buy_times ){
		for( int i = 0 ; i < templates.Count; i++ ){
			PurchaseTemplate t_item = templates[ i ];

			if( t_item.itemId == BUY_STRENGTH_ITEM_ID ){
				if( t_item.times == p_cur_buy_times ){
					return t_item.price;
				}
			}
		}

		Debug.LogError( "not found: " + p_cur_buy_times );

		return 0;
	}

	public static PurchaseTemplate GetBuyXiShuByTime(int tempTime) //元宝购买次数系数
	{
		foreach(PurchaseTemplate _template in templates)
		{
			if(_template.type == 1)
			{
				if(_template.times == tempTime)
				{
					return _template;
				}
			}
		}
		return null;
	}

	public static float GetBuyStrength_Number( int p_cur_buy_times ){
		for( int i = 0 ; i < templates.Count; i++ ){
			PurchaseTemplate t_item = templates[ i ];

			if( t_item.itemId == BUY_STRENGTH_ITEM_ID ){
				if( t_item.times == p_cur_buy_times ){
					return t_item.number;
				}
			}
		}
		
		Debug.LogError( "not found: " + p_cur_buy_times );
		
		return 0;
	}

    public static float GetBuyWorldChat_Price(int buy_Times)
    {
        PurchaseTemplate tempTemplate = templates.Where(item => item.id == 30002).FirstOrDefault();
        if (tempTemplate != null)
        {
            return tempTemplate.price * buy_Times;
        }
        else
        {
            Debug.LogError("Not found id 30002 item.");
            return -1;
        }
    }

	/// <summary>
	/// 元宝购买第几次运镖次数
	/// </summary>
	/// <returns>The purchase temp by time.</returns>
	/// <param name="time">Time.</param>
	public static PurchaseTemplate GetPurchaseTempByTime (int time)
	{
		foreach(PurchaseTemplate _template in templates)
		{
			if(_template.type == 18)
			{
				if(_template.times == time)
				{
					return _template;
				}
			}
		}
		return null;
	}

    public static PurchaseTemplate GetBuyRobCarriageTime(int nowTime)
    {
        var temp = templates.Where(item => item.type == 19 && item.times == nowTime).ToList();
        if (temp != null && temp.Count == 1)
        {
            return temp[0];
        }
        else
        {
            return null;
        }
    }
}
