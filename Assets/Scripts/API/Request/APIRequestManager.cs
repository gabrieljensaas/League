using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulator.API
{
    public class APIRequestManager : MonoBehaviour
    {
        private RiotAPIResponse _riotAPIResponse;
        public RiotAPIResponse RiotAPIResponse => _riotAPIResponse;

        private SimManager simManager;
        private ExternalJS extarnaljs;

        #region Singleton
        private static APIRequestManager _instance;
        public static APIRequestManager Instance { get { return _instance; } }
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }
        #endregion

        private void Start()
        {
            simManager = SimManager.Instance;
            extarnaljs = GetComponent<ExternalJS>();
        }

        public void SendOutputToJS(string[] str)
        {
            extarnaljs.SendData(str);
        }
    }
}