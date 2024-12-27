using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconHandler : MonoBehaviour
{
    [SerializeField] private Image[] icons; // 아이콘 array
    [SerializeField] private Color usedColors;

    public void UseShot(int shotNum)
    {
        for (int i = 0; i < icons.Length; i++)
        {
            if (shotNum == i + 1) // shotnum은 int로 들어오기때문에 + 1
            {
                icons[i].color = usedColors;
                return; // stop
            }
        }
    }
}
