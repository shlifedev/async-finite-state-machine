using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine; 

namespace Macovill.LuckyByte
{
    [Serializable]
    public class AsyncStateMachine<TStateKey> : IDisposable
        , IStateMachine<TStateKey> where TStateKey : struct
    {  
        /// <summary>
        /// 트랜지션끼리 연결시키기 위해 사용
        /// </summary>
        public class TransitionLink
        {
            public TransitionLink(TStateKey from, TStateKey to)
            {
                this.From = from;
                this.To = to;
                this.Callbacks = new List<ITransition>();
            }

            private TStateKey from;
            private TStateKey to;
            private List<ITransition> callbacks;

            public TStateKey From
            {
                get => from;
                set => from = value;
            }

            public TStateKey To
            {
                get => to;
                set => to = value;
            }

            public List<ITransition> Callbacks
            {
                get => callbacks;
                set => callbacks = value;
            } 
        }

        public UniTask? currentTask;
        /// <summary>
        /// 스테이트 머신 생성자입니다. 라이프사이클 오브젝트는 스테이트 머신의 생명주기 관리를 위해 필요합니다.
        /// </summary>
        /// <param name="lifecycleObject"></param>
        public AsyncStateMachine(MonoBehaviour lifecycleObject)
        {
            this._monoBehaviourObject = lifecycleObject; 
        }  
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancelToken"></param>
        public AsyncStateMachine(CancellationToken cancelToken)
        { 
            _disposedToken = cancelToken; 
        }   
        private CancellationTokenSource? _onUpdateCancellation = new CancellationTokenSource();
        private CancellationToken _disposedToken;
        private MonoBehaviour _monoBehaviourObject;
       
        
         
 
 
        private IState m_currentState; 
        public IState CurrentState
        {
            get => m_currentState;
        }
  
        private TStateKey? _mCurState;
        
        /// <summary>
        /// 상태를 변경합니다.
        /// </summary>
        public TStateKey? CurState
        {
            get => _mCurState;
        } 


        private Dictionary<TStateKey, IState> m_States = new Dictionary<TStateKey, IState>();
        public Dictionary<TStateKey, IState> States
        {
            get => m_States;
            set => m_States = value;
        }


        public Dictionary<TStateKey, TransitionLink> Transitions
        {
            get;
            set;
        }
        
        /// <summary>
        /// Awake, 생성자 등에서 호출
        /// </summary>
        /// <param name="defaultState">기본 상태</param> 
        /// <exception cref="Exception"></exception>
        public async UniTask InitializeAndStartLoopAsync(TStateKey defaultState)
        {
          
            if (States.ContainsKey(defaultState) == false) 
                throw new Exception($"등록된 State {defaultState.ToString()} 을(를) 찾을 수 없습니다.");


            UniTask? changeStateTask = null;
            if(_monoBehaviourObject != null)
                changeStateTask = this.ChangeStateAsync(defaultState)
                    .AttachExternalCancellation(this._monoBehaviourObject.GetCancellationTokenOnDestroy());
            else
            {
                changeStateTask = this.ChangeStateAsync(defaultState)
                    .AttachExternalCancellation(_disposedToken);
            }


            await changeStateTask.Value;
                
            if (_monoBehaviourObject != null)
            {
                var token = _monoBehaviourObject.GetCancellationTokenOnDestroy(); 
                this.StartUpdateLoopAsync(token).Forget();
            }
            else
            {
                if (_disposedToken != null)
                    this.StartUpdateLoopAsync(_disposedToken).Forget();
            }  
        }
        
        /// <summary>
        /// 상태 추가
        /// </summary>
        /// <param name="key"></param>
        /// <param name="state"></param>
        public void Add(TStateKey key, IState state)
        {
            this.States.TryAdd(key, state);
            StateBase<TStateKey> addedState = (StateBase<TStateKey>)state;
            addedState.StateMachine = this;
        } 
        
        /// <summary>
        /// 상태 삭제
        /// </summary>
        /// <param name="key"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Remove(TStateKey key)
        {
            if (this.States.ContainsKey(key))
            { 
                var state = this.States[key];
                if (m_currentState.Equals(state))
                {
                    // TODO 구현필요, 근데 삭제할 일이 딱히 있나. 
                    throw new NotImplementedException(
                        $"[구현필요]삭제 하려는 스테이트가 currentState({m_currentState.GetType().Name}) 과 같은 경우 이 작업을 수행할 수 없습니다.");
                }
                StateBase<TStateKey> tstate = (StateBase<TStateKey>)state;
                tstate.StateMachine = null;
                this.States.Remove(key);
            } 
        }

        
        private bool IsKeyValid(TStateKey key) => m_States.ContainsKey(key) == true;
        

        /// <summary>
        /// 트랜지션 조건을 추가합니다.
        /// 트랜지션이 2개이상 중첩된경우 And 연산됩니다.
        /// </summary> 
        public void AddTransition(TStateKey from, TStateKey to, ITransition transition)
        {
            Transitions ??= new();
            if (!IsKeyValid(from) || IsKeyValid(to)) 
                throw new Exception($"{from} or {to} 트랜지션 키를 찾을 수 없습니다. 먼저 키를 등록해주세요.");

            Transitions.TryGetValue(from, out var linkData);
            if(linkData == null) 
                Transitions.Add(from, new TransitionLink(from, to));
            
            Transitions[from].Callbacks.Add(transition);
        }

        /// <summary>
        /// 트랜지션 조건을 Func Predicate로 추가합니다.
        /// 트랜지션이 2개이상 중첩된경우 And 연산됩니다.
        /// </summary> 
        public void AddTransition(TStateKey from, TStateKey to, Func<UniTask<bool>> shouldTransitionPredicate)
        { 
            Transitions ??= new();
            if (!IsKeyValid(from) || !IsKeyValid(to)) 
                throw new Exception($"{from} or {to} 트랜지션 키를 찾을 수 없습니다. 먼저 키를 등록해주세요.");
            
            Transitions.TryGetValue(from, out var linkData);
            if(linkData == null) 
                Transitions.Add(from, new TransitionLink(from, to));

            Transitions[from].Callbacks.Add(new TransitionFunctor(shouldTransitionPredicate));
        }

        
        
        /// <summary>
        /// 이 함수는 CurState에 직접 값을 대입하는것과 동일한 동작을 합니다.
        /// </summary>
        public async UniTask ChangeStateAsync(TStateKey key)
        {
            if (_mCurState.Equals(key))
                return;


            if (m_currentState != null)
            {
                _onUpdateCancellation?.Cancel();
                currentTask = m_currentState.OnStateExit();
               
                await currentTask.Value;
                currentTask = null;
            }

            m_currentState = States[key];
            _mCurState = key;

            if (m_currentState != null)
            {
                currentTask = m_currentState.OnStateEnter(); 
                await currentTask.Value;
                currentTask = null;
            }

            this._mCurState = key;
        }
        
        /// <summary>
        /// 이 함수는 CurState에 직접 값을 대입하는것과 동일한 동작을 합니다.
        /// </summary>
        public async UniTask ChangeStateImediate(TStateKey key)
        {
            if (_mCurState.Equals(key))
            {  
                return;   
            }
                
            if (m_currentState != null)
            {
                await m_currentState.OnStateExit();
            }

            m_currentState = States[key];
            _mCurState = key;

            if (m_currentState != null)
            {
                await m_currentState.OnStateEnter();
            }

            this._mCurState = key;
        }
        async UniTask LogicAsync()
        {
            // 트랜지션이 완료되기 전까지 State 업데이트를 별도로 실행하지 않습니다.
            await UpdateTransitionAsync(); 
            await UpdateState().AttachExternalCancellation(_onUpdateCancellation.Token); 
        }


        async UniTask UpdateTransitionAsync()
        { 
            if (Transitions == null) return;
            if (this.Transitions.ContainsKey(this.CurState.Value))
            {
                var link = this.Transitions[CurState.Value];
                for (var index = 0; index < link.Callbacks.Count; index++)
                {
                    var callback = link.Callbacks[index];
                    var shouldTransition = await callback.ShouldTransition();
                    if (shouldTransition)
                    {
                        await UniTask.WaitUntil(() => currentTask == null);
                        // 현재는 트랜지션이 가능한 상태더라도 현재 state의 task가 종료 되어야만 호출 됨.  
                        await ChangeStateAsync(link.To);
                    }
                }
            }
        }

        private async UniTask UpdateState()
        { 
            if (States != null && CurrentState != null)
            {
                currentTask = CurrentState.OnStateUpdate(); 
                await currentTask.Value;
                currentTask = null;
            } 
        }
        
        
        /// <summary>
        /// FSM 로직을 업데이트 타이밍에 비동기로 처리합니다.
        /// </summary>
        private async UniTaskVoid StartUpdateLoopAsync(CancellationToken token)
        {
            await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(token))
            {
                if (_monoBehaviourObject != null)
                {
                    if (_monoBehaviourObject.gameObject.activeSelf && _monoBehaviourObject.enabled)
                    {
                        await LogicAsync().SuppressCancellationThrow();
                    }
                }
                else
                { 
                    await LogicAsync().SuppressCancellationThrow(); 
                }
            } 
        }
          

        public void Dispose()
        { 
            _onUpdateCancellation?.Cancel();
            _onUpdateCancellation?.Dispose();
            _onUpdateCancellation = null;
             
            this._monoBehaviourObject = null;
            
            
        }
    }
}