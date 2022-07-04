internal interface IActorUIElement
{
    bool Active { get; }
    void SetActive(bool active);
    void UpdateColor(bool update);
}