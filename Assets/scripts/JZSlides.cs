using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JZSlides : MonoBehaviour
{
    int nextSlide = 0;
    public Sprite[] slides;
    SpriteRenderer spr;
    FiducialController fidu;

    public int DegreesToAction;
    float lastDegree;
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        fidu = GetComponent<FiducialController>();
    }

    void Update()
    {
        if (fidu.IsVisible)
        {
            if (fidu.AngleDegrees > lastDegree + DegreesToAction) Slide(false); //stuff turn left back
            if (fidu.AngleDegrees < lastDegree - DegreesToAction) Slide(true);  //stuff turn right forward
        }
    }

    public void Slide(bool next)
    {
        lastDegree = fidu.AngleDegrees;
        nextSlide += next ? 1 : -1;
        if (nextSlide < 0) nextSlide = slides.Length - 1;
        spr.sprite = slides[nextSlide %= slides.Length];
    }

}
