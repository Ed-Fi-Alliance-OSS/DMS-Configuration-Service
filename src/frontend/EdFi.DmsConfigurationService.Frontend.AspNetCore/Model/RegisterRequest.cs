// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Net.Mail;
using System.Text.RegularExpressions;
using FluentValidation;

namespace EdFi.DmsConfigurationService.Frontend.AspNetCore.Model;

public class RegisterRequest
{
    public string? Username { get; set; }
    public string? EmailId { get; set; }
    public string? Password { get; set; }

    public class Validator : AbstractValidator<RegisterRequest>
    {
        public Validator()
        {
            RuleFor(m => m.Username).NotEmpty();

            RuleFor(m => m.Password).NotEmpty();
            RuleFor(m => m.Password)
            .Matches(new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,12}$"))
            .When(m => !string.IsNullOrEmpty(m.Password))
            .WithMessage("Password must contain at least one lowercase letter, one uppercase letter, one number, and one special character, and must be 8 to 12 characters long.");

            RuleFor(m => m.EmailId).NotEmpty();
            RuleFor(m => m.EmailId).Must(BeAValidEmailId)
                .When(m => !string.IsNullOrEmpty(m.EmailId))
                .WithMessage("Please provide valid email id");
        }

        private bool BeAValidEmailId(string? email)
        {
            try
            {
                if (email == null)
                {
                    return false;
                }
                MailAddress address = new(email);
                return !string.IsNullOrEmpty(address.Address) && address.Address == email;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}



