using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeDictionary : MonoBehaviour
{
    [SerializeField]
    public class KeyValPair
    {
        public string key;
        public float[] val = new float[5];
    }

    public List<KeyValPair> myList = new List<KeyValPair>();
    public Dictionary<string, float[]> myDict = new Dictionary<string, float[]>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
