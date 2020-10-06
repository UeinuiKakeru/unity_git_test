using UnityEngine;
using System.Collections;

/// <summary>
/// Makes player shoot bubbles
/// </summary>
/// 
public class Player_Shoot : MonobitEngine.MonoBehaviour {

	// MonobitView コンポーネント
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
	}
	
	// Update is called once per frame
	void Update () 
	{
		// オブジェクト所有権を所持しなければ実行しない
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
