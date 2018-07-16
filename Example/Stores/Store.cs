using System;

namespace HostBridge.Stores
{
	public abstract class Store
	{
		public event Action Change;

		protected void OnChange() => Change?.Invoke();
	}
}