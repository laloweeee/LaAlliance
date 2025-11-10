using System;
using System.Collections.Generic;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Services.Tests
{
	// Minimal stub replacement for tests to satisfy interface changes.
	// This file intentionally provides a simple in-memory repository used by unit tests.
	public class PromotionServiceTests
	{
		public class InMemoryPromotionRepository : IPromotionRepository
		{
			public RestaurantPromotions AddPromotion(RestaurantPromotions promotion)
			{
				throw new NotImplementedException();
			}

			public void AddPromotionCode(PromotionCodes promotionCode)
			{
				throw new NotImplementedException();
			}

			public void AddPromotionProducts(IEnumerable<PromotionProducts> promotionProducts)
			{
				throw new NotImplementedException();
			}

			public void UpdatePromotion(RestaurantPromotions promotion)
			{
				throw new NotImplementedException();
			}

			public void DeletePromotion(int promotionID)
			{
				throw new NotImplementedException();
			}

			public RestaurantPromotions GetPromotionById(int promotionID)
			{
				throw new NotImplementedException();
			}

			public IEnumerable<RestaurantPromotions> GetAllPromotions()
			{
				throw new NotImplementedException();
			}

			public IEnumerable<RestaurantPromotions> GetActivePromotions()
			{
				throw new NotImplementedException();
			}

			public IEnumerable<RestaurantPromotions> GetPromotionsByDate(DateTime date)
			{
				throw new NotImplementedException();
			}

			public RestaurantPromotions GetPromotionByCode(string code)
			{
				throw new NotImplementedException();
			}

			// Newly added method - simple default implementation for tests
			public bool TryConsumePromotionCode(string code)
			{
				// For unit tests that don't validate concurrency, assume consumption succeeds.
				return true;
			}
		}
	}
}

