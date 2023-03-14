using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 将遮挡视线的gameobj变半透明，未完善需求
/// </summary>
public class LookTransparentEffecter : MonoBehaviour
{

    // 所有障碍物的Renderer数组
    private List<Renderer> _ObstacleCollider;

    // 人物主角（之后通过名字识别？还是tag？目前手动拖过来）
    public GameObject _target;
    public LayerMask mask;
    Camera camera;
    // 临时接收，用于存储
    private Renderer[] _tempRenderer;
    void Start()
    {
        _ObstacleCollider = new List<Renderer>();
        _target = gameObject;
        camera = Camera.main;
    }
    void FixedUpdate()
    {
        // 调试使用：红色射线，仅Scene场景可见
#if UNITY_EDITOR
        Debug.DrawLine(_target.transform.position, camera.transform.position, Color.red);
#endif
        RaycastHit[] hit;
        hit = Physics.RaycastAll(_target.transform.position, camera.transform.position, Vector3.Magnitude(camera.transform.position - _target.transform.position), mask);
        //  如果碰撞信息数量大于0条
        if (hit.Length > 0)
        {   // 设置障碍物透明度为0.5
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

        }// 恢复障碍物透明度为1
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

    // 修改障碍物的透明度
    private void SetMaterialsAlpha(Renderer _renderer, float Transpa)
    {
        
            //Renderer _renderer = renderers[j];
            // 一个游戏物体的某个部分都可以有多个材质球
            int materialsCount = _renderer.materials.Length;
            for (int i = 0; i < materialsCount; i++)
            {

                // 获取当前材质球颜色
                Color color = _renderer.materials[i].color;

                // 设置透明度（0--1）
                color.a = Transpa;

                // 设置当前材质球颜色（游戏物体上右键SelectShader可以看见属性名字为_Color）
                _renderer.materials[i].SetColor("_Color", color);
            }
        


    }
}
