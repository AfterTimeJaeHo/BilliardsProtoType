using System;
using TMPro;
using UnityEngine;

namespace Waving.Di
{
    public class DIInstaller : MonoBehaviour
    {
        private void Awake()
        {
            Transform parent = gameObject.transform.parent;
            if (parent != null)
            {
                DontDestroyOnLoad(parent);
            }
            else
            {
                DontDestroyOnLoad(this);    
            }
            
            PlayerTurnContainer playerTurnContainer = new PlayerTurnContainer();
        }
    }   
}