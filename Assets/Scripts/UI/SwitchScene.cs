using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SwitchScene : MonoBehaviour
{
    // Start is called before the first frame update
    public string scene_name;


    public void LoadScene()
    {
        SceneManager.LoadScene(scene_name);
    }
}
