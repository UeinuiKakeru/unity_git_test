using UnityEngine;
using System;
using System.Collections;
  
[RequireComponent(typeof(Animator))]

/// <summary>
/// Base player script
/// </summary>
public class Player : MonoBehaviour {

	// MonobitView �R���|�[�l���g
	MonobitEngine.MonobitView m_MonobitView = null;

    private Animator m_Animator;
    private Locomotion m_Locomotion = null;

	private float m_Speed = 0;	
    private float m_Direction = 0;
	
	public bool hasLog = false;

    void Awake()
    {
        // ���ׂĂ̐e�I�u�W�F�N�g�ɑ΂��� MonobitView �R���|�[�l���g����������
        if (GetComponentInParent<MonobitEngine.MonobitView>() != null)
        {
            m_MonobitView = GetComponentInParent<MonobitEngine.MonobitView>();
        }
        // �e�I�u�W�F�N�g�ɑ��݂��Ȃ��ꍇ�A���ׂĂ̎q�I�u�W�F�N�g�ɑ΂��� MonobitView �R���|�[�l���g����������
        else if (GetComponentInChildren<MonobitEngine.MonobitView>() != null)
        {
            m_MonobitView = GetComponentInChildren<MonobitEngine.MonobitView>();
        }
        // �e�q�I�u�W�F�N�g�ɑ��݂��Ȃ��ꍇ�A���g�̃I�u�W�F�N�g�ɑ΂��� MonobitView �R���|�[�l���g���������Đݒ肷��
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
        // �I�u�W�F�N�g���L�����������Ȃ���Ύ��s���Ȃ�
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
