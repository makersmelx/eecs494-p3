using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class VideoManager : MonoBehaviour
{
    public void SkipScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
