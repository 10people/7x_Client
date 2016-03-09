using UnityEngine;
using System.Collections;

public class TaskSprakEffectManagerment : MonoBehaviour {

	void OnEnable ()
    {
        SparkleEffectItem.OpenSparkle(gameObject, SparkleEffectItem.MenuItemStyle.Common_Icon);
    }

    void OnDisable()
    {
      SparkleEffectItem.CloseSparkle(gameObject);
    }
}
