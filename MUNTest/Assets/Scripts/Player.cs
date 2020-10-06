using UnityEngine;
using System;
using System.Collections;
  
[RequireComponent(typeof(Animator))]

/// <summary>
/// Base player script
/// </summary>
public class Player : MonoBehaviour {

	// MonobitView コンポーネント
	MonobitEngine.MonobitView m_MonobitView = null;

    private Animator m_Animator;
    private Locomotion m_Locomotion = null;

	private float m_Speed = 0;	
    private float m_Direction = 0;
	
	public bool hasLog = false;

    void Awake()
    {
        // すべての親オブジェクトに対して MonobitView コンポーネントを検索する
        if (GetComponentInParent<MonobitEngine.MonobitView>() != null)
        {
            m_MonobitView = GetComponentInParent<MonobitEngine.MonobitView>();
        }
        // 親オブジェクトに存在しない場合、すべての子オブジェクトに対して MonobitView コンポーネントを検索する
        else if (GetComponentInChildren<MonobitEngine.MonobitView>() != null)
        {
            m_MonobitView = GetComponentInChildren<MonobitEngine.MonobitView>();
        }
        // 親子オブジェクトに存在しない場合、自身のオブジェクトに対して MonobitView コンポーネントを検索して設定する
        else
        {
            m_MonobitView = GetComponent<MonobitEngine.MonobitView>();
        }
    }

    void Start () 
	{
        m_Animator = GetComponent<Animator>();        
		m_Animator.logWarnings = false; // so we dont get warning when updating controller in live link ( undocumented/unsupported function!)
	}
    
	void Update ()  
	{
        // オブジェクト所有権を所持しなければ実行しない
        if (!m_MonobitView.isMine)
        {
            return;
        }

        if (m_Locomotion == null) m_Locomotion = new Locomotion(m_Animator);
		
        if (m_Animator && Camera.main)
		{
            JoystickToWorld.ComputeSpeedDirection(transform,ref m_Speed, ref m_Direction);		
		}
		
		
		m_Locomotion.Do(m_Speed * 6, m_Direction * 180);
		
		m_Animator.SetBool("HoldLog", hasLog);
	}		
	
}
