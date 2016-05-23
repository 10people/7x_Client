//#define DEBUG_PREPARE_FOR_CITY_LOAD

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PrepareForAllianceBattle : MonoBehaviour
{
    public enum LoadModule
    {
        UI,
        MODEL,
        EFFECT,
        INIT,
    }

    public struct LoadProgress
    {
        public string Name;
        public int LoadedNum;
        public int TotalNum;
        public int Weight;
    }

    private static Dictionary<LoadModule, LoadProgress> LoadModuleDic = new Dictionary<LoadModule, LoadProgress>();

    public static void UpdateLoadProgress(LoadModule module, string part)
    {
        if (UtilityTool.IsLevelLoaded(SceneTemplate.GetScenePath(SceneTemplate.SceneEnum.ALLIANCE_BATTLE)))
        {
            LoadingHelper.ItemLoaded(StaticLoading.m_loading_sections, LoadModuleDic[module].Name, part);
        }
    }

    public void InitABLoading()
    {
        var itemCount = RTActionTemplate.templates.Count(item => item.CsOnHit > 0) + RTSkillTemplate.templates.Count(item => item.EsOnShot > 0) + RTBuffTemplate.templates.Count(item => item.BuffDisplay > 0);

        LoadModuleDic = new Dictionary<LoadModule, LoadProgress>()
        {
            {LoadModule.UI, new LoadProgress() {Name = "AB_UI", LoadedNum = 0, TotalNum = 1, Weight = 10}},
            {LoadModule.MODEL, new LoadProgress() {Name = "AB_MODEL", LoadedNum = 0, TotalNum = 4, Weight = 10}},
            {LoadModule.EFFECT, new LoadProgress() {Name = "AB_EFFECT", LoadedNum = 0, TotalNum = itemCount, Weight = 10}},
            {LoadModule.INIT, new LoadProgress() {Name = "AB_INIT", LoadedNum = 0, TotalNum = 5, Weight = 15}}
        };

        foreach (var item in LoadModuleDic)
        {
            LoadingHelper.InitSectionInfo(StaticLoading.m_loading_sections, item.Value.Name, item.Value.Weight, item.Value.TotalNum);
        }
    }

    void Update()
    {
        if (LoadModuleDic.Values.All(item => item.LoadedNum >= item.TotalNum))
        {
            ABLoadDone();
        }
    }

    void Awake()
    {
        Prepare_For_AllianceBattle();
    }

    public void ABLoadDone()
    {
        EnterNextScene.Instance().DestroyUI();
        Destroy(this);
    }

    public void Prepare_For_AllianceBattle()
    {
        InitABLoading();

        EnterNextScene.DirectLoadLevel();
        StartCoroutine(LoadAllianceBattleScene());
    }

    private IEnumerator LoadAllianceBattleScene()
    {
        while (!UtilityTool.IsLevelLoaded(SceneTemplate.GetScenePath(SceneTemplate.SceneEnum.ALLIANCE_BATTLE)))
        {
            yield return new WaitForEndOfFrame();
        }

        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ALLIANCE_BATTLE_SCENE), OnABSceneLoaded);
    }

    private void OnABSceneLoaded(ref WWW www, string path, Object prefab)
    {
        var sceneObject = Instantiate(prefab as GameObject);
        TransformHelper.ActiveWithStandardize(null, sceneObject.transform);

        UpdateLoadProgress(LoadModule.UI, "AB_UI");
    }
}