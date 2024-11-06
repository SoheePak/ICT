using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    // 개선: 씬 이름을 사용하여 전환
    public void FadeOutNextScene(string sceneName)
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.DOFade(1f, fadeDuration).OnComplete(() =>
        {
            SceneManager.LoadScene(sceneName);
        });
    }
    public void FadeOutNextScene(int sceneName)
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.DOFade(1f, fadeDuration).OnComplete(() =>
        {
            SceneManager.LoadScene(sceneName);
        });
    }

    // 개선: 씬 이름을 사용하여 전환
    public void NextScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void Restart()
    {
        AudioManager.Instance.PlayOneShot("click");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PlayClickSound(string name)
    {
        AudioManager.Instance.PlayOneShot(name);
    }

    public void PlayMainSceneBGM()
    {
        AudioManager.Instance.ChangeBGM("BGM");
    }

    // 개선: 유틸리티 클래스로 이동 고려
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}