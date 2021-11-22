using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    [SerializeField] VideoPlayer videoPlayer;

    private void Start()
    {
        GameConstants.playCutScene += 1;
        videoPlayer.loopPointReached += (vp) => SkipScene();
    }

    public void SkipScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}