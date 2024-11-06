using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public static CardManager instance;

    [Header("UI Elements")]
    [SerializeField] private Text timeoutText;
    [SerializeField] private Text scoreText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text gameOverText;
    [SerializeField] private GameObject exPop;

    [Header("Game Settings")]
    [SerializeField] private float timeLimit = 60f;
    [SerializeField] private int totalMatches = 6;

    private List<Card> allCards;
    private Card flippedCard;
    private bool isFlipping = false;
    private float currentTime;
    private int matchesFound = 0;
    private bool isGameOver = false;
    private int score = 0;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        isGameOver = false;
        isFlipping = true;
        exPop.SetActive(true);
        Board board = FindObjectOfType<Board>();
        allCards = board.GetCards();
        currentTime = timeLimit;
        matchesFound = 0;
        score = 0;
        UpdateUI();

        gameOverPanel.SetActive(false);
    }

    public void StartClick()
    {
        exPop.SetActive(false);
        AudioManager.Instance.PlayOneShot("shuffle");
        StartCoroutine(FlipAllCardsRoutine());
    }

    private IEnumerator FlipAllCardsRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        FlipAllCards();
        yield return new WaitForSeconds(3f);
        FlipAllCards();
        yield return new WaitForSeconds(0.5f);
        isFlipping = false;
        StartCoroutine(CountDownTimerRoutine());
    }

    private void FlipAllCards()
    {
        foreach (Card card in allCards)
        {
            card.FlipCard();
        }
    }

    private IEnumerator CountDownTimerRoutine()
    {
        while (currentTime > 0 && !isGameOver)
        {
            currentTime -= Time.deltaTime;
            UpdateUI();
            yield return null;
        }
        if (currentTime <= 0) GameOver(false);
    }

    public void CardClicked(Card card)
    {
        if (isFlipping || isGameOver) return;

        card.FlipCard();

        if (flippedCard == null)
        {
            flippedCard = card;
        }
        else
        {
            StartCoroutine(CheckMatchRoutine(flippedCard, card));
        }
    }

    private IEnumerator CheckMatchRoutine(Card card1, Card card2)
    {
        isFlipping = true;
        if (card1.cardID == card2.cardID)
        {
            card1.SetMatched();
            card2.SetMatched();
            matchesFound++;
            score += 10;  // 매치 성공 시 점수 추가

            if (matchesFound == totalMatches)
            {
                GameOver(true);
            }
        }
        else
        {
            yield return new WaitForSeconds(1f);
            card1.FlipCard();
            card2.FlipCard();
            score = Mathf.Max(0, score - 5);  // 매치 실패 시 점수 감소 (최소 0)
        }

        UpdateUI();
        isFlipping = false;
        flippedCard = null;
    }

    private void GameOver(bool success)
    {
        isGameOver = true;
        StopAllCoroutines();
        gameOverPanel.SetActive(true);

        if (success)
        {
            gameOverText.text = "성공하셨습니다!";
            scoreText.text = $"{Mathf.CeilToInt(currentTime)} 점";
        }
        else
        {
            gameOverText.text = "시간 초과입니다.";
        }

        SetScore();
    }

    private async void SetScore()
    {
        int resultScore = CalculateScore();
        try
        {
            await DBManager.Instance.UpdateGameScore("firstGame", resultScore);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to update score: {e.Message}");
        }
    }

    private int CalculateScore()
    {
        if (currentTime >= 30) return 100;
        if (currentTime >= 25) return 80;
        if (currentTime >= 20) return 60;
        if (currentTime >= 15) return 40;
        if (currentTime >= 10) return 20;
        return 0;
    }

    private void UpdateUI()
    {
        timeoutText.text = $"남은 시간: {Mathf.CeilToInt(currentTime)}";
        scoreText.text = $"점수: {score}";
    }

    public void NextStep()
    {
        // 다음 단계로 진행하는 로직 구현
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowGameOverPanel()
    {
        AudioManager.Instance.PlayOneShot("Click1");
        gameOverPanel.SetActive(true);
    }
}