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
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                var newMasterTable = Resources.Load<TextAsset>("MasterTable");
                MasterTable = JsonConvert.DeserializeObject<MasterTable>(newMasterTable.ToString());

                SceneSwitcher = new SceneSwitcher(new SceneSwitcherModel(), null);
                
                var userView = gameObject.AddComponent<UserView>();
                User = new User(new UserModel(), userView, MasterTable.MasterUsers[0], MasterTable);
                userView.Presenter = User;

                CurStage = GenerateStage(MasterTable.MasterStages[0]);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public Stage GenerateStage(MasterStage ms)
        {
            var stageInfo = Util.ToObject<StageInfo>(ms.StageInfo);
            Stage genStage = null;
            switch (stageInfo.Type)
            {
                case nameof(BattleStageInfo):
                    genStage = new BattleStage(new BattleStageModel(stageInfo, MasterTable), null);
                    break;
            }

            return genStage;
        }

        public async UniTask LoadStageScene(MasterStage ms)
        {
            CurStage = GenerateStage(ms);
            await SceneSwitcher.AsyncSceneLoad("InGameScene");
        }
    }
}
