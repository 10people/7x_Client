using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PlayerModelController : MonoBehaviour
{
    //additional operation used for main city tip window.
    public static bool IsDoAdditionalOperation = false;
    public delegate void GameObjectDelegate(GameObject loadedObject);
    public static GameObjectDelegate m_Delegate;
    public static bool m_isNavMesh = false;
    public static PlayerModelController m_playerModelController;

    public bool m_isSetPos = false;
    private bool isMoving = false;

    public Joystick m_joystick;

    //  public PlayerNameInCity m_playerName;

    public Transform m_playerNamePosition;

    public float m_speed = 10.0f;

    public float m_speedY;

    public Camera m_mainCamera;
    public UIRoot m_root;

    public delegate void ShowUILayer();

    public event ShowUILayer m_showLayer;

    private Vector3 m_currenPosition;

    private Vector3 m_targetPosition;

    private CharacterController m_character;

    public GameObject m_ObjHero;

    private Transform m_transform;

    public Animator m_animator;

    public Vector3 m_moveDir;

    [HideInInspector]
    public NavMeshAgent m_agent;

    private float m_timeInterval = 0.0f;

    public int m_iMoveToNpcID = -1;

    private float m_fPDistance = 0f;
    static Vector3 m_PlayerPostion = new Vector3(0, 0.4f, 0);
    private int BigHouseId = 0;

    private Vector3 vec_TargetPos = new Vector3();

    void Awake()
    {
        m_playerModelController = this;

        //  CreatePlayerModel();
        //		Debug.Log ("PlayerModelCol");
        //        JunZhuData.RequestJunZhuInfo();
    }

    void Start()
    {
        StartCoroutine(ManualStart());
    }

    IEnumerator ManualStart()
    {
        while (m_ObjHero == null)
        {
            yield return new WaitForEndOfFrame();
        }

        while (MainCityRoot.Instance().m_objMainUI == null)
        {
            yield return new WaitForEndOfFrame();
        }

        m_PlayerPostion = m_ObjHero.transform.position;
        m_joystick = MainCityRoot.Instance().m_objMainUI.GetComponentInChildren<Joystick>();

        m_root = MainCityRoot.Instance().m_objMainUI.GetComponent<UIRoot>();
        InitWithGlobalData();
    }

    public void CreatePlayerModel(Object p_object)
    {
        //Global.ResourcesDotLoad(ModelTemplate.GetResPathByModelId(100 + CityGlobalData.m_king_model_Id),
        //                        ResourceLoadCallback);
        m_ObjHero = Instantiate(p_object) as GameObject;

        m_ObjHero.SetActive(true);

        if (JunZhuData.Instance().m_junzhuInfo.lianMengId > 0)
        {
            m_ObjHero.AddComponent<TenementPortal>();
        }
        switch (FunctionWindowsCreateManagerment.IsCurrentJunZhuScene())
        {
            case 0:
                {

                    if (CityGlobalData.m_CreateRoleCurrent)
                    {
                        CityGlobalData.m_CreateRoleCurrent = false;
                        m_ObjHero.transform.position = new Vector3(-26.0f, 169.4f, -177.0f);
                    }
                    else
                    {
                        if (!FunctionWindowsCreateManagerment.IsChangeScene() && FunctionWindowsCreateManagerment.GetCurrentPosition() != Vector3.zero)
                        {
                            m_ObjHero.transform.position = FunctionWindowsCreateManagerment.GetCurrentPosition();
                            if (m_ObjHero.transform.position.y < 150 || m_ObjHero.transform.position.y > 177)
                            {
                                m_ObjHero.transform.position = new Vector3(-26.0f, 169.4f, -177.0f);
                            }
                        }
                        else
                        {
                            m_ObjHero.transform.position = new Vector3(-26.0f, 169.4f, -177.0f);
                        }
                    }
                }
                break;
            case 1:
                {//FunctionWindowsCreateManagerment.IsCurrentJunZhuID(JunZhuData.Instance().m_junzhuInfo.id) && */
                    if (!FunctionWindowsCreateManagerment.IsChangeScene() && FunctionWindowsCreateManagerment.GetCurrentPosition() != Vector3.zero)
                    {
                        m_ObjHero.transform.position = FunctionWindowsCreateManagerment.GetCurrentPosition();
                        if (m_ObjHero.transform.position.y > 177.0f || m_ObjHero.transform.position.y < 150.0f)
                        {
                            m_ObjHero.transform.position = new Vector3(-23.0f, 169.4f, -105.0f);
                        }
                    }
                    else
                    {
                        m_ObjHero.transform.position = new Vector3(-23.0f, 169.4f, -105.0f);
                    }
                }
                break;
            case 2:
                {
                    //if (FunctionWindowsCreateManagerment.IsCurrentJunZhuID(JunZhuData.Instance().m_junzhuInfo.id) && !FunctionWindowsCreateManagerment.IsChangeScene())
                    //{
                    //    m_ObjHero.transform.position = FunctionWindowsCreateManagerment.GetCurrentPosition();
                    //}
                    //else
                    //{
                    //    m_ObjHero.transform.position = new Vector3(6.0f, 2.40f, -30.0f);
                    //}
                }
                break;
            default:
                break;
        }
        m_isSetPos = true;
        m_ObjHero.transform.localRotation = Quaternion.Euler(Vector3.zero);

        m_ObjHero.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);

        m_character = m_ObjHero.GetComponent<CharacterController>();

        m_agent = m_ObjHero.GetComponent<NavMeshAgent>();

        m_agent.enabled = false;

        m_transform = m_ObjHero.transform;

        // Debug.Log("m_transform.position.ym_transform.position.y" + m_transform.position.y);

        m_animator = m_ObjHero.GetComponent<Animator>();



        m_currenPosition = m_transform.position;

        m_mainCamera.GetComponent<CameraRun>().target = m_ObjHero.transform;

        m_mainCamera.GetComponent<CameraRun>().setCameraPos();

        ////Execute void delegate.
        //if (m_VoidDelegate != null)
        //{
        //    m_VoidDelegate();
        //    m_VoidDelegate = null;
        //}
        if (JunZhuData.Instance().m_junzhuInfo.lianMengId <= 0)
        {
            SceneGuideManager.Instance().ShowSceneGuide(1010001);
        }
        else
        {
            SceneGuideManager.Instance().ShowSceneGuide(1020001);
        }


        SendPlayerData();
    }

    public void ResourceLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        m_ObjHero = Instantiate(p_object) as GameObject;

        m_ObjHero.SetActive(true);

        if (JunZhuData.Instance().m_junzhuInfo.lianMengId > 0)
        {
            m_ObjHero.AddComponent<TenementPortal>();
        }
        switch (FunctionWindowsCreateManagerment.IsCurrentJunZhuScene())
        {
            case 0:
                {

                    if (CityGlobalData.m_CreateRoleCurrent)
                    {
                        CityGlobalData.m_CreateRoleCurrent = false;
                        m_ObjHero.transform.position = new Vector3(-26.0f, 169.4f, -177.0f);
                    }
                    else
                    {
                        if (!FunctionWindowsCreateManagerment.IsChangeScene() && FunctionWindowsCreateManagerment.GetCurrentPosition() != Vector3.zero)
                        {
                            m_ObjHero.transform.position = FunctionWindowsCreateManagerment.GetCurrentPosition();
                            if (m_ObjHero.transform.position.y < 150 || m_ObjHero.transform.position.y > 177)
                            {
                                m_ObjHero.transform.position = new Vector3(-26.0f, 169.4f, -177.0f);
                            }
                        }
                        else
                        {
                            m_ObjHero.transform.position = new Vector3(-26.0f, 169.4f, -177.0f);
                        }
                    }
                }
                break;
            case 1:
                {//FunctionWindowsCreateManagerment.IsCurrentJunZhuID(JunZhuData.Instance().m_junzhuInfo.id) && */
                    if (!FunctionWindowsCreateManagerment.IsChangeScene() && FunctionWindowsCreateManagerment.GetCurrentPosition() != Vector3.zero)
                    {
                        m_ObjHero.transform.position = FunctionWindowsCreateManagerment.GetCurrentPosition();
                        if (m_ObjHero.transform.position.y > 177.0f || m_ObjHero.transform.position.y < 150.0f)
                        {
                            m_ObjHero.transform.position = new Vector3(-23.0f, 169.4f, -105.0f);
                        }
                    }
                    else
                    {
                        m_ObjHero.transform.position = new Vector3(-23.0f, 169.4f, -105.0f);
                    }
                }
                break;
            case 2:
                {
                    //if (FunctionWindowsCreateManagerment.IsCurrentJunZhuID(JunZhuData.Instance().m_junzhuInfo.id) && !FunctionWindowsCreateManagerment.IsChangeScene())
                    //{
                    //    m_ObjHero.transform.position = FunctionWindowsCreateManagerment.GetCurrentPosition();
                    //}
                    //else
                    //{
                    //    m_ObjHero.transform.position = new Vector3(6.0f, 2.40f, -30.0f);
                    //}
                }
                break;
            default:
                break;
        }
        m_isSetPos = true;
        m_ObjHero.transform.localRotation = Quaternion.Euler(Vector3.zero);

        m_ObjHero.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);

        m_character = m_ObjHero.GetComponent<CharacterController>();

        m_agent = m_ObjHero.GetComponent<NavMeshAgent>();

        m_agent.enabled = false;

        m_transform = m_ObjHero.transform;

        // Debug.Log("m_transform.position.ym_transform.position.y" + m_transform.position.y);

        m_animator = m_ObjHero.GetComponent<Animator>();



        m_currenPosition = m_transform.position;

        m_mainCamera.GetComponent<CameraRun>().target = m_ObjHero.transform;

        m_mainCamera.GetComponent<CameraRun>().setCameraPos();

        ////Execute void delegate.
        //if (m_VoidDelegate != null)
        //{
        //    m_VoidDelegate();
        //    m_VoidDelegate = null;
        //}
        if (JunZhuData.Instance().m_junzhuInfo.lianMengId <= 0)
        {
            SceneGuideManager.Instance().ShowSceneGuide(1010001);
        }
        else
        {
            SceneGuideManager.Instance().ShowSceneGuide(1020001);
        }


        SendPlayerData();
    }

    public bool AddFunaction()
    {
        return (m_showLayer == null) ? (true) : (false);
    }

    //void SetMyselfNamePosition() //更新自己的名字
    //{
    //    m_playerName.transform.localPosition = new Vector3(0f, 0f, 0f);
    //    m_playerName.Init(JunZhuData.Instance().m_junzhuInfo.name, "1", "");

    //}

    // Update is called once per frame

    void UploadPlayerPosition()
    {

        if (m_currenPosition != m_targetPosition) //玩家有位移 发送数据
        {
            SpriteMove tempPositon = new SpriteMove();
            tempPositon.posX = m_targetPosition.x;
            tempPositon.posY = m_targetPosition.y;
            tempPositon.posZ = m_targetPosition.z;
            //tempPositon.uid = JunZhuData.Instance().m_junzhuInfo.id;

            MemoryStream t_tream = new MemoryStream();
            QiXiongSerializer t_qx = new QiXiongSerializer();
            t_qx.Serialize(t_tream, tempPositon);

            // m_currenPosition = m_transform.position;
            m_currenPosition = m_targetPosition;

            byte[] t_protof;
            t_protof = t_tream.ToArray();
            SocketTool.Instance().SendSocketMessage(ProtoIndexes.Sprite_Move, ref t_protof, false);

            //m_playerName.SetMyselfName(m_transform.position); //用于NPC位置；
        }
    }

    Vector3 Pos_Save = Vector3.zero;
    public void SelfNavigation(Vector3 tempPosition) //自动导航
    {
        m_isNavMesh = true;
        m_agent.enabled = true;
        PlayerSelfNameManagerment.AutoNav();
        vec_TargetPos = tempPosition;

        m_agent.enabled = true;

        //move character
        if (!isMoving)
        {
            //            isMoving = true;
        }

        if (!inTurning && Vector3.Distance(vec_TargetPos, Pos_Save) > 0)
        {
            Pos_Save = vec_TargetPos;
            StartCoroutine(turnToDestination(tempPosition));
        }
    }

    protected bool inTurning;

    IEnumerator turnToDestination(Vector3 targetPosition)
    {

        m_agent.Stop();
        m_agent.acceleration = 10000;
        inTurning = true;
        for (; inTurning == true;)
        {
            Vector3 oldangle = m_ObjHero.transform.eulerAngles;

            m_ObjHero.transform.forward = targetPosition - m_ObjHero.transform.position;

            float tar = m_ObjHero.transform.eulerAngles.y;

            //float sp = 120 * Time.deltaTime; 

            float sp = 1080 * Time.deltaTime;

            float angle = Mathf.MoveTowardsAngle(oldangle.y, tar, sp);

            m_ObjHero.transform.eulerAngles = new Vector3(0, angle, 0);

            if (Mathf.Abs(tar - oldangle.y) < 20)
            {
                break;
            }

            yield return new WaitForEndOfFrame();

        }

        yield return new WaitForSeconds(0.01f);
        inTurning = false;
        m_animator.SetBool("inRun", true);

        m_agent.speed = m_speed;

        m_agent.Resume();

        m_agent.SetDestination(targetPosition);

        m_currenPosition = Vector3.zero;

        CityGlobalData.m_selfNavigation = true;

        if (Vector3.Distance(targetPosition, m_ObjHero.transform.position) < 0.1f)
        {
            m_fPDistance = 10;
        }
    }

    public void NavgationToNpc(Vector3 tempPosition)
    {
        SelfNavigation(tempPosition);
    }

    public void StopPlayerNavigation() //停止自动导航
    {
        PlayerSelfNameManagerment.DestroyAutoNav();
        m_iMoveToNpcID = -1;
        Pos_Save = Vector3.zero;
        m_fPDistance = 0f;
        if (!CityGlobalData.m_selfNavigation) return;

        CityGlobalData.m_selfNavigation = false;

        //stay character, disable right buttom btns.
        m_animator.SetBool("inRun", false);
        //  m_animator.Play("zhuchengdile");
        //        if (isMoving)
        //        {
        //            isMoving = false;
        //
        //            MainCityUIRB.IsCanClickButtons = true;
        //            MainCityUI.m_MainCityUI.m_MainCityUIRB.SetPanel(true);
        //            UIYindao.m_UIYindao.setOpenUIEff();
        //        }

        m_agent.Stop();
        m_isNavMesh = false;
        //Tenement Nav Break
        //if (CityGlobalData.m_isAllianceTenentsScene)
        //{
        //    CityGlobalData.m_isNavToHome = false;
        //    CityGlobalData.m_isNavToAllianCity = false;
        //}
        //if (!CityGlobalData.m_isAllianceTenentsScene)
        //{
        //    CityGlobalData.m_isNavToAllianCityToTenement = false;
        //}


        m_character.enabled = true;

        m_agent.enabled = false;

        m_agent.enabled = true;
        if (m_showLayer != null)
        {
            m_showLayer = null;
        }
    }

    public void StopPlayerNavigationMove() //停止自动导航但不停止动作
    {
        m_iMoveToNpcID = -1;
        Pos_Save = Vector3.zero;
        m_fPDistance = 0f;
        PlayerSelfNameManagerment.DestroyAutoNav();
        if (!CityGlobalData.m_selfNavigation) return;

        CityGlobalData.m_selfNavigation = false;
        //        if (isMoving)
        //        {
        //            isMoving = false;
        //
        //            MainCityUIRB.IsCanClickButtons = true;
        //            MainCityUI.m_MainCityUI.m_MainCityUIRB.SetPanel(true);
        //            UIYindao.m_UIYindao.setOpenUIEff();
        //        }

        m_agent.Stop();
        m_isNavMesh = false;
        m_character.enabled = true;

        m_agent.enabled = false;

        m_agent.enabled = true;
        if (m_showLayer != null)
        {
            m_showLayer = null;
        }
    }
    private bool _IsSetPos = false;
    void Update()
    {
        if (m_ObjHero == null || m_joystick == null)
        {
            return;
        }

        //update audio listener positon
        if (m_ObjHero != null)
        {
            ClientMain.m_ClientMainObj.transform.position = new Vector3(m_ObjHero.transform.position.x, m_ObjHero.transform.position.y, m_ObjHero.transform.position.z - 1);
        }
        m_timeInterval += Time.deltaTime;

        //0.24秒刷新一次玩家位置  提交数据        
        if (m_timeInterval >= 0.05f && m_ObjHero != null)
        {
            m_timeInterval = 0.0f;
            m_targetPosition = m_transform.position;
            UploadPlayerPosition();
        }
        //navigation end execute.
        m_moveDir = MainCityUI.m_PlayerPlace == MainCityUI.PlayerPlace.MainCity ? m_joystick.m_uiOffset.normalized : Vector3.zero;
        if (CityGlobalData.m_selfNavigation == true)
        {
            if (m_agent.remainingDistance <= 2.1f && m_fPDistance != 0f)
            {
                if (m_iMoveToNpcID != -1 && !MainCityUI.IsWindowsExist())
                {
                    if (m_iMoveToNpcID != 10000 && Vector3.Distance(vec_TargetPos, m_ObjHero.transform.position) <= 3.0f)
                    {
                        if ((m_iMoveToNpcID > 1000 || m_iMoveToNpcID == 10000) && m_iMoveToNpcID != 1151 && m_iMoveToNpcID < 10060)
                        {
                            TidyTenementNpcInfo();
                        }
                        //else if (CityGlobalData.m_isAllianceTenentsScene)
                        //{
                        //    CityGlobalData.m_isAllianceTenentsScene = false;
                        //    CityGlobalData.m_isAllianceScene = true;
                        //    //  SceneManager.EnterAllianceCity();
                        //    SceneManager.EnterMainCity();
                        //}
                        else
                        {
                            if (m_iMoveToNpcID >= 10060)
                            {
                                if (m_iMoveToNpcID == 10060)
                                {
                                    CityGlobalData.m_iAllianceTenentsSceneNum = 0;

                                }
                                else if (m_iMoveToNpcID == 10061)
                                {
                                    CityGlobalData.m_iAllianceTenentsSceneNum = 1;

                                }
                                else if (m_iMoveToNpcID == 10062)
                                {
                                    CityGlobalData.m_iAllianceTenentsSceneNum = 2;

                                }
                                else if (m_iMoveToNpcID == 10063)
                                {
                                    CityGlobalData.m_iAllianceTenentsSceneNum = 3;
                                }
                                FunctionWindowsCreateManagerment.SetFenChengNum(CityGlobalData.m_iAllianceTenentsSceneNum);
                                CityGlobalData.m_isAllianceScene = false;
                                CityGlobalData.m_isAllianceTenentsScene = true;
                                //  SceneManager.EnterAllianceCityTenentsCityOne();
                            }
                            else
                            {
                                if (m_iMoveToNpcID == 801)
                                {
                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.EMAIL),
                                   EmailLoadCallback);
                                }
                                else
                                {
                                    TidyNpcInfo();
                                }
                            }
                        }
                    }
                    else if (m_iMoveToNpcID == 10000)
                    {
                        TidyNpcInfo();
                    }
                }

                StopPlayerNavigation();
            }
            else
            {
                m_fPDistance = m_agent.remainingDistance;
            }
            if (m_moveDir == Vector3.zero)
            {
                return;
            }
        }

        //character control horizontal
        if (m_moveDir != Vector3.zero)
        {
            MoveTurnToDestination(m_moveDir);
            // m_transform.forward = m_moveDir;
            m_animator.SetBool("inRun", true);
            if (CityGlobalData.m_selfNavigation)
            {
                Global.m_sPanelWantRun = "";
                Global.m_isShowBianqiang = false;
                Global.m_sBianqiangClick = "";
                StopPlayerNavigationMove();
            }

            if (!isMoving)
            {
                isMoving = true;

                MainCityUIRB.IsCanClickButtons = false;
                MainCityUI.m_MainCityUI.m_MainCityUIRB.SetPanel(false);
                UIYindao.m_UIYindao.setCloseUIEff();
            }
        }
        else
        {
            //stop character, enable right buttom btns.
            m_animator.SetBool("inRun", false);

            if (isMoving)
            {
                isMoving = false;

                MainCityUIRB.IsCanClickButtons = true;
                MainCityUI.m_MainCityUI.m_MainCityUIRB.SetPanel(true);
                UIYindao.m_UIYindao.setOpenUIEff();
            }
        }
        //character control vertical
        if (!m_character.isGrounded)
        {
            m_moveDir.y -= m_speedY;
        }
        m_PlayerPostion = m_ObjHero.transform.position;
        string value = m_ObjHero.transform.position.x.ToString() + ":" + m_ObjHero.transform.position.y.ToString() + ":" + m_ObjHero.transform.position.z.ToString();
        //if (_IsSetPos)
        {
            PlayerPrefs.SetString("IsCurrentJunZhuPos", value);
        }
    }

    //void FixedUpdate()
    //{
    //    Debug.Log("FixedUpdate. m_ObjHero.transform.position :" + m_ObjHero.transform.position);
    //}

    //void LateUpdate() {
    //    Debug.Log("LateUpdate. m_ObjHero.transform.position :" + m_ObjHero.transform.position);
    //}


    public void EmailLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = Instantiate(p_object) as GameObject;

        MainCityUI.TryAddToObjectList(tempObject);

        if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
            CityGlobalData.m_isRightGuide = true;
        }
    }
    private GameObject camObject;
    private GameObject offObject;
    private GameObject angleObject;
    void MoveTurnToDestination(Vector3 targetPosition)
    {
        m_agent.enabled = true;
        if (offObject == null)
        {
            offObject = new GameObject();
        }

        offObject.transform.localPosition = targetPosition;

        offObject.transform.localEulerAngles = new Vector3(0, 0, 0);

        offObject.transform.localScale = new Vector3(1, 1, 1);

        if (camObject == null)
        {
            camObject = new GameObject();
        }

        camObject.transform.localPosition = new Vector3(0, 0, 0);

        camObject.transform.localEulerAngles = Vector3.zero;

        camObject.transform.localScale = new Vector3(1, 1, 1);

        offObject.transform.parent = camObject.transform;

        camObject.transform.localEulerAngles = new Vector3(0, Camera.main.transform.localEulerAngles.y, 0);

        targetPosition = offObject.transform.position;



        if (angleObject == null)
        {
            angleObject = new GameObject();
        }

        angleObject.transform.localScale = new Vector3(1, 1, 1);

        angleObject.transform.localPosition = Vector3.zero;

        angleObject.transform.eulerAngles = m_transform.transform.eulerAngles;

        Vector3 oldangle = angleObject.transform.eulerAngles;

        angleObject.transform.forward = targetPosition;

        float tar = angleObject.transform.eulerAngles.y;

        float sp = 1080 * Time.deltaTime;

        float angle = Mathf.MoveTowardsAngle(oldangle.y, tar, sp);

        angleObject.transform.eulerAngles = new Vector3(0, angle, 0);


        {

            //Vector3 oldangle = transform.eulerAngles;

            //transform.forward = targetPosition - transform.position;

            //float tar = transform.eulerAngles.y;

            //float sp = 1080 * Time.deltaTime;

            //float angle = Mathf.MoveTowardsAngle(oldangle.y, tar, sp);

            //transform.eulerAngles = new Vector3(0, angle, 0);

            //if(objTemp == null)
            //objTemp = new GameObject();
            //m_ObjHero.transform.parent = objTemp.transform;
            //objTemp.transform.localEulerAngles = new Vector3(0, Camera.main.transform.localEulerAngles.y, 0);
            //  m_ObjHero.transform.localEulerAngles =  new Vector3(0, Camera.main.transform.localEulerAngles.y, 0);

            Vector3 sp2 = targetPosition * m_speed * Time.deltaTime;

            m_transform.forward = sp2;

            m_character.Move(sp2);
        }

    }

    public void TidyNpcInfo()
    {
        if (NpcManager.m_NpcManager.m_npcObjectItemDic.ContainsKey(m_iMoveToNpcID))
        {
            if (NpcManager.m_NpcManager.m_npcObjectItemDic[m_iMoveToNpcID].GetComponent<NpcAnimationManagerment>() != null)
            {

                NpcManager.m_NpcManager.m_npcObjectItemDic[m_iMoveToNpcID].GetComponent<NpcAnimationManagerment>().setDialogAnimation();
            }
        }
        for (int i = 0; i < ZhuXianTemp.tempTasks.Count; i++)
        {
            if (FreshGuide.Instance().IsActive(ZhuXianTemp.tempTasks[i].id) && ZhuXianTemp.tempTasks[i].doneType == 4)
            {
                if (PlayerModelController.m_playerModelController.m_iMoveToNpcID == ZhuXianTemp.tempTasks[i].NpcId)
                {
                    ClientMain.m_ClientMain.m_UIDialogSystem.setOpenDialog(m_iMoveToNpcID);
                    return;
                }
                break;
            }

        }

        for (int i = 0; i < FunctionOpenTemp.templates.Count; i++)
        {
            if (FunctionOpenTemp.templates[i].m_iNpcID == PlayerModelController.m_playerModelController.m_iMoveToNpcID)
            {
                //show all function ui when open all function.
                if (ConfigTool.GetBool(ConfigTool.CONST_OPEN_ALLTHE_FUNCTION))
                {
                    ShowUIInfo(FunctionOpenTemp.templates[i]);
                    return;
                }

                if (FunctionOpenTemp.templates[i].Level < 0 && FunctionOpenTemp.templates[i].m_iMissionID < 0)
                {
                    if (FunctionOpenTemp.templates[i].type == 2)
                    {
                        ShowUIInfo(FunctionOpenTemp.templates[i]);
                        return;
                    }
                }
                else
                {
                    if (FunctionOpenTemp.templates[i].Level >= 0 && FunctionOpenTemp.templates[i].m_iMissionID < 0)
                    {
                        if (JunZhuData.Instance().m_junzhuInfo.level >= FunctionOpenTemp.templates[i].Level)
                        {


                            if (FunctionOpenTemp.templates[i].type == 2)
                            {
                                ShowUIInfo(FunctionOpenTemp.templates[i]);
                                return;
                            }
                            return;
                        }

                    }
                    else if (FunctionOpenTemp.templates[i].Level < 0 && FunctionOpenTemp.templates[i].m_iMissionID > 0)
                    {
                        if (TaskData.Instance.m_TaskInfoDic.ContainsKey(FunctionOpenTemp.templates[i].m_iMissionID))
                        {
                            if (TaskData.Instance.m_TaskInfoDic[FunctionOpenTemp.templates[i].m_iMissionID].progress == -1)
                            {
                                if (FunctionOpenTemp.templates[i].type == 2)
                                {
                                    ShowUIInfo(FunctionOpenTemp.templates[i]);
                                    return;
                                }
                            }
                        }
                        else
                        {
                            if (FunctionOpenTemp.templates[i].type == 2)
                            {
                                ShowUIInfo(FunctionOpenTemp.templates[i]);
                                return;

                            }

                        }
                    }
                }
            }
        }
    }

    private GameObject equipObject;

    void ShowUIInfo(FunctionOpenTemp template)
    {
        TaskData.Instance.SendData(template.m_iMissionOpenID, 1);

        //return if window opened.
        if (MainCityUI.IsWindowsExist())
        {
            return;
        }

        switch (template.m_iID)
        {
            
            default:
                break;
        }
    }

    

    void OnDisable()
    {
        CityGlobalData.m_selfNavigation = false;
        StopAllCoroutines();
    }

    void OnDestroy()
    {
        m_playerModelController = null;
    }

    void SendPlayerData()
    {

        EnterScene tempEnterScene = new EnterScene();

        tempEnterScene.senderName = SystemInfo.deviceName;

        tempEnterScene.uid = 0;

        tempEnterScene.posX = m_transform.position.x;

        tempEnterScene.posY = m_transform.position.y;

        tempEnterScene.posZ = m_transform.position.z;

        MemoryStream tempStream = new MemoryStream();

        QiXiongSerializer t_qx = new QiXiongSerializer();

        t_qx.Serialize(tempStream, tempEnterScene);

        byte[] t_protof;

        t_protof = tempStream.ToArray();

        SocketTool.Instance().SendSocketMessage(ProtoIndexes.Enter_Scene, ref t_protof);

        {
            //  Debug.Log( "--- Scene Tag --- PlayerModelController.SendPlayerData --- State_LEAGUEOFCITY" );

            PlayerState t_state = new PlayerState();

            t_state.s_state = State.State_LEAGUEOFCITY;

            SocketHelper.SendQXMessage(t_state, ProtoIndexes.PLAYER_STATE_REPORT);
        }

        //        Debug.Log("SendPlayerData. m_ObjHero.transform.position :" + m_ObjHero.transform.position);

    }

    void InitWithGlobalData()
    {
        //		Debug.Log ("PlayerModelCol");
        NGUIDebug.ClearLogs();

        LimitActivityData.Instance.RequestData();//request limit activity info.

        MiBaoGlobleData.Instance();

        GetAllianceData.Instance();
        //CheckXml.Instance ();

        BagData.Instance();

        EquipsOfBody.Instance();

        TopUpLoadManagerment.Instance();

        SettingData.Instance();

        BlockedData.Instance();

        EquipSuoData.Instance();

        NewEmailData.Instance().LoadEmailPrefab();

        //		EmailData.Instance.EmailDataReq();

        //        QXTanBaoData.Instance().TBInfoReq();

        //        YunBiaoData.Instance.YunBiaoInfoReq();

        //        LueDuoData.Instance.LueDuoInfoReq();

        //        FuWenData.Instance.FuWenDataReq();
    }


    public void TidyTenementNpcInfo()
    {
        if (BigHouseId > 0)
        {
            TenementManagerment.Instance.NavgationToTenement(BigHouseId);
        }
        else
        {
            for (int i = 0; i < NpcCityTemplate.m_templates.Count; i++)
            {
                if (NpcCityTemplate.m_templates[i].m_npcId == m_iMoveToNpcID)
                {
                    if (NpcCityTemplate.m_templates[i].m_npcId == 10000)
                    {
                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.WORSHIP_MAIN_LAYER),
                                           Worship_LoadCallback);
                    }
                    else
                    {
                        //if (NpcCityTemplate.m_templates[i].m_npcId == 10060)
                        //{
                        //    CityGlobalData.m_iAllianceTenentsSceneNum = 0;
                        //    CityGlobalData.m_isAllianceTenentsScene = true;
                        //    SceneManager.EnterAllianceCityTenentsCityOne();
                        //}
                        //else if (NpcCityTemplate.m_templates[i].m_npcId == 10061)
                        //{
                        //    CityGlobalData.m_iAllianceTenentsSceneNum = 1;
                        //    CityGlobalData.m_isAllianceTenentsScene = true;
                        //    SceneManager.EnterAllianceCityTenentsCityOne();
                        //}
                        //else if (NpcCityTemplate.m_templates[i].m_npcId == 10062)
                        //{
                        //    CityGlobalData.m_iAllianceTenentsSceneNum = 2;
                        //    CityGlobalData.m_isAllianceTenentsScene = true;
                        //    SceneManager.EnterAllianceCityTenentsCityOne();
                        //}
                        //else if (NpcCityTemplate.m_templates[i].m_npcId == 10063)
                        //{
                        //    CityGlobalData.m_iAllianceTenentsSceneNum = 3;
                        //    CityGlobalData.m_isAllianceTenentsScene = true;
                        //    SceneManager.EnterAllianceCityTenentsCityOne();
                        //}
                        // else
                        {
                            TenementManagerment.Instance.NavgationToTenement(NpcCityTemplate.m_templates[i].m_npcId - 1000);
                        }
                    }
                }
            }
        }
    }
	public void Worship_LoadCallback(ref WWW p_www, string p_path, Object p_object)
	{
		GameObject tempObject = (GameObject)Instantiate(p_object);
		MainCityUI.TryAddToObjectList(tempObject);
		UIYindao.m_UIYindao.CloseUI();
	}
}