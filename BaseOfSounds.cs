using UnityEngine;
using System.Collections;

public class BaseOfSounds : UnitySingleton<BaseOfSounds>
{
    public AudioClip soundTakingScrap;
    public AudioClip soundOfFire;
    public AudioClip constructionOfSuperstructure;
    public AudioClip constructionOfCell;
    public AudioClip saleOfCell;
    public AudioClip destroyingSuperstructure;

    protected override void Awake()
    {
        base.Awake();
    }
}
