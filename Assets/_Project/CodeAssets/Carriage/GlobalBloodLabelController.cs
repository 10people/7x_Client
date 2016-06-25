//#define UNIT_TEST

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalBloodLabelController : Singleton<GlobalBloodLabelController>
{
    public List<GameObject> LabelPrefabList = new List<GameObject>();

    public enum BloodLabelType
    {
        Recover,
        PlayerAttack,
        Other,
        EnemyAttack,
        TreasureSkill,
        Skill
    }

    public void ShowBloodLabel(GameObject itemObject, string labelText, BloodLabelType type, bool isCrit = false)
    {
        BloodLabelControllor.Instance().createBloodLabelGenarl(itemObject, labelText, 2f, isCrit, (int)type);
    }

    void Start()
    {
        BloodLabelControllor.Instance().initStartGenarl(LabelPrefabList);

        PrepareForAllianceBattle.UpdateLoadProgress(PrepareForAllianceBattle.LoadModule.INIT, "AB_Blood");
        PrepareForCarriage.UpdateLoadProgress(PrepareForCarriage.LoadModule.INIT, "Carriage_Blood");
    }

#if UNITY_EDITOR && UNIT_TEST

    void OnGUI()
    {
        if (Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_CARRIAGE)
        {
            if (GUILayout.Button("Test label"))
                GlobalBloodLabelController.Instance.ShowBloodLabel(Carriage.RootManager.Instance.m_SelfPlayerController.gameObject, "TTTTTTTTTTT", BloodLabelType.Recover, true);
            if (GUILayout.Button("Test label"))

                GlobalBloodLabelController.Instance.ShowBloodLabel(Carriage.RootManager.Instance.m_SelfPlayerController.gameObject, "TTTTTTTTTTT", BloodLabelType.PlayerAttack, true);
            if (GUILayout.Button("Test label"))

                GlobalBloodLabelController.Instance.ShowBloodLabel(Carriage.RootManager.Instance.m_SelfPlayerController.gameObject, "TTTTTTTTTTT", BloodLabelType.Other, true);
            if (GUILayout.Button("Test label"))

                GlobalBloodLabelController.Instance.ShowBloodLabel(Carriage.RootManager.Instance.m_SelfPlayerController.gameObject, "TTTTTTTTTTT", BloodLabelType.EnemyAttack, true);
            if (GUILayout.Button("Test label"))

                GlobalBloodLabelController.Instance.ShowBloodLabel(Carriage.RootManager.Instance.m_SelfPlayerController.gameObject, "TTTTTTTTTTT", BloodLabelType.TreasureSkill, true);
            if (GUILayout.Button("Test label"))

                GlobalBloodLabelController.Instance.ShowBloodLabel(Carriage.RootManager.Instance.m_SelfPlayerController.gameObject, "TTTTTTTTTTT", BloodLabelType.Skill, true);
            if (GUILayout.Button("Test label"))


                GlobalBloodLabelController.Instance.ShowBloodLabel(Carriage.RootManager.Instance.m_SelfPlayerController.gameObject, "TTTTTTTTTTT", BloodLabelType.Recover, false);
            if (GUILayout.Button("Test label"))

                GlobalBloodLabelController.Instance.ShowBloodLabel(Carriage.RootManager.Instance.m_SelfPlayerController.gameObject, "TTTTTTTTTTT", BloodLabelType.PlayerAttack, false);
            if (GUILayout.Button("Test label"))

                GlobalBloodLabelController.Instance.ShowBloodLabel(Carriage.RootManager.Instance.m_SelfPlayerController.gameObject, "TTTTTTTTTTT", BloodLabelType.Other, false);
            if (GUILayout.Button("Test label"))

                GlobalBloodLabelController.Instance.ShowBloodLabel(Carriage.RootManager.Instance.m_SelfPlayerController.gameObject, "TTTTTTTTTTT", BloodLabelType.EnemyAttack, false);
            if (GUILayout.Button("Test label"))

                GlobalBloodLabelController.Instance.ShowBloodLabel(Carriage.RootManager.Instance.m_SelfPlayerController.gameObject, "TTTTTTTTTTT", BloodLabelType.TreasureSkill, false);
            if (GUILayout.Button("Test label"))

                GlobalBloodLabelController.Instance.ShowBloodLabel(Carriage.RootManager.Instance.m_SelfPlayerController.gameObject, "TTTTTTTTTTT", BloodLabelType.Skill, false);
        }
    }

#endif

}