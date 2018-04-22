using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFX : MonoBehaviour {

    //Audio Files

    public AudioClip playerAttackEffect;
    public AudioClip playerHitEffect;
    public AudioClip playerBombPlaceEffect;
    public AudioClip playerBombHissEffect;
    public AudioClip playerBombExplodeEffect;
    public AudioClip playerPlayerWalkEffect;
    public AudioClip playerCraftingSoundEffect;
    public AudioClip playerPotionDrinkEffect;
    public AudioClip playerBuringSoundEffect;
    public AudioClip playerFallingSoundEffect;
    public AudioClip playerCoughing1Effect;
    public AudioClip playerCoughing2Effect;
    public AudioClip playerCoughing3Effect;

    public static SoundFX Instance;

	// Use this for initialization
	void Start () {
        Instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
