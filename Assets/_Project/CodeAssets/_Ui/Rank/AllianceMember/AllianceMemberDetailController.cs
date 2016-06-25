using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using qxmobile.protobuf;

namespace Rank
{
    public class AllianceMemberDetailController : DetailController
    {
        public AllianceMemberController m_AllianceMemberController;

        public JunZhuInfo m_JunZhuInfo;

        public UILabel KingLabel;
        public UILabel JobLabel;
        public UILabel LevelLabel;

        public UISprite JunxianSprite;
        public UILabel JunxianLabel;
        public UISprite JunxianLabelSprite;
        public UILabel NoJunxianLabel;

        public UILabel ChongLouLevelLabel;
        public UILabel GongjinLabel;

        public void SetThis()
        {
            KingLabel.text = m_JunZhuInfo.name;

            switch (m_JunZhuInfo.job)
            {
                case 0:
                    JobLabel.text = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_MEMBER_CHENGYUAN);
                    break;
                case 1:
                    JobLabel.text = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_MEMBER_FU_LEADER);
                    break;
                case 2:
                    JobLabel.text = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_MEMBER_LEADER);
                    break;
                default:
                    Debug.LogError("Error job index in AllianceMemberDetailController.");
                    break;
            }

            LevelLabel.text = "Lv" + m_JunZhuInfo.level;
            ChongLouLevelLabel.text = m_JunZhuInfo.chongLouLayer + "层";
            GongjinLabel.text = m_JunZhuInfo.gongjin.ToString();

            //Check junxian empty
            if (m_JunZhuInfo.junxianRank <= 0)
            {
                JunxianSprite.gameObject.SetActive(false);
                JunxianLabelSprite.gameObject.SetActive(false);
                NoJunxianLabel.gameObject.SetActive(true);

                NoJunxianLabel.text = "无军衔";
            }
            else
            {
                JunxianSprite.gameObject.SetActive(true);
                JunxianLabelSprite.gameObject.SetActive(true);
                NoJunxianLabel.gameObject.SetActive(false);

                JunxianLabel.text = m_JunZhuInfo.junxian + " " + m_JunZhuInfo.junxianRank;
                JunxianSprite.spriteName = "junxian" + m_JunZhuInfo.junxianLevel;
            }

            SetBG(false);
        }

        new void OnClick()
        {
            base.OnClick();
            if (m_AllianceMemberController != null)
            {
                m_AllianceMemberController.m_AllianceMemberDetailControllerList.ForEach(item => item.DestroyFloatButtons());
            }

            if (m_AllianceMemberController == null || m_AllianceMemberController.FloatButtonPrefab == null)
            {
                return;
            }

            m_AllianceMemberController.m_RootController.ShieldName = m_JunZhuInfo.name;

            //Create object and set.
            GameObject tempObject = (GameObject)Instantiate(m_AllianceMemberController.FloatButtonPrefab);
            FloatButtonsController = tempObject.GetComponent<FloatButtonsController>();

            FloatButtonsController.Initialize(FloatButtonsConfig.GetConfig(m_JunZhuInfo.junZhuId, m_JunZhuInfo.name, m_JunZhuInfo.lianMeng, new List<GameObject>() { m_AllianceMemberController.gameObject, m_AllianceMemberController.m_RootController.gameObject }, m_AllianceMemberController.ClampScrollView), true);

            TransformHelper.ActiveWithStandardize(FloatButtonsRoot.transform, tempObject.transform);

            StartCoroutine(AdjustFloatButton());
        }

        public new IEnumerator AdjustFloatButton()
        {
            yield return new WaitForEndOfFrame();

            //Cancel adjust cause multi touch may destroy this float buttons gameobject.
            if (FloatButtonsController == null || FloatButtonsController.gameObject == null)
            {
                yield break;
            }

            NGUIHelper.AdaptWidgetInScrollView(m_AllianceMemberController.m_ScrollView, m_AllianceMemberController.m_ScrollBar, FloatButtonsController.m_BG.GetComponent<UIWidget>());
        }

        /// <summary>
        /// use new destroy all float buttons.
        /// </summary>
        /// <param name="isPress"></param>
        new void OnPress(bool isPress)
        {
            base.OnPress(isPress);

            if (m_AllianceMemberController != null)
            {
                m_AllianceMemberController.m_AllianceMemberDetailControllerList.ForEach(item => item.DestroyFloatButtons());
            }
        }
    }
}
