using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public SceneAsset Race01;

    public void LoadRace01Scene() => SceneManager.LoadScene(Race01.name);
}
