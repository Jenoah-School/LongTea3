using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunShock : FighterPower
{
    [SerializeField] ParticleSystem particles;
    [SerializeField] GameObject colliderBox;

    List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

    private ParticleSystem.Particle[] m_particles;

    bool isActive;

    public override void Activate()
    {
        isActive = true;
        particles.Play();
    }

    private void Update()
    {
        if(isActive)SetCollisionBoxPosition();
    }

    private void SetCollisionBoxPosition()
    {
        int numParticlesAlive = particles.GetParticles(m_particles);

        Vector3 averagePos = Vector3.zero;

        for (int i = 0; i < numParticlesAlive; i++)
        {
            averagePos += m_particles[i].position;
        }

        averagePos /= numParticlesAlive;

        particles.SetParticles(m_particles, numParticlesAlive);

        colliderBox.transform.position = averagePos;
    }
}
