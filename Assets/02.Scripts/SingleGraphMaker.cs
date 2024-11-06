using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class SingleGraphMaker : MonoBehaviour
{
    public RectTransform graphContainer;
    public GameObject pointPrefab;
    public GameObject linePrefab;
    public Color graphColor = Color.blue;
    public string gameName;

    private List<Vector2> dataPoints = new List<Vector2>();
    private float graphWidth;
    private float graphHeight;

    void Start()
    {
        graphWidth = graphContainer.rect.width;
        graphHeight = graphContainer.rect.height;
        SetGraph(gameName);
    }

    public async void SetGraph(string gameType)
    {
        ClearGraph();

        UserData userData = await DBManager.Instance.LoadData(DBManager.Instance.userUid);

        if (userData != null)
        {
            GameData gameData = GetGameData(userData, gameType);

            // ���Ϻ� ������ �߰�
            AddDataPoint(0, gameData.GetDailyScore("monday"));
            AddDataPoint(1, gameData.GetDailyScore("tuesday"));
            AddDataPoint(2, gameData.GetDailyScore("wednesday"));
            AddDataPoint(3, gameData.GetDailyScore("thursday"));
            AddDataPoint(4, gameData.GetDailyScore("friday"));
            AddDataPoint(5, gameData.GetDailyScore("saturday"));
            AddDataPoint(6, gameData.GetDailyScore("sunday"));

            ShowGraph();
        }
        else
        {
            Debug.LogError("Failed to load user data");
        }
    }

    private GameData GetGameData(UserData userData, string gameType)
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
                throw new System.ArgumentException("Invalid game type");
        }
    }

    void AddDataPoint(float x, float y)
    {
        dataPoints.Add(new Vector2(x, y));
    }

    void ShowGraph()
    {
        float xMax = 6; // �������̹Ƿ� ����
        float yMax = dataPoints.Max(p => p.y);

        if (yMax == 0) yMax = 1; // 0���� ������ ���� ����

        GameObject lastPointGO = null;
        for (int i = 0; i < dataPoints.Count; i++)
        {
            float xPosition = (dataPoints[i].x / xMax) * graphWidth;
            float yPosition = (dataPoints[i].y / yMax) * graphHeight;

            // ����Ʈ ����
            GameObject pointGO = Instantiate(pointPrefab, graphContainer);
            RectTransform pointRect = pointGO.GetComponent<RectTransform>();
            pointRect.anchorMin = new Vector2(0, 0);
            pointRect.anchorMax = new Vector2(0, 0);
            pointRect.anchoredPosition = new Vector2(xPosition, yPosition);
            pointGO.GetComponent<Image>().color = graphColor;

            // ���� ����
            if (lastPointGO != null)
            {
                GameObject lineGO = Instantiate(linePrefab, graphContainer);
                RectTransform lineRect = lineGO.GetComponent<RectTransform>();
                lineRect.anchorMin = new Vector2(0, 0);
                lineRect.anchorMax = new Vector2(0, 0);
                lineRect.anchoredPosition = lastPointGO.GetComponent<RectTransform>().anchoredPosition;
                Vector2 dir = (pointRect.anchoredPosition - lastPointGO.GetComponent<RectTransform>().anchoredPosition).normalized;
                float distance = Vector2.Distance(pointRect.anchoredPosition, lastPointGO.GetComponent<RectTransform>().anchoredPosition);
                lineRect.sizeDelta = new Vector2(distance, 10f);
                lineRect.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
                lineGO.GetComponent<Image>().color = graphColor;
            }
            lastPointGO = pointGO;
        }
    }

    void ClearGraph()
    {
        foreach (Transform child in graphContainer)
        {
            Destroy(child.gameObject);
        }
        dataPoints.Clear();
    }

    // �׷��� ������Ʈ �޼��� (�ʿ�� �ܺο��� ȣ��)
    public void UpdateGraph(string gameType)
    {
        SetGraph(gameType);
    }
}