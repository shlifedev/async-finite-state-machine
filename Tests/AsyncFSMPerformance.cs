using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks; 
using Macovill.LuckyByte;
using UnityEngine;

public class AsyncFSMPerformance : MonoBehaviour
{

    class SampleState : StateBase<MyEnum>
    { 
        public override async UniTask OnStateEnter()
        {
            
            
        }

        public override async UniTask OnStateUpdate()
        {
          
        }

        public override async UniTask OnStateExit()
        {
       
        }
    }

    enum MyEnum
    {
        SampleA,b,c
    }
    private AsyncStateMachine<MyEnum> fsm;
    // Start is called before the first frame update
    void Awake()
    {
        fsm = new AsyncStateMachine<MyEnum>(this);
        fsm.Add(MyEnum.SampleA, new SampleState()); 
       
    }

    void Start()
    {
        fsm.InitializeAndStartLoopAsync(MyEnum.SampleA).Forget();

    }
 
}
