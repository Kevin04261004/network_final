using UnityEngine;

public class BufferingImage : MonoBehaviour
{
    private int activeCount;

    public void AddCount()
    {
        activeCount++;
        if (activeCount > 0)
        {
            gameObject.SetActive(true);
        }
    }

    public void MinusCount()
    {
        activeCount--;
        if (activeCount <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
