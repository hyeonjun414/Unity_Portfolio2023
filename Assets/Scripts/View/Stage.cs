using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace View
{
    public class Stage : MonoBehaviour
    {
        public Model.Stage stageData;

        public GameObject tempEnemyObj;
        public List<Transform> enemyPosList;
        public List<Transform> heroPosList;
        public void Start()
        {
            if (GameManager.Instance == null) 
                return;
            
            var curStage = GameManager.Instance.MasterTable.MasterStages[0];
            stageData = new Model.Stage();
            stageData.Init(curStage);
            
            // Init Hero
            var user = GameManager.Instance.user;
            for (var index = 0; index < user.MyHeroes.Count; index++)
            {
                var hero = user.MyHeroes[index];
                var inst = Instantiate(tempEnemyObj);
                inst.transform.position = heroPosList[index].position;
                inst.SetActive(true);
            }

            // Init Enemy
            for (var index = 0; index < stageData.Enemies.Count; index++)
            {
                var enemy = stageData.Enemies[index];
                var inst = Instantiate(tempEnemyObj);
                inst.transform.position = enemyPosList[index].position;
                inst.SetActive(true);
            }
        }

    }
}