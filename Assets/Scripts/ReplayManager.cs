using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayManager : MonoBehaviour
{
    [Header("Replay Config")]
    public float m_numOfSecondsToRecord;

    public int m_targetFrameRate;

    public List<ReplayObject> m_replayObjs = new List<ReplayObject>();

    public System.Action OnReplayAction;
    public System.Action OnStopReplayAction;
    public System.Action OnStopRecordingAction;
    public System.Action<int> OnRewindAction;

    private float m_framesToRecord;

    private void Start()
    {
        m_framesToRecord = m_targetFrameRate * m_numOfSecondsToRecord;

        for(int i = 0; i < m_replayObjs.Count; ++i)
        {
            m_replayObjs[i].Init(this, m_framesToRecord);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RewindToFrame((int)m_framesToRecord);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            ReplayAll();
        }
    }

    //Use this function if you wish to register a Gameobject as a replayobject.
    //It also adds replay objects to children, useful for replaying whole hierarchies such as animations
    public void RegisterReplayObj(GameObject obj, bool addChildren)
    {
        var replayObj = obj.AddComponent<ReplayObject>();
        replayObj.Init(this, m_framesToRecord);

        if (addChildren)
        {
            var children = obj.GetComponentsInChildren<Transform>();

            for (int i = 0; i < children.Length; i++)
            {
                var ro = children[i].gameObject.AddComponent<ReplayObject>();
                ro.Init(this, m_framesToRecord);
            }
        }
    }

    public void RewindToFrame(int frame)
    {
        if (OnRewindAction != null)
        {
            OnRewindAction(frame);
        }
    }

    public void StopAllRecording()
    {
        if (OnStopRecordingAction != null)
        {
            OnStopRecordingAction();
        }
    }

    public void RewindAll()
    {
        if (OnReplayAction != null)
        {
            OnReplayAction();
        }
    }

    public void ReplayAll()
    {
        if (OnReplayAction != null)
        {
            OnReplayAction();
        }
    }

    public void StopReplays()
    {
        if (OnStopReplayAction != null)
        {
            OnStopReplayAction();
        }
    }
}
