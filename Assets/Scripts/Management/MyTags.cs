using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTags : MonoBehaviour
{
    public static string SAW_UP_TAG = "Top";
    public static string SAW_DOWN_TAG = "Bottom";
    public static string PUPPY_TAG = "Puppy";
    public static string PLAYER_TAG = "Player";
	public static string GROUND_LAYER_TAG = "Ground";
	public static string JUMPING_COIN_TAG = "JumpingCoin";
	public static string NORMAL_COIN_TAG = "Coin";
	public static string SAW_BLADE_TAG = "SawBlade";
	public static string DISCOBALL_TAG = "DiscoBall";
	public static string CANDLE_TAG = "Candle";
	public static string ENEMY_TAG = "Enemy";
	public static string ENEMY_BIRD_TAG = "Bird";
	public static string BULLET_TAG = "Bullet";
	public static string VCAM_TAG = "VirtualCamera";

	public static int SAW_OBJECT_REF = 0;  
    public static int COIN_OBJECT_REF = 1;
	public static int STAIR_OBJECT_REF = 3;  //These vals refer to the the list in the ObjectPlacer object (gun is 2 but that is handled differntly - see ObjectPlacer.cs)
	public static int LADDER_OBJECT_REF = 4;
	public static int SECRETTREASURE_OBJECT_REF = 5;
    public static string SECRET_TAG = "Secret";  //Player will sense he is standing on this layer and the nojump script will remain off - see SecretSensor.cs on players

    //Sounds
    //Sounds
    public static string SOUND_GAME_OVER = "GameOver";
	public static string SOUND_JUMP = "Jump";
	public static string SOUND_WATERFALL = "Waterfall";
	public static string SOUND_SKELETON_WALK = "SkeletonWalk";
	public static string SOUND_GUNSHOT = "GunShot";

	public static string SOUND_THEMESONG1 = "Theme1";
	public static string SOUND_THEMESONG2 = "Theme2";
	public static string SOUND_THEMESONG3 = "Theme3";
    public static string SOUND_THEMESONG4 = "Theme4";
	public static string SOUND_THEMESONG5 = "NewTheme1";
    public static string SOUND_THEMESONG6 = "NewTheme2";
    public static string SOUND_THEMESONG7 = "NewTheme3";
    public static string SOUND_THEMESONG8 = "NewTheme4";
    public static string SOUND_THEMESONG9 = "NewTheme5";
    public static string SOUND_THEMESONG10 = "NewTheme6";
    public static string SOUND_THEMESONG11 = "NewTheme7";
    public static string SOUND_THEMESONG12 = "NewTheme8";
    public static string SOUND_THEMESONG13 = "NewTheme9";
    public static string SOUND_THEMESONG14 = "NewTheme10";
    public static string SOUND_THEMESONG15 = "NewTheme11";
    public static string SOUND_THEMESONG16 = "NewTheme12";
    public static string SOUND_THEMESONG17 = "NewTheme13";





    public static string SOUND_MICKEYMOUSE = "MickeyMouse";
	public static string SOUND_BOING = "Boing";



    public static string SOUND_COIN = "Coin";
	public static string SOUND_WOOF = "Woof";

	public static string SOUND_WATERSPLASH = "WaterSplash";
	public static string SOUND_HEAVYTHROW = "HeavyThrow";
	public static string SOUND_BOSSTHEME = "BossTheme";
	public static string SOUND_ROCKSTRIKE = "RockHit";
	public static string SOUND_BONUS = "Bonus";
	public static string SOUND_LEVELCOMPLETE = "LevelComplete";
	public static string SOUND_SKELETONDIE = "SkeletonDie";
	public static string SOUND_DOG_DIE = "DogDie";
	public static string SOUND_BUZZ_SAW = "BuzzSaw";
    public static string SOUND_HAWK = "Hawk";
    public static string SOUND_BABYBIRD = "BabyBird";
	public static string SOUND_DONK = "Donk";
	public static string SOUND_ARROW = "Arrow";
	public static string SOUND_INDIAN_SONG = "IndianSong";
	public static string SOUND_EXPLOSION = "Explosion";

    public static string SCENE_HIGHSCORE = "HighScore";
    public static string SCENE_MAIN_GAME = "GameScene";
    public static string SCENE_MAIN_MENU = "MainMenu";

	public static string SPLASH_LIFE_LOST = "RedWhiteSplash";

	public static string TXTLIVES_TAG = "LivesText";
	public static string TXTSCORE_TAG = "ScoreText";
	public static string TXTINFO_TAG = "InfoText";

	public static string LEVEL_CREATOR_TAG = "LevelCreator";

	
}
