//using cakeslice;
using UnityEngine;

public interface IHighlight
{
    //Outline[] outlines { get; set; }
    Transform rootTransform { get; }

    string GetName();
    //float GetDesiredNamePlateHeight();

    void Highlight();
    void Unhighlight();
}