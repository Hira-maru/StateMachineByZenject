using NUnit.Framework;
using Workspace;
using Zenject;

sealed class StateMachineTest
{
    static (SampleContext, IStateMachine<SampleStateId>, TickableManager, DisposableManager) InstallStateMachine(SampleStateId defaultStateId)
    {
        var container = new DiContainer();
        container.BindStateMachine(defaultStateId);
        container.Bind<SampleContext>().AsCached();
        container.Bind<InitializableManager>().AsCached();
        container.Bind<TickableManager>().AsCached();
        container.Bind<DisposableManager>().AsCached();

        var context = container.Resolve<SampleContext>();
        var stateMachine = container.Resolve<IStateMachine<SampleStateId>>();
        container.Resolve<InitializableManager>().Initialize();
        var tickableManager = container.Resolve<TickableManager>();
        var disposableManager = container.Resolve<DisposableManager>();
        return (context, stateMachine, tickableManager, disposableManager);
    }

    [TestCase(SampleStateId.One, SampleStateId.Two, SampleStateId.Three)]
    [TestCase(SampleStateId.One, SampleStateId.Three, SampleStateId.Two)]
    [TestCase(SampleStateId.Two, SampleStateId.One, SampleStateId.Three)]
    [TestCase(SampleStateId.Two, SampleStateId.Three, SampleStateId.One)]
    [TestCase(SampleStateId.Three, SampleStateId.One, SampleStateId.Two)]
    [TestCase(SampleStateId.Three, SampleStateId.Two, SampleStateId.One)]
    public void ChangeState(SampleStateId firstState, SampleStateId secondState, SampleStateId otherState)
    {
        var (context, stateMachine, _, _) = InstallStateMachine(firstState);
        Assert.IsTrue(context.ResultMap[firstState].Begin);

        Assert.IsFalse(context.ResultMap[secondState].Begin);
        Assert.IsFalse(context.ResultMap[firstState].End);
        stateMachine.ChangeState(secondState);
        Assert.IsTrue(context.ResultMap[secondState].Begin);
        Assert.IsTrue(context.ResultMap[firstState].End);

        Assert.IsFalse(context.ResultMap[otherState].Begin);
        Assert.IsFalse(context.ResultMap[otherState].End);
    }

    [TestCase(SampleStateId.One, 5, SampleStateId.Two)]
    [TestCase(SampleStateId.One, 5, SampleStateId.Three)]
    [TestCase(SampleStateId.Two, 10, SampleStateId.One)]
    [TestCase(SampleStateId.Two, 10, SampleStateId.Three)]
    [TestCase(SampleStateId.Three, 15, SampleStateId.One)]
    [TestCase(SampleStateId.Three, 15, SampleStateId.Two)]
    public void Update(SampleStateId stateId, int updateCount, SampleStateId otherState)
    {
        var (context, _, tickableManager, _) = InstallStateMachine(stateId);

        Assert.AreEqual(0, context.ResultMap[stateId].UpdateCount);
        for (var i = 0; i < updateCount; i++) tickableManager.Update();
        Assert.AreEqual(updateCount, context.ResultMap[stateId].UpdateCount);
        Assert.AreEqual(0, context.ResultMap[otherState].UpdateCount);
    }

    [TestCase(SampleStateId.One, 5, SampleStateId.Two)]
    [TestCase(SampleStateId.One, 5, SampleStateId.Three)]
    [TestCase(SampleStateId.Two, 10, SampleStateId.One)]
    [TestCase(SampleStateId.Two, 10, SampleStateId.Three)]
    [TestCase(SampleStateId.Three, 15, SampleStateId.One)]
    [TestCase(SampleStateId.Three, 15, SampleStateId.Two)]
    public void LateUpdate(SampleStateId stateId, int lateUpdateCount, SampleStateId otherState)
    {
        var (context, _, tickableManager, _) = InstallStateMachine(stateId);

        Assert.AreEqual(0, context.ResultMap[stateId].LateUpdateCount);
        for (var i = 0; i < lateUpdateCount; i++) tickableManager.LateUpdate();
        Assert.AreEqual(lateUpdateCount, context.ResultMap[stateId].LateUpdateCount);
        Assert.AreEqual(0, context.ResultMap[otherState].LateUpdateCount);
    }

    [TestCase(SampleStateId.One, SampleStateId.Two)]
    [TestCase(SampleStateId.Two, SampleStateId.Three)]
    [TestCase(SampleStateId.Three, SampleStateId.One)]
    public void Dispose(SampleStateId defaultStateId, SampleStateId otherStateId)
    {
        var (context, _, _, disposableManager) = InstallStateMachine(defaultStateId);
        Assert.IsFalse(context.ResultMap[defaultStateId].Disposed);
        Assert.IsFalse(context.ResultMap[otherStateId].Disposed);

        disposableManager.Dispose();
        Assert.IsTrue(context.ResultMap[defaultStateId].Disposed);
        Assert.IsTrue(context.ResultMap[otherStateId].Disposed);
    }
}
