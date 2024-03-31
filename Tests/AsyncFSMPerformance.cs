using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks; 
using LD.StateMachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class AsyncFSMPerformance : MonoBehaviour
{


    class DestroyState : StateBase<MyEnum>
    {
        private readonly GameObject _a;
 
        public DestroyState(GameObject a) 
        {
            _a = a;
        }

        public override UniTask OnStateEnter()
        { 
            GameObject.Destroy(_a);
            return UniTask.CompletedTask;
            
        }

        public override async UniTask OnStateUpdate()
        {
             
        }

        public override async UniTask OnStateExit()
        {
          
        }
    }
    class CubeMoving : StateBase<MyEnum>
    {
        private readonly GameObject _a;

        public CubeMoving(GameObject a)
        {
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
        CreateCube,Destroy,c
    }
    private AsyncStateMachine<MyEnum> fsm;
    // Start is called before the first frame update
    void Awake()
    {
        fsm = new AsyncStateMachine<MyEnum>(this, true);
        fsm.Add(MyEnum.CreateCube, new CubeMoving(this.gameObject)); 
        fsm.Add(MyEnum.Destroy, new DestroyState(this.gameObject)); 
       
    }

    void Start()
    {
        fsm.InitializeAndStartLoopAsync(MyEnum.CreateCube).Forget();

    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            fsm.ChangeStateAsync(MyEnum.Destroy);
        }
    }
}
