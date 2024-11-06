using UnityEngine;

// ���� ������ �̵��� ����� ������ ������ ���ġ�ϴ� ��ũ��Ʈ
public class BackgroundLoop : MonoBehaviour
{
    private float width; // ����� ���� ����

    // ���� ���̸� �����ϴ� ó��. Start���� �� ������ �� ����
    private void Awake()
    {
        // BoxCollider2D ������Ʈ�� Size �ʵ��� x���� ���� ���̷� ���
        BoxCollider2D backgroundCollider = GetComponent<BoxCollider2D>();
        width = backgroundCollider.size.x;
    }

    // ���� ��ġ�� �˻�
    private void Update()
    {
        // ���� ��ġ�� �������� �������� width �̻� �̵������� ��ġ�� ����
        if (transform.position.x <= -width)
            Reposition();
    }

    // ��ġ�� �����ϴ� �޼���
    private void Reposition()
    {
        // ���� ��ġ���� ���������� ���� ���� * 2��ŭ �̵�
        Vector3 offset = new Vector3(width * 2f, 0, 0);
        transform.position = transform.position + offset;

    }
}