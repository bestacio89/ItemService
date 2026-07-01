using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;

namespace ItemService.Persistence.Persistence.Extensions;

public static class ModelBuilderExtensions
{
  public static PropertyBuilder<TProperty> HasJsonConversion<TEntity, TProperty>(
      this EntityTypeBuilder<TEntity> builder,
      Expression<Func<TEntity, TProperty>> propertyExpression)
      where TEntity : class
      where TProperty : class
  {
    return builder.Property(propertyExpression)
        .HasConversion(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<TProperty>(v, (JsonSerializerOptions?)null)!
        )
        .HasColumnType("jsonb");
  }
}