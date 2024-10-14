using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum ItemType
{
    Health, MaxHealth, Speed, ManaCost, MaxMana, Damage, ManaRecharge, HeatLoss, Money, Crystal, Unique, Ice, Fire, Electric, Melee, Special, Projectile, MaxHeat, FireRate
}


[System.Serializable]
[CreateAssetMenu(fileName = "ItemData.asset", menuName = "ItemData/Item")]
public class ItemData : ScriptableObject
{
    [SerializeField]
    public Sprite ItemSprite;


    public string ItemName;
    public string ItemDescription;
    public int Price;

    [SerializeField]
    public List<ItemProperty> Properties;
    public virtual void Activate(Player player)
    {
        for (int i = 0; i < Properties.Count; i++)
        {
            Properties[i].Activate(player);
        }

    }



}

[System.Serializable]
[CreateAssetMenu(fileName = "ItemData.asset", menuName = "ItemData/SpellItem")]
public class SpellItemData : ItemData
{


    [SerializeField]
    public List<ItemProperty> Properties;


    public Spell SpellRef;


    public override void Activate(Player player)
    {
        for (int i = 0; i < Properties.Count; i++)
        {
            Properties[i].Activate(player);
        }
    }
}


[System.Serializable]
public class ItemProperty
{
    public ItemType Type;
    public float Amount;
    public bool Percentage = false;


    public void Activate(Player player)
    {
        switch (Type)
        {
            case ItemType.Health:
                player.RestoreHealth((int)Amount); break;
            case ItemType.ManaCost:
                player.ModifyCost(Amount); break;
            case ItemType.MaxMana:
                player.ModifyMaxMana(Amount); break;
            case ItemType.MaxHealth:
                player.ModifyMaxHealth((int)Amount); break;
            case ItemType.Damage:
                player.ModifyDamage(Amount); break;
            case ItemType.Speed:
                player.ModifySpeed(Amount); break;
            case ItemType.ManaRecharge:
                player.ModifyManaRecharge(Amount); break;
            case ItemType.HeatLoss:
                player.ModifyHeatLoss(Amount); break;
            case ItemType.MaxHeat:
                player.ModifyMaxHeat((int)Amount); break;


            case ItemType.FireRate:
                player.ModifyFireRate(Amount); break;


            case ItemType.Money:
                player.IncreaseMoney((int)Amount); break;
            case ItemType.Crystal:
                player.IncreaseCrystals((int)Amount); break;

            case ItemType.Projectile:
                player.ModifyProjectile(Amount); break;
            case ItemType.Melee:
                player.ModifyMelee(Amount); break;
            case ItemType.Special:
                player.ModifySpecial(Amount); break;

            case ItemType.Fire:
                player.ModifyFire(Amount); break;
            case ItemType.Ice:
                player.ModifyIce(Amount); break;
            case ItemType.Electric:
                player.ModifyElectric(Amount); break;

            default:
                Debug.Log("BAD ITEM NO GOOD VERY BAD NO TYPE");
                break;

        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(ItemData))]
public class PreviewEditor : UnityEditor.Editor
{
    public static void CreateAsset<Example>() where Example : ScriptableObject
    {
        Example asset = ScriptableObject.CreateInstance<Example>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);

        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(Example).ToString() + ".asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    Texture2D GetSlicedSpriteTexture(Sprite sprite)
    {
        Rect rect = sprite.rect;
        Texture2D slicedTex = new Texture2D((int)rect.width, (int)rect.height);
        slicedTex.filterMode = sprite.texture.filterMode;

        slicedTex.SetPixels(0, 0, (int)rect.width, (int)rect.height, sprite.texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height));
        slicedTex.Apply();

        return slicedTex;
    }


    public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
    {
        ItemData example = (ItemData)target;

        if (example == null || example.ItemSprite == null)
            return null;

        Texture2D tex = GetSlicedSpriteTexture(example.ItemSprite);

        var preview = AssetPreview.GetAssetPreview(tex);

        if (preview == null)
            return null;

        Texture2D cache = new Texture2D(width, height);
        EditorUtility.CopySerialized(preview, cache);

        return cache;
    }
}

#endif