﻿using VDT.Core.DependencyInjection.Tests.Decorators;

namespace VDT.Core.DependencyInjection.Tests {
    public interface ITransientServiceImplementationTarget {
        [TestDecorator]
        public string GetValue();
    }
}
