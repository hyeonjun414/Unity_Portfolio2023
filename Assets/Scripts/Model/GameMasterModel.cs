using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Model
{
    public class GameMasterModel
    {
        public MasterTable masterTable;

        public GameMasterModel()
        {
            // Load Master Table
            var newMasterTable = Resources.Load<TextAsset>("MasterTable");
            masterTable = JsonConvert.DeserializeObject<MasterTable>(newMasterTable.ToString());

        }
    }

}
