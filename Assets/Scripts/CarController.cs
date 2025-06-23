using UnityEngine;

public class CarController : MonoBehaviour
{
    public Component FrontLeftWheel;
    public Component FrontRightWheel;

    public PlayerController Player;

    void Update()
    {
        FrontLeftWheel.transform.rotation = Quaternion.AngleAxis(Player.SteeringAngle, FrontLeftWheel.transform.up);
        FrontRightWheel.transform.rotation = Quaternion.AngleAxis(Player.SteeringAngle, FrontRightWheel.transform.up);
    }
}
