using System;
using System.Collections.Generic;
using UnityEngine;

namespace Title
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance => _instance;
        private static GameManager _instance;

        public SceneSwitcher sceneSwitcher;
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


        
    }
}
