using UnityEngine;

// ���� ������Ʈ�� ��� �������� �����̴� ��ũ��Ʈ
public class ScrollingObject : MonoBehaviour
{
    [Range(0f, 20f)]
    public float speed = 10f; // �̵� �ӵ�

    // ���� ������Ʈ�� �������� ���� �ӵ��� ���� �̵��ϴ� ó��
    private void Update()
    {
            // �ʴ� speed�� �ӵ��� �������� �����̵�
            transform.Translate
                (Vector3.left  * speed * Time.deltaTime);
    }
}