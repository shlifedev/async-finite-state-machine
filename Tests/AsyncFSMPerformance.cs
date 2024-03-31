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
            Debug.Log("Enter" + StateMachine.CurState);
        }

        public override async UniTask OnStateUpdate()
        {
            Debug.Log("hi");
        }

        public override async UniTask OnStateExit()
        {
            Debug.Log("Exit" + StateMachine.CurState);
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
        fsm = new AsyncStateMachine<MyEnum>(this.destroyCancellationToken);
        fsm.Add(MyEnum.SampleA, new SampleState()); 
       
    }

    void Start()
    {
        fsm.InitializeAndStartLoopAsync(MyEnum.SampleA).Forget();

    }
 
}
