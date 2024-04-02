# AsyncStateMachine

Asynchronous state machines help you write smarter game logic. 

- Support await OnEnter
- Support await OnUpdate
- Support await OnExit
- If you enter a state called A, then state A behaves as follows.  
`OnEnter (await) -> OnUpdate (await) -> Wait for ChangeState`
- If you want to transition from state A to state B, you will wait until the Task of the Enter/Exit function in state A is finished.

You can see Example below
[Example Source](https://github.com/shlifedev/async-finite-state-machine/tree/main/Example)

https://github.com/shlifedev/async-finite-state-machine/assets/49047211/d26aefe9-1b41-41fc-9206-a6c5c8bc8efa

