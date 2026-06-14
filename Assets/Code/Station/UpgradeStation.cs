using UnityEngine;
using TMPro;
using System.Collections;

public enum UpgradeTarget
{
    PlayerMaxHealth,
    PlayerMoveSpeed,
    PlayerAttackPower,
    PistolDamage,
    ShotgunDamage,
    ShotgunPelletCount,
    RifleDamage,
    RifleFireRate,
    AllWeapons
}

public class UpgradeStation : MonoBehaviour, IInteractable
{
    [Header("강화 설정")]
    public UpgradeTarget upgradeTarget;
    public float upgradeValue = 10f;

    [Header("UI 설정")]
    public float successTextDisplayTime = 2f; // 텍스트가 표시될 시간 (초)

    [Header("전체 무기 강화 전용 설정 (AllWeapons)")]
    public float allWeapons_PistolDamage = 0f;
    public int allWeapons_ShotgunPellet = 0;
    public float allWeapons_MachineGunFireRate = 0f;

    [Header("요구 재료 개수 설정")]
    public int requiredMaterialA = 0;
    public int requiredMaterialB = 0;
    public int requiredMaterialC = 0;

    public void Interact(GameObject player)
    {
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        if (inventory == null) return;

        if (inventory.matACount >= requiredMaterialA &&
            inventory.matBCount >= requiredMaterialB &&
            inventory.matCCount >= requiredMaterialC)
        {
            inventory.UseItem(ItemType.MaterialA, requiredMaterialA);
            inventory.UseItem(ItemType.MaterialB, requiredMaterialB);
            inventory.UseItem(ItemType.MaterialC, requiredMaterialC);

            ApplyUpgrade(player);
        }
        else
        {
            Debug.Log("강화에 필요한 재료가 부족합니다.");
            PlayerStats ps = player.GetComponent<PlayerStats>();
            if (ps != null) ps.ShowSuccessText("<color=red>Not Enough!</color>", 1f);
        }
    }

    private void ApplyUpgrade(GameObject player)
    {
        AudioManager.Instance.PlaySFX("Upgrade");
        
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        WeaponController weaponController = player.GetComponentInChildren<WeaponController>();

        if (upgradeTarget == UpgradeTarget.PlayerMaxHealth && playerStats != null)
        {
            playerStats.AddMaxHealth(upgradeValue);
        }
        else if (upgradeTarget == UpgradeTarget.PlayerMoveSpeed && playerStats != null)
        {
            playerStats.AddMoveSpeed(upgradeValue);
        }
        else if (upgradeTarget == UpgradeTarget.PlayerAttackPower && playerStats != null)
        {
            playerStats.AddAttackPower(upgradeValue);
        }
        else if (weaponController != null)
        {
            foreach (Weapon weapon in weaponController.weapons)
            {
                WeaponStats wStats = weapon.GetComponent<WeaponStats>();
                if (wStats == null) continue;

                if (upgradeTarget == UpgradeTarget.PistolDamage && weapon is Pistol)
                {
                    wStats.AddDamage(upgradeValue);
                }
                else if (upgradeTarget == UpgradeTarget.ShotgunDamage && weapon is Shotgun)
                {
                    wStats.AddDamage(upgradeValue);
                }
                else if (upgradeTarget == UpgradeTarget.ShotgunPelletCount && weapon is Shotgun)
                {
                    Shotgun shotgun = (Shotgun)weapon;
                    shotgun.pelletCount += (int)upgradeValue;
                    Debug.Log("샷건 탄환 수 추가 완료");
                }
                else if (upgradeTarget == UpgradeTarget.RifleDamage && weapon is MachineGun)
                {
                    wStats.AddDamage(upgradeValue);
                }
                else if (upgradeTarget == UpgradeTarget.RifleFireRate && weapon is MachineGun)
                {
                    wStats.ReduceFireRate(upgradeValue);
                }
                
                // [새롭게 추가된 부분] 전체 무기 동시 강화
                else if (upgradeTarget == UpgradeTarget.AllWeapons)
                {
                    if (weapon is Pistol)
                    {
                        wStats.AddDamage(allWeapons_PistolDamage);
                        Debug.Log("전체 강화: 피스톨 데미지 증가");
                    }
                    else if (weapon is Shotgun)
                    {
                        Shotgun shotgun = (Shotgun)weapon;
                        shotgun.pelletCount += allWeapons_ShotgunPellet;
                        Debug.Log("전체 강화: 샷건 탄환 수 증가");
                    }
                    else if (weapon is MachineGun)
                    {
                        wStats.ReduceFireRate(allWeapons_MachineGunFireRate);
                        Debug.Log("전체 강화: 기관총 연사 속도 증가");
                    }
                }
            }
        }
        Debug.Log("강화 성공");
        if (playerStats != null)
        {
            playerStats.ShowSuccessText("Success!", successTextDisplayTime);
        }
    }
}