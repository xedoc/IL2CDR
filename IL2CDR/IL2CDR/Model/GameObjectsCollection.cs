using System.Collections.Generic;

namespace IL2CDR.Model
{
	public class GameObjectsCollection : Dictionary<int, GameObject>
	{
		private readonly object lockList = new object();

		public new GameObject this[int id]
		{
			get
			{
				GameObject result;
				lock (this.lockList) {
					this.TryGetValue(id, out result);
				}

				return result;
			}
			set
			{
				if (value == null || value.Id <= 0) {
					return;
				}

				lock (this.lockList) {
					var existing = this[value.Id];
					if (existing != null) {
						this.Remove(value.Id);
					}

					this.Add(value.Id, value);
				}
			}
		}

		public void ObjectDestroyed(int id)
		{
			var gameObject = this[id];
			if (gameObject == null) {
				return;
			}

			gameObject.IsDestroyed = true;

			//TODO: handle object destruction
		}
	}
}