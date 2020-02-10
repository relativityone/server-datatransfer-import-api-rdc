// <copyright file="JwtTokenHelperTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit.Helpers
{
	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.Helpers;

	public class JwtTokenHelperTests
	{
		[TestCase("eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IjE3Nl9wRkNtNUwydjc4Q0xZN1FTeDhZREpSZyIsImtpZCI6IkQ3QkVCRkE0NTBBNkU0QkRBRkVGQzA4QjYzQjQxMkM3QzYwMzI1MTgifQ.eyJp"
		          + "c3MiOiJSZWxhdGl2aXR5IiwiYXVkIjoiUmVsYXRpdml0eS9yZXNvdXJjZXMiLCJleHAiOjE2MTE3NTE2MjEsIm5iZiI6MTU4MDk5MzIyMSwiY2xpZW50X2lkIjoiYWVhOGNmNDgtMzdkMC00MGJkLWEyY2UtYz"
		          + "QwZTg2NTRkMGM1Iiwib3JfbGIiOiJUcnVlIiwicmVsX2lucyI6IjM4NTdFQ0U0LUZDMUUtNDM0QS04RkM0LUE3QjJGQTIxQUU3MCIsInJlbF91YWkiOiI3NzciLCJyZWxfdWZuIjoiUmVsYXRpdml0eSIsInJl"
		          + "bF91bG4iOiJTZXJ2aWNlIEFjY291bnQiLCJyZWxfdW4iOiJyZWxhdGl2aXR5LnNlcnZpY2VhY2NvdW50QGtjdXJhLmNvbSIsInNjb3BlIjoiU3lzdGVtVXNlckluZm8iLCJyZWxfb3JpZ2luIjoiOjoxIn0.EM"
		          + "-xUNsXNI7f2IakptZH9UPo9UN-wghvgmhk-73I2mAZrWPAFcLuOCsUywbeGcBil5RgEoW95u-NAWUr20rBYqDJXtTL6NYu10chWAbfdiU-_iSI9ThNV9e2rQgjnUkORKogaCgJLn_90gQYPxhY-6fMhRMBAJuO"
		          + "v3sxLVoz0MFKGCmcfypDFYjiAht6nWIeBPdgl39pFJ_gSd1HsGnXCChR6vdNbPE-psX9keCQ6dX0Hw1NJmwQuhUaP3A8TtvWwP7lgl7Nto5ojkbC-G4O700304IplSXUBF5bXP2R7t1-9U5zvf5ty9w6kskJItq"
		          + "NlbfA6P7bppFuuVdwPilVVA", "3857ECE4-FC1E-434A-8FC4-A7B2FA21AE70", "777")]
		[TestCase("eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ijk5ZjdlUGhRa3drZ1NXS1BBTDNYczNOM2NyYyIsImtpZCI6IkY3RDdGQjc4Rjg1MDkzMDkyMDQ5NjI4RjAwQkREN0IzNzM3NzcyQjcifQ.eyJpc"
		          + "3MiOiJSZWxhdGl2aXR5IiwiYXVkIjoiUmVsYXRpdml0eS9yZXNvdXJjZXMiLCJleHAiOjE1ODEyMDg3MDIsIm5iZiI6MTU4MTA2NDcwMiwiY2xpZW50X2lkIjoiUmVsYXRpdml0eSBEZXNrdG9wIENsaWVudCIs"
		          + "InNjb3BlIjoiVXNlckluZm9BY2Nlc3MiLCJzdWIiOiJhcnR1ci5taXRpYW5pZWNAcmVsYXRpdml0eS5jb20iLCJhdXRoX3RpbWUiOjE1ODEwNjQ3MDIsImlkcCI6Imlkc3J2IiwicmVsX3VhaSI6Ijc5NjkzMzM"
		          + "wIiwicmVsX3VmbiI6IkFydHVyIiwicmVsX3VsbiI6Ik1pdGlhbmllYyIsInJlbF91biI6ImFydHVyLm1pdGlhbmllY0ByZWxhdGl2aXR5LmNvbSIsIm9yX2xiIjoiRmFsc2UiLCJyZWxfaW5zIjoiQjI3RDE3RD"
		          + "ctQTJEMi00NUZDLUExOEYtQTg2NENEMEY1QkYwIiwicmVsX29yaWdpbiI6IjE3Ni4xMTUuOS4xMDg6NjA3OTksIDEwLjE0LjEyOS4yMDo0MDg3NCIsImFtciI6WyJQYXNzd29yZCJdfQ.nFn-drhNqSnwrE9oBR"
		          + "zBfY_ekRJY8QHPKdxNs1rqRwD3viOwO_URFwqgHbE5T2Hijn4ycM74irEiBCwpYn6VZ3laeQyQPGvDOGwdSRfStb877NCL8xpsnkg-W6cAR5jCUziV0uDlj-Ff5BqmCgtmTerVwH5lbCMatqgVsL3CWBF6EpUvf"
		          + "4Y2lcoNQF7X1A4Zo6ualTiyrs5HEG-Rrbuh_CNwtyE_mgVNnkRt5chEkOSDboXnXdZdgAaiLiA82zqnpLAV8e-rl3A47DNcN3P8OiEfS8mqMRVGCfYCgLkDn6xO5CjLVaS4PmhlLivYfWxT3dNxJFNrySmqXKVR"
		          + "Y5a_Hg", "B27D17D7-A2D2-45FC-A18F-A864CD0F5BF0", "79693330")]
		public void ValidateCorrectToken(string token, string expectedInstanceId, string expectedUserId)
		{
			bool result = JwtTokenHelper.TryParse(token, out var jwtTokenHelper);

			Assert.That(result);
			Assert.That(jwtTokenHelper.RelativityInstanceId, Is.EqualTo(expectedInstanceId));
			Assert.That(jwtTokenHelper.RelativityUserId, Is.EqualTo(expectedUserId));
		}

		[TestCase(null)]
		[TestCase("")]
		[TestCase("1")]
		[TestCase("1.1")]
		[TestCase("1.1.1")]

		// Token the does not have valid json in Payload part:
		[TestCase("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.YmxlYmxlCg==.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c")]

		// Token the does not have rel_uai & rel_ins claims:
		[TestCase("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c")]
		public void ValidateInCorrectToken(string token)
		{
			bool result = JwtTokenHelper.TryParse(token, out var jwtTokenHelper);

			Assert.False(result);
			Assert.That(jwtTokenHelper.RelativityInstanceId, Is.Null);
			Assert.That(jwtTokenHelper.RelativityUserId, Is.Null);
		}
	}
}
