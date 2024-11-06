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

    // ������ ���� �̺�Ʈ
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

    // �����Ͱ� �̹� �ִ��� üũ
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

    // ������ ����
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

    // ���� ���� db�� ����
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

    // db���� ������ �ε�
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

    // ������ �ε�
    public async Task<UserData> LoadData(string _uid)
    {
        DataSnapshot snap = await refData.Child("Information").Child(_uid).GetValueAsync();
        if (!snap.Exists)
        {
            userData = CreateNewUserData(_uid);
            await SaveData(userData);  // �� ����� �����͸� DB�� ����
            Debug.Log($"�� ����� uid ���� : {_uid}");
        }
        else
        {
            string json = snap.GetRawJsonValue();
            userData = JsonUtility.FromJson<UserData>(json);
            // ���� �����͸� �� �������� ��ȯ
            userData.firstGame = GameData.FromJson(JsonUtility.ToJson(userData.firstGame));
            userData.secondGame = GameData.FromJson(JsonUtility.ToJson(userData.secondGame));
            userData.thirdGame = GameData.FromJson(JsonUtility.ToJson(userData.thirdGame));
            Debug.Log($"Loaded existing user data for UID: {_uid}");
        }

        // �ְ� ���� Ȯ��
        CheckAndResetWeeklyScores(userData.firstGame);
        CheckAndResetWeeklyScores(userData.secondGame);
        CheckAndResetWeeklyScores(userData.thirdGame);

        OnDataChanged?.Invoke(userData);
        return userData;
    }

    // �������� �������� üũ
    private void CheckAndResetWeeklyScores(GameData gameData)
    {
        if ((DateTime.Now - gameData.LastReset).Days >= 7)
        {
            ResetWeeklyScores(gameData);
        }
    }

    // �ְ� ���� �ʱ�ȭ
    private void ResetWeeklyScores(GameData gameData)
    {
        gameData.weeklyHighScore = 0;
        gameData.dailyScores = new GameData.DailyScores();
        gameData.LastReset = DateTime.Now;
    }

    // ���ο� ���������� ����
    private UserData CreateNewUserData(string _uid)
    {
        return new UserData(_uid);
    }

    // ���� UserData ��������
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
    public long lastResetTicks;  // DateTime�� long���� ����

    public GameData()
    {
        dailyScores = new DailyScores();
        weeklyHighScore = 0;
        allTimeHighScore = 0;
        lastResetTicks = DateTime.Now.Ticks;
    }


    // JSON ����ȭ�� ���� �޼���
    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    // JSON ������ȭ�� ���� ���� �޼���
    public static GameData FromJson(string json)
    {
        GameData data = JsonUtility.FromJson<GameData>(json);
        if (data.dailyScores == null)
        {
            data.dailyScores = new DailyScores();
        }
        return data;
    }

    // DateTime ������Ƽ
    public DateTime LastReset
    {
        get { return new DateTime(lastResetTicks); }
        set { lastResetTicks = value.Ticks; }
    }

    // ���Ϻ� ���� ���� �޼���
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

    // ���Ϻ� ���� �������� �޼���
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