using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Weapon[] weapons;
    private int currentWeaponIndex = 0;

    // [추가된 부분] 최상위 부모인 플레이어의 스탯을 담을 변수
    private PlayerStats playerStats;

    void Start()
    {
        // [추가된 부분] 시작할 때 내 부모(Player)에게서 PlayerStats를 찾아옵니다.
        playerStats = GetComponentInParent<PlayerStats>();

        EquipWeapon(currentWeaponIndex);
    }

    void Update()
    {
        Aim();
        SwitchWeapon();

        // 현재 들고 있는 무기 가져오기
        Weapon currentWeapon = weapons[currentWeaponIndex];

        // 기관총(MachineGun)은 꾹 누르고 있을 때(GetMouseButton) 발사
        if (currentWeapon is MachineGun)
        {
            if (Input.GetMouseButton(0))
            {
                // [수정된 부분] 직접 Shoot()을 부르지 않고, 데미지 계산 함수를 거칩니다.
                PerformShoot(currentWeapon);
            }
        }
        // 권총(Pistol)과 샷건(Shotgun)은 클릭할 때마다(GetMouseButtonDown) 발사
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                PerformShoot(currentWeapon);
            }
        }
    }

    // ----------------------------------------------------
    // [추가된 부분] 최종 데미지를 계산하고 무기에게 발사를 명령하는 함수
    // ----------------------------------------------------
    void PerformShoot(Weapon weapon)
    {
        float finalDamage = 0f;

        // 1. 현재 쏘려는 무기에서 WeaponStats(무기 자체 능력치)를 가져옵니다.
        WeaponStats weaponStats = weapon.GetComponent<WeaponStats>();

        // 2. 플레이어 능력치와 무기 능력치가 모두 잘 있다면?
        if (playerStats != null && weaponStats != null)
        {
            // 최종 데미지 = 캐릭터 공격력 + 무기 데미지
            finalDamage = playerStats.attackPower + weaponStats.baseDamage;
        }

        // 테스트를 위해 콘솔에 최종 데미지를 띄워봅니다. (나중에 지우셔도 됩니다)
        Debug.Log($"💥 [{weapon.gameObject.name}] 발사! 적용될 최종 데미지: {finalDamage}");

        weapon.Shoot(finalDamage);
    }

    void Aim()
    {
        // (기존 코드 그대로)
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector3 aimDirection = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        Vector3 localScale = Vector3.one;
        if (angle > 90 || angle < -90) localScale.y = -1f;
        else localScale.y = 1f;
        transform.localScale = localScale;
    }

    void SwitchWeapon()
    {
        // (기존 코드 그대로)
        if (Input.GetKeyDown(KeyCode.E))
        {
            currentWeaponIndex++;
            if (currentWeaponIndex >= weapons.Length) currentWeaponIndex = 0;
            EquipWeapon(currentWeaponIndex);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentWeaponIndex--;
            if (currentWeaponIndex < 0) currentWeaponIndex = weapons.Length - 1;
            EquipWeapon(currentWeaponIndex);
        }
    }

    void EquipWeapon(int index)
    {
        // (기존 무기 교체 로직: 선택된 무기만 켜지고 나머지는 꺼짐)
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].gameObject.SetActive(i == index);
        }
    }
}