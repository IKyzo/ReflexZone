using UnityEngine;

public class Indicator : MonoBehaviour
{
    public void SetRotation(int index)
    {
        switch (index)
        {
            case 0:
                transform.Rotate(0f, 0f, -145f);
                break;
            case 1:
                transform.Rotate(0f, 0f, -180f);
                break;
            case 2:
                transform.Rotate(0f, 0f, -40f);
                break;
            case 3:
                transform.Rotate(0f, 0f, 0f);
                break;
            default:
                transform.Rotate(0f, 0f, 90f);
                break;
            
        }
    }

}
