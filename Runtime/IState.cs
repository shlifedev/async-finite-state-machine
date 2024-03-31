using Cysharp.Threading.Tasks;

namespace LD.StateMachine
{
    /// <summary>
    /// 상태 인터페이스
    /// 스테이트 머신을 부모로 가지고있음
    /// </summary>
    public interface IState
    {  
        UniTask OnStateEnter();
        UniTask OnStateUpdate();
        UniTask OnStateExit(); 
    }
}