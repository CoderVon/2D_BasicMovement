using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowSprite : MonoBehaviour
{
    private Transform player;

    private SpriteRenderer thisSprite;//shadow里的SpriteRenderer
    private SpriteRenderer playerSprite;//player里的SpriteRenderer；

    private Color color;

    [Header("时间控制参数")]
    public float activeTime;//显示时间
    public float activeStart;//开始显示的时间

    [Header("不透明度控制")]
    private float alpha;
    public float alphaSet;//设置的alpha初始值
    public float alphaMultiplier;//用于alpha通道乘积


    private void OnEnable()
    {
        //初始化设置
        player = GameObject.FindGameObjectWithTag("Player").transform;
        thisSprite = GetComponent<SpriteRenderer>();
        playerSprite = player.GetComponent<SpriteRenderer>();

        alpha = alphaSet;
        activeStart = Time.time;

        //Sprite传递
        thisSprite.sprite = playerSprite.sprite;

        //位置信息传递
        this.transform.position = player.position;
        transform.localScale = player.localScale;
        transform.rotation = player.rotation;



    }
    private void Update()
    {
        alpha *= alphaMultiplier;

        color = new Color(1, 1, 1, alpha);

        thisSprite.color = color;

        if (Time.time >= activeStart + activeTime)
        {
            //返回对象池
            ShadowPool.instance.ReturnPool(this.gameObject);
        }
    }

}
