using NUnit.Framework;
using Workspace;
using Zenject;

sealed class StateMachineTest
{
    SampleContext _context;
    IStateMachine<SampleStateId> _stateMachine;
    TickableManager _tickableManager;

    [SetUp]
    public void SetUp()
    {
        var container = new DiContainer();
        container.Install<TestInstaller>();
        _context = container.Resolve<SampleContext>();
        _stateMachine = container.Resolve<IStateMachine<SampleStateId>>();
        _tickableManager = container.Resolve<TickableManager>();
    }

    [TestCase(SampleStateId.One, SampleStateId.Two, SampleStateId.Three)]
    [TestCase(SampleStateId.One, SampleStateId.Three, SampleStateId.Two)]
    [TestCase(SampleStateId.Two, SampleStateId.One, SampleStateId.Three)]
    [TestCase(SampleStateId.Two, SampleStateId.Three, SampleStateId.One)]
    [TestCase(SampleStateId.Three, SampleStateId.One, SampleStateId.Two)]
    [TestCase(SampleStateId.Three, SampleStateId.Two, SampleStateId.One)]
    public void ChangeState(SampleStateId firstState, SampleStateId secondState, SampleStateId otherState)
    {
        Assert.IsFalse(_context.ResultMap[firstState].Begin);
        _stateMachine.ChangeState(firstState);
        Assert.IsTrue(_context.ResultMap[firstState].Begin);

        Assert.IsFalse(_context.ResultMap[secondState].Begin);
        _stateMachine.ChangeState(secondState);
        Assert.IsTrue(_context.ResultMap[secondState].Begin);
        Assert.IsTrue(_context.ResultMap[firstState].End);

        Assert.IsFalse(_context.ResultMap[otherState].Begin);
        Assert.IsFalse(_context.ResultMap[otherState].End);
    }

    [TestCase(SampleStateId.One, 5, SampleStateId.Two)]
    [TestCase(SampleStateId.One, 5, SampleStateId.Three)]
    [TestCase(SampleStateId.Two, 10, SampleStateId.One)]
    [TestCase(SampleStateId.Two, 10, SampleStateId.Three)]
    [TestCase(SampleStateId.Three, 15, SampleStateId.One)]
    [TestCase(SampleStateId.Three, 15, SampleStateId.Two)]
    public void Update(SampleStateId stateId, int updateCount, SampleStateId otherState)
    {
        _stateMachine.ChangeState(stateId);

        Assert.AreEqual(0, _context.ResultMap[stateId].UpdateCount);
        for (var i = 0; i < updateCount; i++) _tickableManager.Update();
        Assert.AreEqual(updateCount, _context.ResultMap[stateId].UpdateCount);
        Assert.AreEqual(0, _context.ResultMap[otherState].UpdateCount);
    }

    [TestCase(SampleStateId.One, 5, SampleStateId.Two)]
    [TestCase(SampleStateId.One, 5, SampleStateId.Three)]
    [TestCase(SampleStateId.Two, 10, SampleStateId.One)]
    [TestCase(SampleStateId.Two, 10, SampleStateId.Three)]
    [TestCase(SampleStateId.Three, 15, SampleStateId.One)]
    [TestCase(SampleStateId.Three, 15, SampleStateId.Two)]
    public void LateUpdate(SampleStateId stateId, int lateUpdateCount, SampleStateId otherState)
    {
        _stateMachine.ChangeState(stateId);

        Assert.AreEqual(0, _context.ResultMap[stateId].LateUpdateCount);
        for (var i = 0; i < lateUpdateCount; i++) _tickableManager.LateUpdate();
        Assert.AreEqual(lateUpdateCount, _context.ResultMap[stateId].LateUpdateCount);
        Assert.AreEqual(0, _context.ResultMap[otherState].LateUpdateCount);
    }

    sealed class TestInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindStateMachine<SampleStateId>();
            Container.Bind<SampleContext>().AsCached();
            Container.Bind<TickableManager>().AsCached();
        }
    }
}
