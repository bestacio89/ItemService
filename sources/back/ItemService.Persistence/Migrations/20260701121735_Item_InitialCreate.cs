using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItemService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Item_InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    Mythology = table.Column<int>(type: "integer", nullable: false),
                    ClassAffinity = table.Column<int>(type: "integer", nullable: false),
                    SignatureHeroId = table.Column<Guid>(type: "uuid", nullable: true),
                    Condition = table.Column<string>(type: "jsonb", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemBaseStats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    HeroModifier_HealthBonus = table.Column<float>(type: "real", nullable: true),
                    HeroModifier_ManaBonus = table.Column<float>(type: "real", nullable: true),
                    HeroModifier_AttackDamageBonus = table.Column<float>(type: "real", nullable: true),
                    HeroModifier_AbilityPowerBonus = table.Column<float>(type: "real", nullable: true),
                    HeroModifier_AttackSpeedBonus = table.Column<float>(type: "real", nullable: true),
                    HeroModifier_CastSpeedBonus = table.Column<float>(type: "real", nullable: true),
                    HeroModifier_CritChanceBonus = table.Column<float>(type: "real", nullable: true),
                    HeroModifier_CritDamageMultiplierBonus = table.Column<float>(type: "real", nullable: true),
                    HeroModifier_ArmorBonus = table.Column<float>(type: "real", nullable: true),
                    HeroModifier_MagicResistanceBonus = table.Column<float>(type: "real", nullable: true),
                    HeroModifier_DamageReductionBonus = table.Column<float>(type: "real", nullable: true),
                    HeroModifier_ShieldStrengthMultiplierBonus = table.Column<float>(type: "real", nullable: true),
                    HeroModifier_MovementSpeedBonus = table.Column<float>(type: "real", nullable: true),
                    HeroModifier_AttackRangeBonus = table.Column<float>(type: "real", nullable: true),
                    HeroModifier_CooldownReductionBonus = table.Column<float>(type: "real", nullable: true),
                    HeroModifier_ResourceRegenerationBonus = table.Column<float>(type: "real", nullable: true),
                    SkillModifier_CooldownReduction = table.Column<float>(type: "real", nullable: true),
                    SkillModifier_ManaCostReduction = table.Column<float>(type: "real", nullable: true),
                    SkillModifier_DamageBonus = table.Column<float>(type: "real", nullable: true),
                    SkillModifier_HealingBonus = table.Column<float>(type: "real", nullable: true),
                    SkillModifier_ShieldValueBonus = table.Column<float>(type: "real", nullable: true),
                    SkillModifier_CastTimeReduction = table.Column<float>(type: "real", nullable: true),
                    SkillModifier_ChannelDurationBonus = table.Column<float>(type: "real", nullable: true),
                    SkillModifier_CrowdControlDurationBonus = table.Column<float>(type: "real", nullable: true),
                    SkillModifier_RangeBonus = table.Column<float>(type: "real", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemBaseStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemBaseStats_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemBaseStats_ItemId",
                table: "ItemBaseStats",
                column: "ItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_Name",
                table: "Items",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemBaseStats");

            migrationBuilder.DropTable(
                name: "Items");
        }
    }
}
