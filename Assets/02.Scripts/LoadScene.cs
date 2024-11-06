using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    // ����: �� �̸��� ����Ͽ� ��ȯ
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

    // ����: �� �̸��� ����Ͽ� ��ȯ
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

    // ����: ��ƿ��Ƽ Ŭ������ �̵� ���
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}