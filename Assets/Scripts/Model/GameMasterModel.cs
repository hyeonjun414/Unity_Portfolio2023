using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using View;

namespace Model
{
    public class GameMasterModel
    {
        public User user;
        public MasterTable masterTable;

        public GameMasterModel()
        {
            // Load Master Table
            var newMasterTable = Resources.Load<TextAsset>("MasterTable");
            masterTable = JsonConvert.DeserializeObject<MasterTable>(newMasterTable.ToString());

            // Generate User Test
            user = new User
            {
                MyHeroes = new List<EntityModel>()
            };
            for (var i = 0; i < 3; i++)
            {
                var masterHero = masterTable.MasterHeroes[0];
                var hero = new EntityModel(masterHero);
                user.MyHeroes.Add(hero);
            }
        }
    }

}
