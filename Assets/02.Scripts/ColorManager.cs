using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class ColorManager : MonoBehaviour
{
    [System.Serializable]
    private class ColorData
    {
        public Color color;
        public string name;
    }

    [Header("Color Data")]
    [SerializeField] private ColorData[] colorData;

    [Header("UI Elements")]
    [SerializeField] private GameObject[] answerCards;
    [SerializeField] private GameObject wrongCard;
    [SerializeField] private Text countdownText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text timerText;
    [SerializeField] private Text gameOverScoreText;
    [SerializeField] private GameObject tutorial1Popup;
    [SerializeField] private GameObject tutorial2Popup;
    [SerializeField] private GameObject gameOverPopup;

    [Header("Game Settings")]
    [SerializeField] private float gameTime = 60f;
    [SerializeField] private int scorePerCorrect = 10;
    [SerializeField] private int scorePerWrong = 5;

    private List<ColorData> availableColors;
    private int score = 0;
    private float timeLeft;
    private bool isGameActive = false;

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        tutorial1Popup.SetActive(true);
        tutorial2Popup.SetActive(false);
        gameOverPopup.SetActive(false);
        score = 0;
        timeLeft = gameTime;
        UpdateUI();
    }

    public void NextPage()
    {
        tutorial2Popup.SetActive(true);
        tutorial1Popup.SetActive(false);
    }

    public void StartClick()
    {
        tutorial2Popup.SetActive(false);
        StartCoroutine(StartCountdown());
    }

    private IEnumerator StartCountdown()
    {
        countdownText.gameObject.SetActive(true);
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            countdownText.transform.DOScale(2f, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                countdownText.transform.DOScale(1f, 0.5f);
            });
            yield return new WaitForSeconds(1f);
        }
        countdownText.gameObject.SetActive(false);
        StartGame();
    }

    private void StartGame()
    {
        isGameActive = true;
        StartCoroutine(TimerCoroutine());
        SetupRound();
    }

    private IEnumerator TimerCoroutine()
    {
        while (isGameActive && timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            UpdateUI();
            yield return null;
        }

        if (timeLeft <= 0)
        {
            GameOver();
        }
    }

    private void UpdateUI()
    {
        scoreText.text = $"현재 점수: {score}";
        timerText.text = $"남은 시간: {Mathf.CeilToInt(timeLeft)}";
    }

    private void SetupRound()
    {
        availableColors = new List<ColorData>(colorData);
        int wrongCardIndex = Random.Range(0, answerCards.Length);

        for (int i = 0; i < answerCards.Length; i++)
        {
            if (i != wrongCardIndex)
            {
                answerCards[i].SetActive(true);
                SetRandomColor(answerCards[i]);
            }
            else
            {
                answerCards[i].SetActive(false);
            }
        }

        wrongCard.SetActive(true);
        wrongCard.transform.position = answerCards[wrongCardIndex].transform.position;
        SetWrongColor(wrongCard);
    }

    private void SetRandomColor(GameObject obj)
    {
        if (availableColors.Count == 0) return;

        int randomIndex = Random.Range(0, availableColors.Count);
        ColorData randomColorData = availableColors[randomIndex];
        availableColors.RemoveAt(randomIndex);

        Image image = obj.GetComponent<Image>();
        Text text = obj.GetComponentInChildren<Text>();

        if (image != null)
            image.color = randomColorData.color;

        if (text != null)
        {
            text.text = randomColorData.name;
            text.color = IsColorDark(randomColorData.color) ? Color.white : Color.black;
        }
    }

    private void SetWrongColor(GameObject obj)
    {
        if (availableColors.Count < 2) return;

        int colorIndex = Random.Range(0, availableColors.Count);
        int textColorIndex;
        do
        {
            textColorIndex = Random.Range(0, availableColors.Count);
        } while (textColorIndex == colorIndex);

        ColorData colorData = availableColors[colorIndex];
        ColorData textColorData = availableColors[textColorIndex];

        Image image = obj.GetComponent<Image>();
        Text text = obj.GetComponentInChildren<Text>();

        if (image != null)
            image.color = colorData.color;

        if (text != null)
        {
            text.text = textColorData.name;
            text.color = IsColorDark(colorData.color) ? Color.white : Color.black;
        }
    }

    private bool IsColorDark(Color color)
    {
        return (color.r * 0.299f + color.g * 0.587f + color.b * 0.114f) < 0.5f;
    }

    public void Check(int myNum)
    {
        if (!isGameActive) return;

        AudioManager.Instance.PlayOneShot("cardopen");

        if (myNum == -1)
            CorrectAnswer();
        else
            WrongAnswer();
    }

    private void CorrectAnswer()
    {
        score += scorePerCorrect;
        SetupRound();
    }

    private void WrongAnswer()
    {
        score = Mathf.Max(0, score - scorePerWrong);
    }

    private void GameOver()
    {
        AudioManager.Instance.PlayOneShot("Error");
        isGameActive = false;
        gameOverPopup.SetActive(true);
        gameOverScoreText.text = scoreText.text;
        SetScore();
    }

    private async void SetScore()
    {
        int resultScore = CalculateResultScore();
        try
        {
            await DBManager.Instance.UpdateGameScore("secondGame", resultScore);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to update score: {e.Message}");
        }
    }

    private int CalculateResultScore()
    {
        if (score >= 200) return 100;
        if (score >= 170) return 80;
        if (score >= 140) return 60;
        if (score >= 110) return 40;
        if (score >= 80) return 20;
        return 0;
    }
}