// -----------------------------------------------------------------------------------------------------
// <copyright file="WaitAndRetryPolicyTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="WaitAndRetryPolicy"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Client.NUnit
{
    using System;
    using System.Threading;

    using kCura.WinEDDS.TApi;

    using global::NUnit.Framework;

    [TestFixture]
    public class WaitAndRetryPolicyTests
    {
        private int waitTimeMillisecondsBetweenRetryAttempts;
        private int actualRetryCallCount;
        private int actualExecFuncCallCount;
        private int expectedRetryCallCount;
        private int expectedExecFuncCallCount;
        private Func<int, TimeSpan> retryDuration;
        private Action<Exception, TimeSpan> retryAction;
        private Action<CancellationToken> execFunc;

        private class WaitAndRetryPolicyException : Exception
        {
        }

        [SetUp]
        public void Setup()
        {
            waitTimeMillisecondsBetweenRetryAttempts = 1;   //DO NOT INCREASE this VALUE BECAUSE IT WILL INCREASE THE TESTS EXECUTION TIME

            actualRetryCallCount = 0;
            actualExecFuncCallCount = 0;
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(2)]
        [TestCase(12)]
        public void ItShouldWaitAndRetryWithMethodParamsSuccess(int maxRetryCount)
        {
            GivenTheExpectedRetryCallCount(0);
            GivenTheExpectedExecFuncCallCount(1);

            GivenTheRetryDuration();
            GivenTheRetryAction();
            GivenTheExecFunc((token) => { actualExecFuncCallCount++; });

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
        public void ItShouldWaitAndRetryWithConstructorParamsSuccess(int maxRetryCount)
        {
            GivenTheExpectedRetryCallCount(0);
            GivenTheExpectedExecFuncCallCount(1);

            GivenTheRetryDuration();
            GivenTheRetryAction();
            GivenTheExecFunc((token) => { actualExecFuncCallCount++; });
            
            WhenExecutingTheWaitAndRetryWithRetryCountAndDurationAsConstructorParams(maxRetryCount);
            
            ThenTheActualRetryCallCountShouldEqual();
            ThenTheActualExecFuncCallCountShouldEqual();
        }
        
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(10)]
        public void ItShouldWaitAndRetryWithMethodParamsAlwaysFails(int maxRetryCount)
        {
            GivenTheExpectedRetryCallCount(maxRetryCount);
            GivenTheExpectedExecFuncCallCount(maxRetryCount + 1);

            GivenTheRetryDuration();
            GivenTheRetryAction();
            GivenTheExecFunc((token) => {
                actualExecFuncCallCount++;
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
        public void ItShouldWaitAndRetryWithConstructorParamsAlwaysFails(int maxRetryCount)
        {
            GivenTheExpectedRetryCallCount(maxRetryCount);
            GivenTheExpectedExecFuncCallCount(maxRetryCount + 1);

            GivenTheRetryDuration();
            GivenTheRetryAction();
            GivenTheExecFunc((token) => {
                actualExecFuncCallCount++;
                throw new WaitAndRetryPolicyException();
            });
            
            WhenExecutingTheWaitAndRetryWithRetryCountAndDurationAsConstructorParamsThenThwowsException(maxRetryCount);
            
            ThenTheActualRetryCallCountShouldEqual();
            ThenTheActualExecFuncCallCountShouldEqual();
        }
        
        [TestCase(2, 1)]
        [TestCase(3, 2)]
        [TestCase(10, 5)]
        public void ItShouldWaitAndRetryWithMethodParamsNotAllFails(int maxRetryCount, int succesAfterRetryNum)
        {
            GivenTheExpectedRetryCallCount(succesAfterRetryNum);
            GivenTheExpectedExecFuncCallCount(succesAfterRetryNum + 1);

            GivenTheRetryDuration();
            GivenTheRetryAction();
            GivenTheExecFunc((token) => {
                actualExecFuncCallCount++;
                if (actualExecFuncCallCount <= succesAfterRetryNum)
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
        public void ItShouldWaitAndRetryWithConstructorParamsNotAllFails(int maxRetryCount, int succesAfterRetryNum)
        {
            GivenTheExpectedRetryCallCount(succesAfterRetryNum);
            GivenTheExpectedExecFuncCallCount(succesAfterRetryNum + 1);

            GivenTheRetryDuration();
            GivenTheRetryAction();
            GivenTheExecFunc((token) => {
                actualExecFuncCallCount++;
                if (actualExecFuncCallCount <= succesAfterRetryNum)
                {
                    throw new WaitAndRetryPolicyException();
                }
            });
            
            WhenExecutingTheWaitAndRetryWithRetryCountAndDurationAsConstructorParams(maxRetryCount);
            
            ThenTheActualRetryCallCountShouldEqual();
            ThenTheActualExecFuncCallCountShouldEqual();
        }

        private void GivenTheExpectedRetryCallCount(int expectedRetryCallCount)
        {
            this.expectedRetryCallCount = expectedRetryCallCount;
        }

        private void GivenTheExpectedExecFuncCallCount(int expectedExecFuncCallCount)
        {
            this.expectedExecFuncCallCount = expectedExecFuncCallCount;
        }

        private void GivenTheRetryDuration()
        {
            retryDuration = waitTime => TimeSpan.FromMilliseconds(waitTimeMillisecondsBetweenRetryAttempts);
        }

        private void GivenTheRetryAction()
        {
            retryAction = (exception, timeSpan) => { actualRetryCallCount++; };
        }
        
        private void GivenTheExecFunc(Action<CancellationToken> action)
        {
            execFunc = action;
        }

        private void WhenExecutingTheWaitAndRetryWithRetryCountAndDurationAsConstructorParams(int maxRetryCount)
        {
            var waitAndRetryPolicy = new WaitAndRetryPolicy(maxRetryCount, waitTimeMillisecondsBetweenRetryAttempts);
            waitAndRetryPolicy.WaitAndRetry<WaitAndRetryPolicyException>(retryDuration, retryAction, execFunc, CancellationToken.None);
        }

        private void WhenExecutingTheWaitAndRetryWithRetryCountAndDurationAsConstructorParamsThenThwowsException(int maxRetryCount)
        {
            var waitAndRetryPolicy = new WaitAndRetryPolicy(maxRetryCount, waitTimeMillisecondsBetweenRetryAttempts);
            Assert.That(() => waitAndRetryPolicy.WaitAndRetry<WaitAndRetryPolicyException>(retryDuration, retryAction, execFunc, CancellationToken.None),
                Throws.Exception.TypeOf<WaitAndRetryPolicyException>());
        }

        private void WhenExecutingTheWaitAndRetryWithRetryCountAndDurationAsMethodParams(int maxRetryCount)
        {
            var waitAndRetryPolicy = new WaitAndRetryPolicy();
            waitAndRetryPolicy.WaitAndRetry<WaitAndRetryPolicyException>(maxRetryCount, retryDuration, retryAction, execFunc, CancellationToken.None);
        }

        private void WhenExecutingTheWaitAndRetryWithRetryCountAndDurationAsMethodParamsThenThwowsException(int maxRetryCount)
        {
            var waitAndRetryPolicy = new WaitAndRetryPolicy();
            Assert.That(() => waitAndRetryPolicy.WaitAndRetry<WaitAndRetryPolicyException>(maxRetryCount, retryDuration, retryAction, execFunc, CancellationToken.None),
                Throws.Exception.TypeOf<WaitAndRetryPolicyException>());
        }
        
        private void ThenTheActualRetryCallCountShouldEqual()
        {
            Assert.That(actualRetryCallCount, Is.EqualTo(expectedRetryCallCount));
        }

        private void ThenTheActualExecFuncCallCountShouldEqual()
        {
            Assert.That(actualExecFuncCallCount, Is.EqualTo(expectedExecFuncCallCount));
        }
    }
}