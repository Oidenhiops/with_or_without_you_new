using System.Linq;
using UnityEngine;

public class ManagementElevatorMap : MonoBehaviour
{
    [SerializeField] Transform[] positions;
    [SerializeField] float speed = 2;
    [SerializeField] int currentPosition = 1;
    [SerializeField] float delayToNextPos = 0;
    [SerializeField] float maxTimeDelay = 4;
    [SerializeField] Transform elevator;
    float dist = 0;
    void Update()
    {
        dist = Vector3.Distance(elevator.position, positions[currentPosition].position);
        if (dist > 0.01f && delayToNextPos <= 0)
        {
            elevator.transform.position = Vector3.Lerp(elevator.position, positions[currentPosition].position, speed / dist * Time.deltaTime);
        }
        else if (delayToNextPos <= 0)
        {
            elevator.position = positions[currentPosition].position;
            currentPosition = currentPosition + 1 < positions.Length ? currentPosition + 1 : 0;
            delayToNextPos = maxTimeDelay;
        }
        else
        {
            delayToNextPos -= Time.deltaTime;
        }
    }
}
