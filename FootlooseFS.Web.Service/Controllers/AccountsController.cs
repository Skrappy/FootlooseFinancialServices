﻿using FootlooseFS.Models;
using FootlooseFS.Service;
using FootlooseFS.DataPersistence;
using FootlooseFS.Web.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace FootlooseFS.Web.Service.Controllers
{
    public class AccountsController : FootloseFSApiController
    {
        public AccountsController(IFootlooseFSService service) : base(service) { }

        // GET api/accounts
        public HolderAccountsViewModel Get()
        {
            // Get Person data model from the data service
            var person = service.GetPerson(authenticatedUser, new PersonIncludes { Accounts = true, Addressses = false, Phones = false, AccountTransactions = true });

            // Create a Holder view model and populate data from Person data model
            var holderAccounts = new HolderAccountsViewModel();

            // Get the holder's accounts
            holderAccounts.Accounts = (from a in person.Accounts
                               select new AccountViewModel
                               {
                                   AccountID = a.AccountID,
                                   AccountBalance = a.Account.AccountBalance,
                                   AccountName = a.Account.AccountName,
                                   AccountType = a.Account.AccountType.Name
                               });

            // Calculate the total
            holderAccounts.Total = (from a in holderAccounts.Accounts select a.AccountBalance).Sum();

            // Ge thte holder's account transactions
            holderAccounts.Transactions = (from a in person.Accounts
                                   from t in a.Account.Transactions
                                   orderby t.Date descending
                                   select new TransactionViewModel
                                   {
                                       AccountID = a.AccountID,
                                       AccountName = a.Account.AccountName,
                                       Date = t.Date,
                                       TransactionType = t.TransactionType.Name,
                                       Amount = t.Amount
                                   });

            return holderAccounts;
        }
    }
}
