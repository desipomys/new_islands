using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class Editor_AddPlayerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.AddComponent<CharacterEntity>();
        gameObject.AddComponent<PlayerAnimator>();
        gameObject.AddComponent<PlayerController>();
        gameObject.AddComponent<PlayerMovement>();
        gameObject.AddComponent<PlayerRespawner>();
        gameObject.AddComponent<ObjDataCollector>();
        gameObject.AddComponent<CharacterEntity>();
        gameObject.AddComponent<EventCenter>();
        gameObject.AddComponent<EntityInChunkTracer>();
        gameObject.AddComponent<Rigidbody>();
        Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
