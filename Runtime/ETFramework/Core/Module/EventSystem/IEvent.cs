using System;

namespace ET
{
	public interface IEvent
	{
		Type Type { get; }
	}
	
	public abstract class AEvent<A>: IEvent where A: struct
	{
		public Type Type
		{
			get
			{
				return typeof (A);
			}
		}

		protected abstract ETTask Run(Scene scene, A eventMsg);

		public async ETTask Handle(Scene scene, A eventMsg)
		{
			try
			{
				await Run(scene, eventMsg);
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}
	}
}