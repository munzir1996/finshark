﻿using api.Data;
using api.Dtos.Stock;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDBContext _context;
        public StockRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Stock> CreateAsync(Stock stockModel)
        {
            await _context.Stock.AddAsync(stockModel);
            await _context.SaveChangesAsync();

            return stockModel;
        }

        public async Task<Stock?> DeleteAsync(int id)
        {
            var stockModel = await _context.Stock.FirstOrDefaultAsync(stock => stock.Id == id);

            if (stockModel == null)
            {
                return null;
            }

            _context.Stock.Remove(stockModel);
            await _context.SaveChangesAsync();

            return stockModel;
        }

        public async Task<List<Stock>> GetAllAsync(QueryObject query)
        {
            var stocks = _context.Stock.Include(i => i.Comments).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.CompanyName))
            {
                stocks = stocks.Where(stock => stock.CompanyName.Contains(query.CompanyName));
            }

            if (!string.IsNullOrWhiteSpace(query.Symbol))
            {
                stocks = stocks.Where(stock => stock.Symbol.Contains(query.Symbol));
            }

            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
                {
                    stocks = query.IsDecsending ? stocks.OrderByDescending(stock => stock.Symbol) : stocks.OrderBy(stock => stock.Symbol);
                }
            }

            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            return await stocks.Skip(skipNumber).Take(query.PageSize).ToListAsync();
        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            return await _context.Stock.Include(i => i.Comments).FirstOrDefaultAsync(stock => stock.Id == id);
        }

        public async Task<bool> StockExist(int id)
        {
            return await _context.Stock.AnyAsync(stock => stock.Id == id);
        }

        public async Task<Stock?> UpdateAsync(int id, UpdateStockRequestDto updateStockRequestDto)
        {
            var existingStock = await _context.Stock.FirstOrDefaultAsync(stock => stock.Id == id);

            if (existingStock == null)
            {
                return null;
            }

            existingStock.Symbol = updateStockRequestDto.Symbol;
            existingStock.CompanyName = updateStockRequestDto.CompanyName;
            existingStock.Purchase = updateStockRequestDto.Purchase;
            existingStock.LastDiv = updateStockRequestDto.LastDiv;
            existingStock.Industry = updateStockRequestDto.Industry;
            existingStock.MarketCap = updateStockRequestDto.MarketCap;

            await _context.SaveChangesAsync();

            return existingStock;
        }
    }
}
