using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DartGunsPlus.Content.Systems;

// All credit goes to Nurby!
public class CompactParticle
{
    public Vector2 Position;
    public Vector2 Velocity;
    public float Rotation;
    public float Scale;
    public float Opacity;
    public float TimeAlive;
    public Color Color;
    public bool Dead;
}

public class CompactParticleManager
{
    private readonly List<CompactParticle> _particles = new();
    private readonly Action<CompactParticle> _updateParticle;
    private readonly Action<CompactParticle, SpriteBatch, Vector2> _drawParticle;
    
    public CompactParticleManager(Action<CompactParticle> updateParticle, Action<CompactParticle, SpriteBatch, Vector2> drawParticle)
    {
        _updateParticle = updateParticle;
        _drawParticle = drawParticle;
    }
    
    public void AddParticle(Vector2 position, Vector2 velocity, float rotation, float scale, float opacity, Color color)
    {
        _particles.Add(new CompactParticle {
            Position = position,
            Velocity = velocity,
            Rotation = rotation,
            Scale = scale,
            Opacity = opacity,
            TimeAlive = 0,
            Color = color
        });
    }
    
    public void Update()
    {
        for (int i = 0; i < _particles.Count; i++)
        {
            CompactParticle particle = _particles[i];
            _updateParticle(particle);
            particle.TimeAlive++;
            
            if (particle.Dead)
            {
                _particles.RemoveAt(i);
                i--;
            }
        }
    }
    public void Draw(SpriteBatch spriteBatch, Vector2 anchor)
    {
        foreach (CompactParticle particle in _particles)
        {
            _drawParticle(particle, spriteBatch, anchor);
        }
    }
}