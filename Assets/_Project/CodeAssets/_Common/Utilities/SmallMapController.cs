using UnityEngine;
using System.Collections;
using Carriage;

public class SmallMapController : MonoBehaviour
{
    public UIWidget MapBG;

    public Vector4 MapBorderRange;

    public void SetPositionInSmallMap(Transform target, Vector3 position)
    {
        target.localPosition = SmallMapPositionTransfer(position);
    }

    public void SetPositionInSmallMap(Transform target, Vector3 position, float p_rotation)
    {
        SetPositionInSmallMap(target, position);
        target.localEulerAngles = new Vector3(0, 0, p_rotation);
    }

    /// <summary>
    /// Transfer position in world space to small map space.
    /// </summary>
    /// <param name="originalPosition">aixs y not considered</param>
    /// <returns>small map position</returns>
    public Vector3 SmallMapPositionTransfer(Vector3 originalPosition)
    {
        var percentVector2 = new Vector2((originalPosition.x - MapBorderRange.x) / (MapBorderRange.y - MapBorderRange.x), (originalPosition.z - MapBorderRange.z) / (MapBorderRange.w - MapBorderRange.z));

        return new Vector3(-MapBG.width / 2.0f + MapBG.width * percentVector2.x, -MapBG.height / 2.0f + MapBG.height * percentVector2.y, 0);
    }
}
