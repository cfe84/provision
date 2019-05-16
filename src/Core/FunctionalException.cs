using System;

namespace Provision {
    class FunctionalException: Exception {
        public FunctionalException(string message): base(message) {}
    }
}