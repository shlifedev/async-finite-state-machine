using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks; 
using Macovill.LuckyByte;
using UnityEngine;

public class AsyncFSMPerformance : MonoBehaviour
{

    class SampleState : StateBase<MyEnum>
    {
        private readonly GameObject _a;

        public SampleState(GameObject a)
        {`
            _a = a;
        }
        public override async UniTask OnStateEnter()
        {
            var t = GameObject.CreatePrimitive(PrimitiveType.Cube);
               t.transform.SetParent(_a.transform);
               t.transform.localScale = Vector3.one * .1f;

        }

        public override async UniTask OnStateUpdate()
        {
            
            var t= Random.Range(-12f,12f);
            var p = new Vector3(Mathf.Cos(Time.realtimeSinceStartup), Mathf.Sin(Time.realtimeSinceStartup));
            p *= t;
            p *= Time.deltaTime;
            this._a.transform.position += p; 
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
        fsm.Add(MyEnum.SampleA, new SampleState(this.gameObject)); 
       
    }

    void Start()
    {
        fsm.InitializeAndStartLoopAsync(MyEnum.SampleA).Forget();

    }
 
}
