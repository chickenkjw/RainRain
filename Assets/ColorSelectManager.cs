using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CharacterColor
{
    WHITE,
    YELLOW,
    ORANGE,
    RED,
    PINK,
    PURPLE,
    SKYBLUE,
    BLUE,
    LIGHTGREEN,
    GREEN,
    BROWN,
    BLACK
}

public class ColorSelectManager : MonoBehaviour
{
    public bool isSelected;
    public Image CurrentImage;
    public Sprite NonSelectedImage, SelectedImage;
    public CharacterColor color;

    public void UnSelect()
    {
        isSelected = false;
        CurrentImage.sprite = NonSelectedImage;
    }

    public void Select()
    {
        UIManager.instance.ClearColorSelector();
        Debug.LogFormat("{0} 색을 선택했습니다", color.ToString());
        isSelected = true;
        CheckSelected();
    }

    public void CheckSelected()
    {
        if (isSelected) CurrentImage.sprite = SelectedImage;
        else CurrentImage.sprite = NonSelectedImage;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
