using System;

public interface IDamageable
{
    event Action<float> Damaged;
    void Damage(float amount);
}