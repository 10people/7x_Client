using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PlayerInCityManager : MonoBehaviour { //主城玩家管理类

    private static PlayerInCityManager m_instance;
    private List<EnterScene> _PlayerInfo = new List<EnterScene>();
    public static Dictionary<int, GameObject> m_playrDic = new Dictionary<int, GameObject>();  //主城玩家集合
    public GameObject m_NameParent;
	public static PlayerInCityManager Instance(){
		if( m_instance == null ){
			Debug.LogError( "PlayerInCityManager.instance == null." );
		}

		return m_instance;
	}

    void Start(){
        m_instance = this;
    }

	void OnDestroy(){
		m_instance = null;

		m_playrDic.Clear();
	}

	// create player
    public void CreatePlayer( EnterScene p_enter_scene_player ) {
      //  Debug.Log("PlayerInCityManager.CreatePlayer: " + p_enter_scene_player.uid + " " + p_enter_scene_player.roleId + " " + p_enter_scene_player.senderName );

        int size = _PlayerInfo.Count;
        for (int i = 0; i < size; i++)
        {
            if (_PlayerInfo[i].uid == p_enter_scene_player.uid)
            {
                return;
            }
        }
        _PlayerInfo.Add(p_enter_scene_player);
        // TODO: replace with new res.
        //		Global.ResourcesDotLoad( ModelTemplate.GetResPathByModelId( -1 ),
        //	                        ResourceLoadCallback );
        //		Global.ResourcesDotLoad(ModelTemplate.GetResPathByModelId(100 + CityGlobalData.m_king_model_Id),
        //                               LoadCallback);
        //		Global.ResourcesDotLoad(ModelTemplate.GetResPathByModelId(100 + CityGlobalData.m_king_model_Id),
        //                               ResourceLoadCallback);

        //	Debug.Log("p_enter_scene_player.roleId +1)p_enter_scene_player.roleId +1)p_enter_scene_player.roleId +1)" + p_enter_scene_player.roleId);

        //if (p_enter_scene_player.roleId == 0)
        //{
        //    p_enter_scene_player.roleId = 1;
        //}
        //else if (p_enter_scene_player.roleId > 4)
        //{
        //    p_enter_scene_player.roleId = 4;
        //}
       // Debug.Log("NAMeNAMeNAMeNAMe ::" + p_enter_scene_player.senderName + "IDIDIDIDIDIDIDIDIDIDIDIDIDIDIDID ::" + p_enter_scene_player.roleId);
        Global.ResourcesDotLoad( GetModelResPathByRoleId(p_enter_scene_player.roleId ), ResourceLoadCallback );
    }

    public static string GetModelResPathByRoleId( int p_role_id ) {
        return ModelTemplate.GetResPathByModelId(100 + p_role_id );
    }
    
    void LoadingPlayer(EnterScene p_enter_scene,Object player_object)
    {
        if (!m_playrDic.ContainsKey(p_enter_scene.uid))
        {
            Vector3 t_pos = new Vector3(p_enter_scene.posX,
                                        p_enter_scene.posY,
                                        p_enter_scene.posZ);
           GameObject t_gb = Instantiate(player_object, t_pos, Quaternion.Euler(Vector3.zero)) as GameObject;

            //GameObject t_gb = Instantiate(player_object) as GameObject;
            t_gb.name = "PlayerObject:" + p_enter_scene.jzId;
           //t_gb.GetComponent<CharacterController>().enabled = false;
            t_gb.transform.parent = this.transform;
          //  Debug.Log(t_gb.transform.localPosition);
            

            //t_gb.transform.position = t_pos;
            //Debug.Log(t_gb.transform.localPosition);
            //Debug.Log(" t_gb.transform.namet_gb.transform.namet_gb.transform.namet_gb.transform.namet_gb.transform.namet_gb.transform.name " + t_gb.transform.name);

            //Debug.Log("t_post_post_post_post_post_post_post_post_post_post_post_post_post_post_post_pos" + t_pos);
          //  Debug.Log(" t_gb.transform t_gb.transform t_gb.transform X ::" + t_gb.transform.localPosition.x + "YYYYYY ::" + t_gb.transform.localPosition.y + "ZZZZZZZZZZZZZZZZZ ::" + t_gb.transform.localPosition.z);
            t_gb.transform.localScale = Vector3.one*1.5f;

            PlayerInCity tempItem = t_gb.AddComponent<PlayerInCity>();

            tempItem.m_playerID = p_enter_scene.uid;
            m_playrDic.Add(p_enter_scene.uid, t_gb);
            PlayerNameManager.m_PlayerNamesParent = m_NameParent;
            PlayerNameManager.CreatePlayerName(p_enter_scene);
            EffectTool.DisableCityOcclusion(t_gb);


        }
        else
        {
            player_object = null;
        }
      
	}

	//private Object m_player_object = null;

	public void ResourceLoadCallback(ref WWW p_www, string p_path, Object p_object ){
        //m_player_object = p_object;
       // Debug.Log("p_pathp_pathp_pathp_pathp_pathp_pathp_path :::" + p_path);

        for( int i = _PlayerInfo.Count - 1; i >= 0; i--) {
            EnterScene t_info = _PlayerInfo[i];

            if ( GetModelResPathByRoleId(t_info.roleId ) == p_path ){
                LoadingPlayer( t_info, p_object );

                _PlayerInfo.Remove(t_info);
            }
        }
	}

	//更新玩家位置
    public static void UpdatePlayerPosition(SpriteMove tempMove) 
    {
        if ( !m_playrDic.ContainsKey( tempMove.uid ) ){
			return;
		}

        PlayerInCity tempPlayer = m_playrDic[tempMove.uid].GetComponent<PlayerInCity>();
        //if (CityGlobalData.m_isAllianceTenentsScene)
        //{
        //    tempMove.posY = 2.4f;
        //}
        //else
        //{
        //    tempMove.posY = 0.4f;
        //}

        Vector3 targetPosition = new Vector3( tempMove.posX,tempMove.posY,tempMove.posZ );

        tempPlayer.PlayerRun( targetPosition );

     }

    public void UpdatePlayerPosition()
    {
        //foreach (GameObject tempPlayer in m_playrDic.Values)
        //{
        //    if (tempPlayer != null && tempPlayer.GetComponent<NavMeshAgent>().enabled)
        //    {
        //        if (tempPlayer.GetComponent<PlayerInCity>().m_Agent.remainingDistance == 0)
        //        {
        //            tempPlayer.GetComponent<PlayerInCity>().PlayerStop();
        //        }
        //    }
        //}
    }

	//删除玩家
    public static void DestroyPlayer(ExitScene tempPlayer) 
    {
        if (tempPlayer != null)
        {
            if (m_playrDic.ContainsKey(tempPlayer.uid))
            {
                Destroy(m_playrDic[tempPlayer.uid]);
            }

            m_playrDic.Remove(tempPlayer.uid);
        }
    }
}
