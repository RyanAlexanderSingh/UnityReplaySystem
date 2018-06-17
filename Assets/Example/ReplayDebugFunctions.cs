using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayDebugFunctions : MonoBehaviour {

    public ReplayManager m_replayManager;

    private float m_guiSliderValue;
    private float m_cachedSliderValue;

    private float m_timeRecording;

    private bool m_hasFullRecord = false;

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), m_replayManager.m_replayObjs[0].m_State.ToString());
        Rect slider = new Rect((Screen.width * 0.5f) - 75f, Screen.height - 50f, 150f, 30f);
        m_guiSliderValue = GUI.HorizontalSlider(slider, m_guiSliderValue, 0.0F, m_replayManager.m_numOfSecondsToRecord);

        if (GUI.Button(new Rect(10, 70, 50, 30), "Replay"))
        {
            m_replayManager.ReplayAll();
        }
    }

    void Update ()
    {
        //if (m_timeRecording < m_replayManager.m_numOfSecondsToRecord)
        //{
        //    m_timeRecording += Time.deltaTime;
        //    m_guiSliderValue = m_timeRecording;
        //    m_cachedSliderValue = m_guiSliderValue;
        //    if (m_timeRecording >= m_replayManager.m_numOfSecondsToRecord)
        //    {
        //        m_hasFullRecord = true;
        //    }
        //}

        if (m_cachedSliderValue != m_guiSliderValue)
        {
            m_cachedSliderValue = m_guiSliderValue;
            var frames = m_guiSliderValue * 30f;
            m_replayManager.RewindToFrame((int)frames);
        }
    }
}
