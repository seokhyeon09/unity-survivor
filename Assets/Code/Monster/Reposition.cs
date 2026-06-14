using UnityEngine;

public class Reposition : MonoBehaviour
{
    Collider2D coll;
    GameObject player;

    private void Awake()
    {
        coll = GetComponent<Collider2D>();
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        // 바닥 맵은 충돌체 탈출이 아니라, 플레이어와의 거리를 상시 계산해서 이동시킵니다 (대각선 이동 완벽 지원)
        if (transform.CompareTag("Ground") && player != null)
        {
            float diffX = player.transform.position.x - transform.position.x;
            float diffY = player.transform.position.y - transform.position.y;

            // 맵의 실제 크기가 cellGap으로 인해 39.6으로 줄어들었으므로(절댓값 19.8),
            // 플레이어가 중심에서 39.6 이상 멀어지면 79.2(39.6 * 2)만큼 점프시킵니다.
            if (Mathf.Abs(diffX) > 39.6f)
            {
                float dirX = diffX < 0 ? -1f : 1f;
                transform.Translate(Vector3.right * dirX * 79.2f);
            }
            
            if (Mathf.Abs(diffY) > 39.6f)
            {
                float dirY = diffY < 0 ? -1f : 1f;
                transform.Translate(Vector3.up * dirY * 79.2f);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area"))
            return;

        // 보스는 재배치(텔레포트) 대상에서 제외합니다. 
        if (transform.CompareTag("Boss")) return;
        
        // 맵 바닥(Ground) 로직은 위쪽의 Update() 함수로 분리하였으므로 여기서는 패스합니다.
        if (transform.CompareTag("Ground")) return;

        Vector3 playerPos = player.transform.position;
        Vector3 myPos = transform.position;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        Vector3 playerDir = (rb != null) ? (Vector3)rb.linearVelocity.normalized : Vector3.up;

        if (playerDir == Vector3.zero) playerDir = Vector3.up;

        // 몬스터 순간이동 로직 삭제
    }
}