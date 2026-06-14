using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("따라갈 대상 (보통 플레이어)")]
    public Transform target;
    
    [Header("따라가는 속도 (클수록 빠름)")]
    public float smoothing = 5f;

    void LateUpdate()
    {
        // 타겟이 존재하면 부드럽게 따라갑니다.
        if (target != null)
        {
            // 카메라의 z축은 그대로 유지하고 x, y만 따라갑니다.
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.deltaTime);
        }
    }
}
