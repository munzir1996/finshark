using api.Data;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly ApplicationDBContext _context;
        public PortfolioRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Portfolio> CreateAsync(Portfolio portfolio)
        {
            await _context.Portfolios.AddAsync(portfolio);
            await _context.SaveChangesAsync();
            return portfolio;
        }

        public async Task<Portfolio> DeletePortfolio(AppUser appUser, string symbol)
        {
            var portfoilioModel = await _context.Portfolios.FirstOrDefaultAsync(portfoilio => portfoilio.AppUserId == appUser.Id && portfoilio.Stock.Symbol.ToLower() == symbol.ToLower());

            if (portfoilioModel == null)
            {
                return null;
            }

            _context.Portfolios.Remove(portfoilioModel);
            await _context.SaveChangesAsync();

            return portfoilioModel;
        }

        public async Task<List<Stock>> GetUserPortfolio(AppUser user)
        {
            return await _context.Portfolios.Where(portfolio => portfolio.AppUserId == user.Id)
                    .Select(stock => new Stock
                    {
                        Id = stock.StockId,
                        Symbol = stock.Stock.Symbol,
                        CompanyName = stock.Stock.CompanyName,
                        Purchase = stock.Stock.Purchase,
                        LastDiv = stock.Stock.LastDiv,
                        Industry = stock.Stock.Industry,
                        MarketCap = stock.Stock.MarketCap,
                    }).ToListAsync();
        }
        
    }
}
