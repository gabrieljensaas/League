using System.Collections;
using UnityEngine;

namespace Simulator.API
{
    public class APIRequestManager : MonoBehaviour
    {
        private RiotAPIResponse _riotAPIResponse;
        public RiotAPIResponse RiotAPIResponse => _riotAPIResponse;

        private SimManager simManager;
        private ExternalJS externalJS;

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
            externalJS = GetComponent<ExternalJS>();
        }

        public void SendOutputToJS(WebData data)
        {
            externalJS.SendData(data);
        }

        public void LoadChampionData(string response)
        {
            simManager.LoadStats(JsonUtility.FromJson<LSSAPIResponse>(response));
            StartCoroutine(StartBattle());
        }

        public IEnumerator StartBattle()
        {
            yield return new WaitForSeconds(3f);
            simManager.StartBattle();
        }
    }
}