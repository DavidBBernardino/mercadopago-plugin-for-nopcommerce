﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using MercadoPagoSDK = MercadoPago;
using MercadoPago.Resources;
using MercadoPago.DataStructures.Preference;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Payments.MercadoPago.Models;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.MercadoPago.Components
{
    [ViewComponent(Name = "PaymentMercadoPago")]
    public class PaymentMercadoPagoViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var model = new PaymentInfoModel
            {
                CreditCardTypes = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Visa", Value = "visa" },
                    new SelectListItem { Text = "Master card", Value = "MasterCard" },
                    new SelectListItem { Text = "Discover", Value = "Discover" },
                    new SelectListItem { Text = "Amex", Value = "Amex" },
                }
            };

            MercadoPagoSDK.SDK.ClientId = "766960226806413";
            MercadoPagoSDK.SDK.ClientSecret = "Eq42zO6KIaC3SKjqGLsKKabypJTrCPnv";

            // Create a preference object
            Preference preference = new Preference();
            //# Adding an item object
            preference.Items.Add(
              new Item()
              {
                  Id = "1234",
                  Title = "Small Silk Plate",
                  Quantity = 5,
                  CurrencyId = MercadoPagoSDK.Common.CurrencyId.ARS,
                  UnitPrice = (float)44.23
              }
            );
            // Setting a payer object as value for Payer property
            preference.Payer = new Payer()
            {
                Name = "Charles",
                Surname = "Estévez",
                Email = "charles@yahoo.com",
                Phone = new Phone()
                {
                    AreaCode = "",
                    Number = "945-824-586"
                },
                Identification = new Identification()
                {
                    Type = "DNI",
                    Number = "12345678"
                },
                Address = new Address()
                {
                    StreetName = "Lado Mayte",
                    StreetNumber = int.Parse("1725"),
                    ZipCode = "21573"
                }
            };
            // Save and posting preference
            preference.Save();

            model.UrlCheckout = preference.InitPoint;
            //years
            for (var i = 0; i < 15; i++)
            {
                var year = (DateTime.Now.Year + i).ToString();
                model.ExpireYears.Add(new SelectListItem { Text = year, Value = year, });
            }

            //months
            for (var i = 1; i <= 12; i++)
            {
                model.ExpireMonths.Add(new SelectListItem { Text = i.ToString("D2"), Value = i.ToString(), });
            }

            //set postback values (we cannot access "Form" with "GET" requests)
            if (this.Request.Method != WebRequestMethods.Http.Get)
            {
                var form = this.Request.Form;
                model.CardNumber = form["CardNumber"];
                model.CardCode = form["CardCode"];
                var selectedCcType = model.CreditCardTypes.FirstOrDefault(x => x.Value.Equals(form["CreditCardType"], StringComparison.InvariantCultureIgnoreCase));
                if (selectedCcType != null)
                    selectedCcType.Selected = true;
                var selectedMonth = model.ExpireMonths.FirstOrDefault(x => x.Value.Equals(form["ExpireMonth"], StringComparison.InvariantCultureIgnoreCase));
                if (selectedMonth != null)
                    selectedMonth.Selected = true;
                var selectedYear = model.ExpireYears.FirstOrDefault(x => x.Value.Equals(form["ExpireYear"], StringComparison.InvariantCultureIgnoreCase));
                if (selectedYear != null)
                    selectedYear.Selected = true;
            }

            return View("~/Plugins/Payments.MercadoPago/Views/PaymentInfo.cshtml", model);
        }
    }
}
