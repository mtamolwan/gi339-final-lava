using UnityEngine;

public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        // สั่งให้หันหน้า (แกน Z) ไปทิศเดียวกับกล้องหลักของผู้เล่นเสมอ
        if (Camera.main != null)
        {
            transform.forward = Camera.main.transform.forward;
        }
    }
}