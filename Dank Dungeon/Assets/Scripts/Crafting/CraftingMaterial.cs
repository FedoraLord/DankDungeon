using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingMaterial : MonoBehaviour
{

    public Rigidbody2D rigid;
    public BoxCollider2D box;
    public SpriteRenderer sRender;
    public enum MaterialType { Blue, Red, Green, Yellow }
    public MaterialType materialType;

    private bool isFollowingPlayer;
    private GameObject player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!isFollowingPlayer)
            {
                box.size = sRender.sprite.bounds.size;
                isFollowingPlayer = true;
                player = collision.gameObject;
            }
            else
            {
                switch (materialType)
                {
                    case MaterialType.Blue:
                        CraftingMenu.Instance.BlueMaterial++;
                        break;
                    case MaterialType.Red:
                        CraftingMenu.Instance.RedMaterial++;
                        break;
                    case MaterialType.Green:
                        CraftingMenu.Instance.GreenMaterial++;
                        break;
                    case MaterialType.Yellow:
                        CraftingMenu.Instance.YellowMaterial++;
                        break;
                }
                Destroy(gameObject);
            }
        }
    }

    private void FixedUpdate()
    {
        if (isFollowingPlayer)
        {
            Vector2 direction = player.transform.position - transform.position;
            direction.Normalize();
            rigid.velocity = direction * 5;
        }
    }
}
