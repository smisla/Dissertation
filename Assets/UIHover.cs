using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UIHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image targetImage;
    public Sprite normalSprite;
    public Sprite hoverSprite;
    public Animator animator; // Optional if you're using an animation


    public GameObject normalImage;
    public GameObject hoverImage;

    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.SetBool("isHovering", true);
        Wait();
        normalImage.SetActive(false);
        hoverImage.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        normalImage.SetActive(true);
        Wait();
        animator.SetBool("isHovering", false);
        hoverImage.SetActive(false);
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1f);
    }
}
