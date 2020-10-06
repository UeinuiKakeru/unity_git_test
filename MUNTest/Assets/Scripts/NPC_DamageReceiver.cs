using UnityEngine;
using System.Collections;

/// <summary>
/// Damage receiver for NPC
/// </summary>
public class NPC_DamageReceiver : MonobitEngine.MonoBehaviour {

	// MonobitView �R���|�[�l���g
	MonobitEngine.MonobitView m_MonobitView = null;

	Animator m_Animator;

	[MunRPC]
	void CharacterControllerOff()
	{
		GetComponent<CharacterController>().enabled = false; // disable collision when dead.
	}

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

	void Start()
	{
		m_Animator = GetComponent<Animator>();
	}
	
	public void DoDamage()
	{	
		// die right away when recieving damade
		if(m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SimpleLocomotion"))
			m_Animator.SetBool("Die",true);
	}
	
	void Update()
	{
		// �z�X�g�ȊO�͏��������Ȃ�
		if (!MonobitEngine.MonobitNetwork.isHost)
		{
			return;
		}

		if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Dying"))
		{
			m_Animator.SetBool("Die",false);
			// GetComponent<CharacterController>().enabled = false; // disable collision when dead.
			m_MonobitView.RPC("CharacterControllerOff", MonobitEngine.MonobitTargets.All, null);
		}
		
		if(m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Death")) // when dead, character start floating up (rapture!)
		{
			Vector3 position = transform.position;
			position.y += Time.deltaTime;
			transform.position = position;
		}
	}
}
