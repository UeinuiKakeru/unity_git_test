using UnityEngine;
using System.Collections;

/// <summary>
/// Damage receiver for NPC
/// </summary>
public class NPC_DamageReceiver : MonobitEngine.MonoBehaviour {

	// MonobitView コンポーネント
	MonobitEngine.MonobitView m_MonobitView = null;

	Animator m_Animator;

	[MunRPC]
	void CharacterControllerOff()
	{
		GetComponent<CharacterController>().enabled = false; // disable collision when dead.
	}

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
		// ホスト以外は処理をしない
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
