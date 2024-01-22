using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IMortal
{
    int HitPoints { get; set; }

    void OnHit()
    {
        HitPoints--;
        if (HitPoints < 1)
        {
            Die();
        }
    }

    void Die();
}
