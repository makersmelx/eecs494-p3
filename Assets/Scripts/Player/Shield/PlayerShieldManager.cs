using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShieldManager : MonoBehaviour
{
    // Start is called before the first frame update
    public ShieldControl shield;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        SetActive();
        ChangePlayerMaxSpeed();
        print(PlayerMoveControl.Instance.CurrentMaxSpeed);
    }

    private void SetActive()
    {
        shield.gameObject.SetActive(PlayerInputHandler.Instance.GetMouseRightButton());
    }

    private void ChangePlayerMaxSpeed()
    {
        PlayerMoveControl.Instance.SetCurrentMaxSpeedThreshold(
            shield.gameObject.activeInHierarchy
                ? shield.shieldMaxSpeedThreshold
                : 1f);
    }
}