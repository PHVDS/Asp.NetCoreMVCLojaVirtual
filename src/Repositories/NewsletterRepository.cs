using LojaVirtual.Database;
using LojaVirtual.Models;
using LojaVirtual.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Repositories
{
	public class NewsletterRepository : INewsletterRepository
	{
		private readonly LojaVirtualContext _banco;

		public NewsletterRepository(LojaVirtualContext banco)
		{
			_banco = banco;
		}

		public void Cadastrar(NewsletterEmail newsletterEmail)
		{
			_banco.NewsletterEmails.Add(newsletterEmail);
			_banco.SaveChanges();
		}

		public IEnumerable<NewsletterEmail> ObterTodosNewsletter()
		{
			return _banco.NewsletterEmails.ToList();
		}

		public int QuantidadeTotalNewsletters()
		{
			return _banco.NewsletterEmails.Count();
		}
	}
}
