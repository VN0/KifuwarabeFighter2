﻿using UnityEngine;
using System.Text;

/// <summary>
/// 対局の結果☆
/// </summary>
public enum Result
{
    Player1_Win,
    Player2_Win,
    //Double_KnockOut,
    None
}
/// <summary>
/// プレイヤー番号。
/// </summary>
public enum PlayerIndex
{
    Player1,
    Player2,
    Num
}
/// <summary>
/// 使用キャラクター。
/// </summary>
public enum CharacterIndex
{
    Kifuwarabe,
    Ponahiko,
    Roborinko
}
public enum PlayerCharacterSpritesIndex
{
    All,
    Win,
    Lose
}
public enum ButtonIndex
{
    Horizontal,
    Vertical,
    LightPunch,
    MediumPunch,
    HardPunch,
    LightKick,
    MediumKick,
    HardKick,
    Pause,
    Num
}
public enum WeightIndex
{
    Light,
    Medium,
    Hard
}
public enum MotionIndex
{
    Wait,
    LP,
    MP,
    HP,
    LK,
    MK,
    HK,
}

// どこからでも使われるぜ☆
public class CommonScript
{ //: MonoBehaviour

    static CommonScript()
    {
        Result = Result.None;
        Player_To_Computer = new bool[] { true, true };
        Player_To_UseCharacter = new CharacterIndex[] { CharacterIndex.Kifuwarabe, CharacterIndex.Kifuwarabe };
        Teban = PlayerIndex.Player1;
    }

    public const string SCENE_TITLE = "Title";
    public const string SCENE_SELECT = "Select";
    public const string SCENE_MAIN = "Main";
    public const string SCENE_RESULT = "Result";

    /// <summary>
    /// BUTTON はコンフィグ画面でユーザーの目に触れるので、プレイヤー１はP1、プレイヤー２はP2 だぜ☆（＾～＾）
    /// </summary>
    public const string BUTTON_01_P0_HO = "Horizontal";
    public const string BUTTON_02_P0_VE = "Vertical";
    public const string BUTTON_03_P0_LP = "P1LightPunch";
    public const string BUTTON_04_P0_MP = "P1MediumPunch";
    public const string BUTTON_05_P0_HP = "P1HardPunch";
    public const string BUTTON_06_P0_LK = "P1LightKick";
    public const string BUTTON_07_P0_MK = "P1MediumKick";
    public const string BUTTON_08_P0_HK = "P1HardKick";
    public const string BUTTON_09_P0_PA = "P1Pause";
    public const string BUTTON_10_CA = "Cancel";
    public const string BUTTON_11_P1_HO = "P2Horizontal";
    public const string BUTTON_12_P1_VE = "P2Vertical";
    public const string BUTTON_13_P1_LP = "P2LightPunch";
    public const string BUTTON_14_P1_MP = "P2MediumPunch";
    public const string BUTTON_15_P1_HP = "P2HardPunch";
    public const string BUTTON_16_P1_LK = "P2LightKick";
    public const string BUTTON_17_P1_MK = "P2MediumKick";
    public const string BUTTON_18_P1_HK = "P2HardKick";
    public const string BUTTON_19_P1_PA = "P2Pause";

    public const string MOTION_DOWN_DAMAGE2 = "Down_Damage2";
    public const string MOTION_STANDUP0 = "Standup0";
    public const string MOTION_LIGHT_PUNCH0 = "LightPunch0";
    public const string MOTION_MEDIUM_PUNCH0 = "MediumPunch0";
    public const string MOTION_HARD_PUNCH0 = "HardPunch0";
    public const string MOTION_LIGHT_KICK0 = "LightKick0";
    public const string MOTION_MEDIUM_KICK0 = "MediumKick0";
    public const string MOTION_HARD_KICK0 = "HardKick0";
    public const string MOTION_JUMP0 = "Jump0";
    public const string MOTION_JUMP1 = "Jump1";
    public const string MOTION_JUMP2 = "Jump2";
    public const string MOTION_JUMP3 = "Jump3";
    public const string MOTION_JUMP4 = "Jump4";
    public const string MOTION_GIVEUP0 = "Giveup0";

    public const string TRIGGER_PUNCH = "punch";
    public const string TRIGGER_KICK = "kick";
    public const string TRIGGER_DASH = "dash";
    public const string TRIGGER_ESCAPE = "escape";
    public const string TRIGGER_JUMP = "jump";
    public const string TRIGGER_DOWN = "down";
    public const string TRIGGER_DAMAGE_L = "damageL";
    public const string TRIGGER_DAMAGE_M = "damageM";
    public const string TRIGGER_DAMAGE_H = "damageH";
    public const string TRIGGER_GIVEUP = "giveup";

    public const string BOOL_DOWNING = "downing";
    public const string BOOL_INVINCIBLE = "invincible";
    public const string BOOL_GIVEUPING = "giveuping";

    /// <summary>
    /// [player,button]
    /// 内部的には　プレイヤー１はP0、プレイヤー２はP1 だぜ☆（＾▽＾）
    /// </summary>
    public static string[,] PlayerAndButton_To_ButtonName = new string[2, (int)ButtonIndex.Num] {
        { BUTTON_01_P0_HO, BUTTON_02_P0_VE,BUTTON_03_P0_LP,BUTTON_04_P0_MP,BUTTON_05_P0_HP,BUTTON_06_P0_LK,BUTTON_07_P0_MK,BUTTON_08_P0_HK,BUTTON_09_P0_PA},
        { BUTTON_11_P1_HO, BUTTON_12_P1_VE,BUTTON_13_P1_LP,BUTTON_14_P1_MP,BUTTON_15_P1_HP,BUTTON_16_P1_LK,BUTTON_17_P1_MK,BUTTON_18_P1_HK,BUTTON_19_P1_PA},
    };

    public static Result Result { get; set; }
    /// <summary>
    /// 人間か、コンピューターか。
    /// </summary>
    public static bool[] Player_To_Computer { get; set; }
    /// <summary>
    /// [Player] プレイヤーの使用キャラクター。
    /// </summary>
    public static CharacterIndex[] Player_To_UseCharacter { get; set; }
    public static PlayerIndex Teban { get; set; }
    public static PlayerIndex ReverseTeban(PlayerIndex player)
    {
        switch (player)
        {
            case PlayerIndex.Player1: return PlayerIndex.Player2;
            case PlayerIndex.Player2: return PlayerIndex.Player1;
            default: Debug.LogError("未定義のプレイヤー☆"); return player;
        }
    }

    /// <summary>
    /// [x] キャラクター選択画面での並び順 
    /// </summary>
    public static CharacterIndex[] X_To_CharacterInSelectMenu = new CharacterIndex[]
    {
        CharacterIndex.Kifuwarabe, CharacterIndex.Roborinko, CharacterIndex.Ponahiko
    };
    /// <summary>
    /// [character]
    /// </summary>
    public static string[] Character_To_Name = new string[]
    {
        "きふわらべ", "パナ彦", "ろぼりん娘"
    };
    public static string[] Character_To_NameRoma = new string[]
    {
        "KifuWarabe", "Panahiko", "Roborinko"
    };
    public static string[] Character_To_AnimationController = new string[]
    {
        "AnimatorControllers/Middle@Char0",
        "AnimatorControllers/Middle@Char1",
        "AnimatorControllers/Middle@Char2"
    };
    /// <summary>
    /// [character]
    /// </summary>
    public static string Character_To_Attack(CharacterIndex character)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Sprites/Attack");
        sb.Append((int)character);
        sb.Append("a");
        return sb.ToString();
    }
    /// <summary>
    /// [character,スライス番号]
    /// </summary>
    public static string CharacterAndSlice_To_AttackSlice(CharacterIndex character, int slice){
        StringBuilder sb = new StringBuilder();
        sb.Append("Attack");
        sb.Append((int)character);
        sb.Append("a_");
        sb.Append(slice);
        return sb.ToString();
    }
    /// <summary>
    /// [character,スライス番号]
    /// </summary>
    public static string[,] CharacterAndSlice_To_FaceSprites = new string[3,3]{
        { "Sprites/Face0", "Face0_0", "Face0_1" },
        { "Sprites/Face1", "Face1_0", "Face1_1" },
        { "Sprites/Face2", "Face2_0", "Face2_1" },
    };
    public static string[,] CharacterAndMotion_To_Clip = new string[,]
    {
        { "Char0@Wait0", "Char0@LP0", "Char0@MP0", "Char0@HP0", "Char0@LK0", "Char0@MK0", "Char0@HK0", },
        { "Char1@Wait0", "Char1@LP0", "Char1@MP0", "Char1@HP0", "Char1@LK0", "Char1@MK0", "Char1@HK0", },
        { "Char2@Wait0", "Char2@LP0", "Char2@MP0", "Char2@HP0", "Char2@LK0", "Char2@MK0", "Char2@HK0", },
    };
    public static string[] Player_To_Tag = new string[] { "Char0", "Char1" };
    public static string[] Player_To_Attacker = new string[] { "Attacker0", "Attacker1" };
    public static string[] Player_To_AttackerTag = new string[] { "Attacker0", "Attacker1" };

    /// <summary>
    /// 画像を２．５倍角にして使っているぜ☆（＾～＾）
    /// </summary>
    public static float GRAPHIC_SCALE = 2.5f;

    /// <summary>
    /// [character]
    /// </summary>
    public static string[] Character_To_WinMessage = new string[]
    {
        "公園（くに）へ帰れだぜ☆！\nお前にもお父んがいるだろう☆",
        "努力あるのみ☆\n",
        "あかごの方が　はごたえがあるのじゃ！\n辱め詰めで追い回してやろう！"
    };

    public static string Prefab_TakoyakiParticle0 = "TakoyakiParticle0";
}
