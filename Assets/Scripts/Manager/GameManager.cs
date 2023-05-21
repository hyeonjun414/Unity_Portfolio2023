using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Model;
using Newtonsoft.Json;
using Presenter;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using View;
using View.StageView;

namespace Manager
{
    public static class SceneType
    {
        public const string Title = "TitleView";
        public const string Map = "MapView";
        public const string BattleStage = "BattleStageView";
        public const string User = "UserView";
        public const string Shop = "ShopStageView";
    }
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public MasterTable MasterTable;

        public Stage CurStage;
        public User user;
        public Map CurMap;
        public List<SceneView> scenePrefabs;
        public SceneView curScene;
        public LoadingScreenView loadingScreen;

        public Physics2DRaycaster raycaster;
        public Camera mainCam;

        public FloatingTextView floatingText;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Application.targetFrameRate = 60;
                
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.Converters.Add(new EnumConverter<CardType>());
                settings.Converters.Add(new EnumConverter<ArtifactTrigger>());
                settings.Converters.Add(new EnumConverter<TargetType>());
                settings.Converters.Add(new EnumConverter<StatTag>());

                
                var newMasterTable = Resources.Load<TextAsset>("MasterTable");
                MasterTable = JsonConvert.DeserializeObject<MasterTable>(newMasterTable.ToString());

                var task = LoadScene(SceneType.Title);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Equals))
            {
                Time.timeScale *= 2;
            }
            else if (Input.GetKeyDown(KeyCode.Minus))
            {
                Time.timeScale *= 0.5f;
            }
        }

        public async UniTask GameStart()
        {
            await loadingScreen.FadeOut();
            user = new User(new UserModel(), CreateScene<UserView>(SceneType.User), MasterTable.MasterUsers[0], MasterTable);
            user.Init();
            
            var mapModel = new MapModel();
            mapModel.GenerateMap(MasterTable.MasterMaps[0], MasterTable);
            CurMap = new Map(mapModel, CreateScene<MapView>(SceneType.Map));
            CurMap.Init();
            await loadingScreen.FadeIn();
        }

        private T CreateScene<T>(string sceneName) where T : UnityEngine.Object
        {
            var targetView = scenePrefabs.First(t => t.name == sceneName);
            var inst = Instantiate(targetView);
            inst.SetParent(curScene);
            inst.Init();
            curScene = inst;
            return inst as T;
        }

        public async UniTask LoadScene(string sceneType, bool isFadeEft = true)
        {
            if(isFadeEft)
                await loadingScreen.FadeOut();
            var titlePrefab = scenePrefabs.First(t => t.name == sceneType);
            var newScene = Instantiate(titlePrefab);
            newScene.SetParent(curScene);
            newScene.Init();
            curScene = newScene;
            if (isFadeEft)
                await loadingScreen.FadeIn();
        }

        public async UniTask DestroyCurScene()
        {
            await loadingScreen.FadeOut();
            DestroyScene();
            await loadingScreen.FadeIn();
        }

        public void DestroyScene()
        {
            var destroyScene = curScene;
            curScene = destroyScene.Parent;
            Destroy(destroyScene.gameObject);
            if (curScene != null)
            {
                curScene.gameObject.SetActive(true);
            }
        }

        public async UniTask ReturnToMain()
        {
            await loadingScreen.FadeOut();
            while (curScene is not TitleView)
            {
                DestroyScene();
            }
            await loadingScreen.FadeIn();
        }

        public Stage GenerateStage(MapNodeModel mapNode)
        {
            Stage genStage = null;
            switch (mapNode.StageData.Type)
            {
                case nameof(BattleStageInfo):
                    genStage = new BattleStage(new BattleStageModel(mapNode,
                        MasterTable),
                        CreateScene<StageView>(SceneType.BattleStage));
                    genStage.Init();
                    break;
                case nameof(BossStageInfo):
                    genStage = new BossStage(new BossStageModel(mapNode,
                            MasterTable),
                        CreateScene<StageView>(SceneType.BattleStage));
                    genStage.Init();
                    break;
                case nameof(ShopStageInfo):
                    genStage = new ShopStage(new ShopStageModel(mapNode, user, MasterTable),
                        CreateScene<StageView>(SceneType.Shop));
                    genStage.Init();
                    break;
            }
            return genStage;
        }

        public async UniTask LoadMapScene()
        {
            await DestroyCurScene();
            CurMap.ActiveNextNodes();
            
        }
        public async UniTask LoadStageScene(MapNode mapNode)
        {
            await loadingScreen.FadeOut();
            CurStage = GenerateStage(mapNode.Model);
            await loadingScreen.FadeIn();
        }

        public void CreateFloatingText(string str, Vector3 position, TextType textType)
        {
            var textInst = Instantiate(floatingText);
            textInst.SetFloatingText(str, position, textType);
        }
    }
}
