using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    public static StoreManager GM;               // 싱글톤
    public Text stageStep;                      // 현재 스테이지를 알려주는 텍스트
    public bool isGameOver = false;             // 시작 게임오버 값은 false
    public GameObject nextStep;                 // 다음 스탭 게임오브젝트
    public GameObject nextStage;                // 다음 스테이지 게임오브젝트
    public GameObject finishPopup;              // 게임이 끝났을 때 팝업

    public int nowStage;                        // 현재 단계를 나타낼 변수
    public int nowScore;                        // 현재 점수
    int score = 0;                              // 누적 스코어


    public int heart;
    public Text heartText;
    public GameObject exPop;

    int bestscore = 0;                                      // 임시 점수 저장
    void Awake()                                // 싱글톤
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
        stageStep.text = "점수 : " + nowScore.ToString();
        nextStep.SetActive(false);                                  // 다음 스탭버튼 비활성화
        if (nextStage != null)
            nextStage.SetActive(false);                                 // 다음 스테이지 버튼 비활성화
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

    public void ScoreUp(int points)                                  // btn스크립트에서 점수를 받아오는 메서드다
    {
        if (isGameOver)
        {
            return;
        }
        isGameOver = true;


        stageStep.gameObject.SetActive(true);                       // 현재 다음단계 버튼을 켜준다.
                                                                    //nowStage = PlayerPrefs.GetInt("Step");                      // 단계를 저장해줄 저장소
                                                                    //nowStage+= score;                                          // 다음 단계를 1점씩 올려준다.
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

        if (nowStage >= 4)                                          // 현재 스탭이 5와 같거나 커진다면
        {
            nowScore += score;
            nowStage = 0;                                           // 현재 스탭을 0으로 초기화 시키고
            PlayerPrefs.SetInt("Step", nowStage);
            PlayerPrefs.SetInt("Score", nowScore);                  // 여기서 중간점수 가져가라
            if (nowScore >= 15)                                     // 여기가 만점 점수 가져가는 곳이다~
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
        stageStep.text = "점수 : " + nowScore.ToString();       // 현재 단계를 나타내줄  text
    }

    public void NextStep()                                          // 다음 스탭 버튼 클릭 메서드
    {
        AudioManager.Instance.PlayOneShot("click");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 현재 씬을 다시 로드한다.
    }

    public void StopGame()
    {
    }

}