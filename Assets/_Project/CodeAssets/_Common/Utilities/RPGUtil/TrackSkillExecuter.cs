using System;
using UnityEngine;
using System.Collections;

public class TrackSkillExecuter : MonoBehaviour
{
    public int AttackerUID;

    public GameObject SkillPrefab;
    public Vector3 PlayerHeightOffset = new Vector3(0, 1.22f, 0);
    public GameObject SourceObject;
    public GameObject TargetObject;

    public GameObject SkillObject;

    [HideInInspector]
    public float MovingSpeed = 5f;

    public bool isFake = false;
    public Vector3 FakeTargetPosition;

    public void Execute()
    {
        isFake = false;

        if (SkillPrefab == null || SourceObject == null || TargetObject == null)
        {
            return;
        }

        if (SkillObject != null)
        {
            Destroy(SkillObject);
            SkillObject = null;
        }

        SkillObject = Instantiate(SkillPrefab);
        SkillObject.transform.position = SourceObject.transform.position + PlayerHeightOffset;
        SkillObject.SetActive(true);

        if (AttackerUID != PlayerSceneSyncManager.Instance.m_MyselfUid)
        {
            if (EffectNumController.Instance.IsCanPlayEffect())
            {
                EffectNumController.Instance.NotifyPlayingEffect();
            }
        }
    }

    public void ExecuteFake()
    {
        isFake = true;

        if (SkillPrefab == null || SourceObject == null)
        {
            return;
        }

        if (SkillObject != null)
        {
            Destroy(SkillObject);
            SkillObject = null;
        }

        SkillObject = Instantiate(SkillPrefab);
        SkillObject.transform.position = SourceObject.transform.position + PlayerHeightOffset;
        SkillObject.SetActive(true);

        if (AttackerUID != PlayerSceneSyncManager.Instance.m_MyselfUid)
        {
            if (EffectNumController.Instance.IsCanPlayEffect())
            {
                EffectNumController.Instance.NotifyPlayingEffect();
            }
        }
    }

    void FixedUpdate()
    {
        if (isFake)
        {
            if (SkillObject != null)
            {
                var distance = Vector3.Distance(SkillObject.transform.position, FakeTargetPosition + PlayerHeightOffset);
                if (distance < 0.1f)
                {
                    Destroy(SkillObject);
                    SkillObject = null;
                    return;
                }

                iTween.MoveUpdate(SkillObject, FakeTargetPosition + PlayerHeightOffset, distance / MovingSpeed);
                SkillObject.transform.eulerAngles = TransformHelper.Get2DTrackRotation(SkillObject.transform.position, FakeTargetPosition + PlayerHeightOffset);
            }
        }
        else
        {
            if (SkillObject != null)
            {
                if (TargetObject != null)
                {
                    var distance = Vector3.Distance(SkillObject.transform.position, TargetObject.transform.position + PlayerHeightOffset);
                    if (distance < 1f)
                    {
                        Destroy(SkillObject);
                        SkillObject = null;
                        return;
                    }

                    iTween.MoveUpdate(SkillObject, TargetObject.transform.position + PlayerHeightOffset, distance / MovingSpeed);
                    SkillObject.transform.eulerAngles = TransformHelper.Get2DTrackRotation(SkillObject.transform.position, TargetObject.transform.position + PlayerHeightOffset);
                }
                else
                {
                    Destroy(SkillObject);
                    SkillObject = null;
                }
            }
        }
    }

    void OnDestroy()
    {
        Destroy(SkillObject);
        SkillObject = null;
    }
}
