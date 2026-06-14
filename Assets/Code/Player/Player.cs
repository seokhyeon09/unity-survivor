using UnityEngine;



public class Player : MonoBehaviour

{

    public Vector2 inputVec;

    public Transform armPivot;



    Rigidbody2D rigid;

    SpriteRenderer spriter;
    Animator anim;
    PlayerStats stats;
    PlayerHealth health;



    private IInteractable currentInteractable;



    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        stats = GetComponent<PlayerStats>();
        health = GetComponent<PlayerHealth>();
    }



        private float footstepTimer = 0f;
    public float footstepInterval = 0.4f;

    void Update()
    {
        if (health != null && health.isDead)
        {
            if (Input.GetKeyDown(KeyCode.Space)) health.Revive();
            return;
        }
        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.y = Input.GetAxisRaw("Vertical");

        // 발소리 처리
        if (inputVec.magnitude > 0.1f)
        {
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= footstepInterval)
            {
                AudioManager.Instance.PlaySFX("footstep");
                footstepTimer = 0f;
            }
        }
        else
        {
            footstepTimer = footstepInterval; // 움직임 멈추면 초기화 (다음 움직일 때 바로 나도록)
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 상호작용 범위를 줄이고 가장 가까운 오브젝트와 상호작용하도록 수정
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.8f);
            IInteractable closestInteractable = null;
            float minDistance = float.MaxValue;

            foreach (Collider2D hit in colliders)
            {
                IInteractable interactable = hit.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    float dist = Vector2.Distance(transform.position, hit.transform.position);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        closestInteractable = interactable;
                    }
                }
            }

            if (closestInteractable != null)
            {
                closestInteractable.Interact(gameObject);
            }
        }
    }

private void FixedUpdate()
    {
        if (health != null && health.isDead) return;

        float currentSpeed = stats != null ? stats.moveSpeed : 3.0f;
        Vector2 nextVec = inputVec.normalized * currentSpeed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }



    private void LateUpdate()
    {
        if (health != null && health.isDead) return;

        anim.SetFloat("Speed", inputVec.magnitude);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        bool isFacingLeft = mousePos.x < transform.position.x;
        spriter.flipX = isFacingLeft;

        if (armPivot != null)
        {
            Vector3 pivotPos = armPivot.localPosition;
            pivotPos.x = Mathf.Abs(pivotPos.x) * (isFacingLeft ? -1f : 1f);
            armPivot.localPosition = pivotPos;
        }
    }



    private void OnTriggerEnter2D(Collider2D collision)

    {

        IInteractable interactable = collision.GetComponent<IInteractable>();

        if (interactable != null)

        {

            currentInteractable = interactable;

            Debug.Log("상호작용 가능 오브젝트에 접근했습니다.");

        }

    }



    private void OnTriggerExit2D(Collider2D collision)

    {

        IInteractable interactable = collision.GetComponent<IInteractable>();

        if (interactable != null && currentInteractable == interactable)

        {

            currentInteractable = null;

            Debug.Log("상호작용 범위를 벗어났습니다.");

        }

    }

}