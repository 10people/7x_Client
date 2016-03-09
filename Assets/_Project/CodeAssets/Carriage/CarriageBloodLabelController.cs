using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Carriage
{
    public class CarriageBloodLabelController : MonoBehaviour
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
        }
    }
}