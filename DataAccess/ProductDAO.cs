﻿using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class ProductDAO
    {
        public static async Task<List<Product>> GetProducts()
        {
            var listProducts = new List<Product>();
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    listProducts = await context.Products.Where(x => x.IsActive == true).Include(x => x.Category).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listProducts;
        }

        public static async Task<List<Product>> GetProducts(int? categoryId, string? text,string? sortType, decimal? startPrice, decimal? endPrice, bool? isAdmin)
        {
            var listProducts = new List<Product>();
/*            if (string.IsNullOrEmpty(text))
            {
                throw new ApplicationException("Search keyword is empty!!");
            }*/
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    listProducts = await context.Products.Include(x => x.Category).ToListAsync();

                    if (categoryId != null)
                        listProducts =  listProducts.Where(x => x.CategoryId == categoryId).ToList();
                    if(text != null) 
                        listProducts = listProducts.Where(x => x.ProductName.ToLower().Contains(text.ToLower().Trim())).ToList();
                    if (sortType != null)
                    {
                        switch (sortType)
                        {
                            case "name-asc":
                                listProducts = listProducts.OrderBy(x => x.ProductName).ToList();
                                break;
                            case "name-desc":
                                listProducts = listProducts.OrderByDescending(x => x.ProductName).ToList();
                                break;
                            case "price-asc":
                                listProducts = listProducts.OrderBy(x => x.UnitPrice).ToList();
                                break;
                            case "price-desc":
                                listProducts = listProducts.OrderByDescending(x => x.UnitPrice).ToList();
                                break;
                            default:
                                listProducts = listProducts.OrderBy(x => x.ProductName).ToList();
                                break;
                        }
                    }
                    if(startPrice != null)
                        listProducts = listProducts.Where(x => x.UnitPrice >= startPrice).ToList();
                    if (endPrice != null)
                        listProducts = listProducts.Where(x => x.UnitPrice <= endPrice).ToList();
                    if(isAdmin != true)
                        listProducts = listProducts.Where(x => x.IsActive == true).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listProducts;
        }

/*        public static async Task<List<Product>> GetProductsTop3Best(String text)
        {
            var listProducts = new List<Product>();
            if (string.IsNullOrEmpty(text))
            {
                throw new ApplicationException("Search keyword is empty!!");
            }
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    listProducts = await context.Products.Include(x => x.Category).Where(x => x.ProductName
                    .ToLower().Contains(text.ToLower())).ToListAsync();
                }
                foreach (var product in listProducts)
                    
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listProducts;
        }*/

        public static async Task<List<Product>> GetProductsByCategory(int catId)
        {
            var listProducts = new List<Product>();
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    listProducts = await context.Products.Where(x => x.CategoryId == catId && x.IsActive == true).Include(x => x.Category).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listProducts;
        }

        public static async Task<List<Product>> GetProductsByCategoryGeneral(string catGeneral)
        {
            var listProducts = new List<Product>();
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    listProducts = await context.Products.Where(x => x.Category.CategoryGeneral == catGeneral && x.IsActive == true).Include(x => x.Category).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listProducts;
        }

        public static async Task<Product> GetProductById(int id)
        {
            Product product = new Product();
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    product = await context.Products.Include(x => x.Category).SingleOrDefaultAsync(x => x.ProductId == id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return product;
        }

        public static async Task<Product> CreateProduct(Product product)
        {
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    await context.Products.AddAsync(product);
                    await context.SaveChangesAsync();

                    return await context.Products.Include(x => x.Category).SingleOrDefaultAsync(x => x.ProductId == product.ProductId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task<bool> CreateProductMany(List<Product> listProducts)
        {
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    await context.Products.AddRangeAsync(listProducts);
                    return await context.SaveChangesAsync() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task<Product> UpdateProduct(Product product)
        {
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    context.Entry<Product>(product).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    await context.SaveChangesAsync();

                    return await context.Products.Include(x => x.Category).SingleOrDefaultAsync(x => x.ProductId == product.ProductId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task DeleteProduct(int id)
        {
            try
            {
                Product product = new Product();
                using (var context = new ClothesStoreDBContext())
                {
                    product = await context.Products.Include(x => x.Category).SingleOrDefaultAsync(x => x.ProductId == id);
                    product.IsActive = false;
                    context.Entry<Product>(product).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
