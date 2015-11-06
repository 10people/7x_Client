using UnityEngine;
using System.Collections;

public class GetNeedlevel : MonoBehaviour {

	private int  nedlv;
	private int curlevel;

	void Start () {
	
		//MapData mymapdata = GetComponent<MapData>();
		//GetPveTempID getId = GetComponent<GetPveTempID>();

		if(MapData.mapinstance.Lv.ContainsKey(Pve_Level_Info.CurLev))
		{
			UILabel nedlevel = gameObject.GetComponent<UILabel> ();
			nedlv = MapData.mapinstance.Lv[Pve_Level_Info.CurLev].s_level;
			if(MapData.mapinstance.Lv[Pve_Level_Info.CurLev].type == 1&&MapData.mapinstance.Lv[Pve_Level_Info.CurLev].s_pass == true)
			{
				if(nedlv < 5)
				{
					nedlv = 5;
				}
			}
			nedlevel.text = nedlv.ToString ();
		}

	}

}
