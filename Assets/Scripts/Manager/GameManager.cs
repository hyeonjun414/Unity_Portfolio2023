using Cysharp.Threading.Tasks;
using Model;
using Newtonsoft.Json;
using Presenter;
using UnityEngine;
using View;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public SceneSwitcher SceneSwitcher;
        public MasterTable MasterTable;

        public Stage CurStage;
        public User User;
        public Map CurMap;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                var newMasterTable = Resources.Load<TextAsset>("MasterTable");
                MasterTable = JsonConvert.DeserializeObject<MasterTable>(newMasterTable.ToString());

                SceneSwitcher = new SceneSwitcher(new SceneSwitcherModel(), null);
                
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

            await SceneSwitcher.AsyncSceneLoad("MapScene");
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

        public async UniTask LoadStageScene(MapNode mapNode)
        {
            CurStage = GenerateStage(mapNode.Model.StageData);
            await SceneSwitcher.AsyncSceneLoad("InGameScene");
        }
    }
}
