using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CopySizerUI : MonoBehaviour
{
    public Transform obj;
    public TMP_Text TMP_Text;
    Image[] childrens; 

    Vector3 _baseScale;
    
    private void Start()
    {
        _baseScale = transform.localScale;
        childrens = GetComponentsInChildren<Image>();
        
    }


    void Update()
    {
        ResizeSelf();
        AdjusOpacity();

    }

    void ResizeSelf()
    {
        transform.localPosition = obj.localPosition;
        transform.rotation = obj.rotation;
        transform.localScale = new Vector3(obj.localScale.x * _baseScale.x, obj.localScale.y * _baseScale.y, obj.localScale.z * _baseScale.z);
    }

    void AdjusOpacity()
    {
        foreach(Image child in childrens)
        {
            Color c = child.color;
            c.a=TMP_Text.alpha;
            child.color = c;
        }
    }
}
