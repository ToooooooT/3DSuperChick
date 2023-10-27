using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorControl : MonoBehaviour
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
            isCursorLocked = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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
