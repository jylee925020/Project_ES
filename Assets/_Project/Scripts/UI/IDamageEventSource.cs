using System;

public interface IDamageEventSource
{
    event Action<int> OnDamaged;
}