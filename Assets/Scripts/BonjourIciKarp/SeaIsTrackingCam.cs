using UnityEngine;

public class SeaIsTrackingCam : MonoBehaviour
{
    public Transform cam;

    [Header("Param")]
    [SerializeField] Vector2 offSetXZ = new Vector2(0, 0);
    [SerializeField] float height = 1f;

    private void Awake()
    {
        cam = Camera.main.transform;
    }

    private void Update()
    {
        Vector3 temp = new Vector3(offSetXZ.x + cam.position.x, height, offSetXZ.y + cam.position.z);
        transform.position = temp;
    }
}
