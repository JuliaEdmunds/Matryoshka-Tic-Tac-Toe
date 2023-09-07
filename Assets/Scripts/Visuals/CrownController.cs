using UnityEngine;

public class CrownController : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(0, 40 * Time.deltaTime, 0);
    }
}
