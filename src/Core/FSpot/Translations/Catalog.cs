
using System.Resources;

namespace FSpot.Translations
{
	public static class Catalog
	{
		public static string GetPluralString (string singular, string plural, int count)
		{
			return (count > 1) ? plural : singular;
		}

		// TODO: Maybe lookup for random strings?????
		/// <summary>
		/// Currently just returns the same string
		/// </summary>
		/// <param name="_string"></param>
		/// <returns></returns>
		public static string GetString (string _string)
		{
			return _string;
		}
	}
}
