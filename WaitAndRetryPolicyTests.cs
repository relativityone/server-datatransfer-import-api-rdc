using System;
using System.Threading;
using NUnit.Framework;

namespace kCura.WinEDDS.TApi.NUnit.Integration
{
    [TestFixture]
    public class WaitAndRetryPolicyTests
    {
        private int _waitTimeMillisecondsBetweenRetryAttempts;
        private int _actualRetryCallCount;
        private int _actualExecFuncCallCount;
        private int _expectedRetryCallCount;
        private int _expectedExecFuncCallCount;
        private Func<int, TimeSpan> _retryDuration;
        private Action<Exception, TimeSpan> _retryAction;
        private Action<CancellationToken> _execFunc;
	    private CancellationToken token;

        private class WaitAndRetryPolicyException : Exception
        {
        }

        [SetUp]
        public void Setup()
        {
            _waitTimeMillisecondsBetweenRetryAttempts = 1;   //DO NOT INCREASE this VALUE BECAUSE IT WILL INCREASE THE TESTS EXECUTION TIME

            _actualRetryCallCount = 0;
            _actualExecFuncCallCount = 0;
        }

        #region "Tests"

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(2)]
        [TestCase(12)]
        public void ItShouldWaitAndRetry_withMethodParams_success(int maxRetryCount)
        {
            GivenTheExpectedRetryCallCount(0);
            GivenTheExpectedExecFuncCallCount(1);

            GivenTheRetryDuration();
            GivenTheRetryAction();
            GivenTheExecFunc((token) => { _actualExecFuncCallCount++; });

            //Act
            WhenExecutingTheWaitAndRetryWithRetryCountAndDurationAsMethodParams(maxRetryCount);

            //Assert
            ThenTheActualRetryCallCountShouldEqual();
            ThenTheActualExecFuncCallCountShouldEqual();
        }


        [TestCase(1)]
        [TestCase(2)]
        [TestCase(2)]
        [TestCase(12)]
        public void ItShouldWaitAndRetry_withConstructorParams_success(int maxRetryCount)
        {
            GivenTheExpectedRetryCallCount(0);
            GivenTheExpectedExecFuncCallCount(1);

            GivenTheRetryDuration();
            GivenTheRetryAction();
            GivenTheExecFunc((token) => { _actualExecFuncCallCount++; });
            
            WhenExecutingTheWaitAndRetryWithRetryCountAndDurationAsConstructorParams(maxRetryCount);
            
            ThenTheActualRetryCallCountShouldEqual();
            ThenTheActualExecFuncCallCountShouldEqual();
        }
        
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(10)]
        public void ItShouldWaitAndRetry_withMethodParams_alwaysFails(int maxRetryCount)
        {
            GivenTheExpectedRetryCallCount(maxRetryCount);
            GivenTheExpectedExecFuncCallCount(maxRetryCount + 1);

            GivenTheRetryDuration();
            GivenTheRetryAction();
            GivenTheExecFunc((token) => {
                _actualExecFuncCallCount++;
                throw new WaitAndRetryPolicyException();
            });
            
            WhenExecutingTheWaitAndRetryWithRetryCountAndDurationAsMethodParamsThenThwowsException(maxRetryCount);
            
            ThenTheActualRetryCallCountShouldEqual();
            ThenTheActualExecFuncCallCountShouldEqual();
        }
        
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(10)]
        public void ItShouldWaitAndRetry_withConstructorParams_alwaysFails(int maxRetryCount)
        {
            GivenTheExpectedRetryCallCount(maxRetryCount);
            GivenTheExpectedExecFuncCallCount(maxRetryCount + 1);

            GivenTheRetryDuration();
            GivenTheRetryAction();
            GivenTheExecFunc((token) => {
                _actualExecFuncCallCount++;
                throw new WaitAndRetryPolicyException();
            });
            
            WhenExecutingTheWaitAndRetryWithRetryCountAndDurationAsConstructorParamsThenThwowsException(maxRetryCount);
            
            ThenTheActualRetryCallCountShouldEqual();
            ThenTheActualExecFuncCallCountShouldEqual();
        }
        
        [TestCase(2, 1)]
        [TestCase(3, 2)]
        [TestCase(10, 5)]
        public void ItShouldWaitAndRetry_withMethodParams_notAllFails(int maxRetryCount, int succesAfterRetryNum)
        {
            GivenTheExpectedRetryCallCount(succesAfterRetryNum);
            GivenTheExpectedExecFuncCallCount(succesAfterRetryNum + 1);

            GivenTheRetryDuration();
            GivenTheRetryAction();
            GivenTheExecFunc((token) => {
                _actualExecFuncCallCount++;
                if (_actualExecFuncCallCount <= succesAfterRetryNum)
                {
                    throw new WaitAndRetryPolicyException();
                }
            });
            
            WhenExecutingTheWaitAndRetryWithRetryCountAndDurationAsMethodParams(maxRetryCount);
            
            ThenTheActualRetryCallCountShouldEqual();
            ThenTheActualExecFuncCallCountShouldEqual();
        }

        [TestCase(2, 1)]
        [TestCase(3, 2)]
        [TestCase(10, 5)]
        public void ItShouldWaitAndRetry_withConstructorParams_notAllFails(int maxRetryCount, int succesAfterRetryNum)
        {
            GivenTheExpectedRetryCallCount(succesAfterRetryNum);
            GivenTheExpectedExecFuncCallCount(succesAfterRetryNum + 1);

            GivenTheRetryDuration();
            GivenTheRetryAction();
            GivenTheExecFunc((token) => {
                _actualExecFuncCallCount++;
                if (_actualExecFuncCallCount <= succesAfterRetryNum)
                {
                    throw new WaitAndRetryPolicyException();
                }
            });
            
            WhenExecutingTheWaitAndRetryWithRetryCountAndDurationAsConstructorParams(maxRetryCount);
            
            ThenTheActualRetryCallCountShouldEqual();
            ThenTheActualExecFuncCallCountShouldEqual();
        }

        #endregion

        #region "Helper methods"

        private void GivenTheExpectedRetryCallCount(int expectedRetryCallCount)
        {
            _expectedRetryCallCount = expectedRetryCallCount;
        }

        private void GivenTheExpectedExecFuncCallCount(int expectedExecFuncCallCount)
        {
            _expectedExecFuncCallCount = expectedExecFuncCallCount;
        }

        private void GivenTheRetryDuration()
        {
            _retryDuration = waitTime => TimeSpan.FromMilliseconds(_waitTimeMillisecondsBetweenRetryAttempts);
        }

        private void GivenTheRetryAction()
        {
            _retryAction = (exception, timeSpan) => { _actualRetryCallCount++; };
        }
        
        private void GivenTheExecFunc(Action<CancellationToken> action)
        {
            _execFunc = action;
        }

        private void WhenExecutingTheWaitAndRetryWithRetryCountAndDurationAsConstructorParams(int maxRetryCount)
        {
            var waitAndRetryPolicy = new WaitAndRetryPolicy(maxRetryCount, _waitTimeMillisecondsBetweenRetryAttempts);
            waitAndRetryPolicy.WaitAndRetry<WaitAndRetryPolicyException>(_retryDuration, _retryAction, _execFunc, CancellationToken.None);
        }

        private void WhenExecutingTheWaitAndRetryWithRetryCountAndDurationAsConstructorParamsThenThwowsException(int maxRetryCount)
        {
            var waitAndRetryPolicy = new WaitAndRetryPolicy(maxRetryCount, _waitTimeMillisecondsBetweenRetryAttempts);
            Assert.That(() => waitAndRetryPolicy.WaitAndRetry<WaitAndRetryPolicyException>(_retryDuration, _retryAction, _execFunc, CancellationToken.None),
                Throws.Exception.TypeOf<WaitAndRetryPolicyException>());
        }

        private void WhenExecutingTheWaitAndRetryWithRetryCountAndDurationAsMethodParams(int maxRetryCount)
        {
            var waitAndRetryPolicy = new WaitAndRetryPolicy();
            waitAndRetryPolicy.WaitAndRetry<WaitAndRetryPolicyException>(maxRetryCount, _retryDuration, _retryAction, _execFunc, CancellationToken.None);
        }

        private void WhenExecutingTheWaitAndRetryWithRetryCountAndDurationAsMethodParamsThenThwowsException(int maxRetryCount)
        {
            var waitAndRetryPolicy = new WaitAndRetryPolicy();
            Assert.That(() => waitAndRetryPolicy.WaitAndRetry<WaitAndRetryPolicyException>(maxRetryCount, _retryDuration, _retryAction, _execFunc, CancellationToken.None),
                Throws.Exception.TypeOf<WaitAndRetryPolicyException>());
        }
        
        private void ThenTheActualRetryCallCountShouldEqual()
        {
            Assert.That(_actualRetryCallCount, Is.EqualTo(_expectedRetryCallCount));
        }

        private void ThenTheActualExecFuncCallCountShouldEqual()
        {
            Assert.That(_actualExecFuncCallCount, Is.EqualTo(_expectedExecFuncCallCount));
        }

        #endregion
    }
}
