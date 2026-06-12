using Franz.Common.EntityFramework;
using Franz.Common.Mediator.Dispatchers;
using ItemService.Domain.Items.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ItemService.Persistence;

public sealed class ApplicationDbContext : DbContextBase
{
  public ApplicationDbContext(
      DbContextOptions<ApplicationDbContext> options,
      IDispatcher dispatcher)
      : base(options, dispatcher)
  {
  }

  public DbSet<Item> Items => Set<Item>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    ConfigureItem(modelBuilder);
  }

  private static void ConfigureItem(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Item>(builder =>
    {
      builder.ToTable("Items");

      builder.HasKey(x => x.Id);

      builder.Property(x => x.Name)
          .HasMaxLength(150)
          .IsRequired();

      builder.HasIndex(x => x.Name)
          .IsUnique();
      
      builder.Property(x => x.Category)
          .HasConversion<int>();

      builder.Property(x => x.Mythology)
          .HasConversion<int>();

      builder.Property(x => x.ClassAffinity)
          .HasConversion<int>();

      builder.Property(x => x.SignatureHeroId);

      ConfigureCondition(builder);

      ConfigureBaseStats(builder);
    });
  }

  private static void ConfigureCondition(
      EntityTypeBuilder<Item> builder)
  {
    builder.OwnsOne(x => x.Condition, condition =>
    {
      condition.Property(x => x.WindowSeconds);

      condition.Property(x => x.ConsecutiveEffectType);

      condition.Property(x => x.ConsecutiveCount);

      condition.Property(x => x.DistinctEffectCount);

      condition.Property(x => x.StackThreshold);

      condition.Property("_patternJson")
          .HasColumnName("Pattern");
    });
  }

  private static void ConfigureBaseStats(
      EntityTypeBuilder<Item> builder)
  {
    builder.OwnsOne(x => x.BaseStats, stats =>
    {
      stats.Property(x => x.ItemId);

      stats.OwnsOne(x => x.HeroModifier, hero =>
      {
        hero.Property(x => x.HealthBonus);
        hero.Property(x => x.ManaBonus);

        hero.Property(x => x.AttackDamageBonus);
        hero.Property(x => x.AbilityPowerBonus);

        hero.Property(x => x.AttackSpeedBonus);
        hero.Property(x => x.CastSpeedBonus);

        hero.Property(x => x.CritChanceBonus);
        hero.Property(x => x.CritDamageMultiplierBonus);

        hero.Property(x => x.ArmorBonus);
        hero.Property(x => x.MagicResistanceBonus);

        hero.Property(x => x.DamageReductionBonus);
        hero.Property(x => x.ShieldStrengthMultiplierBonus);

        hero.Property(x => x.MovementSpeedBonus);
        hero.Property(x => x.AttackRangeBonus);

        hero.Property(x => x.CooldownReductionBonus);
        hero.Property(x => x.ResourceRegenerationBonus);
      });

      stats.OwnsOne(x => x.SkillModifier, skill =>
      {
        skill.Property(x => x.CooldownReduction);

        skill.Property(x => x.ManaCostReduction);

        skill.Property(x => x.DamageBonus);

        skill.Property(x => x.HealingBonus);

        skill.Property(x => x.ShieldValueBonus);

        skill.Property(x => x.CastTimeReduction);

        skill.Property(x => x.ChannelDurationBonus);

        skill.Property(x => x.CrowdControlDurationBonus);

        skill.Property(x => x.RangeBonus);
      });
    });
  }
}