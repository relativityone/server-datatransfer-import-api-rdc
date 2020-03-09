// <copyright file="StackOfDisposables.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
namespace Relativity.DataExchange.Logging
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// This class will dispose pushed IDisposable objects in LIFO manner.
	/// </summary>
	internal class StackOfDisposables : IDisposable
	{
		private readonly Stack<IDisposable> disposables = new Stack<IDisposable>();

		/// <summary>
		/// Finalizes an instance of the <see cref="StackOfDisposables"/> class.
		/// </summary>
		~StackOfDisposables()
		{
			this.Dispose(false);
		}

		/// <summary>
		/// push the item at the stack.
		/// </summary>
		/// <param name="disposable">disposable object.</param>
		public void Push(IDisposable disposable)
		{
			this.disposables.Push(disposable);
		}

		/// <summary>
		/// Dispose all objects in the stack.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				while (this.disposables.Count > 0)
				{
					IDisposable disposable = this.disposables.Pop();
					disposable?.Dispose();
				}
			}
		}
	}
}
