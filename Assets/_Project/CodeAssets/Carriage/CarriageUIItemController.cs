using UnityEngine;
using System.Collections;
using System.Linq;
using qxmobile.protobuf;

namespace Carriage
{
    public class CarriageUIItemController : MonoBehaviour
    {
        public RootManager m_RootManager;
        public CarriageUI m_CarriageUi;

        public YabiaoJunZhuInfo m_YabiaoJunZhuInfo;

        public EventHandler m_EventHandler;

        public UILabel m_KingNameLabel;
        public UILabel m_AllianceNameLabel;
        public UILabel m_BloodLabel;
        public UIProgressBar m_BloodBar;

        public UILabel m_AttackValueLabel;
        public UILabel m_MoneyLabel;
        public UILabel m_StateLabel;
        public UISpriteAnimation m_StateSpriteAnimation;
        public GameObject m_StateObject;

        public GameObject ShieldObject;
        public UILabel ShieldPercentLabel;

        public GameObject m_KingIconParent;
        public IconSampleManager m_IconSampleManager;

        public UIAtlas CanAttack_CarriageState_Atlas;

        public enum State
        {
            CanAttack,
            BeenAttack,
            Protected,
            End,
            Other
        }

        /// <summary>
        /// Start at 0.
        /// </summary>
        public State m_State
        {
            get { return m_state; }
            set
            {
                m_previousState = m_state;
                m_state = value;

                //refresh state ui if state changed.
                if (m_state != m_previousState)
                {
                    RefreshStateUI();
                }

                if (m_state == State.Protected)
                {
                    m_StateLabel.text = ColorTool.Color_Blue_016bc5 + "保护期" + m_protectTime + "秒" + "[-]";
                }
            }
        }

        public void RefreshStateUI()
        {
            if (!CarriageUI.IsOpened)
            {
                Debug.LogWarning("Cancel refresh carriage ui item state ui cause grid not opened");
                return;
            }

            if (m_state == State.CanAttack)
            {
                //m_StateLabel.color = Color.white;
                //m_StateLabel.text = ColorTool.Color_Green_00ff00 + "可拦截" + "[-]";

                m_StateLabel.gameObject.SetActive(false);
                UI3DEffectTool.Instance().ClearUIFx(m_StateObject);
                //m_StateCountLabel.gameObject.SetActive(false);
                m_StateSpriteAnimation.GetComponent<UISprite>().atlas = CanAttack_CarriageState_Atlas;
                m_StateSpriteAnimation.gameObject.SetActive(true);
            }
            else if (m_state == State.BeenAttack)
            {
                m_StateLabel.color = Color.white;
                m_StateLabel.text = ColorTool.Color_Red_c40000 + "应战中" + "[-]";
                m_StateLabel.gameObject.SetActive(true);
                m_StateSpriteAnimation.gameObject.SetActive(false);
                UI3DEffectTool.Instance().ClearUIFx(m_StateObject);
                //m_StateCountLabel.gameObject.SetActive(false);
                //UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.MainUI_0, m_StateObject, EffectTemplate.getEffectTemplateByEffectId(100171).path);
            }
            else if (m_state == State.Protected)
            {
                m_StateLabel.color = Color.white;
                m_StateLabel.text = ColorTool.Color_Blue_016bc5 + "保护期" + m_protectTime + "秒" + "[-]";
                m_StateLabel.gameObject.SetActive(true);
                m_StateSpriteAnimation.gameObject.SetActive(false);
                UI3DEffectTool.Instance().ClearUIFx(m_StateObject);
                //m_StateCountLabel.gameObject.SetActive(true);
                //UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.MainUI_0, m_StateObject, EffectTemplate.getEffectTemplateByEffectId(100172).path);
            }
            else if (m_state == State.End)
            {
                m_StateSpriteAnimation.gameObject.SetActive(false);
                UI3DEffectTool.Instance().ClearUIFx(m_StateObject);
                //m_StateCountLabel.gameObject.SetActive(false);
                m_StateLabel.gameObject.SetActive(true);
                m_StateLabel.color = Color.white;
                m_StateLabel.text = ColorTool.Color_Red_c40000 + "结束" + "[-]";
            }
        }

        private State m_state = State.Other;
        private State m_previousState = State.Other;

        private float m_protectTime;
        private float m_lastCalcTime;

        void Update()
        {
            if (m_protectTime > 0 && Time.realtimeSinceStartup - m_lastCalcTime > 1.0f)
            {
                m_lastCalcTime = Time.realtimeSinceStartup;

                m_protectTime--;
                m_State = (State)(m_YabiaoJunZhuInfo.state / 10 - 1);
            }
        }

        public void SetThis()
        {
            m_KingNameLabel.text = m_YabiaoJunZhuInfo.junZhuName;
            if (string.IsNullOrEmpty(m_YabiaoJunZhuInfo.lianMengName))
            {
                m_AllianceNameLabel.text = "无联盟";
            }
            else
            {
                m_AllianceNameLabel.text = "<" + m_YabiaoJunZhuInfo.lianMengName + ">";
            }
            m_BloodLabel.text = m_YabiaoJunZhuInfo.hp + "/" + m_YabiaoJunZhuInfo.maxHp;
            m_BloodBar.value = m_YabiaoJunZhuInfo.hp / (float)m_YabiaoJunZhuInfo.maxHp;
            m_AttackValueLabel.text = m_YabiaoJunZhuInfo.zhanLi.ToString();
            m_MoneyLabel.text = m_YabiaoJunZhuInfo.worth.ToString();
            m_protectTime = m_YabiaoJunZhuInfo.baohuCD;
            if (m_YabiaoJunZhuInfo.huDun > 0)
            {
                ShieldObject.SetActive(true);
                ShieldPercentLabel.text = ColorTool.Color_Blue_016bc5 + "+" + m_YabiaoJunZhuInfo.huDun + "[-]" + "%";
            }
            else
            {
                ShieldObject.SetActive(false);
            }
            m_State = (State)(m_YabiaoJunZhuInfo.state / 10 - 1);

            SetKingIcon();
        }

        public void SetKingIcon()
        {
            //Refresh
            if (m_IconSampleManager != null)
            {
                SetIconSampleManager();
            }
            //Init
            else
            {
                while (m_KingIconParent.transform.childCount > 0)
                {
                    var child = m_KingIconParent.transform.GetChild(0);
                    child.parent = null;
                    Destroy(child.gameObject);
                }

                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconLoadCallBack);
            }
        }

        private const string enemyDeco = "CarriageEnemyDeco";
        private const string selfDeco = "CarriageSelfDeco";

        private void OnIconLoadCallBack(ref WWW www, string path, Object loadedObject)
        {
            var tempObject = Instantiate(loadedObject) as GameObject;
            TransformHelper.ActiveWithStandardize(m_KingIconParent.transform, tempObject.transform);

            m_IconSampleManager = tempObject.GetComponent<IconSampleManager>();
            SetIconSampleManager();
        }

        private void SetIconSampleManager()
        {
            if (m_IconSampleManager == null)
            {
                Debug.LogError("Cannot set iconsample manager cause it's null");
                return;
            }

            int horseType = m_YabiaoJunZhuInfo.horseType;
            m_IconSampleManager.SetIconType(IconSampleManager.IconType.Carriage);
            //[FIX]
            m_IconSampleManager.SetIconBasic(5, "horseIcon" + m_YabiaoJunZhuInfo.horseType, "Lv" + m_YabiaoJunZhuInfo.level, "pinzhi" + (IconSampleManager.TransferOldQualityID(m_YabiaoJunZhuInfo.horseType) - 1));
            if (m_YabiaoJunZhuInfo.isEnemy)
            {
                m_IconSampleManager.SetIconDecoSprite(enemyDeco);
            }
            else if (m_YabiaoJunZhuInfo.junZhuId == JunZhuData.Instance().m_junzhuInfo.id)
            {
                m_IconSampleManager.SetIconDecoSprite(selfDeco);
            }

            m_IconSampleManager.transform.localScale = Vector3.one * 1.6f;
        }

        void OnColliderClick(GameObject go)
        {
            m_CarriageUi.SetCarriageInfo(m_YabiaoJunZhuInfo);

            CarriageController controller = m_RootManager.m_CarriageManager.m_CarriageControllers.Where(item => item.m_YabiaoJunZhuInfo.junZhuId == m_YabiaoJunZhuInfo.junZhuId).First();
            if (controller != null)
            {
                m_RootManager.m_CarriagePlayerController.m_NavigationTransform = controller.transform;
                m_RootManager.m_CarriagePlayerController.StartNavigation(controller.transform.position);
            }
            else
            {
                Debug.LogError("Fail to find carriage controller when click carriage ui item.");
            }
        }

        void Awake()
        {
            m_EventHandler.m_handler += OnColliderClick;
        }

        void OnDestroy()
        {
            m_EventHandler.m_handler -= OnColliderClick;
        }
    }
}