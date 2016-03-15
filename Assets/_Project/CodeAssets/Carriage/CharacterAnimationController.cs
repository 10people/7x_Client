using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    public GameObject AnimParentObject;
    public GameObject AnimObject;

    public void ShowAnimation(string resPath)
    {
        if (AnimObject != null)
        {
            StopAnimation();
        }

        Global.ResourcesDotLoad(resPath, AnimLoadCallback);
    }

    private void AnimLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        AnimObject = (GameObject)Instantiate(p_object);
        TransformHelper.ActiveWithStandardize(AnimParentObject.transform, AnimObject.transform);
    }

    public void StopAnimation()
    {
        Destroy(AnimObject);
        AnimObject = null;
    }
}
