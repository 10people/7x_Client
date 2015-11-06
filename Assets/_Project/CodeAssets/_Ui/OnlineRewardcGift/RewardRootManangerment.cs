using UnityEngine;
using System.Collections;

public class RewardRootManangerment : MonoBehaviour
{
    private static string _RewardInfo = "";
    public static void CreateRoot(string rewardInfo)
    {
        _RewardInfo = rewardInfo;
        _RewardInfo = "0:900001:2000#0:900006:300#0:900015:10";
        if (!string.IsNullOrEmpty(_RewardInfo))
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.REWARD_EFFECT_ROOT),
                                   ResLoaded);
        }
    }
    private int _indexNum = 0;
    private static void ResLoaded(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {

        GameObject tempObject = (GameObject)Instantiate(p_object);
        tempObject.name = p_object.name;
        tempObject.GetComponent<RewardMovingEffectManagerment>().AwardsInfoTidy(_RewardInfo);

    }
}
