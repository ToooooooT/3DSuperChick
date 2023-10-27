using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCursorControl : MonoBehaviour
{
    private bool isCursorLocked = true;

    void Start()
    {
        // ?�w��?�b�̹�����
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = true;
    }

    void Update()
    {
        // ??��?��???
        if (Input.GetMouseButtonDown(1))
        {
            isCursorLocked = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
        }

        // ??ESC?���U
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isCursorLocked = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
