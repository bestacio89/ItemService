using Franz.Common.EntityFramework;
using Franz.Common.Mediator.Dispatchers;
using ItemService.Domain.Core;
using ItemService.Domain.Items.Core;
using ItemService.Persistence.Persistence.Extensions;
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

  public DbSet<ItemEntity> Items => Set<ItemEntity>();
  public DbSet<ItemBaseStats> ItemBaseStats => Set<ItemBaseStats>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    ConfigureItem(modelBuilder);
    ConfigureItemBaseStats(modelBuilder);
  }

  // =========================================================
  // ITEM
  // =========================================================
  private static void ConfigureItem(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<ItemEntity>(builder =>
    {
      builder.ToTable("Items");

      builder.HasKey(x => x.Id);

      builder.Property(x => x.Name)
          .HasMaxLength(150)
          .IsRequired();

      builder.HasIndex(x => x.Name)
          .IsUnique();

      builder.Property(x => x.Category).HasConversion<int>();
      builder.Property(x => x.Mythology).HasConversion<int>();
      builder.Property(x => x.ClassAffinity).HasConversion<int>();

      builder.Property(x => x.SignatureHeroId);

      // 1:1 relationship (Item is principal)
      builder.HasOne(x => x.BaseStats)
          .WithOne()
          .HasForeignKey<ItemBaseStats>(x => x.ItemId)
          .OnDelete(DeleteBehavior.Cascade)
          .IsRequired();

      // JSON value object (safe to keep here)
      builder.HasJsonConversion<ItemEntity, ItemBehaviouralCondition>(
          x => x.Condition);
    });
  }

  // =========================================================
  // ITEM BASE STATS (REAL ENTITY)
  // =========================================================
  private static void ConfigureItemBaseStats(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<ItemBaseStats>(builder =>
    {
      builder.ToTable("ItemBaseStats");

      builder.HasKey(x => x.Id);

      builder.Property(x => x.ItemId)
          .IsRequired();

      builder.HasIndex(x => x.ItemId)
          .IsUnique();

      // HERO MODIFIER (owned value object)
      builder.OwnsOne(x => x.HeroModifier, hero =>
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

      // SKILL MODIFIER (owned value object)
      builder.OwnsOne(x => x.SkillModifier, skill =>
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