﻿// <auto-generated />
namespace InterpolSystem.Data.Migrations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Metadata;
    using System;

    [DbContext(typeof(InterpolDbContext))]
    partial class InterpolDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("InterpolSystem.Data.Models.Article", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AuthorId");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(5000);

                    b.Property<DateTime>("PublishDate");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("Articles");
                });

            modelBuilder.Entity("InterpolSystem.Data.Models.Charges", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<int>("IdentityParticularsWantedId");

                    b.HasKey("Id");

                    b.HasIndex("IdentityParticularsWantedId");

                    b.ToTable("Charges");
                });

            modelBuilder.Entity("InterpolSystem.Data.Models.ChargesCountries", b =>
                {
                    b.Property<int>("ChargesId");

                    b.Property<int>("CountryId");

                    b.HasKey("ChargesId", "CountryId");

                    b.HasIndex("CountryId");

                    b.ToTable("ChargesCountries");
                });

            modelBuilder.Entity("InterpolSystem.Data.Models.Countinent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(2);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.HasKey("Id");

                    b.ToTable("Countinents");
                });

            modelBuilder.Entity("InterpolSystem.Data.Models.CountriesNationalitiesMissing", b =>
                {
                    b.Property<int>("CountryId");

                    b.Property<int>("IdentityParticularsMissingId");

                    b.HasKey("CountryId", "IdentityParticularsMissingId");

                    b.HasIndex("IdentityParticularsMissingId");

                    b.ToTable("CountriesNationalitiesMissing");
                });

            modelBuilder.Entity("InterpolSystem.Data.Models.CountriesNationalitiesWanted", b =>
                {
                    b.Property<int>("CountryId");

                    b.Property<int>("IdentityParticularsWantedId");

                    b.HasKey("CountryId", "IdentityParticularsWantedId");

                    b.HasIndex("IdentityParticularsWantedId");

                    b.ToTable("CountriesNationalitiesWanted");
                });

            modelBuilder.Entity("InterpolSystem.Data.Models.Country", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(2);

                    b.Property<int>("CountinentId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("CountinentId");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("InterpolSystem.Data.Models.IdentityParticularsMissing", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AllNames")
                        .HasMaxLength(100);

                    b.Property<DateTime>("DateOfBirth");

                    b.Property<DateTime>("DateOfDisappearance");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<int>("Gender");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<int?>("PhysicalDescriptionId");

                    b.Property<string>("PlaceOfBirth")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("PlaceOfDisappearance")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("PhysicalDescriptionId");

                    b.ToTable("IdentityParticularsMissing");
                });

            modelBuilder.Entity("InterpolSystem.Data.Models.IdentityParticularsWanted", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AllNames")
                        .HasMaxLength(100);

                    b.Property<DateTime>("DateOfBirth");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<int>("Gender");

                    b.Property<bool>("IsCaught");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<int?>("PhysicalDescriptionId");

                    b.Property<string>("PlaceOfBirth")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<decimal>("Reward");

                    b.HasKey("Id");

                    b.HasIndex("PhysicalDescriptionId");

                    b.ToTable("IdentityParticularsWanted");
                });

            modelBuilder.Entity("InterpolSystem.Data.Models.Language", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("Languages");
                });

            modelBuilder.Entity("InterpolSystem.Data.Models.LanguagesMissing", b =>
                {
                    b.Property<int>("LanguageId");

                    b.Property<int>("IdentityParticularsMissingId");

                    b.HasKey("LanguageId", "IdentityParticularsMissingId");

                    b.HasIndex("IdentityParticularsMissingId");

                    b.ToTable("LanguagesMissing");
                });

            modelBuilder.Entity("InterpolSystem.Data.Models.LanguagesWanted", b =>
                {
                    b.Property<int>("LanguageId");

                    b.Property<int>("IdentityParticularsWantedId");

                    b.HasKey("LanguageId", "IdentityParticularsWantedId");

                    b.HasIndex("IdentityParticularsWantedId");

                    b.ToTable("LanguagesWanted");
                });

            modelBuilder.Entity("InterpolSystem.Data.Models.LogEmployee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ActionName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("ControllerName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<DateTime>("Date");

                    b.Property<string>("ExceptionMessage");

                    b.Property<string>("ExceptionType");

                    b.Property<string>("IpAddress")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.ToTable("LogEmployees");
                });

            modelBuilder.Entity("InterpolSystem.Data.Models.PhysicalDescription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("EyeColor");

                    b.Property<int>("HairColor");

                    b.Property<double>("Height");

                    b.Property<string>("PictureUrl")
                        .IsRequired()
                        .HasMaxLength(2000);

                    b.Property<string>("ScarsOrDistinguishingMarks")
                        .HasMaxLength(100);

                    b.Property<double>("Weight");

                    b.HasKey("Id");

                    b.ToTable("PhysicalDescriptions");
                });

            modelBuilder.Entity("InterpolSystem.Data.Models.SubmitForm", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("IdentityParticularsMissingId");

                    b.Property<int?>("IdentityParticularsWantedId");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<byte[]>("PersonImage")
                        .HasMaxLength(2097152);

                    b.Property<string>("PoliceDepartment");

                    b.Property<string>("SenderEmail")
                        .IsRequired();

                    b.Property<int>("Status");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("SubmissionDate");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("IdentityParticularsMissingId");

                    b.HasIndex("IdentityParticularsWantedId");

                    b.HasIndex("UserId");

                    b.ToTable("SubmitForms");
                });

            modelBuilder.Entity("InterpolSystem.Data.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<DateTime>("DateOfBirth");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("InterpolSystem.Data.Models.Article", b =>
                {
                    b.HasOne("InterpolSystem.Data.Models.User", "Author")
                        .WithMany("Articles")
                        .HasForeignKey("AuthorId");
                });

            modelBuilder.Entity("InterpolSystem.Data.Models.Charges", b =>
                {
                    b.HasOne("InterpolSystem.Data.Models.IdentityParticularsWanted", "IdentityParticularsWanted")
                        .WithMany("Charges")
                        .HasForeignKey("IdentityParticularsWantedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("InterpolSystem.Data.Models.ChargesCountries", b =>
                {
                    b.HasOne("InterpolSystem.Data.Models.Charges", "Charges")
                        .WithMany("CountryWantedAuthorities")
                        .HasForeignKey("ChargesId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("InterpolSystem.Data.Models.Country", "Country")
                        .WithMany("ChargesCountries")
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("InterpolSystem.Data.Models.CountriesNationalitiesMissing", b =>
                {
                    b.HasOne("InterpolSystem.Data.Models.Country", "Country")
                        .WithMany("NationalitiesMissingPeople")
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("InterpolSystem.Data.Models.IdentityParticularsMissing", "IdentityParticularsMissing")
                        .WithMany("Nationalities")
                        .HasForeignKey("IdentityParticularsMissingId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("InterpolSystem.Data.Models.CountriesNationalitiesWanted", b =>
                {
                    b.HasOne("InterpolSystem.Data.Models.Country", "Country")
                        .WithMany("NationalitiesWantedPeople")
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("InterpolSystem.Data.Models.IdentityParticularsWanted", "IdentityParticularsWanted")
                        .WithMany("Nationalities")
                        .HasForeignKey("IdentityParticularsWantedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("InterpolSystem.Data.Models.Country", b =>
                {
                    b.HasOne("InterpolSystem.Data.Models.Countinent", "Countinent")
                        .WithMany("Countries")
                        .HasForeignKey("CountinentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("InterpolSystem.Data.Models.IdentityParticularsMissing", b =>
                {
                    b.HasOne("InterpolSystem.Data.Models.PhysicalDescription", "PhysicalDescription")
                        .WithMany()
                        .HasForeignKey("PhysicalDescriptionId");
                });

            modelBuilder.Entity("InterpolSystem.Data.Models.IdentityParticularsWanted", b =>
                {
                    b.HasOne("InterpolSystem.Data.Models.PhysicalDescription", "PhysicalDescription")
                        .WithMany()
                        .HasForeignKey("PhysicalDescriptionId");
                });

            modelBuilder.Entity("InterpolSystem.Data.Models.LanguagesMissing", b =>
                {
                    b.HasOne("InterpolSystem.Data.Models.IdentityParticularsMissing", "IdentityParticularsMissing")
                        .WithMany("SpokenLanguages")
                        .HasForeignKey("IdentityParticularsMissingId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("InterpolSystem.Data.Models.Language", "Language")
                        .WithMany("MissingPeopleLanguages")
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("InterpolSystem.Data.Models.LanguagesWanted", b =>
                {
                    b.HasOne("InterpolSystem.Data.Models.IdentityParticularsWanted", "IdentityParticularsWanted")
                        .WithMany("SpokenLanguages")
                        .HasForeignKey("IdentityParticularsWantedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("InterpolSystem.Data.Models.Language", "Language")
                        .WithMany("WantedPeopleLanguages")
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("InterpolSystem.Data.Models.SubmitForm", b =>
                {
                    b.HasOne("InterpolSystem.Data.Models.IdentityParticularsMissing", "MissingPerson")
                        .WithMany("SubmitedForms")
                        .HasForeignKey("IdentityParticularsMissingId");

                    b.HasOne("InterpolSystem.Data.Models.IdentityParticularsWanted", "WantedPerson")
                        .WithMany("SubmitedForms")
                        .HasForeignKey("IdentityParticularsWantedId");

                    b.HasOne("InterpolSystem.Data.Models.User", "User")
                        .WithMany("SubmitForms")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("InterpolSystem.Data.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("InterpolSystem.Data.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("InterpolSystem.Data.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("InterpolSystem.Data.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
