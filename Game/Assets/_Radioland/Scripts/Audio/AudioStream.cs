﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

// Reference: http://forum.unity3d.com/threads/web-radio-streaming-with-bass-dll.168046/

public class AudioStream : MonoBehaviour
{
    [SerializeField] private string url;

    public float[] spectrum;

    private int stream;
    private static bool initialized = false; // Only initialize BASS once between all instances.

    public enum flags
    {
        BASS_DEFAULT
    }

    public enum configs
    {
        BASS_CONFIG_NET_PLAYLIST = 21
    }

    public enum attribs
    {
        BASS_ATTRIB_VOL = 2
    }

    public enum lengths
    {
        BASS_DATA_FFT1024 = -2147483646,
        BASS_DATA_FFT2048 = -2147483645
    }

    [SerializeField] private float m_volume = 0f;
    public float volume {
        get { return m_volume; }
        set {
            m_volume = value;
            BASS_ChannelSetAttribute(stream, attribs.BASS_ATTRIB_VOL, m_volume);
        }
    }

    #region DLL - Stream Configuration and Initialization
    [DllImport("bass")]
    public static extern bool BASS_Init(int device, int freq, int flag, IntPtr hwnd, IntPtr clsid);

    [DllImport("bass")]
    public static extern int BASS_ErrorGetCode();

    [DllImport("bass")]
    public static extern bool BASS_SetConfig(configs config, int valuer);

    [DllImport("bass")]
    public static extern Int32 BASS_StreamCreateURL(string url, int offset, flags Flag, IntPtr dproc, IntPtr user);

    [DllImport("bass")]
    public static extern bool BASS_ChannelPlay(int stream, bool restart);

    [DllImport("bass")]
    public static extern bool BASS_StreamFree(int stream);

    [DllImport("bass")]
    public static extern bool BASS_Free();
    #endregion DLL - Stream Configuration and Initialization

    #region DLL - Stream Control and Analysis
    [DllImport("bass")]
    public static extern bool BASS_SetVolume(float volume);

    [DllImport("bass")]
    public static extern bool BASS_ChannelSetAttribute(int handle, attribs attrib, float value);

    [DllImport("bass")]
    public static extern bool BASS_ChannelSlideAttribute(int handle, attribs attrib, float value, float time);

    [DllImport("bass")]
    public static extern long BASS_ChannelSeconds2Bytes(int handle, double pos);

    [DllImport("bass")]
    public static extern int BASS_ChannelGetData(int handle, float[] buffer, lengths length);
    #endregion DLL - Stream Control and Analysis

    private void Awake() {
        spectrum = new float[512];

        if (!initialized) {
            BASS_Free();

            initialized = BASS_Init(-1, 44100, 0, IntPtr.Zero, IntPtr.Zero);
            if (!initialized) {
                Debug.LogError("Unable to initialize BASS, error code: " + BASS_ErrorGetCode());
            }
        }
    }

    private void Start() {
        if (initialized) {
            BASS_SetConfig(configs.BASS_CONFIG_NET_PLAYLIST, 2);
            stream = BASS_StreamCreateURL(url, 0, flags.BASS_DEFAULT, IntPtr.Zero, IntPtr.Zero);

            if (stream != 0) {
                volume = 0;
                BASS_ChannelPlay(stream, false);
            } else {
                Debug.LogError("Unable to create stream.");
            }
        }
    }

    private void Update() {
        BASS_ChannelGetData(stream, spectrum, lengths.BASS_DATA_FFT1024);
    }

    private void OnApplicationQuit() {
        BASS_StreamFree(stream);
        BASS_Free();
    }
}