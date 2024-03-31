using Cysharp.Threading.Tasks;

namespace LD.StateMachine
{    
    public interface IStateMachine
    {
        public IState CurrentState { get; } 
    }
    /// <summary>
    /// 스테이트 머신 
    /// </summary>
    /// <typeparam name="TStateKey"></typeparam>
    public interface IStateMachine<TStateKey> : IStateMachine
        where TStateKey : struct
    { 
        void Add(TStateKey key, IState state);
        void Remove(TStateKey key);

        void AddTransition(TStateKey from, TStateKey to, ITransition transition);
        void AddTransition(TStateKey from, TStateKey to, System.Func<UniTask<bool>> shouldTransitionPredicate); 
    }
}