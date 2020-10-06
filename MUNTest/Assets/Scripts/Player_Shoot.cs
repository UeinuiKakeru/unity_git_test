using UnityEngine;
using System.Collections;

/// <summary>
/// Makes player shoot bubbles
/// </summary>
/// 
public class Player_Shoot : MonobitEngine.MonoBehaviour {

	// MonobitView �R���|�[�l���g
	MonobitEngine.MonobitView m_MonobitView = null;

	public GameObject Bullet;
	public Transform BulletSpawnPoint;
	public Transform BulletParent;
	
	const float m_BulletSpeed = 20.0f;
	const float m_BulletDuration = 2.0f;
	float m_Timer = 0;
	
	Animator m_Animator;

	[MunRPC]
	void SpawnBazookaBullet()
	{
		GameObject newBullet = Instantiate(Bullet, BulletSpawnPoint.position, Quaternion.Euler(0, 0, 0)) as GameObject;
		Destroy(newBullet, m_BulletDuration);
		newBullet.GetComponent<Rigidbody>().velocity = -BulletSpawnPoint.forward * m_BulletSpeed;
		newBullet.GetComponent<DamageProvider>().SetScaleBullet();
		newBullet.SetActive(true);

		if (BulletParent) newBullet.transform.parent = BulletParent;
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

	void Start () 
	{
		m_Animator = GetComponent<Animator>();	
	}
	
	// Update is called once per frame
	void Update () 
	{
		// �I�u�W�F�N�g���L�����������Ȃ���Ύ��s���Ȃ�
		if (!m_MonobitView.isMine)
		{
			return;
		}

		bool shoot = Input.GetButton("Fire1");
		m_Animator.SetBool("Shoot",shoot);

		if(CanShoot() && shoot)
		{			
			if(m_Timer > 0.1f) // firing rate
			{
				m_MonobitView.RPC("SpawnBazookaBullet", MonobitEngine.MonobitTargets.All, null);
				
				m_Timer = 0;
			}
		}	
				
		m_Timer += Time.deltaTime;
	}
	
	bool CanShoot()
	{
		AnimatorStateInfo info = m_Animator.GetCurrentAnimatorStateInfo(0);
		
		if(info.IsName("Base Layer.Death") || info.IsName("Base Layer.Reviving") || info.IsName("Base Layer.Dying"))
			return false;
			
		return m_Animator.GetBool("HoldLog");
		
	}
}
