using UnityEngine;

[CreateAssetMenu(fileName = "OverworldAppearance_Data", menuName = "Scriptable Objects/Data/OverworldEntity/OverworldEntityAppearanceSO")]
public class OverworldEntityAppearanceSO : ScriptableObject
{
    [field: SerializeField]
    public RuntimeAnimatorController AnimCtrller { get; private set; }
    [field: SerializeField]
    public Sprite DefaultSprite { get; private set; }

    public void UpdateAppearance(EntityOverworldController controller)
    {
        controller.Animator.runtimeAnimatorController = AnimCtrller;
        SpriteRenderer spriteRenderer = controller.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer)
            spriteRenderer.sprite = DefaultSprite;
    }

    public void UpdateAnimator(EntityOverworldController controller)
    {
        var speed = controller.AIPath.desiredVelocity.magnitude;

        if (speed > 0.1f)
        {

            float theta = Vector2.SignedAngle(Vector2.right, controller.AIPath.desiredVelocity);

            if (Mathf.Abs(theta) < 44)
            {
                controller.Animator.SetInteger("Direction", 0);
                controller.InstanceData.facingDire = Vector2.right;
            }
            else if (theta < 136 && theta > 44)
            {
                controller.Animator.SetInteger("Direction", 1);
                controller.InstanceData.facingDire = Vector2.up;
            }
            else if (Mathf.Abs(theta) > 136)
            {
                controller.Animator.SetInteger("Direction", 2);
                controller.InstanceData.facingDire = Vector2.left;
            }
            else
            {
                controller.Animator.SetInteger("Direction", 3);
                controller.InstanceData.facingDire = Vector2.down;
            }
        }


        controller.Animator.SetBool("Walking", !controller.AIPath.reachedEndOfPath);
        controller.Animator.SetFloat("Speed", speed * 0.3f);

    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        if (!AnimCtrller)
            Debug.LogError($"AnimCtrller must not be left empty", this);
    }

#endif
}
