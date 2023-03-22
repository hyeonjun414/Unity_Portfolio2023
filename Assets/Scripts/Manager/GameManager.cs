using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Model;
using Newtonsoft.Json;
using Presenter;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace View
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
                
                GenerateStage(MasterTable.MasterStages[0]);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void GenerateStage(MasterStage ms)
        {
            CurStage = new Stage(new StageModel(ms, MasterTable), null);
        }

        public async UniTask LoadStageScene(MasterStage ms)
        {
            CurStage = new Stage(new StageModel(ms, MasterTable), null);
            await SceneSwitcher.AsyncSceneLoad("InGameScene");
        }
    }
}
