using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EnemyBattle : MonoBehaviour, IBattleCharacterBase
{
    public string Name { get; set; }
    public string Text { get; set; }
    public Image Img { get; set; }
    public int MaxHP { get; set; }
    public int HP { get; set; }
    public int Damage { get; set; }
    public Vector3 OriginalImagePos { get; set; }
    public Color OriginalImageColor { get; set; }

    [SerializeField] private GameObject action;

    public int SaveHP { get; set; }

    public void Awake()
    {
        Name = "ObsessedGirl";
        Text = "This is Test";
        MaxHP = 100;
        Damage = 10;
        HP = MaxHP;
        SaveHP = MaxHP;

        Img = transform.GetChild(0).GetComponent<Image>();
        Img.sprite = Resources.Load<Sprite>("TemporaryAssets/Art/Trianers/CoolTrainer_M");

        OriginalImagePos = Img.transform.localPosition;
        OriginalImageColor = Img.color;
    }

    public void SetUp()
    {
        // 배틀 시작시 초기화 해야할 부분 초기화 해야한다.
        // Initialize();

        PlayEnterAnimation();
    }

    public GameObject GetAction() { return action; }

    public bool TakeAction(BattleActionComponent action)
    {
        switch (action.Type)
        {
            case BattleActionType.Attack: case BattleActionType.SpecialAttack:
                HP -= action.EffectAmount;
                if (SaveHP - HP > 30 || HP < 10)
                {
                    SaveHP = HP;
                    // HP가 10 이하일 때 마지막 결투에 대한 처리 해주기

                    return true;
                }
                break;
            case BattleActionType.DamageBuff:
                this.action.GetComponent<BattleActionComponent>().IncreaseEffectAmount(action.EffectAmount);
                break;
            case BattleActionType.HealthBuff:
                HP += action.EffectAmount;
                break;
            default:
                break;
        }
        return false;
    }

    public void PlayEnterAnimation()
    {
        Img.transform.localPosition = new Vector3(500f, OriginalImagePos.y);
        Img.transform.DOLocalMoveX(OriginalImagePos.x, 1f);
    }

    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();

        sequence.Append(Img.transform.DOLocalMoveX(OriginalImagePos.x - 50f, 0.25f));
        sequence.Append(Img.transform.DOLocalMoveX(OriginalImagePos.x, 0.25f));
    }
    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();

        sequence.Append(Img.DOColor(Color.gray, 0.1f));
        sequence.Append(Img.DOColor(OriginalImageColor, 0.1f));

        Img.transform.DOShakePosition(1f, 3f);
    }

    public void PlayLoseAnimation()
    {
        Img.transform.DOLocalMoveX(OriginalImagePos.x + 500f, 1f);
    }
}