﻿using BusinessObject.DTO;
using BusinessObject.Models;
using DataAccess.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class AccountDAO
    {
        public static async Task<Account> GetAccount(LoginDTO req)
        {
            Account? account;
            using (var context = new ClothesStoreDBContext())
            {
                account = await context.Accounts.Include(c => c.Customer).Include(c => c.Employee).Where(a => a.Email!.Equals(req.Email) && a.Password!.Equals(req.Password)).FirstOrDefaultAsync(); 
            }
            return account ?? new();
        }
        public static async Task<List<Account>> GetAccounts(string? searchString)
        {
            var listAccounts = new List<Account>();
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    listAccounts = await context.Accounts.Include(x => x.Employee).Include(x => x.Customer).ToListAsync();

                    if(searchString != null)
                    {
                        string txt = searchString.ToLower().Trim();
                        listAccounts = listAccounts.Where(x => x.Email.ToLower().Contains(txt)).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listAccounts;
        }

        public static async Task<Account> GetAccountById(int id)
        {
            Account? account = new Account();
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    account = await context.Accounts.Include(x => x.Employee).Include(x => x.Customer).SingleOrDefaultAsync(x => x.AccountId == id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return account;
        }

        public static async Task<Account> GetAccountByCustomer(string id)
        {
            Account? account = new Account();
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    account = await  context.Accounts.Include(x => x.Employee).Include(x => x.Customer).SingleOrDefaultAsync(x => x.CustomerId.Equals(id));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return account;
        }

        public static async Task<Account> GetAccountByEmail(string email)
        {
            Account? account = new Account();
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    account = await context.Accounts.Include(x => x.Employee).Include(x => x.Customer).SingleOrDefaultAsync(x => x.Email.Equals(email));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return account;
        }

        public static async Task DeleteAccount(int id)
        {
            try
            {
                Account? account = new Account();
                using (var context = new ClothesStoreDBContext())
                {
                    account = await context.Accounts.Include(x => x.Customer).Include(x => x.Employee).SingleOrDefaultAsync(x => x.AccountId == id);
                    if(account != null)
                    {
                        account.IsActive = false;
                        if (account.Role == 1)
                        {
                            account.Employee.IsActive = false;
                        }
                        if (account.Role == 2)
                        {
                            account.Customer.IsActive = false;
                        }
                        context.Entry<Account>(account).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task<string?> GetResetPassword(string? email)
        {
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    Account? account = new Account();
                    account = await context.Accounts.Include(x => x.Customer).Include(x => x.Employee).SingleOrDefaultAsync(x => x.Email == email);
                    if (account != null)
                    {
                        string newPassword = RandomUtils.GenerateNewPassword(8);
                        account.Password = newPassword;
                        context.Entry<Account>(account).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        await context.SaveChangesAsync();

                        return newPassword;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static async Task<bool> SaveCustomer(SignUpDTO req)
        {
            req.customer!.CustomerId = RandomUtils.RandomString(5);
            while (await CustomerDAO.GetCustomerById(req.customer.CustomerId) != null)
            {
                req.customer!.CustomerId = RandomUtils.RandomString(5);
            }
            using (var context = new ClothesStoreDBContext())
            {
                Account account = new Account
                {
                    Email = req.Email,
                    Password = req.Password,
                    CustomerId = req.customer!.CustomerId,
                    IsActive = true,
                    Customer = new Customer()
                    {
                        CustomerId = req.customer!.CustomerId,
                        CompanyName = req.customer!.CompanyName ?? "Notyet",
                        ContactName = req.customer!.ContactName,
                        ContactTitle = req.customer!.ContactTitle,
                        Address = req.customer!.Address,
                        IsActive = true,
                    },
                    Role = req.Role
                    
                };
                await context.Accounts.AddAsync(account);
                return await context.SaveChangesAsync() > 0;
            }
        }
        public static async Task<bool> UpdateCustomer(SignUpDTO req)
        {
            var cus = CustomerDAO.GetCustomerByIdNotTask(req.customer!.CustomerId);
            var acc = await GetAccountByCustomer(req.customer!.CustomerId);
            using (var context = new ClothesStoreDBContext())
            {

                acc.Email = req.Email;
                acc.Password = req.Password;                
                cus.CustomerId = req.customer!.CustomerId;
                cus.CompanyName = req.customer!.CompanyName ?? "Notyet";
                cus.ContactName = req.customer!.ContactName;
                cus.ContactTitle = req.customer!.ContactTitle;
                cus.Address = req.customer!.Address;
                context.Entry<Customer>(cus).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.Entry<Account>(acc).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                return await context.SaveChangesAsync() > 0;
            }
        }
/*        public static async Task<SignUpDTO> GetInfoCustomer(SignUpDTO req)
        {

            var acc = await GetAccountByCustomer(req.customer!.CustomerId);
            

                req.Email = acc.Email;
                req.Password = acc.Password;
                req.customer!.CustomerId = acc.Customer!.CustomerId;
                req.customer!.CompanyName = acc.Customer!.CompanyName  ?? "Notyet";
                req.customer!.ContactName = acc.Customer!.ContactName;
                req.customer!.ContactTitle = acc.Customer!.ContactTitle;
                req.customer!.Address = acc.Customer!.Address;
                return req;
            
        }*/
    }
}
