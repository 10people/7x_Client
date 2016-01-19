//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright 漏 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

/// <summary>
/// This script adds the NGUI menu options to the Unity Editor.
/// </summary>

public static class NGUIMenu
{
#region Selection

	public static GameObject SelectedRoot () { return NGUIEditorTools.SelectedRoot(); }

	[MenuItem("NGUI/Selection/Bring To Front &#=", false, 0)]
	public static void BringForward2 ()
	{
		int val = 0;
		for (int i = 0; i < Selection.gameObjects.Length; ++i)
			val |= NGUITools.AdjustDepth(Selection.gameObjects[i], 1000);

		if ((val & 1) != 0)
		{
			NGUITools.NormalizePanelDepths();
			if (UIPanelTool.instance != null)
				UIPanelTool.instance.Repaint();
		}
		if ((val & 2) != 0) NGUITools.NormalizeWidgetDepths();
	}

	[MenuItem("NGUI/Selection/Bring To Front &#=", true)]
	public static bool BringForward2Validation () { return (Selection.activeGameObject != null); }

	[MenuItem("NGUI/Selection/Push To Back &#-", false, 0)]
	public static void PushBack2 ()
	{
		int val = 0;
		for (int i = 0; i < Selection.gameObjects.Length; ++i)
			val |= NGUITools.AdjustDepth(Selection.gameObjects[i], -1000);

		if ((val & 1) != 0)
		{
			NGUITools.NormalizePanelDepths();
			if (UIPanelTool.instance != null)
				UIPanelTool.instance.Repaint();
		}
		if ((val & 2) != 0) NGUITools.NormalizeWidgetDepths();
	}

	[MenuItem("NGUI/Selection/Push To Back &#-", true)]
	public static bool PushBack2Validation () { return (Selection.activeGameObject != null); }

	[MenuItem("NGUI/Selection/Adjust Depth By +1 %=", false, 0)]
	public static void BringForward ()
	{
		int val = 0;
		for (int i = 0; i < Selection.gameObjects.Length; ++i)
			val |= NGUITools.AdjustDepth(Selection.gameObjects[i], 1);
		if (((val & 1) != 0) && UIPanelTool.instance != null)
			UIPanelTool.instance.Repaint();
	}

	[MenuItem("NGUI/Selection/Adjust Depth By +1 %=", true)]
	public static bool BringForwardValidation () { return (Selection.activeGameObject != null); }

	[MenuItem("NGUI/Selection/Adjust Depth By -1 %-", false, 0)]
	public static void PushBack ()
	{
		int val = 0;
		for (int i = 0; i < Selection.gameObjects.Length; ++i)
			val |= NGUITools.AdjustDepth(Selection.gameObjects[i], -1);
		if (((val & 1) != 0) && UIPanelTool.instance != null)
			UIPanelTool.instance.Repaint();
	}

	[MenuItem("NGUI/Selection/Adjust Depth By -1 %-", true)]
	public static bool PushBackValidation () { return (Selection.activeGameObject != null); }

	[MenuItem("NGUI/Selection/Make Pixel Perfect &#p", false, 0)]
	static void PixelPerfectSelection ()
	{
		foreach (Transform t in Selection.transforms)
			NGUITools.MakePixelPerfect(t);
	}

	[MenuItem("NGUI/Selection/Make Pixel Perfect &#p", true)]
	static bool PixelPerfectSelectionValidation ()
	{
		return (Selection.activeTransform != null);
	}

#endregion
#region Create

	[MenuItem("NGUI/Create/Sprite &#s", false, 6)]
	public static void AddSprite ()
	{
		GameObject go = NGUIEditorTools.SelectedRoot(true);

		if (go != null)
		{
#if UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2
			Undo.RegisterSceneUndo("Add a Sprite");
#endif
			Selection.activeGameObject = NGUISettings.AddSprite(go).gameObject;
		}
		else
		{
			Debug.Log("You must select a game object first.");
		}
	}

	[MenuItem("NGUI/Create/Label &#l", false, 6)]
	public static void AddLabel ()
	{
		GameObject go = NGUIEditorTools.SelectedRoot(true);

		if (go != null)
		{
#if UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2
			Undo.RegisterSceneUndo("Add a Label");
#endif
			Selection.activeGameObject = NGUISettings.AddLabel(go).gameObject;
		}
		else
		{
			Debug.Log("You must select a game object first.");
		}
	}

	[MenuItem("NGUI/Create/Texture &#t", false, 6)]
	public static void AddTexture ()
	{
		GameObject go = NGUIEditorTools.SelectedRoot(true);

		if (go != null)
		{
#if UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2
			Undo.RegisterSceneUndo("Add a Texture");
#endif
			Selection.activeGameObject = NGUISettings.AddTexture(go).gameObject;
		}
		else
		{
			Debug.Log("You must select a game object first.");
		}
	}

#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2
	[MenuItem("NGUI/Create/Unity 2D Sprite &#r", false, 6)]
	public static void AddSprite2D ()
	{
		GameObject go = NGUIEditorTools.SelectedRoot(true);
		if (go != null) Selection.activeGameObject = NGUISettings.Add2DSprite(go).gameObject;
		else Debug.Log("You must select a game object first.");
	}
#endif

	[MenuItem("NGUI/Create/Widget &#w", false, 6)]
	public static void AddWidget ()
	{
		GameObject go = NGUIEditorTools.SelectedRoot(true);

		if (go != null)
		{
#if UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2
			Undo.RegisterSceneUndo("Add a Widget");
#endif
			Selection.activeGameObject = NGUISettings.AddWidget(go).gameObject;
		}
		else
		{
			Debug.Log("You must select a game object first.");
		}
	}

	[MenuItem("NGUI/Create/", false, 6)]
	static void AddBreaker123 () {}

	[MenuItem("NGUI/Create/Anchor (Legacy)", false, 6)]
	static void AddAnchor2 () { Add<UIAnchor>(); }

	[MenuItem("NGUI/Create/Panel", false, 6)]
	static void AddPanel ()
	{
		UIPanel panel = NGUISettings.AddPanel(SelectedRoot());
		Selection.activeGameObject = (panel == null) ? NGUIEditorTools.SelectedRoot(true) : panel.gameObject;
	}

	[MenuItem("NGUI/Create/Scroll View", false, 6)]
	static void AddScrollView ()
	{
		UIPanel panel = NGUISettings.AddPanel(SelectedRoot());
		if (panel == null) panel = NGUIEditorTools.SelectedRoot(true).GetComponent<UIPanel>();
		panel.clipping = UIDrawCall.Clipping.SoftClip;
		panel.name = "Scroll View";
		panel.gameObject.AddComponent<UIScrollView>();
		Selection.activeGameObject = panel.gameObject;
	}

	[MenuItem("NGUI/Create/Grid", false, 6)]
	static void AddGrid () { Add<UIGrid>(); }

	[MenuItem("NGUI/Create/Table", false, 6)]
	static void AddTable () { Add<UITable>(); }

	static T Add<T> () where T : MonoBehaviour
	{
		T t = NGUITools.AddChild<T>(SelectedRoot());
		Selection.activeGameObject = t.gameObject;
		return t;
	}

	[MenuItem("NGUI/Create/2D UI", false, 6)]
	[MenuItem("Assets/NGUI/Create 2D UI", false, 1)]
	static void Create2D () { UICreateNewUIWizard.CreateNewUI(UICreateNewUIWizard.CameraType.Simple2D); }

	[MenuItem("NGUI/Create/2D UI", true)]
	[MenuItem("Assets/NGUI/Create 2D UI", true, 1)]
	static bool Create2Da () { return UIRoot.list.Count == 0 || UICamera.list.size == 0 || !UICamera.list[0].GetComponent<Camera>().orthographic; }

	[MenuItem("NGUI/Create/3D UI", false, 6)]
	[MenuItem("Assets/NGUI/Create 3D UI", false, 1)]
	static void Create3D () { UICreateNewUIWizard.CreateNewUI(UICreateNewUIWizard.CameraType.Advanced3D); }

	[MenuItem("NGUI/Create/3D UI", true)]
	[MenuItem("Assets/NGUI/Create 3D UI", true, 1)]
	static bool Create3Da () { return UIRoot.list.Count == 0 || UICamera.list.size == 0 || UICamera.list[0].GetComponent<Camera>().orthographic; }

#endregion
#region Attach

	static void AddIfMissing<T> () where T : Component
	{
		GameObject go = Selection.activeGameObject;
		if (go != null) go.AddMissingComponent<T>();
		else Debug.Log("You must select a game object first.");
	}

	static bool Exists<T> () where T : Component
	{
		GameObject go = Selection.activeGameObject;
		if (go != null) return go.GetComponent<T>() != null;
		return false;
	}

	[MenuItem("NGUI/Attach/Collider &#c", false, 7)]
	public static void AddCollider ()
	{
		GameObject go = Selection.activeGameObject;

		if (NGUIEditorTools.WillLosePrefab(go))
		{
			if (go != null)
			{
				NGUIEditorTools.RegisterUndo("Add Widget Collider", go);
				NGUITools.AddWidgetCollider(go);
			}
			else
			{
				Debug.Log("You must select a game object first, such as your button.");
			}
		}
	}

	//[MenuItem("NGUI/Attach/Anchor", false, 7)]
	//public static void Add1 () { AddIfMissing<UIAnchor>(); }

	//[MenuItem("NGUI/Attach/Anchor", true)]
	//public static bool Add1a () { return !Exists<UIAnchor>(); }

	//[MenuItem("NGUI/Attach/Stretch (Legacy)", false, 7)]
	//public static void Add2 () { AddIfMissing<UIStretch>(); }

	//[MenuItem("NGUI/Attach/Stretch (Legacy)", true)]
	//public static bool Add2a () { return !Exists<UIStretch>(); }

	//[MenuItem("NGUI/Attach/", false, 7)]
	//public static void Add3s () {}

	[MenuItem("NGUI/Attach/Button Script", false, 7)]
	public static void Add3 () { AddIfMissing<UIButton>(); }

	[MenuItem("NGUI/Attach/Toggle Script", false, 7)]
	public static void Add4 () { AddIfMissing<UIToggle>(); }

	[MenuItem("NGUI/Attach/Slider Script", false, 7)]
	public static void Add5 () { AddIfMissing<UISlider>(); }

	[MenuItem("NGUI/Attach/Scroll Bar Script", false, 7)]
	public static void Add6 () { AddIfMissing<UIScrollBar>(); }

	[MenuItem("NGUI/Attach/Progress Bar Script", false, 7)]
	public static void Add7 () { AddIfMissing<UIProgressBar>(); }

	[MenuItem("NGUI/Attach/Popup List Script", false, 7)]
	public static void Add8 () { AddIfMissing<UIPopupList>(); }

	[MenuItem("NGUI/Attach/Input Field Script", false, 7)]
	public static void Add9 () { AddIfMissing<UIInput>(); }

	[MenuItem("NGUI/Attach/Key Binding Script", false, 7)]
	public static void Add10 () { AddIfMissing<UIKeyBinding>(); }

	[MenuItem("NGUI/Attach/Play Tween Script", false, 7)]
	public static void Add11 () { AddIfMissing<UIPlayTween>(); }

	[MenuItem("NGUI/Attach/Play Animation Script", false, 7)]
	public static void Add12 () { AddIfMissing<UIPlayAnimation>(); }

	[MenuItem("NGUI/Attach/Play Sound Script", false, 7)]
	public static void Add13 () { AddIfMissing<UIPlaySound>(); }

	[MenuItem("NGUI/Attach/Localization Script", false, 7)]
	public static void Add14 () { AddIfMissing<UILocalize>(); }

#endregion
#region Tweens

	[MenuItem("NGUI/Tween/Alpha", false, 8)]
	static void Tween1 () { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenAlpha>(); }

	[MenuItem("NGUI/Tween/Alpha", true)]
	static bool Tween1a () { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<UIWidget>() != null); }

	[MenuItem("NGUI/Tween/Color", false, 8)]
	static void Tween2 () { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenColor>(); }

	[MenuItem("NGUI/Tween/Color", true)]
	static bool Tween2a () { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<UIWidget>() != null); }

	[MenuItem("NGUI/Tween/Width", false, 8)]
	static void Tween3 () { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenWidth>(); }

	[MenuItem("NGUI/Tween/Width", true)]
	static bool Tween3a () { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<UIWidget>() != null); }

	[MenuItem("NGUI/Tween/Height", false, 8)]
	static void Tween4 () { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenHeight>(); }

	[MenuItem("NGUI/Tween/Height", true)]
	static bool Tween4a () { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<UIWidget>() != null); }

	[MenuItem("NGUI/Tween/Position", false, 8)]
	static void Tween5 () { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenPosition>(); }

	[MenuItem("NGUI/Tween/Position", true)]
	static bool Tween5a () { return (Selection.activeGameObject != null); }

	[MenuItem("NGUI/Tween/Rotation", false, 8)]
	static void Tween6 () { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenRotation>(); }

	[MenuItem("NGUI/Tween/Rotation", true)]
	static bool Tween6a () { return (Selection.activeGameObject != null); }

	[MenuItem("NGUI/Tween/Scale", false, 8)]
	static void Tween7 () { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenScale>(); }

	[MenuItem("NGUI/Tween/Scale", true)]
	static bool Tween7a () { return (Selection.activeGameObject != null); }

	[MenuItem("NGUI/Tween/Transform", false, 8)]
	static void Tween8 () { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenTransform>(); }

	[MenuItem("NGUI/Tween/Transform", true)]
	static bool Tween8a () { return (Selection.activeGameObject != null); }

	[MenuItem("NGUI/Tween/Volume", false, 8)]
	static void Tween9 () { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenVolume>(); }

	[MenuItem("NGUI/Tween/Volume", true)]
	static bool Tween9a () { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<AudioSource>() != null); }

	[MenuItem("NGUI/Tween/Field of View", false, 8)]
	static void Tween10 () { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenFOV>(); }

	[MenuItem("NGUI/Tween/Field of View", true)]
	static bool Tween10a () { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<Camera>() != null); }

	[MenuItem("NGUI/Tween/Orthographic Size", false, 8)]
	static void Tween11 () { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenOrthoSize>(); }

	[MenuItem("NGUI/Tween/Orthographic Size", true)]
	static bool Tween11a () { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<Camera>() != null); }

#endregion
#region Open

	[MenuItem("NGUI/Open/Atlas Maker", false, 9)]
	[MenuItem("Assets/NGUI/Open Atlas Maker", false, 0)]
	public static void OpenAtlasMaker ()
	{
		EditorWindow.GetWindow<UIAtlasMaker>(false, "Atlas Maker", true);
	}

	[MenuItem("NGUI/Open/Font Maker", false, 9)]
	[MenuItem("Assets/NGUI/Open Bitmap Font Maker", false, 0)]
	public static void OpenFontMaker ()
	{
		EditorWindow.GetWindow<UIFontMaker>(false, "Font Maker", true);
	}

	[MenuItem("NGUI/Open/", false, 9)]
	[MenuItem("Assets/NGUI/", false, 0)]
	public static void OpenSeparator2 () { }

	[MenuItem("NGUI/Open/Panel Tool", false, 9)]
	public static void OpenPanelWizard ()
	{
		EditorWindow.GetWindow<UIPanelTool>(false, "Panel Tool", true);
	}

	[MenuItem("NGUI/Open/Draw Call Tool", false, 9)]
	public static void OpenDCTool ()
	{
		EditorWindow.GetWindow<UIDrawCallViewer>(false, "Draw Call Tool", true);
	}

	[MenuItem("NGUI/Open/Camera Tool", false, 9)]
	public static void OpenCameraWizard ()
	{
		EditorWindow.GetWindow<UICameraTool>(false, "Camera Tool", true);
	}

	[MenuItem("NGUI/Open/Widget Wizard (Legacy)", false, 9)]
	public static void CreateWidgetWizard ()
	{
		EditorWindow.GetWindow<UICreateWidgetWizard>(false, "Widget Tool", true);
	}

	//[MenuItem("NGUI/Open/UI Wizard (Legacy)", false, 9)]
	//public static void CreateUIWizard ()
	//{
	//    EditorWindow.GetWindow<UICreateNewUIWizard>(false, "UI Tool", true);
	//}

#endregion
#region Options

	[MenuItem("NGUI/Options/Handles/Turn On", false, 10)]
	public static void TurnHandlesOn () { UIWidget.showHandlesWithMoveTool = true; }

	[MenuItem("NGUI/Options/Handles/Turn On", true, 10)]
	public static bool TurnHandlesOnCheck () { return !UIWidget.showHandlesWithMoveTool; }

	[MenuItem("NGUI/Options/Handles/Turn Off", false, 10)]
	public static void TurnHandlesOff () { UIWidget.showHandlesWithMoveTool = false; }

	[MenuItem("NGUI/Options/Handles/Turn Off", true, 10)]
	public static bool TurnHandlesOffCheck () { return UIWidget.showHandlesWithMoveTool; }

	[MenuItem("NGUI/Options/Handles/Set to Blue", false, 10)]
	public static void SetToBlue () { NGUISettings.colorMode = NGUISettings.ColorMode.Blue; }

	[MenuItem("NGUI/Options/Handles/Set to Blue", true, 10)]
	public static bool SetToBlueCheck () { return UIWidget.showHandlesWithMoveTool && NGUISettings.colorMode != NGUISettings.ColorMode.Blue; }

	[MenuItem("NGUI/Options/Handles/Set to Orange", false, 10)]
	public static void SetToOrange () { NGUISettings.colorMode = NGUISettings.ColorMode.Orange; }

	[MenuItem("NGUI/Options/Handles/Set to Orange", true, 10)]
	public static bool SetToOrangeCheck () { return UIWidget.showHandlesWithMoveTool && NGUISettings.colorMode != NGUISettings.ColorMode.Orange; }

	[MenuItem("NGUI/Options/Handles/Set to Green", false, 10)]
	public static void SetToGreen () { NGUISettings.colorMode = NGUISettings.ColorMode.Green; }

	[MenuItem("NGUI/Options/Handles/Set to Green", true, 10)]
	public static bool SetToGreenCheck () { return UIWidget.showHandlesWithMoveTool && NGUISettings.colorMode != NGUISettings.ColorMode.Green; }

	[MenuItem("NGUI/Options/Snapping/Turn On", false, 10)]
	public static void TurnSnapOn () { NGUISnap.allow = true; }

	[MenuItem("NGUI/Options/Snapping/Turn On", true, 10)]
	public static bool TurnSnapOnCheck () { return !NGUISnap.allow; }

	[MenuItem("NGUI/Options/Snapping/Turn Off", false, 10)]
	public static void TurnSnapOff () { NGUISnap.allow = false; }

	[MenuItem("NGUI/Options/Snapping/Turn Off", true, 10)]
	public static bool TurnSnapOffCheck () { return NGUISnap.allow; }

	[MenuItem("NGUI/Options/Guides/Always On", false, 10)]
	public static void TurnGuidesOn () { NGUISettings.drawGuides = true; }

	[MenuItem("NGUI/Options/Guides/Always On", true, 10)]
	public static bool TurnGuidesOnCheck () { return !NGUISettings.drawGuides; }

	[MenuItem("NGUI/Options/Guides/Only When Needed", false, 10)]
	public static void TurnGuidesOff () { NGUISettings.drawGuides = false; }

	[MenuItem("NGUI/Options/Guides/Only When Needed", true, 10)]
	public static bool TurnGuidesOffCheck () { return NGUISettings.drawGuides; }

#endregion

	[MenuItem("NGUI/Normalize Depth Hierarchy &#0", false, 11)]
	public static void Normalize () { NGUITools.NormalizeDepths(); }
	
	[MenuItem("NGUI/", false, 11)]
	static void Breaker () { }

	[MenuItem("NGUI/Help", false, 12)]
	public static void Help () { NGUIHelp.Show(); }
}
