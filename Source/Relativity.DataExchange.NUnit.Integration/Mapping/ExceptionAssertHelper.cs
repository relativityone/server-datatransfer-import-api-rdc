// <copyright file="ExceptionAssertHelper.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit.Integration.Mapping
{
	using System;
	using System.Web.Services.Protocols;

	using global::NUnit.Framework;

	internal static class ExceptionAssertHelper
	{
		internal static string CleanupText(string inputText)
		{
			if (string.IsNullOrEmpty(inputText))
			{
				return inputText;
			}

			return inputText
				.Replace("\r", string.Empty)
				.Replace("\n", string.Empty)
				.Replace("\\r", string.Empty)
				.Replace("\\n", string.Empty);
		}

		internal static void EnsureWebApiAndKeplerExceptionsAreTheSame(Exception webApiException, Exception keplerException)
		{
			Assert.NotNull(webApiException);
			Assert.NotNull(keplerException);
			Assert.AreEqual(CleanupText(webApiException.Message), CleanupText(keplerException.Message));
		}

		internal static void EnsureWebApiAndKeplerExceptionsAreTheSame(SoapException webApiException, SoapException keplerException)
		{
			Assert.NotNull(webApiException);
			Assert.NotNull(keplerException);

			EnsureSoapExceptionDetailValuesAreTheSame(webApiException, keplerException, "ExceptionType");
			EnsureSoapExceptionDetailValuesContainsTheSameText(webApiException, keplerException, "ExceptionMessage");
			EnsureSoapExceptionDetailValuesContainsTheSameText(webApiException, keplerException, "ExceptionFullText");

			Assert.True(keplerException.Message.Contains(webApiException.Message));
		}

		private static void EnsureSoapExceptionDetailValuesAreTheSame(SoapException webApiException, SoapException keplerException, string key)
		{
			var webApiValue = GetSoapExceptionDetailValue(webApiException, key);
			var keplerValue = GetSoapExceptionDetailValue(keplerException, key);
			Assert.AreEqual(webApiValue, keplerValue);
		}

		private static void EnsureSoapExceptionDetailValuesContainsTheSameText(SoapException webApiException, SoapException keplerException, string key)
		{
			var webApiValue = GetSoapExceptionDetailValue(webApiException, key);
			var keplerValue = GetSoapExceptionDetailValue(keplerException, key);
			Assert.True(webApiValue.Contains(keplerValue));
		}

		private static string GetSoapExceptionDetailValue(SoapException soapException, string key)
		{
			return soapException.Detail.SelectNodes(key).Item(0).InnerText;
		}
	}
}
