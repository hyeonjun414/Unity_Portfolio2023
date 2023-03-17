using System;
using System.Collections.Generic;
using Core;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

namespace Title
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance => _instance;
        private static GameManager _instance;

        public SceneSwitcher sceneSwitcher;
        public SceneMaster sceneMaster;
        public User user;
        public MasterTable MasterTable;
        
        public void Awake()
        {
            
            if (Instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Start()
        {
            var newMasterTable = Resources.Load<TextAsset>("MasterTable");
            MasterTable = JsonConvert.DeserializeObject<MasterTable>(newMasterTable.ToString());
            print(MasterTable);
            
            //User Generate Test
            user = new User
            {
                MyHeroes = new List<Hero>()
            };
            for (var i = 0; i < 3; i++)
            {
                var masterHero = MasterTable.MasterHeroes[0];
                var hero = new Hero();
                hero.Init(masterHero);
                user.MyHeroes.Add(hero);
            }
        }
    }
}
