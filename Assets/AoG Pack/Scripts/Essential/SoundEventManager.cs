using UnityEngine;

public class SoundEventManager : MonoBehaviour
{
    [Range(0, 1)]
    [SerializeField] private float masterVolume = 1f;

    private void Start()
    {
        AudioListener.volume = masterVolume;

        Quest.OnQuestTaken += (q, a) =>
        {
            var aso = PoolSystem.GetPoolObject("SFXObject", ObjectPoolingCategory.SFX).GetComponent<AudioSource>();
            aso.clip = ResourceManager.sfx_notify_queststarted;
            aso.transform.position = a.transform.position;
            aso.Play();
        };
        //QuestMaster.OnQuestReminder += (q) => {

        //    AudioSource aso = ResourceManager.GetPoolObject("SFXObject", ObjectPoolingCategory.SFX).GetComponent<AudioSource>();
        //    aso.clip = ResourceManager.notify;
        //    aso.Play();
        //};
    }
}