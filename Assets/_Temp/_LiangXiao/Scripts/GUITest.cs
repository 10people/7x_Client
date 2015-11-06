using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;
using Carriage;
using qxmobile.protobuf;

public class GUITest : MonoBehaviour
{
    public UIPanel panel;
    public UIScrollView ScrollView;
    public UIScrollBar ScrollBar;
    public UISprite sprite;

    private string value = "";

    void OnGUI()
    {
        value = GUILayout.TextField(value);

        if (GUILayout.Button("DODODODODODODDODO"))
        {
            ScrollView.SetWidgetValueRelativeToScrollView(sprite, float.Parse(value));
        }

        if (GUILayout.Button("DODODODODODODDODO"))
        {
            //ScrollView.UpdateScrollbars(true);
            ScrollBar.value = 0.99f;
        }

        GUILayout.TextField(panel.baseClipRegion.ToString());
        GUILayout.TextField(panel.finalClipRegion.ToString());
        GUILayout.TextField(ScrollView.bounds.ToString());
        //GUILayout.TextField(panel.transform.worldToLocalMatrix.ToString());
        //GUILayout.TextField(panel.transform.worldToLocalMatrix.MultiplyPoint3x4(Vector3.one).ToString());
        GUILayout.TextField(ScrollView.GetFreeScrollViewValue().ToString());
        GUILayout.TextField(ScrollView.GetWidgetValueRelativeToBound(sprite).ToString());
        GUILayout.TextField(ScrollView.GetWidgetValueRelativeToScrollView(sprite).ToString());
    }
}
