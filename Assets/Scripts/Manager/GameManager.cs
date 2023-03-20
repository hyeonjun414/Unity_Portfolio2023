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
        public SceneSwitchPresenter SceneSwitchPresenter;
        public MasterTable MasterTable;

        public StagePresenter CurStage;
        public UserPresenter User;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                var newMasterTable = Resources.Load<TextAsset>("MasterTable");
                MasterTable = JsonConvert.DeserializeObject<MasterTable>(newMasterTable.ToString());

                SceneSwitchPresenter = new SceneSwitchPresenter(new SceneSwitchModel(), null);
                
                var userView = gameObject.AddComponent<UserView>();
                User = new UserPresenter(new UserModel(MasterTable.MasterHeroes[0]), userView);
                
                GenerateStage(MasterTable.MasterStages[0]);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void GenerateStage(MasterStage ms)
        {
            CurStage = new StagePresenter(new StageModel(ms, MasterTable), null);
        }

        public async UniTask LoadStageScene(MasterStage ms)
        {
            CurStage = new StagePresenter(new StageModel(ms, MasterTable), null);
            await SceneSwitchPresenter.AsyncSceneLoad("InGameScene");
        }
    }
}
