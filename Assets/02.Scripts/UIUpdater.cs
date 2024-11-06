using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIUpdater : MonoBehaviour
{
    public Text firstGameTotalText;
    public Text secondGameTotalText;
    public Text thirdGameTotalText;
    public Text firstGameDailyText;
    public Text secondGameDailyText;
    public Text thirdGameDailyText;

    public Slider bgmVolumeSlider;

    private AudioManager audioManager;
    private UserData userData;


    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            audioManager = AudioManager.Instance;
            bgmVolumeSlider.value = audioManager.GetVolume("BGM");
        }
        else
        {
            // DBManager�� �̺�Ʈ�� ����
            DBManager.Instance.OnDataChanged += UpdateUI;
            Init();
            // �ʱ� UI ������Ʈ
        }
    }

    private async void Init()
    {
        userData = await DBManager.Instance.LoadData(DBManager.Instance.userUid);
    }

    // �����̴��� ������ ����
    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            SetBGMVolume();
        }
        else
        {
            UpdateUI(userData);
        }
    }

    private void OnDestroy()
    {
        // �̺�Ʈ ���� ����
        if (DBManager.Instance != null)
        {
            DBManager.Instance.OnDataChanged -= UpdateUI;
        }
    }

    private void UpdateUI(UserData data)
    {
        string today = System.DateTime.Now.DayOfWeek.ToString().ToLower();
        if (data != null)
        {
            if (firstGameTotalText != null) firstGameTotalText.text = data.firstGame.allTimeHighScore.ToString();
            if (secondGameTotalText != null) secondGameTotalText.text = data.secondGame.allTimeHighScore.ToString();
            if (thirdGameTotalText != null) thirdGameTotalText.text = data.thirdGame.allTimeHighScore.ToString();
            if (firstGameDailyText != null) firstGameDailyText.text = data.firstGame.GetDailyScore(today).ToString();
            if (secondGameDailyText != null) secondGameDailyText.text = data.secondGame.GetDailyScore(today).ToString();
            if (thirdGameDailyText != null) thirdGameDailyText.text = data.thirdGame.GetDailyScore(today).ToString();
        }
    }

    private void SetBGMVolume()
    {
        audioManager.SetVolume("BGM", bgmVolumeSlider.value);
    }

    public void Mute()
    {
        bgmVolumeSlider.value = 0;
    }
}