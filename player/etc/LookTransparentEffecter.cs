using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ڵ����ߵ�gameobj���͸����δ��������
/// </summary>
public class LookTransparentEffecter : MonoBehaviour
{

    // �����ϰ����Renderer����
    private List<Renderer> _ObstacleCollider;

    // �������ǣ�֮��ͨ������ʶ�𣿻���tag��Ŀǰ�ֶ��Ϲ�����
    public GameObject _target;
    public LayerMask mask;
    Camera camera;
    // ��ʱ���գ����ڴ洢
    private Renderer[] _tempRenderer;
    void Start()
    {
        _ObstacleCollider = new List<Renderer>();
        _target = gameObject;
        camera = Camera.main;
    }
    void FixedUpdate()
    {
        // ����ʹ�ã���ɫ���ߣ���Scene�����ɼ�
#if UNITY_EDITOR
        Debug.DrawLine(_target.transform.position, camera.transform.position, Color.red);
#endif
        RaycastHit[] hit;
        hit = Physics.RaycastAll(_target.transform.position, camera.transform.position, Vector3.Magnitude(camera.transform.position - _target.transform.position), mask);
        //  �����ײ��Ϣ��������0��
        if (hit.Length > 0)
        {   // �����ϰ���͸����Ϊ0.5
            for (int i = 0; i < hit.Length; i++)
            {
                _tempRenderer = hit[i].collider.gameObject.GetComponentsInChildren<Renderer>();
                if (_tempRenderer != null)
                {
                    for (int j = 0; j < _tempRenderer.Length; j++)
                    {
                        if (!_ObstacleCollider.Contains(_tempRenderer[j]))
                        {
                            _ObstacleCollider.Add(_tempRenderer[j]);
                            SetMaterialsAlpha(_tempRenderer[j], 0.5f);
                        }
                    }
                    
                }
                //Debug.Log(hit[i].collider.name);
            }

        }// �ָ��ϰ���͸����Ϊ1
        else
        {
            for (int i = 0; i < _ObstacleCollider.Count; i++)
            {
                //_tempRenderer = _ObstacleCollider[i];
                SetMaterialsAlpha(_ObstacleCollider[i], 1f);
            }
           
            _ObstacleCollider.Clear();
        }

    }

    // �޸��ϰ����͸����
    private void SetMaterialsAlpha(Renderer _renderer, float Transpa)
    {
        
            //Renderer _renderer = renderers[j];
            // һ����Ϸ�����ĳ�����ֶ������ж��������
            int materialsCount = _renderer.materials.Length;
            for (int i = 0; i < materialsCount; i++)
            {

                // ��ȡ��ǰ��������ɫ
                Color color = _renderer.materials[i].color;

                // ����͸���ȣ�0--1��
                color.a = Transpa;

                // ���õ�ǰ��������ɫ����Ϸ�������Ҽ�SelectShader���Կ�����������Ϊ_Color��
                _renderer.materials[i].SetColor("_Color", color);
            }
        


    }
}
