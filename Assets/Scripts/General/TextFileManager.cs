using System.IO;
using UnityEngine;

public class TextFileManager : MonoBehaviour
{
    #region Singleton
    private static TextFileManager _instance;
    public static TextFileManager Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    public static void DeleteFileExists(string fileName)
    {
        string path = Application.persistentDataPath + "/" + fileName + ".txt";
        if (File.Exists(path))
            File.Delete(path);
    }

    public static void WriteString(string fileName, string text)
    {
        string path = Application.persistentDataPath + "/" + fileName + ".txt";
        StreamWriter writer = new(path, true);
        writer.WriteLine(text);
        writer.Close();
    }
}
