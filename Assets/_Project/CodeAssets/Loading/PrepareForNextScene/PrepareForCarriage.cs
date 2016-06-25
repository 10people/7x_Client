using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PrepareForCarriage : MonoBehaviour
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
        if (UtilityTool.IsLevelLoaded(SceneTemplate.GetScenePath(SceneTemplate.SceneEnum.CARRIAGE)))
        {
            LoadingHelper.ItemLoaded(StaticLoading.m_loading_sections, LoadModuleDic[module].Name, part);
        }
    }

    public void InitCarriageLoading()
    {
        var itemCount = RTActionTemplate.templates.Count(item => item.CsOnHit > 0) + RTSkillTemplate.templates.Count(item => item.EsOnShot > 0) + RTBuffTemplate.templates.Count(item => item.BuffDisplay > 0);

        LoadModuleDic = new Dictionary<LoadModule, LoadProgress>()
        {
            {LoadModule.UI, new LoadProgress() {Name = "Carriage_UI", LoadedNum = 0, TotalNum = 1, Weight = 10}},
            {LoadModule.MODEL, new LoadProgress() {Name = "Carriage_MODEL", LoadedNum = 0, TotalNum = 5, Weight = 10}},
            {LoadModule.EFFECT, new LoadProgress() {Name = "Carriage_EFFECT", LoadedNum = 0, TotalNum = itemCount, Weight = 10}},
            {LoadModule.INIT, new LoadProgress() {Name = "Carriage_INIT", LoadedNum = 0, TotalNum = 5, Weight = 15}}
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
            CarriageLoadDone();
        }
    }

    void Awake()
    {
        Prepare_For_Carriage();
    }

    public void CarriageLoadDone()
    {
        EnterNextScene.Instance().DestroyUI();
        Destroy(this);
    }

    public void Prepare_For_Carriage()
    {
        InitCarriageLoading();

        EnterNextScene.DirectLoadLevel();
        StartCoroutine(LoadCarriageScene());
    }

    private IEnumerator LoadCarriageScene()
    {
        while (!UtilityTool.IsLevelLoaded(SceneTemplate.GetScenePath(SceneTemplate.SceneEnum.CARRIAGE)))
        {
            yield return new WaitForEndOfFrame();
        }

        Global.ResourcesDotLoad("_UIs/Carriage/CarriageScene", OnCarriageSceneLoaded);
    }

    private void OnCarriageSceneLoaded(ref WWW www, string path, Object prefab)
    {
        var sceneObject = Instantiate(prefab as GameObject);
        TransformHelper.ActiveWithStandardize(null, sceneObject.transform);

        UpdateLoadProgress(LoadModule.UI, "Carriage_UI");
    }
}