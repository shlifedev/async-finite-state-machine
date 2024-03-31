# AsyncStateMachine

Asynchronous state machines help you write smarter game logic. 

- Support await OnEnter
- Support await OnUpdate
- Support await OnExit
- If you enter a state called A, then state A behaves as follows.  
`OnEnter (await) -> OnUpdate (await) -> Wait for ChangeState`
- If you want to transition from state A to state B, you will wait until the Task of the Enter/Exit function in state A is finished.
