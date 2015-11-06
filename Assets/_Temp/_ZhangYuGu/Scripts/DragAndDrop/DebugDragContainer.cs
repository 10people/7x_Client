using UnityEngine;
using System.Collections;

public class DebugDragContainer : MonoBehaviour {

	public GameObject m_scroll_root;

	public int t_c_w = 180;

	public int t_c_h = 210;

	// Use this for initialization
	void Start () {
		ResortRoot();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDrop (GameObject go){
		Debug.Log( gameObject + ".OnDrop( " + go + " )" );

		if( go != null ){
			DebugDragItem t_item = go.GetComponent<DebugDragItem>();

			if( t_item != null ){
				Debug.Log( "Item id: " + t_item.m_item_id );

				int t_child = m_scroll_root.transform.childCount;

				for( int i = 0; i < t_child; i++ ){
					GameObject t_gb = m_scroll_root.transform.GetChild( i ).gameObject;

					DebugDragItem t_child_item = t_gb.GetComponent<DebugDragItem>();

					if( t_child_item != null ){
						if( t_child_item.m_item_id == t_item.m_item_id ){
							t_gb.SetActive( false );

							Destroy( t_gb );

							ResortRoot();

							return;
						}
					}
				}
			}
		}
	}

	void ResortRoot(){
		int t_child = m_scroll_root.transform.childCount;
				
		Debug.Log( "ResortRoot.child count:" + t_child );

		int t_index = 0;

		for( int i = 0; i < t_child; i++ ){
			GameObject t_gb = m_scroll_root.transform.GetChild( i ).gameObject;

			if( !t_gb.activeSelf ){
				continue;
			}

			t_gb.transform.localPosition = new Vector3( 
			                                           ( t_index % 2 ) * t_c_w, 
			                                           ( -t_index / 2 ) * t_c_h,
													 	0 );

			t_index++;
		}
	}
}
