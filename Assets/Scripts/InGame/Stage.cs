using System;
using System.Collections.Generic;
using Core;
using Title;
using UnityEngine;

namespace InGame
{
    public class Stage : MonoBehaviour
    {
        public Core.Stage stageData;

        public GameObject tempEnemyObj;
        public List<Transform> enemyPosList; 
        public void Start()
        {
            if (GameManager.Instance == null) 
                return;
            
            var curStage = GameManager.Instance.MasterTable.MasterStages[0];
            stageData = new Core.Stage();
            stageData.Init(curStage);

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
