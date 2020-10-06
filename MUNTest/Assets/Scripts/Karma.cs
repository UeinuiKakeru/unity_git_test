using UnityEngine;
using System.Collections;


/// <summary>
/// Karma for the player. If he falls under the floor move him high up in the air
/// </summary>
public class Karma : MonoBehaviour 
{
	// MonobitView �R���|�[�l���g
	MonobitEngine.MonobitView m_MonobitView = null;

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

    void Update () 
	{

        // �I�u�W�F�N�g���L�����������Ȃ���Ύ��s���Ȃ�
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
