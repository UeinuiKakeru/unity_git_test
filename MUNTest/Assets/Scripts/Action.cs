using UnityEngine;
using System;
using System.Collections;
  
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]  

/// <summary>
/// Enables Actions for the main character
/// </summary>/
/// 
public class Action : MonoBehaviour {

	// MonobitView コンポーネント
	MonobitEngine.MonobitView m_MonobitView = null;

	public bool Slide; 				// slide under obstacles
	public bool Vault;				// vaults over obstacles
	public bool DeactivateCollider; // deactivates collider when action enable
	public bool MatchTarget;		// activates match target during actions
	
	// Values are determined by looking at animations in the previewer
	private const float m_VaultMatchTargetStart 	= 0.40f;
	private const float m_VaultMatchTargetStop 		= 0.51f;
	private const float m_SlideMatchTargetStart 	= 0.11f;
	private const float m_SlideMatchTargetStop 		= 0.40f;
	
    private Animator m_Animator;
	private CharacterController m_Controller ;
	Vector3 m_Target = new Vector3();

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
		m_Controller = GetComponent<CharacterController>();	
	}
    
	void Update ()  
	{
		// オブジェクト所有権を所持しなければ実行しない
		if (!m_MonobitView.isMine)
		{
			return;
		}

		if (GetComponent<Recorder>().enabled && !GetComponent<Recorder>().isRecording) return;

		if (m_Animator)
		{														
			if(Slide) ProcessSlide();		
			if(Vault) ProcessVault();	
			if(DeactivateCollider)  m_Controller.enabled = m_Animator.GetFloat("Collider") > 0.5f; // disable character controller based on additionnal curve on action animations
			ProcessMatchTarget();					
		}		
	}		
	void ProcessSlide()
	{				
		bool slide = false;
		RaycastHit hitInfo;					
		Vector3 dir = transform.TransformDirection(Vector3.forward);
		
		if(m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion.Run"))
		{					
			if (Physics.Raycast(transform.position  + new Vector3(0,1.5f,0),dir,out hitInfo,10))
			{				
				if(hitInfo.collider.tag == "Obstacle")
				{					
					m_Target = transform.position + 1.25f * hitInfo.distance * dir;
					slide =  (hitInfo.distance < 6);
				}
			}
		}		
		
		m_Animator.SetBool("Slide",slide);		
	}

	void ProcessVault()
	{
		bool vault = false;
		RaycastHit hitInfo;					
		Vector3 dir = transform.TransformDirection(Vector3.forward);
		
		if(m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion.Run"))
		{
			if (Physics.Raycast(transform.position + new Vector3(0,0.3f,0),dir,out hitInfo,10))
			{
			
				if(hitInfo.collider.tag == "Obstacle")
				{						
					m_Target = hitInfo.point;
					m_Target.y = hitInfo.collider.bounds.center.y + 0.5f * GetComponent<Collider>().bounds.extents.y + 0.075f;
									
					vault =  (hitInfo.distance < 4.5 && hitInfo.distance > 4);						
				}
			}
		}	
		m_Animator.SetBool("Vault",vault);
	}
	
	
	void ProcessMatchTarget()
	{
		if( m_Animator.IsInTransition(0) )
		   return;

		AnimatorStateInfo info = m_Animator.GetCurrentAnimatorStateInfo(0);
		if(info.IsName("Base Layer.Vault"))
		{			
			if(MatchTarget) 
			{
				m_Animator.MatchTarget(m_Target,new Quaternion(),AvatarTarget.LeftHand,new MatchTargetWeightMask(Vector3.one,0),m_VaultMatchTargetStart,m_VaultMatchTargetStop); // start and stop time 
			}
		}
		else if(info.IsName("Base Layer.Slide")) // always do match targeting.
		{
			m_Animator.MatchTarget(m_Target,new Quaternion(),AvatarTarget.Root,new MatchTargetWeightMask(new Vector3(1,0,1),0),m_SlideMatchTargetStart,m_SlideMatchTargetStop);				
		}
	}
	
}