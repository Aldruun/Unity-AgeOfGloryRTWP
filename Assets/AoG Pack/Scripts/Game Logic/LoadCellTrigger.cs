using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadCellTrigger : MonoBehaviour
{
    public string sceneName;
    public string markerRef;

    private void OnTriggerEnter(Collider other)
    {
        //if(other.GetComponent<PlayerController>())
        //{
        //    SaveManager.SetSceneTransitionData(markerRef);
        //    //GameMaster.Instance.gameStage = GameStage.SceneTransition;
        //    SceneManager.LoadScene(sceneName);
        //}
    }
}