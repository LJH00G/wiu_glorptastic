using UnityEngine;

public class IndirectParentingController : MonoBehaviour
{

    [SerializeField] Transform parent;
    [SerializeField] Vector3 offsetPos;
    [SerializeField] Quaternion offsetRot = Quaternion.identity;

    public void Set(Transform parent, Vector3? offsetPos = null, Quaternion? offsetRot = null)
    {
        this.parent = parent;
        if (offsetPos.HasValue)
            this.offsetPos = offsetPos.Value;
        if (offsetRot.HasValue)
            this.offsetRot = offsetRot.Value;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        if (parent)
            transform.SetPositionAndRotation(parent.position + parent.rotation * offsetPos, parent.rotation * offsetRot);
    }
}
