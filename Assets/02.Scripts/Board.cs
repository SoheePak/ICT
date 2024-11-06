using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Sprite[] cardSprites;

    // ����: ���� �ѹ��� ����� ����
    private const float SPACE_Y = 2.3f;
    private const float SPACE_X = 1.5f;
    private const float MOVE_X = 0.7f;
    private const float MOVE_Y = 0.75f;
    private const int REQUIRED_PAIRS = 6;
    private const int ROW_COUNT = 4;
    private const int COL_COUNT = 3;

    private List<int> cardIDList = new List<int>();
    private List<Card> cardList = new List<Card>();

    private void Start()
    {
        GenerateCardID();
        ShuffleCardID();
        InitBoard();
    }

    // ����: �޼��� �и� �� ���� ó��
    private void GenerateCardID()
    {
        if (cardSprites.Length < REQUIRED_PAIRS)
        {
            Debug.LogError("Not enough card sprites for the required pairs.");
            return;
        }

        HashSet<int> uniqueIDs = new HashSet<int>();
        while (uniqueIDs.Count < REQUIRED_PAIRS)
        {
            int randomNum = UnityEngine.Random.Range(0, cardSprites.Length);
            if (uniqueIDs.Add(randomNum))
            {
                cardIDList.Add(randomNum);
                cardIDList.Add(randomNum);
            }
        }
    }

    // ����: �� ȿ������ ���� �˰��� ���
    private void ShuffleCardID()
    {
        System.Random rng = new System.Random();
        int n = cardIDList.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            int value = cardIDList[k];
            cardIDList[k] = cardIDList[n];
            cardIDList[n] = value;
        }
    }

    // ����: �޼��� �и�
    private void InitBoard()
    {
        for (int i = 0; i < ROW_COUNT; i++)
        {
            for (int j = 0; j < COL_COUNT; j++)
            {
                CreateCard(i, j);
            }
        }
    }

    // ����: ī�� ���� ���� �и�
    private void CreateCard(int row, int col)
    {
        float posY = (row - (ROW_COUNT / 2)) * SPACE_Y;
        float posX = (col - (COL_COUNT / 2)) * SPACE_X + (SPACE_X / 2);
        Vector3 pos = new Vector3(posX - MOVE_X, posY + MOVE_Y, 0f);

        GameObject cardObject = Instantiate(cardPrefab, pos, Quaternion.identity);
        Card card = cardObject.GetComponent<Card>();
        int cardID = cardIDList[row * COL_COUNT + col];
        card.SetCardID(cardID);
        card.SetAnimalSprite(cardSprites[cardID]);
        cardList.Add(card);
    }

    public List<Card> GetCards() => cardList;
}