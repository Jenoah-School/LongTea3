using UnityEngine.InputSystem;

public interface IWeapon
{
    public void ActivateWeapon(InputAction.CallbackContext context);
    public void CheckCollision();
}
