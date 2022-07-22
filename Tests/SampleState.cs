using System.Collections.Generic;

namespace Workspace
{
    enum SampleStateId
    {
        One,
        Two,
        Three,
    }

    sealed class SampleContext
    {
        public readonly Dictionary<SampleStateId, StateResult> ResultMap = new()
        {
            { SampleStateId.One, new StateResult() },
            { SampleStateId.Two, new StateResult() },
            { SampleStateId.Three, new StateResult() },
        };
    }

    sealed class StateResult
    {
        public bool Begin;
        public int UpdateCount;
        public int LateUpdateCount;
        public bool End;
    }

    sealed class SampleStateOne : PlaceholderState<SampleStateId>
    {
        public override SampleStateId Id => SampleStateId.One;

        readonly SampleContext _context;

        public SampleStateOne(SampleContext context)
        {
            _context = context;
        }

        protected override void OnBegin()
        {
            _context.ResultMap[Id].Begin = true;
        }

        protected override void OnUpdate()
        {
            _context.ResultMap[Id].UpdateCount++;
        }

        protected override void OnLateUpdate()
        {
            _context.ResultMap[Id].LateUpdateCount++;
        }

        protected override void OnEnd()
        {
            _context.ResultMap[Id].End = true;
        }
    }

    sealed class SampleStateTwo : PlaceholderState<SampleStateId>
    {
        public override SampleStateId Id => SampleStateId.Two;

        readonly SampleContext _context;

        public SampleStateTwo(SampleContext context)
        {
            _context = context;
        }

        protected override void OnBegin()
        {
            _context.ResultMap[Id].Begin = true;
        }

        protected override void OnUpdate()
        {
            _context.ResultMap[Id].UpdateCount++;
        }

        protected override void OnLateUpdate()
        {
            _context.ResultMap[Id].LateUpdateCount++;
        }

        protected override void OnEnd()
        {
            _context.ResultMap[Id].End = true;
        }
    }

    sealed class SampleStateThree : PlaceholderState<SampleStateId>
    {
        public override SampleStateId Id => SampleStateId.Three;

        readonly SampleContext _context;

        public SampleStateThree(SampleContext context)
        {
            _context = context;
        }

        protected override void OnBegin()
        {
            _context.ResultMap[Id].Begin = true;
        }

        protected override void OnUpdate()
        {
            _context.ResultMap[Id].UpdateCount++;
        }

        protected override void OnLateUpdate()
        {
            _context.ResultMap[Id].LateUpdateCount++;
        }

        protected override void OnEnd()
        {
            _context.ResultMap[Id].End = true;
        }
    }
}
