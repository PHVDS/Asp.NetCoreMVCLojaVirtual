using Coravel.Invocable;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Libraries.Gerenciador.Scheduler.Invocable
{
	public class PedidoPagamentoSituacao : IInvocable
	{
		public Task Invoke()
		{
			Debug.WriteLine("--Executado--");

			return Task.CompletedTask;
		}
	}
}
