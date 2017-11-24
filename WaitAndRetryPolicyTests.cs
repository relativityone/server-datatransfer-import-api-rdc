using System;
using NUnit.Framework;

namespace kCura.WinEDDS.TApi.NUnit.Integration
{
    [TestFixture]
    public class WaitAndRetryPolicyTests
    {
        private class WaitAndRetryPolicyException : Exception
        {
        }

        private int _waitTimeBetweenRetryAttempts;
        private int _actualRetryCallCount;
        private int _actualExecFuncCallCount;
        
        [SetUp]
        public void Setup()
        {
            _waitTimeBetweenRetryAttempts = 1;   //DO NOT INCREASE VALUE BECAUSE IT INCREASE THE TESTS EXECUTION TIME

            _actualRetryCallCount = 0;
            _actualExecFuncCallCount = 0;
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(2)]
        [TestCase(12)]
        public void ItShouldWaitAndRetry_withParameters_success(int maxRetryCount)
        {
            //Arrange
            var policy = new WaitAndRetryPolicy();

            const int expectedRetryCallCount = 0;
            const int expectedExecFuncCallCount = 1;

            Func<int, TimeSpan> retryDuration = waitTime => TimeSpan.FromMilliseconds(_waitTimeBetweenRetryAttempts);
            Action<Exception, TimeSpan> retryAction = (exception, timeSpan) => { _actualRetryCallCount++; };
            Action execFunc = () => { _actualExecFuncCallCount++; };

            //Act 
            policy.WaitAndRetry<Exception>(maxRetryCount, retryDuration, retryAction, execFunc);

            //Assert
            Assert.That(_actualRetryCallCount, Is.EqualTo(expectedRetryCallCount));
            Assert.That(_actualExecFuncCallCount, Is.EqualTo(expectedExecFuncCallCount));
        }
        
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(2)]
        [TestCase(12)]
        public void ItShouldWaitAndRetry_withConstructor_success(int maxRetryCount)
        {
            //Arrange
            var policy = new WaitAndRetryPolicy(maxRetryCount, _waitTimeBetweenRetryAttempts);

            const int expectedRetryCallCount = 0;
            const int expectedExecFuncCallCount = 1;
            
            Func<int, TimeSpan> retryDuration = waitTime => TimeSpan.FromMilliseconds(_waitTimeBetweenRetryAttempts);
            Action<Exception, TimeSpan> retryAction = (exception, timeSpan) => { _actualRetryCallCount++; };
            Action execFunc = () => { _actualExecFuncCallCount++; };

            //Act 
            policy.WaitAndRetry<Exception>(retryDuration, retryAction, execFunc);

            //Assert
            Assert.That(_actualRetryCallCount, Is.EqualTo(expectedRetryCallCount));
            Assert.That(_actualExecFuncCallCount, Is.EqualTo(expectedExecFuncCallCount));
        }
        
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(10)]
        public void ItShouldWaitAndRetry_withParameters_alwaysFails(int maxRetryCount)
        {
            //Arrange
            var policy = new WaitAndRetryPolicy();

            int expectedRetryCallCount = maxRetryCount;
            int expectedExecFuncCallCount = maxRetryCount + 1;

            Func<int, TimeSpan> retryDuration = waitTime => TimeSpan.FromMilliseconds(_waitTimeBetweenRetryAttempts);
            Action<Exception, TimeSpan> retryAction = (exception, timeSpan) => { _actualRetryCallCount++; };
            Action execFunc = () => { _actualExecFuncCallCount++;
                throw new WaitAndRetryPolicyException();
            };

            //Act 
            Assert.That(() => policy.WaitAndRetry<WaitAndRetryPolicyException>(maxRetryCount, retryDuration, retryAction, execFunc), 
                Throws.Exception.TypeOf<WaitAndRetryPolicyException>());

            //Assert
            Assert.That(_actualRetryCallCount, Is.EqualTo(expectedRetryCallCount));
            Assert.That(_actualExecFuncCallCount, Is.EqualTo(expectedExecFuncCallCount));
        }
        
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(10)]
        public void ItShouldWaitAndRetry_withConstructor_alwaysFails(int maxRetryCount)
        {
            //Arrange
            var policy = new WaitAndRetryPolicy(maxRetryCount, _waitTimeBetweenRetryAttempts);

            int expectedRetryCallCount = maxRetryCount;
            int expectedExecFuncCallCount = maxRetryCount + 1;

            Func<int, TimeSpan> retryDuration = waitTime => TimeSpan.FromMilliseconds(_waitTimeBetweenRetryAttempts);
            Action<Exception, TimeSpan> retryAction = (exception, timeSpan) => { _actualRetryCallCount++; };
            Action execFunc = () => {
                _actualExecFuncCallCount++;
                throw new WaitAndRetryPolicyException();
            };

            //Act 
            Assert.That(() => policy.WaitAndRetry<WaitAndRetryPolicyException>(retryDuration, retryAction, execFunc),
                Throws.Exception.TypeOf<WaitAndRetryPolicyException>());

            //Assert
            Assert.That(_actualRetryCallCount, Is.EqualTo(expectedRetryCallCount));
            Assert.That(_actualExecFuncCallCount, Is.EqualTo(expectedExecFuncCallCount));
        }
        
        [TestCase(2, 1)]
        [TestCase(3, 2)]
        [TestCase(10, 5)]
        public void ItShouldWaitAndRetry_withParameters_notAllFails(int maxRetryCount, int succesAfterRetryNum)
        {
            //Arrange
            var policy = new WaitAndRetryPolicy();

            int expectedRetryCallCount = succesAfterRetryNum;
            int expectedExecFuncCallCount = succesAfterRetryNum + 1;

            Func<int, TimeSpan> retryDuration = waitTime => TimeSpan.FromMilliseconds(_waitTimeBetweenRetryAttempts);
            Action<Exception, TimeSpan> retryAction = (exception, timeSpan) => { _actualRetryCallCount++; };
            Action execFunc = () => {
                _actualExecFuncCallCount++;
                if (_actualExecFuncCallCount <= succesAfterRetryNum)
                {
                    throw new WaitAndRetryPolicyException();
                }
            };

            //Act 
            policy.WaitAndRetry<WaitAndRetryPolicyException>(maxRetryCount, retryDuration, retryAction, execFunc);

            //Assert
            Assert.That(_actualRetryCallCount, Is.EqualTo(expectedRetryCallCount));
            Assert.That(_actualExecFuncCallCount, Is.EqualTo(expectedExecFuncCallCount));
        }

        [TestCase(2, 1)]
        [TestCase(3, 2)]
        [TestCase(10, 5)]
        public void ItShouldWaitAndRetry_withConstructor_notAllFails(int maxRetryCount, int succesAfterRetryNum)
        {
            //Arrange
            var policy = new WaitAndRetryPolicy(maxRetryCount, _waitTimeBetweenRetryAttempts);

            int expectedRetryCallCount = succesAfterRetryNum;
            int expectedExecFuncCallCount = succesAfterRetryNum + 1;

            Func<int, TimeSpan> retryDuration = waitTime => TimeSpan.FromMilliseconds(_waitTimeBetweenRetryAttempts);
            Action<Exception, TimeSpan> retryAction = (exception, timeSpan) => { _actualRetryCallCount++; };
            Action execFunc = () => {
                _actualExecFuncCallCount++;
                if (_actualExecFuncCallCount <= succesAfterRetryNum)
                {
                    throw new WaitAndRetryPolicyException();
                }
            };

            //Act 
            policy.WaitAndRetry<WaitAndRetryPolicyException>(retryDuration, retryAction, execFunc);

            //Assert
            Assert.That(_actualRetryCallCount, Is.EqualTo(expectedRetryCallCount));
            Assert.That(_actualExecFuncCallCount, Is.EqualTo(expectedExecFuncCallCount));
        }
    }
}
