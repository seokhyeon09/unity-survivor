using UnityEngine;

public class Mine : MonoBehaviour
{
    public GameObject explosionPrefab;
    public float damage = 50f;
    [Tooltip("폭발 이펙트의 크기입니다. 폭발 프리팹의 크기(Scale)가 이 수치만큼 커집니다.")]
    public float explosionSize = 3f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 몬스터(Enemy), 보스(Boss), 또는 더미몬스터(Monster) 태그를 감지
        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss") || collision.CompareTag("Monster"))
        {
            AudioManager.Instance.PlaySFX("explosion");
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            Explosion explosionScript = explosion.GetComponent<Explosion>();
            if (explosionScript != null)
            {
                explosionScript.Setup(damage, explosionSize);
            }

            Destroy(gameObject); // 지뢰 폭발 후 제거
        }
    }
}