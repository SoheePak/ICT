using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class DBManager : MonoBehaviour
{
    public static DBManager Instance { get; private set; }
    private DatabaseReference refData;
    private UserData userData;
    public string userUid;

    // 데이터 변경 이벤트
    public event Action<UserData> OnDataChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        refData = FirebaseDatabase.DefaultInstance.RootReference;
    }

    void Start()
    {
    }

    // 데이터가 이미 있는지 체크
    public async Task InitAsync()
    {
        if (string.IsNullOrEmpty(userUid))
        {
            Debug.LogError("User UID is not set. Please set the userUid before calling Init.");
            return;
        }

        try
        {
            userData = await LoadData(userUid);
            Debug.Log($"User data initialized for UID: {userUid}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error initializing user data: {e.Message}");
        }
    }

    // 데이터 저장
    public async Task SaveData(UserData data)
    {
        string jsonData = JsonUtility.ToJson(data);
        await refData.Child("Information").Child(data.uid).SetRawJsonValueAsync(jsonData);
        OnDataChanged?.Invoke(data);
    }
    
    public void SaveData()
    {
        SaveData(userData);
    }

    // 게임 점수 db에 저장
    public async Task UpdateGameScore(string gameType, int score)
    {
        try
        {
            if (userData == null)
            {
                userData = await LoadData(userUid);
            }

            GameData gameData = GetGameData(gameType);
            string today = System.DateTime.Now.DayOfWeek.ToString().ToLower();
            gameData.SetDailyScore(today, Mathf.Max(gameData.GetDailyScore(today), score));
            gameData.weeklyHighScore = Mathf.Max(gameData.weeklyHighScore, score);
            gameData.allTimeHighScore = Mathf.Max(gameData.allTimeHighScore, score);

            if ((System.DateTime.Now - gameData.LastReset).Days >= 7)
            {
                ResetWeeklyScores(gameData);
            }

            await SaveData(userData);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error updating game score: {e.Message}");
        }
    }

    // db에서 데이터 로드
    private GameData GetGameData(string gameType)
    {
        switch (gameType)
        {
            case "firstGame":
                return userData.firstGame;
            case "secondGame":
                return userData.secondGame;
            case "thirdGame":
                return userData.thirdGame;
            default:
                throw new ArgumentException("Invalid game type");
        }
    }

    // 데이터 로드
    public async Task<UserData> LoadData(string _uid)
    {
        DataSnapshot snap = await refData.Child("Information").Child(_uid).GetValueAsync();
        if (!snap.Exists)
        {
            userData = CreateNewUserData(_uid);
            await SaveData(userData);  // 새 사용자 데이터를 DB에 저장
            Debug.Log($"새 사용자 uid 저장 : {_uid}");
        }
        else
        {
            string json = snap.GetRawJsonValue();
            userData = JsonUtility.FromJson<UserData>(json);
            // 기존 데이터를 새 형식으로 변환
            userData.firstGame = GameData.FromJson(JsonUtility.ToJson(userData.firstGame));
            userData.secondGame = GameData.FromJson(JsonUtility.ToJson(userData.secondGame));
            userData.thirdGame = GameData.FromJson(JsonUtility.ToJson(userData.thirdGame));
            Debug.Log($"Loaded existing user data for UID: {_uid}");
        }

        // 주간 리셋 확인
        CheckAndResetWeeklyScores(userData.firstGame);
        CheckAndResetWeeklyScores(userData.secondGame);
        CheckAndResetWeeklyScores(userData.thirdGame);

        OnDataChanged?.Invoke(userData);
        return userData;
    }

    // 일주일이 지났는지 체크
    private void CheckAndResetWeeklyScores(GameData gameData)
    {
        if ((DateTime.Now - gameData.LastReset).Days >= 7)
        {
            ResetWeeklyScores(gameData);
        }
    }

    // 주간 점수 초기화
    private void ResetWeeklyScores(GameData gameData)
    {
        gameData.weeklyHighScore = 0;
        gameData.dailyScores = new GameData.DailyScores();
        gameData.LastReset = DateTime.Now;
    }

    // 새로운 유저데이터 생성
    private UserData CreateNewUserData(string _uid)
    {
        return new UserData(_uid);
    }

    // 현재 UserData 가져오기
    public UserData GetCurrentUserData()
    {
        return userData;
    }
}

[System.Serializable]
public class UserData
{
    public string uid;
    public GameData firstGame;
    public GameData secondGame;
    public GameData thirdGame;

    public UserData(string _uid)
    {
        uid = _uid;
        firstGame = new GameData();
        secondGame = new GameData();
        thirdGame = new GameData();
    }
}

[System.Serializable]
public class GameData
{
    [System.Serializable]
    public class DailyScores
    {
        public int monday;
        public int tuesday;
        public int wednesday;
        public int thursday;
        public int friday;
        public int saturday;
        public int sunday;
    }

    public DailyScores dailyScores;
    public int weeklyHighScore;
    public int allTimeHighScore;
    public long lastResetTicks;  // DateTime을 long으로 저장

    public GameData()
    {
        dailyScores = new DailyScores();
        weeklyHighScore = 0;
        allTimeHighScore = 0;
        lastResetTicks = DateTime.Now.Ticks;
    }


    // JSON 직렬화를 위한 메서드
    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    // JSON 역직렬화를 위한 정적 메서드
    public static GameData FromJson(string json)
    {
        GameData data = JsonUtility.FromJson<GameData>(json);
        if (data.dailyScores == null)
        {
            data.dailyScores = new DailyScores();
        }
        return data;
    }

    // DateTime 프로퍼티
    public DateTime LastReset
    {
        get { return new DateTime(lastResetTicks); }
        set { lastResetTicks = value.Ticks; }
    }

    // 요일별 점수 설정 메서드
    public void SetDailyScore(string day, int score)
    {
        switch (day.ToLower())
        {
            case "monday": dailyScores.monday = score; break;
            case "tuesday": dailyScores.tuesday = score; break;
            case "wednesday": dailyScores.wednesday = score; break;
            case "thursday": dailyScores.thursday = score; break;
            case "friday": dailyScores.friday = score; break;
            case "saturday": dailyScores.saturday = score; break;
            case "sunday": dailyScores.sunday = score; break;
            default: throw new ArgumentException("Invalid day");
        }
    }

    // 요일별 점수 가져오기 메서드
    public int GetDailyScore(string day)
    {
        switch (day.ToLower())
        {
            case "monday": return dailyScores.monday;
            case "tuesday": return dailyScores.tuesday;
            case "wednesday": return dailyScores.wednesday;
            case "thursday": return dailyScores.thursday;
            case "friday": return dailyScores.friday;
            case "saturday": return dailyScores.saturday;
            case "sunday": return dailyScores.sunday;
            default: throw new ArgumentException("Invalid day");
        }
    }
}