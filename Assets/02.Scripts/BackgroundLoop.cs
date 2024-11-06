using UnityEngine;

// 왼쪽 끝으로 이동한 배경을 오른쪽 끝으로 재배치하는 스크립트
public class BackgroundLoop : MonoBehaviour
{
    private float width; // 배경의 가로 길이

    // 가로 길이를 측정하는 처리. Start보다 한 프레임 더 빠름
    private void Awake()
    {
        // BoxCollider2D 컴포넌트의 Size 필드의 x값을 가로 길이로 사용
        BoxCollider2D backgroundCollider = GetComponent<BoxCollider2D>();
        width = backgroundCollider.size.x;
    }

    // 현재 위치를 검사
    private void Update()
    {
        // 현재 위치가 원점에서 왼쪽으로 width 이상 이동했을때 위치를 리셋
        if (transform.position.x <= -width)
            Reposition();
    }

    // 위치를 리셋하는 메서드
    private void Reposition()
    {
        // 현재 위치에서 오른쪽으로 가로 길이 * 2만큼 이동
        Vector3 offset = new Vector3(width * 2f, 0, 0);
        transform.position = transform.position + offset;

    }
}