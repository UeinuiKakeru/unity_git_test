using UnityEngine;
using System.Collections;


/// <summary>
/// Karma for the player. If he falls under the floor move him high up in the air
/// </summary>
public class Karma : MonoBehaviour 
{
	// MonobitView コンポーネント
	MonobitEngine.MonobitView m_MonobitView = null;

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

    void Update () 
	{

        // オブジェクト所有権を所持しなければ実行しない
        if (!m_MonobitView.isMine)
        {
            return;
        }
        
        if (transform.position.y < -5)
		{
			transform.position = new Vector3(0,25,0);
		}	
	}
}
