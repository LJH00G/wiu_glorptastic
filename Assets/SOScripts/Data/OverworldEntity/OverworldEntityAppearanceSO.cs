using Game.SO.Behaviour.EntityOverworld.InstanceData;
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

        if (speed > controller.Behaviour.Speed * 0.05f)
        {
            float theta = Vector2.SignedAngle(Vector2.right, controller.AIPath.desiredVelocity);

            if (Mathf.Abs(theta) < 44)
                controller.InstanceData.facingDire = DIRECTION.RIGHT;
            else if (theta < 136 && theta > 44)
                controller.InstanceData.facingDire = DIRECTION.UP;
            else if (Mathf.Abs(theta) > 136)
                controller.InstanceData.facingDire = DIRECTION.LEFT;
            else
                controller.InstanceData.facingDire = DIRECTION.DOWN;
        }


        switch (controller.InstanceData.facingDire)
        {
            default:
            case DIRECTION.DOWN:
                controller.Animator.SetInteger("Direction", 3);
                break;
            case DIRECTION.UP:
                controller.Animator.SetInteger("Direction", 1);
                break;
            case DIRECTION.RIGHT:
                controller.Animator.SetInteger("Direction", 0);
                break;
            case DIRECTION.LEFT:
                controller.Animator.SetInteger("Direction", 2);
                break;
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
