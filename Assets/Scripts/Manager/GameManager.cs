using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Model;
using Newtonsoft.Json;
using Presenter;
using UnityEngine;
using View;

namespace Manager
{
    public static class SceneType
    {
        public const string Title = "TitleView";
        public const string Map = "MapView";
        public const string BattleStage = "BattleStageView";
    }
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public MasterTable MasterTable;

        public Stage CurStage;
        public User User;
        public Map CurMap;
        public List<SceneView> scenePrefabs;
        public SceneView curScene;
        public LoadingScreenView loadingScreen;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                var newMasterTable = Resources.Load<TextAsset>("MasterTable");
                MasterTable = JsonConvert.DeserializeObject<MasterTable>(newMasterTable.ToString());

                var task = CreateScene(SceneType.Title);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public async UniTask GameStart()
        {
            var userView = gameObject.AddComponent<UserView>();
            User = new User(new UserModel(), userView, MasterTable.MasterUsers[0], MasterTable);
            userView.Presenter = User;
            
            var mapModel = new MapModel();
            mapModel.GenerateMap(MasterTable.MasterMaps[0], MasterTable);
            CurMap = new Map(mapModel, null);

            await CreateScene(SceneType.Map);
            //await LoadScene(SceneType.Map);
        }

        public async UniTask CreateScene(string sceneType)
        {
            await loadingScreen.FadeOut();
            var titlePrefab = scenePrefabs.First(t => t.name == sceneType);
            var newScene = Instantiate(titlePrefab);
            newScene.SetParent(curScene);
            curScene = newScene;
            await loadingScreen.FadeIn();
            
        }

        public async UniTask DestroyCurScene()
        {
            await loadingScreen.FadeOut();
            var destroyScene = curScene;
            curScene = destroyScene.Parent;
            Destroy(destroyScene.gameObject);
            if (curScene != null)
            {
                curScene.gameObject.SetActive(true);
            }

            await loadingScreen.FadeIn();
        }

        public Stage GenerateStage(StageInfo stageInfo)
        {
            Stage genStage = null;
            switch (stageInfo.Type)
            {
                case nameof(BattleStageInfo):
                    genStage = new BattleStage(new BattleStageModel(stageInfo, MasterTable), null);
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
            CurStage = GenerateStage(mapNode.Model.StageData);
            await CreateScene(SceneType.BattleStage);
            //await LoadScene(SceneType.Stage);
        }
    }
}
