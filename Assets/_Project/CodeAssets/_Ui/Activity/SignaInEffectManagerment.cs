using UnityEngine;
using System.Collections;

public class SignaInEffectManagerment : MonoBehaviour
{
    public ActivitySignalInItemManagerment m_Item;
    void AnimationPlayFinish()
    {
        m_Item.AnimationPlay();
    }
}
