using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEmitter : BaseParticleEmitter {

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }
    
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void spawnParticle()
    {
        spawnAtPosition((Vector2)this.transform.position, Quaternion.identity);
    }

}
