using UnityEngine;

public class Larissa_Boss : BossController
{
    public override BossState ChooseNextState() {
        return ChaseState;
    }
}
