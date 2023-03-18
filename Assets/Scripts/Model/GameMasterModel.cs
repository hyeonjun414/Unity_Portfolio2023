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
            var masterHero = masterTable.MasterHeroes[0];
            user = new User
            {
                Hero = new EntityModel(masterHero)
            };
        }
    }

}
