using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PlayerInCityManager : MonoBehaviour { //主城玩家管理类

    public static PlayerInCityManager m_PlayerInCity;
    private List<PlayersManager.OtherPlayerInfo> _PlayerInfo = new List<PlayersManager.OtherPlayerInfo>();
    public static Dictionary<int, GameObject> m_playrDic = new Dictionary<int, GameObject>();  //主城玩家集合
    public GameObject m_NameParent;
    struct ModelInfo
    {
        public int _UID;
        public int _RoleId;
        public string _Path;
    };
    List<ModelInfo> _listMainInf = new List<ModelInfo>();
    List<ModelInfo> _listSkeletonInf = new List<ModelInfo>();
    void Start(){
        m_PlayerInCity = this;
    }

	void OnDestroy(){
        m_PlayerInCity = null;

		m_playrDic.Clear();
        PlayerNameManager.DicClear();
    }

    // create player
    public void CreatePlayer(PlayersManager.OtherPlayerInfo u_info)
    {
        int size = _PlayerInfo.Count;
        for (int i = 0; i < size; i++)
        {
            if (_PlayerInfo[i]._UID == u_info._UID)
            {
                return;
            }
        }
        _PlayerInfo.Add(u_info);
        ModelInfo info = new ModelInfo();
        info._UID = u_info._UID;
        info._RoleId = u_info._RoleId;
        info._Path = ModelTemplate.GetResPathByModelId(100 + u_info._RoleId);
        _listMainInf.Add(info);
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.MODEL_PARENT),
                       ResourceLoad_Main_Callback);
      
    }
    Dictionary<int, GameObject> _MainParentDic = new Dictionary<int, GameObject>();
    public void ResourceLoad_Main_Callback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject t_gb = Instantiate(p_object) as GameObject;
        t_gb.transform.localScale = Vector3.one * 1.5f;
        t_gb.transform.Rotate(0, _PlayerRotation, 0);
        PlayerInCity tempItem = t_gb.AddComponent<PlayerInCity>();
        t_gb.AddComponent<ModelTouchShowOrMoveManagerment>();
   
        EffectTool.DisableCityOcclusion(t_gb);
        for (int i = _PlayerInfo.Count - 1; i >= 0; i--)
        {
            if (GetModelResPathByRoleId(_PlayerInfo[i]._RoleId) == _listMainInf[i]._Path && _listMainInf[i]._UID == _PlayerInfo[i]._UID)
            {
                t_gb.transform.localPosition = _PlayerInfo[i]._SeverPos;
                _PlayerRotation = Random.Range(0.0f, 180.0f);
                t_gb.name = "PlayerObject:" + _PlayerInfo[i]._MonarchId;
                tempItem.m_playerID = _PlayerInfo[i]._UID;
                m_playrDic.Add(_PlayerInfo[i]._UID, t_gb);
                PlayerNameManager.m_PlayerNamesParent = m_NameParent;
                Create_Name(_PlayerInfo[i]);
                _MainParentDic.Add(_PlayerInfo[i]._UID, t_gb);

                Global.ResourcesDotLoad(GetModelResPathByRoleId(_PlayerInfo[i]._RoleId), ResourceLoadCallback);
                _listMainInf.Remove(_listMainInf[i]);
            }
        }
    }
        float _PlayerRotation = 0;
    public void ResourceLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        for (int i = _PlayerInfo.Count - 1; i >= 0; i--)
        {
            if (GetModelResPathByRoleId(_PlayerInfo[i]._RoleId) == p_path)
            {
                LoadingPlayer(_PlayerInfo[i], p_object, _MainParentDic[_PlayerInfo[i]._UID]);
                _MainParentDic.Remove(_PlayerInfo[i]._UID);
                _PlayerInfo.Remove(_PlayerInfo[i]);
            }
        }
    }


 
    public static string GetModelResPathByRoleId( int p_role_id )
    {
        return ModelTemplate.GetResPathByModelId(100 + p_role_id );
    }

    void LoadingPlayer(PlayersManager.OtherPlayerInfo u_info, Object player_object, GameObject parent)
    {
        GameObject t_gb = Instantiate(player_object) as GameObject;
   
        // GameObject t_gb = Instantiate(player_object, , Quaternion.Euler(Vector3.zero)) as GameObject;
        parent.GetComponent<PlayerShadowManagerment>().m_Skeleton = t_gb;
        parent.GetComponent<PlayerInCity>().m_animation = t_gb.GetComponent<Animator>();
        parent.GetComponent<PlayerShadowManagerment>().m_RoleID = u_info._RoleId;
        t_gb.transform.parent = parent.transform;
        t_gb.transform.localScale = Vector3.one;
        t_gb.transform.localRotation = Quaternion.Euler(Vector3.zero);
        t_gb.transform.localPosition = Vector3.zero;
    }
    public void Reload_Skeleton(int uid,int roleid)
    {
        foreach (KeyValuePair<int,GameObject> item in m_playrDic)
        {
            if (item.Key == uid && item.Value.GetComponent<PlayerShadowManagerment>().m_RoleID != roleid)
            {
                ModelInfo info = new ModelInfo();
                info._UID = uid;
                info._RoleId = roleid;
                info._Path = ModelTemplate.GetResPathByModelId(100 + roleid);
                _listSkeletonInf.Add(info);
                Global.ResourcesDotLoad(GetModelResPathByRoleId(roleid), Reload_ResourceLoadCallback);
            }
        }
    }

    public void Reload_ResourceLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject t_gb = Instantiate(p_object) as GameObject;
   
        for (int i = _listSkeletonInf.Count - 1; i >= 0; i--)
        {
            if (GetModelResPathByRoleId(_listSkeletonInf[i]._RoleId) == p_path)
            {
                t_gb.transform.parent = m_playrDic[_listSkeletonInf[i]._UID].transform;
                Destroy(m_playrDic[_listSkeletonInf[i]._UID].GetComponent<PlayerShadowManagerment>().m_Skeleton);
                m_playrDic[_listSkeletonInf[i]._UID].GetComponent<PlayerShadowManagerment>().m_Skeleton = t_gb;
                t_gb.transform.localScale = Vector3.one;
                t_gb.transform.localPosition = Vector3.zero;
                _listSkeletonInf.Remove(_listSkeletonInf[i]);
            }
        }
    }
    void Create_Name(PlayersManager.OtherPlayerInfo u_info)
    {
       PlayerNameManager.Create_MainCity_PlayerName(u_info);
    }
    //private Object m_player_object = null;


	//更新玩家位置
    public  void UpdatePlayerPosition(SpriteMove tempMove) 
    {
        if ( !m_playrDic.ContainsKey( tempMove.uid ) )
        {
			return;
		}

        PlayerInCity tempPlayer = m_playrDic[tempMove.uid].GetComponent<PlayerInCity>();
        Vector3 targetPosition = new Vector3( tempMove.posX,tempMove.posY,tempMove.posZ );

        tempPlayer.PlayerRun( targetPosition );

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
