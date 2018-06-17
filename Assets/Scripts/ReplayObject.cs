using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public struct ReplayKeyframe
{
    public Vector3 position;
    public Quaternion rotation;

    public ReplayKeyframe(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
}

[DisallowMultipleComponent]
public class ReplayObject : MonoBehaviour
{
    public enum State
    {
        Idle = 0,
        Recording,
        Rewind,
        Replay,
        Invalid,
    }

    [Header("Replay Config")]
    public float m_testReplayTime;

    [SerializeField]
    ReplayManager m_replayManager;

    Queue<ReplayKeyframe> m_recordFrameQueue = new Queue<ReplayKeyframe>();
    Queue<ReplayKeyframe> m_replayFramesQueue = new Queue<ReplayKeyframe>();
    List<ReplayKeyframe> m_rewindFrames = new List<ReplayKeyframe>();

    public State m_State { get; private set; }

    private Vector3 currentPosition;
    private Vector3 previousPosition;
    private Quaternion currentRotation;
    private Quaternion previousRotation;

    private float m_maxFrames;

    ReplayKeyframe m_currentFrame;

    public void Init(ReplayManager rm, float m_framesToRecord)
    {
        m_State = State.Recording;
        m_maxFrames = m_framesToRecord;

        m_replayManager = rm;
        m_replayManager.OnStopRecordingAction += StopRecording;
        m_replayManager.OnReplayAction += StartReplayFromBeginning;
        m_replayManager.OnStopReplayAction += StopReplay;
        m_replayManager.OnRewindAction += RewindToPoint;
    }

    void SetOtherComponentsEnabled(bool enabled)
    {
        var components = GetComponents<Behaviour>();
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i] != this)
            {
                components[i].enabled = enabled;
            }
        }
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (enabled)
            {
                rb.isKinematic = false;
            }
            else
            {
                rb.isKinematic = true;
            }
        }
    }

    void FixedUpdate()
    {
        switch (m_State)
        {
            case State.Recording:
                {
                    Record();
                    break;
                }
            case State.Rewind:
                {
                    Playback();
                    break;
                }
            case State.Replay:
                {
                    Replay();
                    break;
                }
        }
    }

    void StartRecording()
    {
        m_State = State.Recording;
        SetOtherComponentsEnabled(true);
    }

    void StartRewind()
    {
        m_rewindFrames = m_recordFrameQueue.ToList();
        m_State = State.Rewind;
        SetOtherComponentsEnabled(false);
    }

    public void StartReplayFromBeginning()
    {
        m_State = State.Replay;
        m_replayFramesQueue = m_recordFrameQueue;
        SetOtherComponentsEnabled(false);
    }

    void RewindToPoint(int frame)
    {
        m_State = State.Idle;
        SetOtherComponentsEnabled(false);
        var playbackFrame = m_recordFrameQueue.ElementAt(frame);
        transform.position = playbackFrame.position;
        transform.rotation = playbackFrame.rotation;
    }

    void Record()
    {
        m_currentFrame.position = transform.position;
        m_currentFrame.rotation = transform.rotation;
        m_recordFrameQueue.Enqueue(m_currentFrame);
        if (m_recordFrameQueue.Count > m_maxFrames)
        {
            m_recordFrameQueue.Dequeue();
        }
    }

    void Playback()
    {
        if (m_rewindFrames.Count > 0)
        {
            int lastIndex = m_rewindFrames.Count - 1;
            var nextPlaybackFrame = m_rewindFrames[lastIndex];
            transform.position = nextPlaybackFrame.position;
            transform.rotation = nextPlaybackFrame.rotation;
            m_rewindFrames.RemoveAt(lastIndex);
        }
        else
        {
            StartRecording();
        }
    }

    void Replay()
    {
        var playbackFrame = m_replayFramesQueue.Peek();
        transform.position = playbackFrame.position;
        transform.rotation = playbackFrame.rotation;
        m_replayFramesQueue.Dequeue();
        if (m_replayFramesQueue.Count == 0)
        {
            StartRecording();
        }
    }

    void StopRecording()
    {
        m_State = State.Idle;
    }

    public void StopReplay()
    {
        StartRecording();
    }


}
