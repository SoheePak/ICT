using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    public static StoreManager GM;               // �̱���
    public Text stageStep;                      // ���� ���������� �˷��ִ� �ؽ�Ʈ
    public bool isGameOver = false;             // ���� ���ӿ��� ���� false
    public GameObject nextStep;                 // ���� ���� ���ӿ�����Ʈ
    public GameObject nextStage;                // ���� �������� ���ӿ�����Ʈ
    public GameObject finishPopup;              // ������ ������ �� �˾�

    public int nowStage;                        // ���� �ܰ踦 ��Ÿ�� ����
    public int nowScore;                        // ���� ����
    int score = 0;                              // ���� ���ھ�


    public int heart;
    public Text heartText;
    public GameObject exPop;

    int bestscore = 0;                                      // �ӽ� ���� ����
    void Awake()                                // �̱���
    {
        if (GM == null)
        {
            GM = this;
        }
        else
        {
            Destroy(gameObject);
        }
        exPop.SetActive(false);
    }
    void Start()
    {
        isGameOver = false;
        nowStage = PlayerPrefs.GetInt("Step", 0);
        nowScore = PlayerPrefs.GetInt("Score", 0);
        heart = PlayerPrefs.GetInt("Heart", 3);
        stageStep.text = "���� : " + nowScore.ToString();
        nextStep.SetActive(false);                                  // ���� ���ǹ�ư ��Ȱ��ȭ
        if (nextStage != null)
            nextStage.SetActive(false);                                 // ���� �������� ��ư ��Ȱ��ȭ
        heartText.text = heart.ToString();
        if (nowScore == 0 || nowScore == 5 || nowScore == 10)
        {
            exPop.SetActive(true);
        }
    }

    public void minusHeart()
    {
        heart--;
        heartText.text = heart.ToString();
        PlayerPrefs.SetInt("Heart", heart);
    }

    public void StartClick()
    {
        AudioManager.Instance.PlayOneShot("click");
        exPop.SetActive(false);
    }

    void Update()
    {

    }

    public void ScoreUp(int points)                                  // btn��ũ��Ʈ���� ������ �޾ƿ��� �޼����
    {
        if (isGameOver)
        {
            return;
        }
        isGameOver = true;


        stageStep.gameObject.SetActive(true);                       // ���� �����ܰ� ��ư�� ���ش�.
                                                                    //nowStage = PlayerPrefs.GetInt("Step");                      // �ܰ踦 �������� �����
                                                                    //nowStage+= score;                                          // ���� �ܰ踦 1���� �÷��ش�.
                                                                    //nowStage += points;
        score += points;

        if (nowStage == 4)
        {
            nextStage.SetActive(true);
            nextStep.SetActive(false);
        }
        else
        {
            nextStep.SetActive(true);
            nextStage.SetActive(false);
        }

        if (nowStage >= 4)                                          // ���� ������ 5�� ���ų� Ŀ���ٸ�
        {
            nowScore += score;
            nowStage = 0;                                           // ���� ������ 0���� �ʱ�ȭ ��Ű��
            PlayerPrefs.SetInt("Step", nowStage);
            PlayerPrefs.SetInt("Score", nowScore);                  // ���⼭ �߰����� ��������
            if (nowScore >= 15)                                     // ���Ⱑ ���� ���� �������� ���̴�~
            {
                DBManager.Instance.UpdateGameScore("thirdGame", 100);
                nowScore = 0;
                PlayerPrefs.SetInt("Score", nowScore);
                PlayerPrefs.SetInt("Heart", 3);
                finishPopup.SetActive(true);
            }
        }
        else
        {
            nowStage += score;
            nowScore += score;
            PlayerPrefs.SetInt("Score", nowScore);
            PlayerPrefs.SetInt("Step", nowStage);
        }
        stageStep.text = "���� : " + nowScore.ToString();       // ���� �ܰ踦 ��Ÿ����  text
    }

    public void NextStep()                                          // ���� ���� ��ư Ŭ�� �޼���
    {
        AudioManager.Instance.PlayOneShot("click");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // ���� ���� �ٽ� �ε��Ѵ�.
    }

    public void StopGame()
    {
    }

}