using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentUIController : MonoBehaviour
{
    public GameObject m_uiHighlightPrefab;

    private void Awake()
    {

        // WorldUpdater.OnAgentInit += PrepareAgentUI;
    }

    private void OnDisable()
    {

        // WorldUpdater.OnAgentInit -= PrepareAgentUI;
    }

    private void PrepareAgentUI(ActorStats agent)
    {


    }
}
