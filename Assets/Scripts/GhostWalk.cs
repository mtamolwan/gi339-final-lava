using UnityEngine;

public class GhostWalk : MonoBehaviour
{
    public float speed = 4f;
    public Transform cam;      // MainCamera
    public Transform player;   // PlayerArmature
    public Transform visual;   // PlayerGhost

    void Update()
    {
        if (!GhostFormManager.isGhost) return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 fwd = cam.forward; fwd.y = 0; fwd.Normalize();
        Vector3 right = cam.right; right.y = 0; right.Normalize();
        Vector3 dir = (fwd * v + right * h).normalized;

        if (dir.sqrMagnitude > 0.01f)
        {
            player.position += dir * speed * Time.deltaTime;
            visual.rotation = Quaternion.Slerp(
                visual.rotation, Quaternion.LookRotation(dir), 10f * Time.deltaTime);
        }
    }
}