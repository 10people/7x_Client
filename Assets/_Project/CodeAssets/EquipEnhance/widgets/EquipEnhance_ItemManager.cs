using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

using qxmobile;
using qxmobile.protobuf;

public class EquipEnhance_ItemManager : MonoBehaviour {


	public UISprite m_sp_icon;

	public UISprite m_sp_frame;

	public UISprite m_sp_selected;
	public bool m_show_selected = true;
	public bool m_disable_collider_if_selected = false;

	public UISprite m_sp_add;
	public bool m_show_add = false;


	public UILabel m_lb_name;
	public bool m_show_lb_name = false;

	public UILabel m_lb_level;
	
	// dbId
	public long m_bag_item_db_id;

	public const long BAG_ITEM_EMPTY_DB_ID = 0;




    //#region UI Interaction

    //public void ClearItem(){
    //    Debug.Log( "EquipEnhance_ItemManager.ClearItem()" );

    //    UpdateItem_With_DbId( BAG_ITEM_EMPTY_DB_ID );
    //}

    //public void UpdateItem(){
    //    UpdateItem_With_DbId( m_bag_item_db_id );
    //}

    //public void UpdateItem_With_InstId( long p_inst_id ){
    //    BagItem t_item =  BagData.Instance().GetBagEquip_With_InstId( p_inst_id );

    //    m_bag_item_db_id = t_item.dbId;

    //    UpdateItem_With_BagItem( IsEmpty()? null : t_item );
    //}

    //public void UpdateItem_With_DbId( long p_item_db_id, BagItem p_item = null ){
    //    Debug.Log( "UpdateItem_With_DbId( " + p_item_db_id + " )" );

    //    m_bag_item_db_id = p_item_db_id;

    //    if( p_item == null ){
    //        p_item = IsEmpty() ? null : BagData.Instance().GetBagEquip_With_DbId( p_item_db_id );
    //    }

    //    if( p_item != null ){
    //        UpdateItem_With_BagItem( p_item );
    //    }
    //    else{
    //        Debug.LogError( "Empty Equip." );
    //    }
    //}

    //public void UpdateItem_With_BagItem( BagItem p_item ){
    //    bool t_visible = !IsEmpty();
		
    //    if( t_visible ){
    //        m_lb_level.text = "dbId: " + ( p_item == null ? "Wait 4 Update" : p_item.dbId + "" );
    //    }
		
    //    if( m_sp_icon != null )
    //        m_sp_icon.gameObject.SetActive( t_visible );
		
    //    if( m_sp_frame != null ){
    //        m_sp_frame.gameObject.SetActive( t_visible );
			
    //        if( t_visible && p_item != null ){
    //            UpdateQualitySprite( p_item.pinZhi );
    //        }
    //    }
		
    //    if( m_sp_selected != null )
    //        m_sp_selected.gameObject.SetActive( m_show_selected && t_visible );
		
    //    if( m_lb_name != null ){
    //        //Debug.Log( m_show_lb_name + " , " + t_visible );
			
    //        m_lb_name.gameObject.SetActive( m_show_lb_name && t_visible );
			
    //        if( p_item != null ){
    //            ZhuangBei t_equip = ZhuangBei.GetItem( p_item.itemId );
				
    //            t_equip.Log();
				
    //            UpdateRquipQualityName( t_equip.pinZhi, NameIdTemplate.GetName_By_NameId( t_equip.itemName ) );
    //        }
    //    }
		
    //    if( m_lb_level != null )
    //        m_lb_level.gameObject.SetActive( t_visible );
		
    //    if( m_sp_add != null )
    //        m_sp_add.gameObject.SetActive( m_show_add && IsEmpty() );
		
    //    // update collider
    //    {
    //        if( m_disable_collider_if_selected && m_sp_selected.gameObject.activeInHierarchy ){
    //            gameObject.collider.enabled = false;
    //        }
    //        else{
    //            gameObject.collider.enabled = true;
    //        }
    //    }
    //}
	
    //public void UpdateItem_With_ItemTemp( ItemTemp p_item ){
    //    if( m_sp_icon != null )
    //        m_sp_icon.gameObject.SetActive( true );
		
    //    if( m_sp_frame != null ){
    //        m_sp_frame.gameObject.SetActive( true );
			
    //        if( p_item != null ){
    //            UpdateQualitySprite( p_item.quality );
    //        }
    //    }
		
    //    if( m_sp_selected != null )
    //        m_sp_selected.gameObject.SetActive( false );
		
    //    if( m_lb_name != null ){
    //        //Debug.Log( m_show_lb_name + " , " + t_visible );
			
    //        m_lb_name.gameObject.SetActive( m_show_lb_name );
			
    //        UpdateRquipQualityName( p_item.quality, NameIdTemplate.GetName_By_NameId( p_item.itemName ) );
    //    }
		
    //    if( m_lb_level != null )
    //        m_lb_level.gameObject.SetActive( true );
		
    //    if( m_sp_add != null )
    //        m_sp_add.gameObject.SetActive( false );
		
    //    // update collider
    //    {
    //        gameObject.collider.enabled = false;
    //    }
    //}
	
    //public void UpdateItem_With_ZhuangBei( ZhuangBei p_equip ){
    //    if( m_sp_icon != null )
    //        m_sp_icon.gameObject.SetActive( true );
		
    //    if( m_sp_frame != null ){
    //        m_sp_frame.gameObject.SetActive( true );
			
    //        if( p_equip != null ){
    //            UpdateQualitySprite( p_equip.pinZhi );
    //        }
    //    }
		
    //    if( m_sp_selected != null )
    //        m_sp_selected.gameObject.SetActive( false );
		
    //    if( m_lb_name != null ){
    //        //Debug.Log( m_show_lb_name + " , " + t_visible );
			
    //        m_lb_name.gameObject.SetActive( m_show_lb_name );
			
    //        UpdateRquipQualityName( p_equip.pinZhi, NameIdTemplate.GetName_By_NameId( p_equip.itemName ) );
    //    }
		
    //    if( m_lb_level != null )
    //        m_lb_level.gameObject.SetActive( true );
		
    //    if( m_sp_add != null )
    //        m_sp_add.gameObject.SetActive( false );
		
    //    // update collider
    //    {
    //        gameObject.collider.enabled = false;
    //    }
    //}
	
    //public void UpdateItem_With_HeroProtoType( HeroProtoTypeTemplate p_hero ){
    //    if( m_sp_icon != null )
    //        m_sp_icon.gameObject.SetActive( true );
		
    //    if( m_sp_frame != null ){
    //        m_sp_frame.gameObject.SetActive( true );
			
    //        if( p_hero != null ){
    //            UpdateQualitySprite( p_hero.quality );
    //        }
    //    }
		
    //    if( m_sp_selected != null )
    //        m_sp_selected.gameObject.SetActive( false );
		
    //    if( m_lb_name != null ){
    //        //Debug.Log( m_show_lb_name + " , " + t_visible );
			
    //        m_lb_name.gameObject.SetActive( m_show_lb_name );
			
    //        UpdateRquipQualityName( p_hero.quality, NameIdTemplate.GetName_By_NameId( p_hero.heroName ) );
    //    }
		
    //    if( m_lb_level != null )
    //        m_lb_level.gameObject.SetActive( true );
		
    //    if( m_sp_add != null )
    //        m_sp_add.gameObject.SetActive( false );
		
    //    // update collider
    //    {
    //        gameObject.collider.enabled = false;
    //    }
    //}

    //#endregion



	#region Utilities

	public bool IsEmpty(){
		return m_bag_item_db_id <= BAG_ITEM_EMPTY_DB_ID;
	}

	private void UpdateQualitySprite( int p_quality ){
		switch( p_quality ){
		case 0:
		case 1:
			m_sp_frame.spriteName = "quality_white";
			break;
			
		case 2:
			m_sp_frame.spriteName = "quality_green";
			break;
			
		case 3:
			m_sp_frame.spriteName = "quality_bule";
			break;
			
		case 4:
			m_sp_frame.spriteName = "quality_purple";
			break;
			
		case 5:
			m_sp_frame.spriteName = "quality_orange";
			break;
		}
	}

	private void UpdateRquipQualityName( int p_quality, string p_name ){
		switch( p_quality ){
		case 0:
		case 1:
			m_lb_name.text = "[ffffff]";
			break;
			
		case 2:
			m_lb_name.text = "[57d452]";
			break;
			
		case 3:
			m_lb_name.text = "[08a3dc]";
			break;
			
		case 4:
			m_lb_name.text = "[aa31cc]";
			break;
			
		case 5:
			m_lb_name.text = "[db780a]";
			break;
		}
		
		m_lb_name.text = m_lb_name.text + p_name;
		
		//Debug.Log( "name: " + m_lb_name.text );
	}


	#endregion



}
