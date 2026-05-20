IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [FirstName] nvarchar(100) NOT NULL,
    [LastName] nvarchar(100) NOT NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    [MustChangePassword] bit NOT NULL DEFAULT CAST(1 AS bit),
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Branches] (
    [Id] int NOT NULL IDENTITY,
    [Guid] uniqueidentifier NOT NULL,
    [Code] nvarchar(20) NOT NULL,
    [Name] nvarchar(200) NOT NULL,
    [DisplayName] nvarchar(250) NULL,
    [Description] nvarchar(max) NULL,
    [IsActive] bit NOT NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAt] datetime2 NULL,
    [DeletedBy] nvarchar(100) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(100) NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_Branches] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [StudyPrograms] (
    [Id] int NOT NULL IDENTITY,
    [Guid] uniqueidentifier NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [ShortName] nvarchar(max) NOT NULL,
    [Level] nvarchar(max) NOT NULL,
    [DurationInYears] int NOT NULL,
    [TotalSemesters] int NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedByUserId] nvarchar(max) NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedByUserId] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAt] datetime2 NULL,
    [DeletedByUserId] nvarchar(max) NULL,
    CONSTRAINT [PK_StudyPrograms] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Subjects] (
    [Id] int NOT NULL IDENTITY,
    [Guid] uniqueidentifier NOT NULL,
    [Code] nvarchar(20) NOT NULL,
    [Name] nvarchar(250) NOT NULL,
    [DisplayName] nvarchar(100) NULL,
    [Description] nvarchar(500) NULL,
    [Credits] int NOT NULL,
    [Type] nvarchar(30) NOT NULL,
    [IsElective] bit NOT NULL DEFAULT CAST(0 AS bit),
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedBy] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAt] datetime2 NULL,
    [DeletedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Subjects] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Universities] (
    [Id] int NOT NULL IDENTITY,
    [Guid] uniqueidentifier NOT NULL,
    [Code] nvarchar(50) NOT NULL,
    [Name] nvarchar(250) NOT NULL,
    [DisplayName] nvarchar(250) NULL,
    [Address] nvarchar(500) NULL,
    [City] nvarchar(150) NULL,
    [State] nvarchar(150) NULL,
    [Country] nvarchar(3) NULL,
    [PostalCode] nvarchar(30) NULL,
    [ContactEmail] nvarchar(254) NULL,
    [ContactPhone] nvarchar(30) NULL,
    [WebsiteUrl] nvarchar(500) NULL,
    [AccreditationBody] nvarchar(200) NULL,
    [Status] nvarchar(50) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedByUserId] nvarchar(450) NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedByUserId] nvarchar(450) NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [DeletedAt] datetime2 NULL,
    [DeletedByUserId] nvarchar(450) NULL,
    [RowVersion] rowversion NULL,
    CONSTRAINT [PK_Universities] PRIMARY KEY ([Id]),
    CONSTRAINT [AK_Universities_Code] UNIQUE ([Code])
);
GO

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [RefreshTokens] (
    [Id] uniqueidentifier NOT NULL,
    [TokenHash] nvarchar(512) NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [ExpiresAt] datetime2 NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [Revoked] bit NOT NULL,
    [ReplacedByTokenHash] nvarchar(max) NULL,
    CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RefreshTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Colleges] (
    [Id] int NOT NULL IDENTITY,
    [Guid] uniqueidentifier NOT NULL,
    [UniversityCode] nvarchar(50) NOT NULL,
    [Code] nvarchar(50) NOT NULL,
    [Name] nvarchar(250) NOT NULL,
    [DisplayName] nvarchar(max) NULL,
    [Address] nvarchar(max) NULL,
    [City] nvarchar(max) NULL,
    [State] nvarchar(max) NULL,
    [Country] nvarchar(max) NULL,
    [PostalCode] nvarchar(max) NULL,
    [ContactEmail] nvarchar(max) NULL,
    [ContactPhone] nvarchar(max) NULL,
    [WebsiteUrl] nvarchar(max) NULL,
    [Status] nvarchar(50) NOT NULL DEFAULT N'Active',
    [CreatedAt] datetime2 NOT NULL,
    [CreatedByUserId] nvarchar(max) NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedByUserId] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAt] datetime2 NULL,
    [DeletedByUserId] nvarchar(max) NULL,
    [RowVersion] rowversion NULL,
    [UniversityId] int NULL,
    CONSTRAINT [PK_Colleges] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Colleges_Universities_UniversityCode] FOREIGN KEY ([UniversityCode]) REFERENCES [Universities] ([Code]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Colleges_Universities_UniversityId] FOREIGN KEY ([UniversityId]) REFERENCES [Universities] ([Id])
);
GO

CREATE TABLE [AcademicMaps] (
    [Id] int NOT NULL IDENTITY,
    [Guid] uniqueidentifier NOT NULL,
    [CollegeId] int NOT NULL,
    [StudyProgramId] int NOT NULL,
    [BranchId] int NOT NULL,
    [Regulation] nvarchar(20) NOT NULL,
    [AliasCode] nvarchar(100) NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedBy] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAt] datetime2 NULL,
    [DeletedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_AcademicMaps] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AcademicMaps_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_AcademicMaps_Colleges_CollegeId] FOREIGN KEY ([CollegeId]) REFERENCES [Colleges] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_AcademicMaps_StudyPrograms_StudyProgramId] FOREIGN KEY ([StudyProgramId]) REFERENCES [StudyPrograms] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Students] (
    [Id] int NOT NULL IDENTITY,
    [Guid] uniqueidentifier NOT NULL,
    [ApplicationUserId] nvarchar(450) NOT NULL,
    [RegistrationNumber] nvarchar(30) NOT NULL,
    [Batch] nvarchar(15) NOT NULL,
    [AcademicAliasCode] nvarchar(100) NOT NULL,
    [AcademicMapId] int NOT NULL,
    [IsActive] bit NOT NULL,
    [FatherName] nvarchar(100) NOT NULL,
    [DateOfBirth] datetime2 NOT NULL,
    [Gender] nvarchar(15) NOT NULL,
    [ContactNumber] nvarchar(20) NULL,
    [Address] nvarchar(500) NULL,
    [BloodGroup] nvarchar(10) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(100) NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAt] datetime2 NULL,
    [DeletedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_Students] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Students_AcademicMaps_AcademicMapId] FOREIGN KEY ([AcademicMapId]) REFERENCES [AcademicMaps] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Students_AspNetUsers_ApplicationUserId] FOREIGN KEY ([ApplicationUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [StudentAcademicRecords] (
    [Id] int NOT NULL IDENTITY,
    [Guid] uniqueidentifier NOT NULL,
    [StudentId] int NOT NULL,
    [AcademicMapId] int NOT NULL,
    [AcademicAliasCode] nvarchar(100) NOT NULL,
    [Semester] int NOT NULL,
    [AcademicYear] nvarchar(15) NOT NULL,
    [AcademicSessionSlug] nvarchar(100) NOT NULL,
    [Standing] int NOT NULL,
    [IsCurrentSemester] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(100) NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_StudentAcademicRecords] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StudentAcademicRecords_AcademicMaps_AcademicMapId] FOREIGN KEY ([AcademicMapId]) REFERENCES [AcademicMaps] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_StudentAcademicRecords_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_AcademicMaps_BranchId] ON [AcademicMaps] ([BranchId]);
GO

CREATE UNIQUE INDEX [IX_AcademicMaps_Guid] ON [AcademicMaps] ([Guid]);
GO

CREATE INDEX [IX_AcademicMaps_StudyProgramId] ON [AcademicMaps] ([StudyProgramId]);
GO

CREATE UNIQUE INDEX [IX_Unique_Academic_Path_Regulation] ON [AcademicMaps] ([CollegeId], [StudyProgramId], [BranchId], [Regulation]);
GO

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
GO

CREATE INDEX [IX_AspNetUsers_FirstName] ON [AspNetUsers] ([FirstName]);
GO

CREATE INDEX [IX_AspNetUsers_LastName] ON [AspNetUsers] ([LastName]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

CREATE UNIQUE INDEX [IX_Branches_Code] ON [Branches] ([Code]);
GO

CREATE UNIQUE INDEX [IX_Branches_Guid] ON [Branches] ([Guid]);
GO

CREATE UNIQUE INDEX [IX_Colleges_UniversityCode_Code] ON [Colleges] ([UniversityCode], [Code]) WHERE [IsDeleted] = 0;
GO

CREATE INDEX [IX_Colleges_UniversityId] ON [Colleges] ([UniversityId]);
GO

CREATE INDEX [IX_RefreshTokens_TokenHash] ON [RefreshTokens] ([TokenHash]);
GO

CREATE INDEX [IX_RefreshTokens_UserId] ON [RefreshTokens] ([UserId]);
GO

CREATE INDEX [IX_StudentAcademicRecords_AcademicMapId] ON [StudentAcademicRecords] ([AcademicMapId]);
GO

CREATE INDEX [IX_StudentAcademicRecords_AcademicSessionSlug] ON [StudentAcademicRecords] ([AcademicSessionSlug]);
GO

CREATE INDEX [IX_StudentAcademicRecords_IsCurrentSemester_Standing] ON [StudentAcademicRecords] ([IsCurrentSemester], [Standing]);
GO

CREATE UNIQUE INDEX [IX_StudentAcademicRecords_StudentId_Semester] ON [StudentAcademicRecords] ([StudentId], [Semester]);
GO

CREATE INDEX [IX_Students_AcademicMapId] ON [Students] ([AcademicMapId]);
GO

CREATE UNIQUE INDEX [IX_Students_ApplicationUserId] ON [Students] ([ApplicationUserId]);
GO

CREATE UNIQUE INDEX [IX_Students_Guid] ON [Students] ([Guid]) WHERE [IsDeleted] = 0;
GO

CREATE INDEX [IX_Students_IsActive] ON [Students] ([IsActive]);
GO

CREATE UNIQUE INDEX [IX_Students_RegistrationNumber] ON [Students] ([RegistrationNumber]) WHERE [IsDeleted] = 0;
GO

CREATE UNIQUE INDEX [IX_Subjects_Code] ON [Subjects] ([Code]);
GO

CREATE UNIQUE INDEX [IX_Subjects_Guid] ON [Subjects] ([Guid]);
GO

CREATE UNIQUE INDEX [IX_Universities_Code] ON [Universities] ([Code]) WHERE [IsDeleted] = 0;
GO

CREATE UNIQUE INDEX [IX_Universities_Guid] ON [Universities] ([Guid]) WHERE [IsDeleted] = 0;
GO

CREATE INDEX [IX_Universities_IsDeleted_Status] ON [Universities] ([IsDeleted], [Status]);
GO

CREATE INDEX [IX_Universities_Name] ON [Universities] ([Name]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260518201442_InitialDbCreate', N'8.0.0');
GO

COMMIT;
GO

