using System.Collections.Generic;
using UnityEngine;
namespace Simulator.API
{
    public class APIRequestManager : MonoBehaviour
    {
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
        /// <summary>
        /// Generates a hand written mock up data of ashe or garen
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public RiotAPIResponse GetMockChampionData(string name)
        {
            if (name == "Ashe") return new RiotAPIResponse()
            {
                ChampionsRes = new List<ChampionsRe>()
                {
                    new ChampionsRe()
                    {
                        _id = "Ashe",
                        champData = new ChampData()
                        {
                            type = "champion",
                            format = "standAloneComplex",
                            version = "12.7.1",
                            data = new Data()
                            {
                                Champion = new Champion()
                                {
                                    name = "Ashe",
                                    key ="22",
                                    stats = new Stats()
                                    {
                                        hp = 570,
                                        mp = 280,
                                        hpperlevel = 87,
                                        mpperlevel = 32,
                                        movespeed = 325,
                                        armor = 26,
                                        armorperlevel = 3.4d,
                                        spellblock = 30,
                                        spellblockperlevel = 0.5d,
                                        attackrange = 600,
                                        hpregen = 3.5d,
                                        hpregenperlevel = 0.55d,
                                        mpregen = 6.4d,
                                        mpregenperlevel = 0.4d,
                                        crit = 0,
                                        critperlevel = 0,
                                        attackdamage = 59,
                                        attackdamageperlevel = 2.96d,
                                        attackspeed = 0.658d,
                                        attackspeedperlevel = 3.33d,
                                    }
                                }
                            }
                        }
                    }
                }
            };
            else if (name == "Garen") return new RiotAPIResponse()
            {
                ChampionsRes = new List<ChampionsRe>()
                {
                    new ChampionsRe()
                    {
                        _id = "Garen",
                        champData = new ChampData()
                        {
                            type = "champion",
                            format = "standAloneComplex",
                            version = "12.7.1",
                            data = new Data()
                            {
                                Champion = new Champion()
                                {
                                    name = "Garen",
                                    key ="86",
                                    stats = new Stats()
                                    {
                                        hp = 620,
                                        mp = 0,
                                        hpperlevel = 84,
                                        mpperlevel = 0,
                                        movespeed = 340,
                                        armor = 36,
                                        armorperlevel = 3d,
                                        spellblock = 32,
                                        spellblockperlevel = 0.75d,
                                        attackrange = 175,
                                        hpregen = 8d,
                                        hpregenperlevel = 0.5d,
                                        mpregen = 0d,
                                        mpregenperlevel = 0d,
                                        crit = 0,
                                        critperlevel = 0,
                                        attackdamage = 66,
                                        attackdamageperlevel = 4.5d,
                                        attackspeed = 0.625d,
                                        attackspeedperlevel = 3.65d,
                                    }
                                }
                            }
                        }
                    }
                }
            };
            else return null;
        }
    }
}