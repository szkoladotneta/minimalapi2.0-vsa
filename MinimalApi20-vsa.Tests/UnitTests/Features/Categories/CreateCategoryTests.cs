using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MinimalApi20_vsa.Api.Common.Models;
using MinimalApi20_vsa.Api.Domain;
using MinimalApi20_vsa.Api.Domain.Entities;
using MinimalApi20_vsa.Api.Features.Categories;
using Moq;
using Moq.EntityFrameworkCore;
using Xunit;

namespace MinimalApi20_vsa.Tests.UnitTests.Features.Categories;

[Trait("Category", TestCategories.Unit)]
public class CreateCategoryTests
{
    [Fact]
    public void Validator_ShouldHaveError_WhenNameIsEmpty()
    {
        // Arrange
        var validator = new CreateCategory.Validator();
        var request = new CreateCategory.Request("", "Description");
        
        // Act
        var result = validator.TestValidate(request);
        
        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Name);
    }
    
    [Fact]
    public void Validator_ShouldHaveError_WhenDescriptionIsEmpty()
    {
        // Arrange
        var validator = new CreateCategory.Validator();
        var request = new CreateCategory.Request("Name", "");
        
        // Act
        var result = validator.TestValidate(request);
        
        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Description);
    }
    
    [Fact]
    public void Validator_ShouldNotHaveError_WhenRequestIsValid()
    {
        // Arrange
        var validator = new CreateCategory.Validator();
        var request = new CreateCategory.Request("Name", "Description");
        
        // Act
        var result = validator.TestValidate(request);
        
        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
