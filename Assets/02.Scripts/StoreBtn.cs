using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class StoreBtn : MonoBehaviour
{
    [Header("Text")]
    public Text numtext1000;                    // 10000�� ����� ��Ÿ�� text
    public Text numtext500;                     // 5000�� ����� ��Ÿ�� text
    public Text numtext100;                     // 1000�� ����� ��Ÿ�� text
    public Text[] foodMoney;                    // ���� ����

    [Header("Button")]
    public Button upBut1000;                    // 10000�� �÷��� ��ư
    public Button downBut1000;                  // 10000�� ������ ��ư
    public Button upBut500;                     // 5000�� �÷��� ��ư
    public Button downBut500;                   // 5000�� ������ ��ư
    public Button upBut100;                     // 1000�� �÷��� ��ư
    public Button downBut100;                   // 1000�� ������ ��ư
    public Button answer_Btn;                   // ���� ��ư

    //public Text money;                        // ���� �� ��Ÿ����

    [Header("Popup UI")]
    public GameObject gameOverUI;
    public GameObject rightAnswerPopup;
    public GameObject wrongAnswerPopup;

    int a;                                      // 1000�� 
    int b;                                      // 500��
    int c;                                      // 100��
    int sum;                                    // ����ڰ� ������ �ݾ�
    int foodsum = 0;                            // ��ä ����(����)
    int[] random;
    int upScore = 1;
    int nScore = 0;

    void Start()
    {
        a = b = c = 0;
        rightAnswerPopup.SetActive(false);
        wrongAnswerPopup.SetActive(false);
        gameOverUI.SetActive(false);
        nScore = PlayerPrefs.GetInt("Score");

        if (SceneManager.GetActiveScene().name == "5.1_StoreGame")
        {
            random = new int[3];
            foodsum = 0;
            random[0] = Random.Range(10, 50) * 1000;    // ��� ������
            random[1] = Random.Range(1, 10) * 1000;     // ��ä ������
            random[2] = Random.Range(1, 10) * 1000;     // ����� ������

            for (int i = 0; i < random.Length; i++)
            {
                foodMoney[i].text = random[i].ToString() + "��";
                foodsum += random[i];
            }
        }
        if (SceneManager.GetActiveScene().name == "6.2_StoreGame")
        {
            random = new int[1];
            random[0] = Random.Range(1, 480) * 10;
            foodMoney[0].text = random[0].ToString() + "��";
            foodsum = random[0];
        }
        if (SceneManager.GetActiveScene().name == "7.3_StoreGame")
        {
            random = new int[2];
            foodsum = 0;
            for (int i = 0; i < random.Length; i++)
            {
                random[i] = Random.Range(1, 50) * 50;   //��ä ������ �ִ� 3õ��
                Debug.Log(random[i]);
                foodMoney[i].text = random[i].ToString() + "��";
                foodsum += random[i];
            }
        }
        numtext1000.text = a.ToString();
        numtext1000.text = b.ToString();
        numtext1000.text = c.ToString();
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "5.1_StoreGame")
        {
            if (StoreManager.GM.isGameOver == false)
            {
                sum = (a * 10000) + (b * 5000) + (c * 1000);
            }
        }
        if (SceneManager.GetActiveScene().name == "6.2_StoreGame")
        {
            if (StoreManager.GM.isGameOver == false)
            {
                sum = (a * 1000) + (b * 100) + (c * 10);
            }
        }
        if (SceneManager.GetActiveScene().name == "7.3_StoreGame")
        {
            if (StoreManager.GM.isGameOver == false)
            {
                sum = (a * 1000) + (b * 100) + (c * 50);
            }
        }

    }

    public void upOnClick1000()
    {
        if (StoreManager.GM.isGameOver == false)
        {
            AudioManager.Instance.PlayOneShot("coin");
            a++;
            numtext1000.text = a.ToString();
        }
    }

    public void downOnClick1000()
    {
        if (StoreManager.GM.isGameOver == false)
        {
            AudioManager.Instance.PlayOneShot("dcoin");
            if (a > 0) { a--; }
            numtext1000.text = a.ToString();
        }
    }
    public void upOnClick500()
    {
        if (StoreManager.GM.isGameOver == false)
        {
            AudioManager.Instance.PlayOneShot("coin");
            b++;
            numtext500.text = b.ToString();
        }
    }

    public void downOnClick500()
    {
        if (StoreManager.GM.isGameOver == false)
        {
            AudioManager.Instance.PlayOneShot("dcoin");
            if (b > 0) { b--; }
            numtext500.text = b.ToString();
        }
    }
    public void upOnClick100()
    {
        if (StoreManager.GM.isGameOver == false)
        {
            AudioManager.Instance.PlayOneShot("coin");
            c++;
            numtext100.text = c.ToString();
        }
    }

    public void downOnClick100()
    {
        if (StoreManager.GM.isGameOver == false)
        {
            AudioManager.Instance.PlayOneShot("dcoin");
            if (c > 0) { c--; }
            numtext100.text = c.ToString();
        }
    }

    public void submitOnclik()
    {
        if (StoreManager.GM.isGameOver == true)
        {
            return;
        }
        if (SceneManager.GetActiveScene().name == "5.1_StoreGame")
        {
            if (foodsum == sum)
            {
                StoreManager.GM.ScoreUp(upScore);                // ���ھ� ���� 1���� �ִ� �޼���
                StoreManager.GM.nextStep.SetActive(true);        // 
                rightAnswerPopup.SetActive(true);
                AudioManager.Instance.PlayOneShot("right");
            }
            else
            {
                if (StoreManager.GM.heart > 1)
                {
                    StoreManager.GM.minusHeart();
                    StoreManager.GM.isGameOver = true;
                    wrongAnswerPopup.SetActive(true);
                    Invoke("Resume", 2f);
                    AudioManager.Instance.PlayOneShot("wrong");
                }
                else
                {
                    // ���ӿ���
                    nScore = PlayerPrefs.GetInt("Score");
                    SetScore(nScore);
                    StoreManager.GM.minusHeart();               // Ʋ������ ������ ��ư �ִµ� ���������� ���⼭ ��������
                    PlayerPrefs.SetInt("Score", 0);
                    StoreManager.GM.isGameOver = true;
                    gameOverUI.SetActive(true);
                    AudioManager.Instance.PlayOneShot("Error");
                }
            }
        }
        else
        {
            if (5000 - foodsum == sum)
            {
                StoreManager.GM.ScoreUp(upScore);               // ���ھ� ���� 1���� �ִ� �޼���
                StoreManager.GM.nextStep.SetActive(true);
                rightAnswerPopup.SetActive(true);
                AudioManager.Instance.PlayOneShot("right");
            }
            else
            {
                if (StoreManager.GM.heart > 1)
                {
                    StoreManager.GM.minusHeart();
                    StoreManager.GM.isGameOver = true;
                    wrongAnswerPopup.SetActive(true);           // Ʋ������ ������ ��ư �ִµ� ���������� ���⼭ ��������
                    Invoke("Resume", 2f);
                    AudioManager.Instance.PlayOneShot("wrong");
                }
                else
                {
                    nScore = PlayerPrefs.GetInt("Score");
                    SetScore(nScore);
                    StoreManager.GM.minusHeart();
                    PlayerPrefs.SetInt("Score", 0);
                    StoreManager.GM.isGameOver = true;
                    gameOverUI.SetActive(true);
                    AudioManager.Instance.PlayOneShot("Error");
                }
            }
        }
    }
    public void Resume()
    {
        wrongAnswerPopup.SetActive(false);
        StoreManager.GM.isGameOver = false;
    }

    public void ResetHart()
    {
        AudioManager.Instance.PlayOneShot("click");
        PlayerPrefs.SetInt("Heart", 3);
        DBManager.Instance.SaveData();
    }
    async void SetScore(int score)
    {
        int resultScore = 0;
        if (score >= 12) resultScore = 80;
        else if (score >= 10) resultScore = 60;
        else if (score >= 8) resultScore = 40;
        else if (score >= 6) resultScore = 20;
        else resultScore = 0;

        await DBManager.Instance.UpdateGameScore("thirdGame", resultScore);
    }
}