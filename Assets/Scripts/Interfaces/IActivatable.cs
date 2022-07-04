using UnityEngine;

public interface IActivatable
{
	string GetName();
	float GetBoundingBoxHeight();
	void DisplayInfo();
	void Activate(GameObject target);
	void CloseInfo();
}