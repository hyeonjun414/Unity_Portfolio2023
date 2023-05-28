using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Model;
using Newtonsoft.Json;
using Presenter;
using UnityEngine;
using UnityEngine.EventSystems;
using View;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public MasterTable MasterTable;
        public IntroScene introScene;
        
        public GameCore GameCore;
        public Stage CurStage => GameCore.CurScene as Stage;
        public Map CurMap => GameCore.CurScene as Map;
        public User user => GameCore.User;
        public SeedRandom Rand => GameCore.Rand;
        
        public List<SceneView> scenePrefabs;
        public Scene Title;
        public LoadingScreenView loadingScreen;

        public Physics2DRaycaster raycaster;
        public Camera mainCam;

        public List<ParticleSystem> Particles;

        public FloatingTextView floatingText;
        
        private void Awake()
        {
            if (Instance == null)
            {
                
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Application.targetFrameRate = 60;
                introScene.StartTimeLine();
                //Init();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Init()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new EnumConverter<CardType>());
            settings.Converters.Add(new EnumConverter<ArtifactTrigger>());
            settings.Converters.Add(new EnumConverter<TargetType>());
            settings.Converters.Add(new EnumConverter<StatTag>());

            Particles = Resources.LoadAll<ParticleSystem>("Particle").ToList();

            var newMasterTable = Resources.Load<TextAsset>("MasterTable");
            MasterTable = JsonConvert.DeserializeObject<MasterTable>(newMasterTable.ToString());

            Title = new Title(new SceneModel());
            var task = CreateTitle();
        }

        private async UniTask CreateTitle()
        {
            loadingScreen.Init();
            await UniTask.Delay(500);
            Title.SetView(CreateSceneView(Title));
            await loadingScreen.FadeIn();
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

        public async UniTask StartGame()
        {
            await loadingScreen.FadeOut();
            Title.SceneActive(false);
            GameCore = new GameCore();
            GameCore.Init();
            GameCore.User = new User(new UserModel(), MasterTable.MasterUsers[0], MasterTable);
            GameCore.User.SetView(CreateSceneView(GameCore.User));
            AddScene(GameCore.User);
            
            var map = new Map(new MapModel(), MasterTable.MasterMaps[0], MasterTable);
            map.SetView(CreateSceneView(map));
            AddScene(map);

            SaveGame();
            await loadingScreen.FadeIn();
        }

        public async UniTask ContinueGame()
        {
            await loadingScreen.FadeOut();
            Title.SceneActive(false);
            var saveData = PlayerPrefs.GetString("SaveData", "");
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.All
            };
            print(saveData);
            GameCore = JsonConvert.DeserializeObject<GameCore>(saveData, settings);
            GameCore.Load(this);
            await loadingScreen.FadeIn();
        }

        public SceneView CreateSceneView(Scene scene)
        {
            var viewTypeName = scene.GetType().Name + "View";
            var targetView = scenePrefabs.First(t => t.GetType().Name == viewTypeName);
            var inst = Instantiate(targetView);
            inst.Init();
            return inst;
        }

        public void AddScene(Scene scene)
        {
            GameCore.OpenScene(scene);
        }
        
        public async UniTask DestroyCurScene()
        {
            await loadingScreen.FadeOut();
            DestroyScene();
            await loadingScreen.FadeIn();
        }

        public void DestroyScene()
        {
            GameCore.CloseCurScene();
            SaveGame();
        }

        public void SaveGame()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.All
            };
            
            var data = JsonConvert.SerializeObject(GameCore, settings);
            PlayerPrefs.SetString("SaveData", data);
            PlayerPrefs.Save();
        }

        public async UniTask ReturnToMain()
        {
            await loadingScreen.FadeOut();
            SoundManager.Instance.SaveVolumeData();
            GameCore.Reset();
            GameCore = null;
            Title.SceneActive(true);
            await loadingScreen.FadeIn();
        }

        public Stage GenerateStage(MapNodeModel mapNode)
        {
            Stage genStage = null;
            switch (mapNode.StageData)
            {
                case BattleStageInfo:
                    genStage = new BattleStage(mapNode.stageModel);
                    break;
                case BossStageInfo:
                    genStage = new BossStage(mapNode.stageModel);
                    break;
                case ShopStageInfo:
                    genStage = new ShopStage(mapNode.stageModel);
                    break;
                case ChestStageInfo:
                    genStage = new ChestStage(mapNode.stageModel);
                    break;
            }
            if(genStage != null)
                genStage.SetView(CreateSceneView(genStage));
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
            GameCore.OpenScene(GenerateStage(mapNode.Model));
            await loadingScreen.FadeIn();
        }

        public void CreateFloatingText(string str, Vector3 position, TextType textType)
        {
            var textInst = Instantiate(floatingText);
            textInst.SetFloatingText(str, position, textType);
        }

        public void CreateEft(string effect, Transform pivot)
        {
            var particle = Particles.Find(t => t.name == effect);
            var inst = Instantiate(particle, pivot);
            Destroy(inst.gameObject, inst.main.duration);
        }

       
    }
}
