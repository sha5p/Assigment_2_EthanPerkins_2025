using UnityEngine;
using System.Reflection;

public class GameAssets : MonoBehaviour
{

    private static GameAssets _i;

    public static GameAssets i
    {
        get
        {
            if (_i == null) _i = Instantiate(Resources.Load<GameAssets>("GameAssets"));
            return _i;
        }
    }


    public Sprite car;
    public Sprite SpeedUpgrade;
    public Sprite multiplier;
    public Sprite invest;
    public Sprite gamble;




}
