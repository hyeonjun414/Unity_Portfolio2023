using System;
using Model;
using Newtonsoft.Json;
using Presenter;
using UnityEngine;

namespace View
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public SceneSwitchView sceneSwitchView;
        public MasterTable MasterTable;

        public StagePresenter curStage;
        public UserPresenter user;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                var newMasterTable = Resources.Load<TextAsset>("MasterTable");
                MasterTable = JsonConvert.DeserializeObject<MasterTable>(newMasterTable.ToString());
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
