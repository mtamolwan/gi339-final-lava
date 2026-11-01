using UnityEngine;
using StarterAssets;

public class GhostFormManager : MonoBehaviour
{
   public GameObject humanGeometry;   // Geometry
public GameObject humanSkeleton;   // Skeleton
public GameObject ghostModel;      // PlayerGhost

    public static bool isGhost = false;

    private CharacterController cc;
    private ThirdPersonController tpc;

    void Start()
    {
        isGhost = false;
        var player = GameObject.FindGameObjectWithTag("Player");
        cc = player.GetComponent<CharacterController>();
        tpc = player.GetComponent<ThirdPersonController>();
    }

    public void BecomeGhost()
{
    if (isGhost) return;
    isGhost = true;
    humanGeometry.SetActive(false);
    humanSkeleton.SetActive(false);
    ghostModel.SetActive(true);
    tpc.enabled = false;
    cc.enabled = false;
}

public void RestoreHuman()
{
    isGhost = false;
    ghostModel.SetActive(false);
    humanGeometry.SetActive(true);
    humanSkeleton.SetActive(true);
    cc.enabled = true;
    tpc.enabled = true;
}
}